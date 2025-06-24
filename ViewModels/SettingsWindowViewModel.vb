Imports System.IO
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Input

Public Class SettingsWindowViewModel
    Implements INotifyPropertyChanged

    Private _creoPath As String
    Private _originalCreoPath As String ' To revert on cancel
    Private _isEditing As Boolean = False

    Public Property CreoPath As String
        Get
            Return _creoPath
        End Get
        Set(value As String)
            If _creoPath <> value Then
                _creoPath = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property IsEditing As Boolean
        Get
            Return _isEditing
        End Get
        Set(value As Boolean)
            If _isEditing <> value Then
                _isEditing = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(NotIsEditing))
                ' Raise CanExecuteChanged for commands relying on IsEditing
                CType(EditCommand, RelayCommand).RaiseCanExecuteChanged()
                CType(SaveCommand, RelayCommand).RaiseCanExecuteChanged()
                CType(CancelCommand, RelayCommand).RaiseCanExecuteChanged()
                CType(ConfirmCommand, RelayCommand).RaiseCanExecuteChanged()
            End If
        End Set
    End Property

    Public ReadOnly Property NotIsEditing As Boolean
        Get
            Return Not IsEditing
        End Get
    End Property

    ' Commands
    Public Property EditCommand As ICommand
    Public Property SaveCommand As ICommand
    Public Property CancelCommand As ICommand
    Public Property ConfirmCommand As ICommand

    ' Event to notify the view to close
    Public Event RequestClose As Action

    Public Sub New()
        _creoPath = My.Settings.CreoPath
        _originalCreoPath = _creoPath

        EditCommand = New RelayCommand(AddressOf Edit, Function() Not IsEditing)
        SaveCommand = New RelayCommand(AddressOf Save, Function() IsEditing)
        CancelCommand = New RelayCommand(AddressOf Cancel, Function() IsEditing)
        ConfirmCommand = New RelayCommand(AddressOf Confirm, Function() Not IsEditing)
    End Sub

    Private Sub Edit()
        _originalCreoPath = CreoPath ' Save current path to revert if cancel
        IsEditing = True
    End Sub

    Private Sub Save()
        If Not File.Exists(CreoPath) Then
            MessageBox.Show("Invalid path. Please enter a valid parametric.exe path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return
        End If

        ' Save to settings
        My.Settings.CreoPath = CreoPath
        My.Settings.Save()

        IsEditing = False
    End Sub

    Private Sub Cancel()
        ' Revert changes
        CreoPath = _originalCreoPath
        IsEditing = False
    End Sub

    Private Sub Confirm()
        ' Confirm only allowed when not editing, save path to settings in case
        My.Settings.CreoPath = CreoPath
        My.Settings.Save()

        ' Request the view (window) to close
        RaiseEvent RequestClose()
    End Sub

    ' INotifyPropertyChanged implementation
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional prop As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

End Class

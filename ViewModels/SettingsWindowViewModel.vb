Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows
Imports System.Windows.Input
Imports Microsoft.Win32

Public Class SettingsWindowViewModel
    Implements INotifyPropertyChanged

    Private _creoPath As String
    Private _originalCreoPath As String
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
    Public Property BrowseCommand As ICommand
    Public Property SaveCommand As ICommand
    Public Property CancelCommand As ICommand
    Public Property ConfirmCommand As ICommand

    Public Event RequestClose As Action

    Public Sub New()
        _creoPath = My.Settings.CreoPath
        _originalCreoPath = _creoPath
        BrowseCommand = New RelayCommand(AddressOf Browse)
        SaveCommand = New RelayCommand(AddressOf Save)
        CancelCommand = New RelayCommand(AddressOf Cancel)
        ConfirmCommand = New RelayCommand(AddressOf Confirm)
    End Sub

    Private Sub Browse()
        Dim dlg As New OpenFileDialog()
        dlg.Filter = "Creo Executable (parametric.exe)|parametric.exe"
        dlg.Title = "Select Creo parametric.exe"
        dlg.InitialDirectory = "C:\Program Files\PTC"
        If dlg.ShowDialog() = True Then
            CreoPath = dlg.FileName
        End If
    End Sub

    Private Sub Save()
        If Not File.Exists(CreoPath) OrElse Not CreoPath.ToLower().EndsWith("parametric.exe") Then
            MessageBox.Show("Invalid file. Please select a valid parametric.exe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return
        End If

        My.Settings.CreoPath = CreoPath
        My.Settings.Save()
        _originalCreoPath = CreoPath
    End Sub

    Private Sub Cancel()
        CreoPath = _originalCreoPath
        RaiseEvent RequestClose()
    End Sub

    Private Sub Confirm()
        If Not File.Exists(CreoPath) OrElse Not CreoPath.ToLower().EndsWith("parametric.exe") Then
            MessageBox.Show("Invalid file. Please select a valid parametric.exe.", "Error", MessageBoxButton.OK, MessageBoxImage.Error)
            Return
        Else
            MessageBox.Show("Parametric path is updated.")
        End If

        My.Settings.CreoPath = CreoPath
        My.Settings.Save()
        RaiseEvent RequestClose()
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional name As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

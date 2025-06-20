Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Windows.Input
Imports CreoPublisherApp.CreoPublisherApp.ViewModels

Public Class SettingsWindowViewModel
    Implements INotifyPropertyChanged

    ' Custom Margins
    Private _leftMargin As String
    Public Property LeftMargin As String
        Get
            Return _leftMargin
        End Get
        Set(value As String)
            _leftMargin = value
            OnPropertyChanged()
        End Set
    End Property

    Private _topMargin As String
    Public Property TopMargin As String
        Get
            Return _topMargin
        End Get
        Set(value As String)
            _topMargin = value
            OnPropertyChanged()
        End Set
    End Property

    ' Other Export Options
    Public Property SelectedResolution As String
    Public Property SelectedPaperSize As String
    Public Property SelectedOrientation As String

    ' Command
    Public Property ConfirmCommand As ICommand

    Public Sub New(mainVm As MainViewModel, window As Window)
        ' Load values from MainViewModel
        LeftMargin = mainVm.LeftMargin
        TopMargin = mainVm.TopMargin
        SelectedResolution = mainVm.SelectedResolutions.FirstOrDefault()
        SelectedPaperSize = mainVm.SelectedPaperSize
        SelectedOrientation = mainVm.SelectedOrientation

        ' Confirm button logic
        ConfirmCommand = New RelayCommand(Sub()
                                              mainVm.LeftMargin = LeftMargin
                                              mainVm.TopMargin = TopMargin
                                              mainVm.SelectedResolutions.Clear()
                                              mainVm.SelectedResolutions.Add(SelectedResolution)
                                              mainVm.SelectedPaperSize = SelectedPaperSize
                                              mainVm.SelectedOrientation = SelectedOrientation
                                              window.Close()
                                          End Sub)
    End Sub

    ' INotifyPropertyChanged implementation
    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional prop As String = "")
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
End Class

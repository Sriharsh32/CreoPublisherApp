Imports System.IO
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Windows.Input
Imports Microsoft.Win32
Imports Ookii.Dialogs.Wpf
Imports System.Linq
Imports Models ' Project-specific namespace

Public Class MainViewModel
    Implements INotifyPropertyChanged

    Private _inputFiles As New List(Of String)
    Private _outputPath As String
    Private _log As New StringBuilder

    Public Property OutputPath As String
        Get
            Return _outputPath
        End Get
        Set(value As String)
            _outputPath = value
            OnPropertyChanged()
        End Set
    End Property

    Public ReadOnly Property LogText As String
        Get
            Return _log.ToString()
        End Get
    End Property

    Public ReadOnly Property BrowseFilesCommand As ICommand
    Public ReadOnly Property BrowseFolderCommand As ICommand
    Public ReadOnly Property BrowseOutputFolderCommand As ICommand
    Public ReadOnly Property PublishCommand As ICommand

    Public Sub New()
        BrowseFilesCommand = New RelayCommand(AddressOf BrowseFiles)
        BrowseFolderCommand = New RelayCommand(AddressOf BrowseFolder)
        BrowseOutputFolderCommand = New RelayCommand(AddressOf BrowseOutputFolder)
        PublishCommand = New RelayCommand(AddressOf Publish)
    End Sub

    Private Sub BrowseFiles()
        Dim dlg = New OpenFileDialog With {
            .Multiselect = True,
            .Filter = "Creo Drawing Files (*.drw)|*.drw"
        }
        If dlg.ShowDialog() Then
            _inputFiles = dlg.FileNames.ToList()
            AppendLog($"Selected {_inputFiles.Count} files.")
        End If
    End Sub

    Private Sub BrowseFolder()
        Dim dlg = New VistaFolderBrowserDialog()
        If dlg.ShowDialog() Then
            _inputFiles = Directory.GetFiles(dlg.SelectedPath, "*.drw").ToList()
            AppendLog($"Loaded {_inputFiles.Count} files from folder: {dlg.SelectedPath}")
        End If
    End Sub

    Private Sub BrowseOutputFolder()
        Dim dlg = New VistaFolderBrowserDialog()
        If dlg.ShowDialog() Then
            OutputPath = dlg.SelectedPath
        End If
    End Sub

    Private Sub Publish()
        If _inputFiles.Count = 0 OrElse String.IsNullOrEmpty(OutputPath) Then
            AppendLog("Select input files and output folder first.")
            Return
        End If

        AppendLog("Starting Creo session...")

        Dim opener As New OpenProeObjectClass()
        ' Use the folder path of the first input file as working directory
        Dim workingDir = Path.GetDirectoryName(_inputFiles(0))

        If Not opener.RunProe(workingDir) Then
            AppendLog("Failed to launch Creo.")
            Return
        End If

        CreoSessionManager.Instance.InitializeCreoSession()
        AppendLog("Creo session ready.")

        Dim util = New ProECommonFunctionalites()
        util.ExportAllDrawingsAsPDF(_inputFiles, OutputPath) ' Passing file list, see next fix

        AppendLog("Publishing complete.")

        opener.KillCreO()
        AppendLog("Creo closed.")
    End Sub

    Private Sub AppendLog(message As String)
        _log.AppendLine($"{DateTime.Now:HH:mm:ss} - {message}")
        OnPropertyChanged(NameOf(LogText))
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional name As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(name))
    End Sub
End Class

Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Input
Imports Microsoft.Win32
Imports Ookii.Dialogs.Wpf
Imports System.Linq

Namespace CreoPublisherApp.ViewModels

    Public Class MainViewModel
        Implements INotifyPropertyChanged

        Private _inputPath As String = ""
        Private _outputPath As String = ""
        Private _files As ObservableCollection(Of FileItemModel) = New ObservableCollection(Of FileItemModel)()
        Private _log As String = ""
        Private _filesCountText As String = "No files selected."

        Private _createdDateStart As Date?
        Private _createdDateEnd As Date?
        Private _modifiedDateStart As Date?
        Private _modifiedDateEnd As Date?
        Private _isAllSelected As Boolean = False
        Private _selectDeselectButtonText As String = "Select All"
        Private _searchText As String = String.Empty

        Private _openProe As New OpenProeObjectClass()
        Private _proeCommonFuncs As New ProECommonFunctionalites()

        Public Property InputPath As String
            Get
                Return _inputPath
            End Get
            Set(value As String)
                If _inputPath <> value Then
                    _inputPath = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Property OutputPath As String
            Get
                Return _outputPath
            End Get
            Set(value As String)
                If _outputPath <> value Then
                    _outputPath = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Property SearchText As String
            Get
                Return _searchText
            End Get
            Set(value As String)
                If _searchText <> value Then
                    _searchText = value
                    OnPropertyChanged()
                    OnPropertyChanged(NameOf(FilteredFiles))
                End If
            End Set
        End Property

        Public ReadOnly Property Files As ObservableCollection(Of FileItemModel)
            Get
                Return _files
            End Get
        End Property

        Public ReadOnly Property FilteredFiles As IEnumerable(Of FileItemModel)
            Get
                Return _files.Where(Function(f)
                                        Return (String.IsNullOrWhiteSpace(SearchText) OrElse f.FileName.ToLower().Contains(SearchText.ToLower())) AndAlso
                                               (Not CreatedDateStart.HasValue OrElse f.CreatedDate >= CreatedDateStart.Value) AndAlso
                                               (Not CreatedDateEnd.HasValue OrElse f.CreatedDate <= CreatedDateEnd.Value) AndAlso
                                               (Not ModifiedDateStart.HasValue OrElse f.ModifiedDate >= ModifiedDateStart.Value) AndAlso
                                               (Not ModifiedDateEnd.HasValue OrElse f.ModifiedDate <= ModifiedDateEnd.Value)
                                    End Function)
            End Get
        End Property

        Public Property CreatedDateStart As Date?
            Get
                Return _createdDateStart
            End Get
            Set(value As Date?)
                _createdDateStart = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilteredFiles))
            End Set
        End Property

        Public Property CreatedDateEnd As Date?
            Get
                Return _createdDateEnd
            End Get
            Set(value As Date?)
                _createdDateEnd = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilteredFiles))
            End Set
        End Property

        Public Property ModifiedDateStart As Date?
            Get
                Return _modifiedDateStart
            End Get
            Set(value As Date?)
                _modifiedDateStart = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilteredFiles))
            End Set
        End Property

        Public Property ModifiedDateEnd As Date?
            Get
                Return _modifiedDateEnd
            End Get
            Set(value As Date?)
                _modifiedDateEnd = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilteredFiles))
            End Set
        End Property

        Public Property Log As String
            Get
                Return _log
            End Get
            Set(value As String)
                If _log <> value Then
                    _log = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public ReadOnly Property FilesCountText As String
            Get
                Return _filesCountText
            End Get
        End Property

        Public Property SelectDeselectButtonText As String
            Get
                Return _selectDeselectButtonText
            End Get
            Set(value As String)
                If _selectDeselectButtonText <> value Then
                    _selectDeselectButtonText = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

        Public Property BrowseFolderCommand As ICommand
        Public Property BrowseFilesCommand As ICommand
        Public Property BrowseOutputFolderCommand As ICommand
        Public Property SelectAllCommand As ICommand
        Public Property DeselectAllCommand As ICommand
        Public Property InvertSelectionCommand As ICommand
        Public Property PublishCommand As ICommand
        Public Property ToggleSelectCommand As ICommand
        Public Property ClearFiltersCommand As ICommand

        Public Sub New()
            BrowseFolderCommand = New RelayCommand(AddressOf BrowseFolder)
            BrowseFilesCommand = New RelayCommand(AddressOf BrowseFiles)
            BrowseOutputFolderCommand = New RelayCommand(AddressOf BrowseOutputFolder)
            SelectAllCommand = New RelayCommand(AddressOf SelectAllFiles)
            DeselectAllCommand = New RelayCommand(AddressOf DeselectAllFiles)
            InvertSelectionCommand = New RelayCommand(AddressOf InvertSelectionFiles)
            PublishCommand = New RelayCommand(AddressOf PublishFiles)
            ToggleSelectCommand = New RelayCommand(AddressOf ToggleSelectDeselect)
            ClearFiltersCommand = New RelayCommand(AddressOf ClearFilters)
        End Sub

        Private Sub BrowseFolder()
            Dim dialog As New VistaFolderBrowserDialog()
            If dialog.ShowDialog() = True Then
                InputPath = dialog.SelectedPath
                LoadFilesFromFolder(InputPath)
            End If
        End Sub

        Private Sub BrowseFiles()
            Dim dlg As New OpenFileDialog()
            dlg.Multiselect = True
            dlg.Filter = "Creo Drawing Files (*.drw)|*.drw|All files (*.*)|*.*"

            If dlg.ShowDialog() = True Then
                _files.Clear()
                For Each filePath In dlg.FileNames
                    _files.Add(New FileItemModel(filePath))
                Next
                InputPath = "Multiple files selected"
                UpdateFilesCountText()
                OnPropertyChanged(NameOf(FilteredFiles))
            End If
        End Sub

        Private Sub BrowseOutputFolder()
            Dim dialog As New VistaFolderBrowserDialog()
            If dialog.ShowDialog() = True Then
                OutputPath = dialog.SelectedPath
            End If
        End Sub

        Private Sub LoadFilesFromFolder(folderPath As String)
            Try
                _files.Clear()
                Dim filesInFolder = Directory.GetFiles(folderPath, "*.drw")
                For Each filePath In filesInFolder
                    _files.Add(New FileItemModel(filePath))
                Next
                UpdateFilesCountText()
                OnPropertyChanged(NameOf(FilteredFiles))
            Catch ex As Exception
                Log &= $"Error loading files from folder: {ex.Message}{Environment.NewLine}"
            End Try
        End Sub

        Private Sub UpdateFilesCountText()
            _filesCountText = $"{_files.Count} file(s) selected"
            OnPropertyChanged(NameOf(FilesCountText))
        End Sub

        Private Sub SelectAllFiles()
            For Each f In FilteredFiles
                f.IsSelected = True
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub

        Private Sub DeselectAllFiles()
            For Each f In FilteredFiles
                f.IsSelected = False
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub

        Private Sub InvertSelectionFiles()
            For Each f In FilteredFiles
                f.IsSelected = Not f.IsSelected
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub

        Private Sub ToggleSelectDeselect()
            _isAllSelected = Not _isAllSelected
            For Each f In FilteredFiles
                f.IsSelected = _isAllSelected
            Next
            SelectDeselectButtonText = If(_isAllSelected, "Deselect All", "Select All")
            OnPropertyChanged(NameOf(Files))
        End Sub

        Private Sub ClearFilters()
            CreatedDateStart = Nothing
            CreatedDateEnd = Nothing
            ModifiedDateStart = Nothing
            ModifiedDateEnd = Nothing
            SearchText = String.Empty
            OnPropertyChanged(NameOf(FilteredFiles))
        End Sub

        Private Sub PublishFiles()
            Dim selectedFiles = FilteredFiles.Where(Function(f) f.IsSelected).ToList()
            If selectedFiles.Count = 0 Then
                Log &= "No files selected for publishing." & Environment.NewLine
                Return
            End If
            If String.IsNullOrEmpty(OutputPath) Then
                Log &= "Please select an output folder before publishing." & Environment.NewLine
                Return
            End If

            Log &= $"Starting Creo launch and export for {selectedFiles.Count} files..." & Environment.NewLine

            Dim workingDir = Environment.CurrentDirectory
            Dim launchSuccess = _openProe.RunProe(workingDir)
            If Not launchSuccess Then
                Log &= "Failed to launch Creo." & Environment.NewLine
                Return
            End If

            CreoSessionManager.Instance.InitializeCreoSession()
            If CreoSessionManager.Instance.Session Is Nothing Then
                Log &= "Failed to initialize Creo session." & Environment.NewLine
                Return
            End If

            _proeCommonFuncs.setConfigOption("display_planes", "no")
            _proeCommonFuncs.setConfigOption("display_axes", "no")
            _proeCommonFuncs.setConfigOption("display_coord_sys", "no")
            _proeCommonFuncs.ExportAllDrawingsAsPDF(selectedFiles, OutputPath)

            For Each file In selectedFiles
                OnPropertyChanged(NameOf(Files))
            Next
            Log &= "Publishing complete." & Environment.NewLine
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

    End Class

End Namespace

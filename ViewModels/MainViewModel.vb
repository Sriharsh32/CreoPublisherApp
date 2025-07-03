Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Data
Imports System.Windows.Input
Imports Microsoft.Win32
Imports Ookii.Dialogs.Wpf
Imports System.Linq
Imports System.Windows ' For MessageBox and Window

Namespace CreoPublisherApp.ViewModels

    Public Class MainViewModel
        Implements INotifyPropertyChanged

        Private _inputPath As String = ""
        Private _outputPath As String = ""
        Private _files As ObservableCollection(Of FileItemModel) = New ObservableCollection(Of FileItemModel)()
        Private _filteredFilesView As ICollectionView

        Private _log As String = ""
        Private _filesCountText As String = "No files selected."

        Private _createdDateStart As Date?
        Private _createdDateEnd As Date?
        Private _modifiedDateStart As Date?
        Private _modifiedDateEnd As Date?
        Private _isAllSelected As Boolean = False
        Private _selectDeselectButtonText As String = "Select All"
        Private _searchText As String = String.Empty

        ' Creo automation classes
        Private _openProe As New OpenProeObjectClass()
        Private _proeCommonFuncs As New ProECommonFunctionalites()

        ' Command to open Settings window
        Public Property OpenSettingsWindowCommand As ICommand

        Public Sub New()
            _filteredFilesView = CollectionViewSource.GetDefaultView(_files)
            _filteredFilesView.Filter = AddressOf FilterFiles


            ' Commands
            BrowseFolderCommand = New RelayCommand(AddressOf BrowseFolder)
            BrowseFilesCommand = New RelayCommand(AddressOf BrowseFiles)
            BrowseOutputFolderCommand = New RelayCommand(AddressOf BrowseOutputFolder)
            SelectAllCommand = New RelayCommand(AddressOf SelectAllFiles)
            DeselectAllCommand = New RelayCommand(AddressOf DeselectAllFiles)
            InvertSelectionCommand = New RelayCommand(AddressOf InvertSelectionFiles)
            PublishCommand = New RelayCommand(AddressOf PublishFiles)
            ToggleSelectCommand = New RelayCommand(AddressOf ToggleSelectDeselect)
            ClearFiltersCommand = New RelayCommand(AddressOf ClearFilters)

            ' Settings window open command
            OpenSettingsWindowCommand = New RelayCommand(AddressOf OpenSettingsWindow)
        End Sub

        ' Properties

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
                    RefreshFilter()
                End If
            End Set
        End Property

        Public ReadOnly Property Files As ObservableCollection(Of FileItemModel)
            Get
                Return _files
            End Get
        End Property

        Public ReadOnly Property FilteredFiles As ICollectionView
            Get
                Return _filteredFilesView
            End Get
        End Property

        Public Property CreatedDateStart As Date?
            Get
                Return _createdDateStart
            End Get
            Set(value As Date?)
                _createdDateStart = value
                OnPropertyChanged()
                RefreshFilter()
            End Set
        End Property

        Public Property CreatedDateEnd As Date?
            Get
                Return _createdDateEnd
            End Get
            Set(value As Date?)
                _createdDateEnd = value
                OnPropertyChanged()
                RefreshFilter()
            End Set
        End Property

        Public Property ModifiedDateStart As Date?
            Get
                Return _modifiedDateStart
            End Get
            Set(value As Date?)
                _modifiedDateStart = value
                OnPropertyChanged()
                RefreshFilter()
            End Set
        End Property

        Public Property ModifiedDateEnd As Date?
            Get
                Return _modifiedDateEnd
            End Get
            Set(value As Date?)
                _modifiedDateEnd = value
                OnPropertyChanged()
                RefreshFilter()
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


        ' End of properties 

        ' Commands
        Public Property BrowseFolderCommand As ICommand
        Public Property BrowseFilesCommand As ICommand
        Public Property BrowseOutputFolderCommand As ICommand
        Public Property SelectAllCommand As ICommand
        Public Property DeselectAllCommand As ICommand
        Public Property InvertSelectionCommand As ICommand
        Public Property PublishCommand As ICommand
        Public Property ToggleSelectCommand As ICommand
        Public Property ClearFiltersCommand As ICommand

        ' Filtering logic
        Private Function FilterFiles(obj As Object) As Boolean
            Dim file = TryCast(obj, FileItemModel)
            If file Is Nothing Then Return False

            Dim matchesSearch As Boolean = String.IsNullOrWhiteSpace(SearchText) OrElse file.FileName.ToLower().Contains(SearchText.ToLower())
            Dim matchesCreatedStart As Boolean = Not CreatedDateStart.HasValue OrElse file.CreatedDateRaw >= CreatedDateStart.Value
            Dim matchesCreatedEnd As Boolean = Not CreatedDateEnd.HasValue OrElse file.CreatedDateRaw <= CreatedDateEnd.Value
            Dim matchesModifiedStart As Boolean = Not ModifiedDateStart.HasValue OrElse file.ModifiedDateRaw >= ModifiedDateStart.Value
            Dim matchesModifiedEnd As Boolean = Not ModifiedDateEnd.HasValue OrElse file.ModifiedDateRaw <= ModifiedDateEnd.Value

            Return matchesSearch AndAlso matchesCreatedStart AndAlso matchesCreatedEnd AndAlso matchesModifiedStart AndAlso matchesModifiedEnd
        End Function

        Private Sub RefreshFilter()
            _filteredFilesView.Refresh()
        End Sub

        ' File loading and updating

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
            dlg.Filter = "Creo Drawing Files (*.drw)|*.drw"

            If dlg.ShowDialog() = True Then
                _files.Clear()
                For Each filePath In dlg.FileNames
                    _files.Add(New FileItemModel(filePath))
                Next
                InputPath = "Multiple files selected"
                UpdateFilesCountText()
                RefreshFilter()
            End If
        End Sub

        Private Sub BrowseOutputFolder()
            Dim dialog As New VistaFolderBrowserDialog()
            If dialog.ShowDialog() = True Then
                OutputPath = dialog.SelectedPath
            End If
        End Sub
        'Method for loading files from folder
        Private Sub LoadFilesFromFolder(folderPath As String)
            Try
                _files.Clear()
                Dim filesInFolder = Directory.GetFiles(folderPath).
                Where(Function(f) Path.GetExtension(f).ToLower() = ".drw").ToArray()
                For Each filePath In filesInFolder
                    _files.Add(New FileItemModel(filePath))
                Next
                UpdateFilesCountText()
                RefreshFilter()
            Catch ex As Exception
                Log &= $"Error loading files from folder: {ex.Message}{Environment.NewLine}"
            End Try
        End Sub
        'Method for updating the select count if multiple files selected
        Private Sub UpdateFilesCountText()
            _filesCountText = $"{_files.Count} file(s) selected"
            OnPropertyChanged(NameOf(FilesCountText))
        End Sub

        ' Selection manipulation on filtered view

        Private Sub ToggleSelectDeselect()
            _isAllSelected = Not _isAllSelected
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = _isAllSelected
            Next
            SelectDeselectButtonText = If(_isAllSelected, "Deselect All", "Select All")
            OnPropertyChanged(NameOf(Files))
        End Sub
        'Clear Filters
        Private Sub ClearFilters()
            CreatedDateStart = Nothing
            CreatedDateEnd = Nothing
            ModifiedDateStart = Nothing
            ModifiedDateEnd = Nothing
            SearchText = String.Empty
            RefreshFilter()
        End Sub
        'Select all
        Private Sub SelectAllFiles()
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = True
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub
        'Deselect All
        Private Sub DeselectAllFiles()
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = False
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub
        'Invert Selection
        Private Sub InvertSelectionFiles()
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = Not f.IsSelected
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub

        ' Publishing
        'Method to publish the selected creo files from folder/files.
        Private Sub PublishFiles()
            Dim selectedFiles = _files.Where(Function(f) f.IsSelected).ToList()
            If selectedFiles.Count = 0 Then
                MessageBox.Show("No Files selected for publishing")
                Log &= "No files selected for publishing." & Environment.NewLine
                Return
            End If
            If String.IsNullOrEmpty(OutputPath) Then
                MessageBox.Show("Please select an output folder before publishing")
                Log &= "Please select an output folder before publishing." & Environment.NewLine
                Return
            End If
            ' It checks for the filename duplication conflicts and throws error
            Dim overwriteAll As Boolean? = Nothing
            Dim conflictFiles As New List(Of String)
            Dim filesToExport As New List(Of FileItemModel)

            For Each file In selectedFiles
                Dim pdfName As String = If(String.IsNullOrWhiteSpace(file.CustomPdfName),
                               Path.GetFileNameWithoutExtension(file.FileName),
                               Path.GetFileNameWithoutExtension(file.CustomPdfName))
                Dim pdfPath As String = Path.Combine(OutputPath, pdfName & ".pdf")

                If System.IO.File.Exists(pdfPath) Then
                    If overwriteAll Is Nothing Then
                        Dim dlg As New FileConflictDialog(pdfName & ".pdf")
                        dlg.Owner = Application.Current.MainWindow
                        dlg.ShowDialog()
                        'Select case based on the user selection the skipping of files takes place
                        Select Case dlg.UserChoice
                            Case FileConflictDialog.ConflictResult.Yes
                                filesToExport.Add(file) ' Add only this one
                            Case FileConflictDialog.ConflictResult.No
                                file.Status = "Error: File exists, not overwritten."
                                conflictFiles.Add(pdfName & ".pdf")
                                Continue For
                            Case FileConflictDialog.ConflictResult.YesToAll
                                overwriteAll = True
                                filesToExport.Add(file)
                            Case FileConflictDialog.ConflictResult.NoToAll
                                overwriteAll = False
                                file.Status = "Error: File exists, not overwritten."
                                conflictFiles.Add(pdfName & ".pdf")
                                Continue For
                        End Select
                        'If the user wants to overwrite all the files
                    ElseIf overwriteAll = True Then
                        filesToExport.Add(file)
                    Else
                        file.Status = "Error: File exists, not overwritten."
                        conflictFiles.Add(pdfName & ".pdf")
                        Continue For
                    End If
                Else
                    filesToExport.Add(file)
                End If
            Next
            'It the user skips any files then it will show skipped files in the messagebox
            If conflictFiles.Any() Then
                MessageBox.Show("The following files were skipped due to overwrite being declined:" & vbCrLf &
        String.Join(vbCrLf, conflictFiles), "Skipped Files", MessageBoxButton.OK, MessageBoxImage.Warning)
                Log &= "Skipped files: " & String.Join(", ", conflictFiles) & vbCrLf
            End If

            If filesToExport.Count = 0 Then
                Log &= "No files to publish conflicting filenames." & vbCrLf
                Return
            End If

            'Log &= $"Starting Creo launch and export for {selectedFiles.Count} files..." & Environment.NewLine
            'Command to export only selected files.
            Log &= $"Starting Creo launch and export for {filesToExport.Count} files..." & Environment.NewLine

            'Opening the working directory and creo executable path
            Dim workingDir = "C:\Tempp"
            Dim savedCreoPath = My.Settings.CreoPath
            If String.IsNullOrEmpty(savedCreoPath) OrElse Not File.Exists(savedCreoPath) Then
                MessageBox.Show("Invalid or missing Creo executable path. Please set it in Settings.")
                Log &= "Invalid or missing Creo path." & Environment.NewLine
                Return
            End If

            Dim launchSuccess = _openProe.RunProeWithPath(savedCreoPath, workingDir)
            'If creo launch fails.
            If Not launchSuccess Then
                MessageBox.Show("Failed to launch Creo")
                Log &= "Failed to launch Creo." & Environment.NewLine
                Return
            End If
            'If initiliaze creo session is failed.
            CreoSessionManager.Instance.InitializeCreoSession()
            If CreoSessionManager.Instance.Session Is Nothing Then
                MessageBox.Show("Failed to initialize Creo session")
                Log &= "Failed to initialize Creo session." & Environment.NewLine
                Return
            End If

            _proeCommonFuncs.setConfigOption("display_planes", "no")
            _proeCommonFuncs.setConfigOption("display_axes", "no")
            _proeCommonFuncs.setConfigOption("display_coord_sys", "no")
            '_proeCommonFuncs.ExportAllDrawingsAsPDF(selectedFiles, OutputPath)
            _proeCommonFuncs.ExportAllDrawingsAsPDF(filesToExport, OutputPath)
            _openProe.KillCreO()
            'After exporting all the selected files and publishing.
            For Each file In selectedFiles
                OnPropertyChanged(NameOf(Files))
            Next
            Log &= "Publishing complete." & Environment.NewLine
        End Sub

        ' Open Settings window dialog box which contains creo ex. path
        Private Sub OpenSettingsWindow()
            Dim settingsWindow As New SettingsWindow()
            settingsWindow.DataContext = New SettingsWindowViewModel()
            settingsWindow.ShowDialog()
        End Sub

        ' INotifyPropertyChanged implementation
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

    End Class

End Namespace

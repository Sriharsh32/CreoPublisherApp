Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Data
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
        Private _leftMargin As String = ""
        Private _topMargin As String = ""
        Private _selectedResolutions As New ObservableCollection(Of String) From {"300"}
        Private _selectedPaperSize As String = "A4"
        Private _selectedOrientation As String = "Portrait"

        ' New property for Settings popup visibility
        Private _isSettingsVisible As Boolean = False

        ' Creo automation classes
        Private _openProe As New OpenProeObjectClass()
        Private _proeCommonFuncs As New ProECommonFunctionalites()

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
            OpenSettingsWindowCommand = New RelayCommand(AddressOf OpenSettingsWindow)


            ' New command for toggling Settings popup
            ToggleSettingsVisibilityCommand = New RelayCommand(AddressOf ToggleSettingsVisibility)
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
        Public Property LeftMargin As String
            Get
                Return _leftMargin
            End Get
            Set(value As String)
                _leftMargin = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property TopMargin As String
            Get
                Return _topMargin
            End Get
            Set(value As String)
                _topMargin = value
                OnPropertyChanged()
            End Set
        End Property
        Public Property SelectedResolutions As ObservableCollection(Of String)
            Get
                Return _selectedResolutions
            End Get
            Set(value As ObservableCollection(Of String))
                _selectedResolutions = value
                OnPropertyChanged()
            End Set
        End Property

        Public Property SelectedPaperSize As String
            Get
                Return _selectedPaperSize
            End Get
            Set(value As String)
                _selectedPaperSize = value
                OnPropertyChanged()
            End Set
        End Property

        Public Property SelectedOrientation As String
            Get
                Return _selectedOrientation
            End Get
            Set(value As String)
                _selectedOrientation = value
                OnPropertyChanged()
            End Set
        End Property

        ' New Property for Settings Popup Visibility
        Public Property IsSettingsVisible As Boolean
            Get
                Return _isSettingsVisible
            End Get
            Set(value As Boolean)
                If _isSettingsVisible <> value Then
                    _isSettingsVisible = value
                    OnPropertyChanged()
                End If
            End Set
        End Property

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

        Public Property OpenSettingsWindowCommand As ICommand


        ' New command for toggling Settings popup
        Public Property ToggleSettingsVisibilityCommand As ICommand

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
            dlg.Filter = "Creo Drawing Files (*.drw)|*.drw|All files (*.*)|*.*"

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

        Private Sub ClearFilters()
            CreatedDateStart = Nothing
            CreatedDateEnd = Nothing
            ModifiedDateStart = Nothing
            ModifiedDateEnd = Nothing
            SearchText = String.Empty
            RefreshFilter()
        End Sub

        Private Sub SelectAllFiles()
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = True
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub

        Private Sub DeselectAllFiles()
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = False
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub

        Private Sub InvertSelectionFiles()
            For Each f As FileItemModel In _filteredFilesView
                f.IsSelected = Not f.IsSelected
            Next
            OnPropertyChanged(NameOf(Files))
        End Sub
        Private Sub OpenSettingsWindow()
            Dim settingsWin As New SettingsWindow()
            settingsWin.DataContext = New SettingsWindowViewModel(Me, settingsWin)
            settingsWin.Owner = Application.Current.MainWindow
            settingsWin.ShowDialog()
        End Sub

        ' Publishing

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

            Log &= $"Starting Creo launch and export for {selectedFiles.Count} files..." & Environment.NewLine

            Dim workingDir = Environment.CurrentDirectory
            Dim launchSuccess = _openProe.RunProe(workingDir)
            If Not launchSuccess Then
                MessageBox.Show("Failed to launch Creo")
                Log &= "Failed to launch Creo." & Environment.NewLine
                Return
            End If

            CreoSessionManager.Instance.InitializeCreoSession()
            If CreoSessionManager.Instance.Session Is Nothing Then
                MessageBox.Show("Failed to initialize Creo session")
                Log &= "Failed to initialize Creo session." & Environment.NewLine
                Return
            End If

            _proeCommonFuncs.setConfigOption("display_planes", "no")
            _proeCommonFuncs.setConfigOption("display_axes", "no")
            _proeCommonFuncs.setConfigOption("display_coord_sys", "no")
            _proeCommonFuncs.ExportAllDrawingsAsPDF(selectedFiles, OutputPath, LeftMargin, TopMargin, SelectedResolutions.ToList(), SelectedPaperSize, SelectedOrientation)

            For Each file In selectedFiles
                OnPropertyChanged(NameOf(Files))
            Next
            Log &= "Publishing complete." & Environment.NewLine
        End Sub

        ' New method to toggle settings popup visibility
        Private Sub ToggleSettingsVisibility()
            IsSettingsVisible = Not IsSettingsVisible
        End Sub

        ' INotifyPropertyChanged implementation
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub

    End Class

End Namespace
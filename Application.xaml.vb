Imports CreoPublisherApp.CreoPublisherApp
Imports System.IO
Imports System.Windows

Partial Class Application
    Inherits System.Windows.Application

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)

        Dim args = e.Args

        ' CLI Mode: Expect exactly 3 arguments
        If args.Length = 3 Then
            Dim inputPath = args(0)
            Dim outputPath = args(1)
            Dim creoPath = args(2)

            ' Validate paths
            If Not Directory.Exists(inputPath) Then
                MessageBox.Show("Invalid input folder path.")
                Environment.Exit(1)
            End If

            If Not File.Exists(creoPath) OrElse Not creoPath.ToLower().EndsWith("parametric.exe") Then
                MessageBox.Show("Invalid Creo executable path.")
                Environment.Exit(1)
            End If

            If Not Directory.Exists(outputPath) Then
                Directory.CreateDirectory(outputPath)
            End If

            ' Save Creo path to settings
            My.Settings.CreoPath = creoPath
            My.Settings.Save()

            ' Run publishing
            Dim vm As New ViewModels.MainViewModel()

            ' ✅ Manually load .drw files since no UI is shown
            Dim drwFiles = Directory.GetFiles(inputPath, "*.drw", SearchOption.TopDirectoryOnly)
            For Each file In drwFiles
                Dim fileModel As New FileItemModel(file)
                fileModel.IsSelected = True
                vm.Files.Add(fileModel)
            Next

            vm.OutputPath = outputPath

            vm.PublishFiles()

            Environment.Exit(0)
        Else
            ' GUI Mode
            Dim intro = New IntroWindow()
            intro.Show()
        End If
    End Sub

End Class

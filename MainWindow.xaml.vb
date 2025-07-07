Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media.Animation
Imports CreoPublisherApp.CreoPublisherApp.ViewModels

Namespace CreoPublisherApp

    Partial Public Class MainWindow
        Inherits Window

        Private ReadOnly viewModel As MainViewModel

        ' Keep a reference to the fadeOut Completed handler for removal
        Private fadeOutCompletedHandler As EventHandler

        Public Sub New()
            InitializeComponent()
            viewModel = New MainViewModel()
            DataContext = viewModel
        End Sub

        Private Sub DataGrid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
            ' Your existing code or leave empty
        End Sub

        Private Sub Grid_DragEnter(sender As Object, e As DragEventArgs)
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effects = DragDropEffects.Copy
                ShowDragDropOverlay()
            Else
                e.Effects = DragDropEffects.None
            End If
            e.Handled = True
        End Sub

        Private Sub Grid_DragLeave(sender As Object, e As DragEventArgs)
            HideDragDropOverlay()
            e.Handled = True
        End Sub

        Private Sub Grid_Drop(sender As Object, e As DragEventArgs)
            HideDragDropOverlay()
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim droppedItems() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
                If droppedItems IsNot Nothing AndAlso droppedItems.Length > 0 Then
                    viewModel.AddDroppedFiles(droppedItems)

                    ' Set InputPath to folder of first dropped file
                    Dim firstFolder As String = System.IO.Path.GetDirectoryName(droppedItems(0))
                    If Not String.IsNullOrEmpty(firstFolder) Then
                        viewModel.InputPath = firstFolder
                    End If
                End If
            End If
            e.Handled = True
        End Sub

        Private Sub ShowDragDropOverlay()
            DragDropOverlay.Visibility = Visibility.Visible
            Dim fadeIn As Storyboard = CType(Resources("FadeInOverlay"), Storyboard)
            Dim pulse As Storyboard = CType(Resources("PulseOverlay"), Storyboard)

            fadeIn.Begin(DragDropOverlay)
            pulse.Begin(DragDropOverlay)
        End Sub

        Private Sub HideDragDropOverlay()
            Dim fadeOut As Storyboard = CType(Resources("FadeOutOverlay"), Storyboard)
            Dim pulse As Storyboard = CType(Resources("PulseOverlay"), Storyboard)

            ' Define the handler delegate so it can be removed properly
            fadeOutCompletedHandler = Sub(s As Object, e As EventArgs)
                                          DragDropOverlay.Visibility = Visibility.Collapsed
                                          RemoveHandler fadeOut.Completed, fadeOutCompletedHandler
                                      End Sub

            AddHandler fadeOut.Completed, fadeOutCompletedHandler

            pulse.Stop(DragDropOverlay)
            fadeOut.Begin(DragDropOverlay)
        End Sub
        Private Sub ShowDragDropInstructions(sender As Object, e As RoutedEventArgs)
            MessageBox.Show("To add drawing files, simply drag and drop them (or folders) anywhere on this window.",
                    "How to Drag and Drop", MessageBoxButton.OK, MessageBoxImage.Information)
        End Sub

    End Class

End Namespace

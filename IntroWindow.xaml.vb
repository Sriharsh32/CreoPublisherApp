Namespace CreoPublisherApp
    Public Class IntroWindow
        Inherits Window

        Private Sub ContinueButton_Click(sender As Object, e As RoutedEventArgs)
            Dim mainWindow = New MainWindow()

            ' When MainWindow closes, also close IntroWindow
            AddHandler mainWindow.Closed, Sub()
                                              Me.Close() ' this will close IntroWindow when MainWindow closes
                                          End Sub

            mainWindow.Show()

            Me.Hide() ' hide IntroWindow but keep it alive
        End Sub
    End Class
End Namespace

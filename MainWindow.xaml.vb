Imports CreoPublisherApp.CreoPublisherApp.ViewModels

Namespace CreoPublisherApp

    Partial Public Class MainWindow
        Inherits Window

        Public Sub New()
            InitializeComponent()
            DataContext = New MainViewModel()
        End Sub

        Private Sub DataGrid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

        End Sub


    End Class

End Namespace

Imports CreoPublisherApp.CreoPublisherApp.ViewModels

Namespace CreoPublisherApp

    Partial Public Class MainWindow
        Inherits Window

        Public Sub New()
            InitializeComponent()
            DataContext = New MainViewModel()
        End Sub
    End Class

End Namespace

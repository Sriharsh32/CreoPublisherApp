Imports System.Windows

Namespace CreoPublisherApp

    Class SettingsWindow
        Private Sub SettingsWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
            Dim vm = TryCast(DataContext, SettingsWindowViewModel)
            If vm IsNot Nothing Then
                AddHandler vm.RequestClose, Sub() Me.Close()
            End If
        End Sub
    End Class


End Namespace

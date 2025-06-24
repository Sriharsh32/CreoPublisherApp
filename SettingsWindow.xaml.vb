Imports System.Windows

Namespace CreoPublisherApp

    Partial Public Class SettingsWindow
        Inherits Window

        Private ReadOnly _vm As SettingsWindowViewModel

        Public Sub New()
            InitializeComponent()

            _vm = New SettingsWindowViewModel()
            Me.DataContext = _vm

            AddHandler _vm.RequestClose, Sub() Me.Close()
        End Sub

    End Class

End Namespace

Imports CreoPublisherApp.CreoPublisherApp

Class Application

    ' Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    ' can be handled in this file.

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        MyBase.OnStartup(e)
        Dim intro = New IntroWindow()
        intro.Show()
    End Sub



End Class

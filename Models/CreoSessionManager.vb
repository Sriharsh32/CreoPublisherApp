Imports pfcls

Public Class CreoSessionManager
    Private Shared _instance As CreoSessionManager

    Public Shared ReadOnly Property Instance As CreoSessionManager
        Get
            If _instance Is Nothing Then _instance = New CreoSessionManager()
            Return _instance
        End Get
    End Property

    Public Property Session As IpfcBaseSession

    Public Sub InitializeCreoSession()
        Try
            Dim conn As IpfcAsyncConnection = (New CCpfcAsyncConnection()).GetActiveConnection()
            Session = conn.Session
        Catch ex As Exception
            MessageBox.Show("Session initialization failed: " & ex.Message)
        End Try
    End Sub
End Class

Imports pfcls
Imports System.IO

Public Class OpenProeObjectClass
    Private _asyncConn As IpfcAsyncConnection

    Public Function RunProe(workingDir As String) As Boolean
        Try
            Dim proeExe = Environment.GetEnvironmentVariable("PROE_PATH")
            If String.IsNullOrEmpty(proeExe) OrElse Not File.Exists(proeExe) Then
                Throw New Exception("PROE_PATH environment variable is missing or invalid.")
            End If

            Dim launcher As CCpfcAsyncConnection = New CCpfcAsyncConnection()
            _asyncConn = launcher.Start(proeExe, workingDir) ' This already launches and connects to Creo

            ' DO NOT call _asyncConn.Connect(Nothing)

            Return True
        Catch ex As Exception
            MessageBox.Show("Launch Error: " & ex.Message)
            Return False
        End Try
    End Function

    Public Sub KillCreO()
        Try
            If _asyncConn IsNot Nothing Then
                _asyncConn.Disconnect(1)
                _asyncConn = Nothing
            End If
        Catch ex As Exception
            MessageBox.Show("Shutdown Error: " & ex.Message)
        End Try
    End Sub
End Class

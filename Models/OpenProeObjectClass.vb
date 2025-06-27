Imports pfcls
Imports System.IO
Imports System.Windows

Public Class OpenProeObjectClass
    Private _asyncConn As IpfcAsyncConnection

    ' Updated method using saved path
    Public Function RunProeWithPath(proeExePath As String, workingDir As String) As Boolean
        Try
            If Not File.Exists(proeExePath) Then
                MessageBox.Show("Creo executable not found.")
                Return False
            End If
            Environment.SetEnvironmentVariable("PROE_PATH", proeExePath, EnvironmentVariableTarget.User)
            Dim launcher As CCpfcAsyncConnection = New CCpfcAsyncConnection()
            _asyncConn = launcher.Start(proeExePath, workingDir)

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

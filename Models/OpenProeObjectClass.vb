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
        Dim proc As System.Diagnostics.Process
        Try
            For Each proc In System.Diagnostics.Process.GetProcessesByName("xtop")

                If proc.HasExited = False Then
                    If proc.Responding = False Then
                        proc.Kill()
                    End If
                    proc.Kill()
                End If
            Next
            For Each proc In System.Diagnostics.Process.GetProcessesByName("nmsd")
                If proc.HasExited = False Then
                    proc.Kill()
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub
End Class

Imports pfcls
Imports System.IO
Imports Microsoft.Win32 ' For OpenFileDialog
Imports System.Windows ' For MessageBox

Public Class OpenProeObjectClass
    Private _asyncConn As IpfcAsyncConnection

    Public Function RunProe(workingDir As String) As Boolean
        Try
            ' Prompt user to select parametric.exe
            Dim dlg As New OpenFileDialog()
            dlg.Title = "Select Creo Parametric Executable (e.g., C:\Program Files\PTC\Creo 2.0\Parametric\bin\parametric.exe)"
            dlg.Filter = "Creo Parametric Executable|parametric.exe"
            dlg.InitialDirectory = "C:\Program Files" ' Adjust if needed

            Dim selectedPath As String = ""

            If dlg.ShowDialog() = True Then
                selectedPath = dlg.FileName
            Else
                MessageBox.Show("Creo Parametric executable not selected.")
                Return False
            End If

            If Not File.Exists(selectedPath) Then
                MessageBox.Show("Invalid executable path.")
                Return False
            End If

            ' Optionally save the path as a user environment variable
            Environment.SetEnvironmentVariable("PROE_PATH", selectedPath, EnvironmentVariableTarget.User)

            ' Launch Creo
            Dim launcher As CCpfcAsyncConnection = New CCpfcAsyncConnection()
            _asyncConn = launcher.Start(selectedPath, workingDir)

            ' Do NOT call _asyncConn.Connect(Nothing)
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

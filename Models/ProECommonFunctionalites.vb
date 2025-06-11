Imports pfcls
Imports System.IO

Public Class ProECommonFunctionalites
    Public Sub ExportAllDrawingsAsPDF(inputFiles As List(Of String), outputFolder As String)
        Dim session = CreoSessionManager.Instance.Session

        For Each drwPath In inputFiles
            Try
                Dim name = Path.GetFileNameWithoutExtension(drwPath)
                Dim dir = Path.GetDirectoryName(drwPath)

                session.ChangeDirectory(dir)

                Dim desc = (New CCpfcModelDescriptor()).Create(EpfcModelType.EpfcMDL_DRAWING, name, Nothing)
                Dim model As IpfcModel = session.RetrieveModel(desc)

                If model Is Nothing Then
                    MessageBox.Show($"Failed to retrieve: {drwPath}")
                    Continue For
                End If

                Dim pdfInstr As IpfcPDFExportInstructions = (New CCpfcPDFExportInstructions()).Create()
                pdfInstr.FilePath = Path.Combine(outputFolder, name & ".pdf")

                model.Display() ' Required to avoid XToolkitNotDisplayed
                model.Export(pdfInstr.FilePath, pdfInstr)
            Catch ex As Exception
                MessageBox.Show($"Error exporting {drwPath}:" & vbCrLf & ex.Message)
            End Try
        Next
    End Sub
End Class

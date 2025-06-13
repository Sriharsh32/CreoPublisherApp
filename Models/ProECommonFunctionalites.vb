Imports pfcls
Imports System.IO
Imports System.Windows

Public Class ProECommonFunctionalites


    'for disable of ucs 
    Public Sub setConfigOption(configName As String, configValue As String)
        Dim creoSession As IpfcBaseSession = CreoSessionManager.Instance.Session
        creoSession.SetConfigOption(configName, configValue)

    End Sub
    Public Sub ExportAllDrawingsAsPDF(fileItems As List(Of FileItemModel), outputFolder As String)
        Dim session = CreoSessionManager.Instance.Session

        For Each item In fileItems.Where(Function(f) f.IsSelected)
            Try
                Dim name = Path.GetFileNameWithoutExtension(item.FullPath)
                Dim dir = Path.GetDirectoryName(item.FullPath)

                session.ChangeDirectory(dir)

                Dim desc = (New CCpfcModelDescriptor()).Create(EpfcModelType.EpfcMDL_DRAWING, name, Nothing)
                Dim model As IpfcModel = session.RetrieveModel(desc)

                If model Is Nothing Then
                    item.Status = "Failed to retrieve"
                    Continue For
                End If

                Dim pdfInstr As IpfcPDFExportInstructions = (New CCpfcPDFExportInstructions()).Create()
                pdfInstr.FilePath = Path.Combine(outputFolder, name & ".pdf")

                model.Display()
                model.Export(pdfInstr.FilePath, pdfInstr)

                item.Status = "Success"
            Catch ex As Exception
                item.Status = "Error: " & ex.Message
            End Try
        Next
    End Sub
End Class

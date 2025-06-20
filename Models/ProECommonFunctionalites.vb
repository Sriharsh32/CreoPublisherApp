Imports pfcls
Imports System.IO

Public Class ProECommonFunctionalites

    ' Set Creo config option
    Public Sub setConfigOption(configName As String, configValue As String)
        Dim creoSession As IpfcBaseSession = CreoSessionManager.Instance.Session
        creoSession.SetConfigOption(configName, configValue)
    End Sub

    ' Export all selected drawings with custom PDF names
    Public Sub ExportAllDrawingsAsPDF(fileItems As List(Of FileItemModel),
                                      outputFolder As String,
                                      leftMargin As String,
                                      topMargin As String,
                                      resolutions As List(Of String),
                                      paperSize As String,
                                      orientation As String)

        Dim session = CreoSessionManager.Instance.Session

        ' Optional: set resolution
        Dim dpi As String = If(resolutions.Count > 0, resolutions.Max(), "300")
        Try
            session.SetConfigOption("pdf_image_dpi", dpi)
        Catch ex As Exception
            ' Some Creo versions may not support this option
        End Try

        ' Set margins (if Creo version supports it)
        Try
            session.SetConfigOption("pdf_left_margin", leftMargin)
            session.SetConfigOption("pdf_top_margin", topMargin)
        Catch ex As Exception
            ' Older Creo versions (e.g., 2.0) may not support margin config options
        End Try

        ' Note: paperSize and orientation may also require custom plot config files

        For Each item In fileItems.Where(Function(f) f.IsSelected)
            Try
                Dim modelName = Path.GetFileNameWithoutExtension(item.FullPath)
                Dim directory = Path.GetDirectoryName(item.FullPath)

                session.ChangeDirectory(directory)

                Dim desc = (New CCpfcModelDescriptor()).Create(EpfcModelType.EpfcMDL_DRAWING, modelName, Nothing)
                Dim model As IpfcModel = session.RetrieveModel(desc)

                If model Is Nothing Then
                    item.Status = "Failed to retrieve"
                    Continue For
                End If

                ' Create basic PDF export instructions
                Dim pdfInstr As IpfcPDFExportInstructions = (New CCpfcPDFExportInstructions()).Create()

                ' Set output file name (custom if given)
                Dim customName = If(String.IsNullOrWhiteSpace(item.CustomPdfName), modelName, item.CustomPdfName)
                Dim pdfPath = Path.Combine(outputFolder, customName & ".pdf")
                pdfInstr.FilePath = pdfPath

                model.Display()
                model.Export(pdfPath, pdfInstr)

                item.Status = "Success"
            Catch ex As Exception
                item.Status = "Error: " & ex.Message
            End Try
        Next
    End Sub

End Class

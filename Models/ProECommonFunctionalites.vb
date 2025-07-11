﻿Imports pfcls
Imports System.IO

Public Class ProECommonFunctionalites

    ' Set Creo config option
    Public Sub setConfigOption(configName As String, configValue As String)
        Dim creoSession As IpfcBaseSession = CreoSessionManager.Instance.Session
        creoSession.SetConfigOption(configName, configValue)
    End Sub

    ' Export all selected drawings with custom PDF names
    Public Sub ExportAllDrawingsAsPDF(fileItems As List(Of FileItemModel),
                                      outputFolder As String)

        Dim session = CreoSessionManager.Instance.Session
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

                ' PDF options - don't launch PDF viewer
                Dim pdfOptions As IpfcPDFOptions = New CpfcPDFOptions()
                Dim pdfOpt1 As New CCpfcPDFOption
                Dim opt1 As IpfcPDFOption = pdfOpt1.Create()
                opt1.OptionType = EpfcPDFOptionType.EpfcPDFOPT_LAUNCH_VIEWER
                Dim arg1 As New CMpfcArgument
                opt1.OptionValue = arg1.CreateBoolArgValue(False)
                pdfOptions.Append(opt1)
                pdfInstr.Options = pdfOptions
                model.Display()
                model.Export(pdfPath, pdfInstr)
                ' Close the active window after export 
                Dim activeWindow As IpfcWindow = session.CurrentWindow
                If activeWindow IsNot Nothing Then
                    activeWindow.Close()
                End If
                ' Clear undisplayed models to free up resources
                session.EraseUndisplayedModels()
                item.Status = "Success"
            Catch ex As Exception
                item.Status = "Error: " & ex.Message
            End Try
        Next
    End Sub

End Class

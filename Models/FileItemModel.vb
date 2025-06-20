Imports System.IO
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class FileItemModel
    Implements INotifyPropertyChanged

    Private _isSelected As Boolean = True
    Private _status As String = "Pending"
    Private _customPdfName As String

    Public Property IsSelected As Boolean
        Get
            Return _isSelected
        End Get
        Set(value As Boolean)
            If _isSelected <> value Then
                _isSelected = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property FileName As String
    Public Property FullPath As String

    Public Property CustomPdfName As String
        Get
            If String.IsNullOrWhiteSpace(_customPdfName) Then
                ' Manual logic to remove extension
                If FileName IsNot Nothing AndAlso FileName.Contains(".") Then
                    Return FileName.Substring(0, FileName.LastIndexOf("."c))
                Else
                    Return FileName
                End If
            Else
                Return _customPdfName
            End If
        End Get
        Set(value As String)
            If _customPdfName <> value Then
                _customPdfName = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property CreatedDateRaw As DateTime
    Public Property ModifiedDateRaw As DateTime

    Public ReadOnly Property CreatedDate As String
        Get
            Return CreatedDateRaw.ToString("g")
        End Get
    End Property

    Public ReadOnly Property ModifiedDate As String
        Get
            Return ModifiedDateRaw.ToString("g")
        End Get
    End Property

    Public Property FileSize As String

    Public Property Status As String
        Get
            Return _status
        End Get
        Set(value As String)
            If _status <> value Then
                _status = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Sub New(path As String)
        Dim fi As New FileInfo(path)
        FileName = fi.Name
        FullPath = path
        CreatedDateRaw = fi.CreationTime
        ModifiedDateRaw = fi.LastWriteTime
        FileSize = $"{Math.Round(fi.Length / 1024.0, 2)} KB"

        ' Use safe manual filename stripping
        If FileName IsNot Nothing AndAlso FileName.Contains(".") Then
            _customPdfName = FileName.Substring(0, FileName.LastIndexOf("."c))
        Else
            _customPdfName = FileName
        End If
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

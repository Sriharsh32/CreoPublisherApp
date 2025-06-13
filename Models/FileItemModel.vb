Imports System.IO
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class FileItemModel
    Implements INotifyPropertyChanged

    Private _isSelected As Boolean = True
    Private _status As String = "Pending"

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
    Public Property CreatedDate As String
    Public Property ModifiedDate As String
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
        Dim fi = New FileInfo(path)
        FileName = fi.Name
        FullPath = path
        CreatedDate = fi.CreationTime.ToString("g")
        ModifiedDate = fi.LastWriteTime.ToString("g")
        FileSize = $"{Math.Round(fi.Length / 1024.0, 2)} KB"
    End Sub

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class

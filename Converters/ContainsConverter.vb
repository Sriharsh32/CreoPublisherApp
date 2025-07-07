
'Imports System.Globalization

'    Imports System.Windows.Data
'    Imports System.Collections.ObjectModel
'Namespace CreoPublisherApp
'    Public Class ContainsConverter
'        Implements IValueConverter

'        Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
'            Dim list = TryCast(value, ObservableCollection(Of String))
'            If list Is Nothing OrElse parameter Is Nothing Then Return False
'            Return list.Contains(parameter.ToString())
'        End Function

'        Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
'            Dim list = TryCast(value, ObservableCollection(Of String))
'            If list Is Nothing OrElse parameter Is Nothing Then Return Nothing

'            Dim val = parameter.ToString()
'            If CType(value, Boolean) Then
'                If Not list.Contains(val) Then list.Add(val)
'            Else
'                If list.Contains(val) Then list.Remove(val)
'            End If
'            Return list
'        End Function
'    End Class
'End Namespace

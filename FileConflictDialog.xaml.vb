Imports System.Windows

Namespace CreoPublisherApp
    Partial Public Class FileConflictDialog
        Inherits Window

        Public Enum ConflictResult
            Yes
            No
            YesToAll
            NoToAll
        End Enum

        Public Property UserChoice As ConflictResult = ConflictResult.No

        Public Sub New(fileName As String)
            InitializeComponent()

            MessageText.Text = $"The file '{fileName}' already exists in the output folder." & vbCrLf &
                               "Do you want to overwrite it?"

            ' Attach handlers
            AddHandler YesButton.Click, AddressOf Yes_Click
            AddHandler NoButton.Click, AddressOf No_Click
            AddHandler YesAllButton.Click, AddressOf YesAll_Click
            AddHandler NoAllButton.Click, AddressOf NoAll_Click
        End Sub

        Private Sub Yes_Click(sender As Object, e As RoutedEventArgs)
            UserChoice = ConflictResult.Yes
            Me.DialogResult = True
        End Sub

        Private Sub No_Click(sender As Object, e As RoutedEventArgs)
            UserChoice = ConflictResult.No
            Me.DialogResult = True
        End Sub

        Private Sub YesAll_Click(sender As Object, e As RoutedEventArgs)
            UserChoice = ConflictResult.YesToAll
            Me.DialogResult = True
        End Sub

        Private Sub NoAll_Click(sender As Object, e As RoutedEventArgs)
            UserChoice = ConflictResult.NoToAll
            Me.DialogResult = True
        End Sub
    End Class
End Namespace

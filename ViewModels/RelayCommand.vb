Imports System
Imports System.Windows.Input

''' <summary>
''' A command whose sole purpose is to relay its functionality 
''' to other objects by invoking delegates. The default return value for the CanExecute method is 'True'.
''' </summary>
Public Class RelayCommand
    Implements ICommand

    ''' <summary>
    ''' The action to execute when the command is invoked.
    ''' </summary>
    Private ReadOnly _execute As Action(Of Object)

    ''' <summary>
    ''' The function that determines whether the command can execute.
    ''' </summary>
    Private ReadOnly _canExecute As Func(Of Object, Boolean)

    ''' <summary>
    ''' Initializes a new instance of the <see cref="RelayCommand"/> class.
    ''' </summary>
    ''' <param name="execute">The execution logic.</param>
    ''' <param name="canExecute">The execution status logic.</param>
    Public Sub New(execute As Action(Of Object), Optional canExecute As Func(Of Object, Boolean) = Nothing)
        _execute = execute
        _canExecute = canExecute
    End Sub


    ''' Initializes a new instance of the <see cref="RelayCommand"/> class with a parameterless action.

    ''' <param name="execute">The execution logic.</param>
    Public Sub New(execute As Action)
        _execute = Sub(o) execute()
        _canExecute = Nothing
    End Sub


    ''' Determines whether the command can execute in its current state.

    ''' <param name="parameter">Data used by the command.</param>
    ''' <returns>true if this command can be executed; otherwise, false.</returns>
    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If _canExecute Is Nothing Then Return True
        Return _canExecute(parameter)
    End Function


    ''' Occurs when changes occur that affect whether or not the command should execute.
    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged


    ''' Raises the <see cref="CanExecuteChanged"/> event.

    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub


    ''' Executes the command.

    ''' <param name="parameter">Data used by the command.</param>
    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute(parameter)
    End Sub
End Class

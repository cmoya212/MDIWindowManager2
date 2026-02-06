Imports System.ComponentModel

'** Instantiate this in any UserControl Constructor (NEW)
'** Evan Aussenberg 2/24/2003
'** MailTo: JustMe @ EvanRon . Com

#Region " Example "

'Public Class UserControlBase
'    Inherits System.Windows.Forms.UserControl

'    '*** The Fixer class
'    Private Fixer As UserControlFixer

'    Public Sub New()
'        MyBase.New()

'        'This call is required by the Windows Form Designer.
'        MyBase.InitializeComponent()

'        'Add any initialization after the InitializeComponent() call
'        '** Initialize the Fixer Class
'        Fixer = New UserControlFixer(Me)
'    End Sub

'    'UserControl overrides dispose to clean up the component list.
'    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
'        If disposing Then

'            '** UserControlBase - Remember to Dispose
'            Fixer.Dispose()
'            Fixer = Nothing

'            If Not (components Is Nothing) Then
'                components.Dispose()
'            End If
'        End If
'        MyBase.Dispose(disposing)
'    End Sub
'End Class

#End Region

<Description("Fixes some Focus problems with User/ContainerControls!")> _
Friend Class SelectableUserControlFixer
    Implements IDisposable

    Private _ContainerControl As System.Windows.Forms.ContainerControl
    Private ActiveControl As Control

#Region " IDisposable "
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            RemoveHandler _ContainerControl.Leave, _
                AddressOf _ContainerControl_Leave
            RemoveHandler _ContainerControl.Enter, _
                AddressOf _ContainerControl_Enter
            _ContainerControl = Nothing
            ActiveControl = Nothing
        End If
        Debug.WriteLine("UserControlFixer Disposed.")
    End Sub

    Public Sub Dispose() Implements System.IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    '** Add Event Handlers for Enter/Leave of 
    '** ContainerControl (UserControl)
    Public Sub New(ByVal ContainerControl As _
            System.Windows.Forms.ContainerControl)
        _ContainerControl = _
              ContainerControl
        AddHandler _ContainerControl.Leave, _
              AddressOf _ContainerControl_Leave
        AddHandler _ContainerControl.Enter, _
              AddressOf _ContainerControl_Enter
    End Sub

    Public ReadOnly Property ContainerControl() As _
            System.Windows.Forms.ContainerControl
        Get
            Return _ContainerControl
        End Get
    End Property

    '** If the focus LEAVES the ContainerControl (UserControl) then
    '** track what the ActiveControl had been in the container.
    Private Sub _ContainerControl_Leave(ByVal sender As Object, _
            ByVal e As System.EventArgs)
        ActiveControl = _ContainerControl.ActiveControl
        _ContainerControl.ActiveControl = Nothing
    End Sub

    '** If the focus ENTERS the ContainerControl (UserControl) then
    '** re-enter the ex-ActiveControl, or find the first selectable 
    '** child control.
    Private Sub _ContainerControl_Enter(ByVal sender As Object, _
            ByVal e As System.EventArgs)
        Try
            If TypeOf _ContainerControl.Parent Is IContainerControl Then
                Dim ParentContainer As IContainerControl = _
                     CType(_ContainerControl.Parent, IContainerControl)
                ParentContainer.ActiveControl = _ContainerControl
                If Not ActiveControl Is Nothing Then
                    _ContainerControl.ActiveControl = ActiveControl
                ElseIf _ContainerControl.HasChildren Then
                    _ContainerControl.SelectNextControl _
                      (_ContainerControl.Controls(0), True, True, True, False)
                End If
            End If
        Catch
        End Try
    End Sub

End Class

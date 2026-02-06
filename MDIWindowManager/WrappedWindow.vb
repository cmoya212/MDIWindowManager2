''' <summary>
''' MDI Child Window Wrapper.
''' </summary>
''' <remarks>Used by WindowManagerPanel to
''' facilitate managing an MDI Child
''' window. This class controls the
''' appearance of the window and
''' intercepts events on behalf of the
''' child window.
''' </remarks>
Public Class WrappedWindow
    Implements IDisposable

    Public Event WindowActivated As EventHandler
    Public Event WindowDeactivate As EventHandler
    Public Event WindowClosing As System.Windows.Forms.FormClosingEventHandler
    Public Event WindowClosed As System.Windows.Forms.FormClosedEventHandler
    Public Event WindowEnter As EventHandler
    Public Event WindowLeave As EventHandler
    Public Event WindowTextChanged As EventHandler
    Public Event WindowVisibleChanged As EventHandler
    Public Event PopInRequested As System.ComponentModel.HandledEventHandler

    Private m_windowForm As Form
    Private WithEvents m_subclassedSystemMenu As SubclassedSystemMenu
    Private m_originalFormBorderStyle As FormBorderStyle
    Private m_originalBounds As Rectangle
    Private m_originalStartupPosition As FormStartPosition
    Private m_active As Boolean = False
    Private m_closing As Boolean = False
    Private m_closed As Boolean = False

    Public Sub New()

    End Sub

    Public Sub New(ByVal window As Form)

        Me.Window = window

    End Sub

    Public Overloads Sub Dispose() Implements System.IDisposable.Dispose

        Dispose(True)

    End Sub

    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)

        If disposing Then
            'save ref to window 
            Dim oldWindow As Form = Me.Window
            'disassociate wrapper from window
            Me.Window = Nothing
            'dispose of the window
            If Not oldWindow Is Nothing Then
                oldWindow.Dispose()
            End If

            GC.SuppressFinalize(Me)
        End If

    End Sub

    Protected Overrides Sub Finalize()

        Dispose(False)
        MyBase.Finalize()

    End Sub

    Public Property Window() As Form

        Get
            Return m_windowForm
        End Get

        Set(ByVal value As Form)
            If Not value Is m_windowForm Then
                If Not m_windowForm Is Nothing Then
                    RemoveHandler m_windowForm.Activated, AddressOf HandleWindowActivated
                    RemoveHandler m_windowForm.Deactivate, AddressOf HandleWindowDeactivate
                    RemoveHandler m_windowForm.FormClosing, AddressOf HandleWindowClosing
                    RemoveHandler m_windowForm.FormClosed, AddressOf HandleWindowClosed
                    RemoveHandler m_windowForm.Enter, AddressOf HandleWindowEnter
                    RemoveHandler m_windowForm.Leave, AddressOf HandleWindowLeave
                    RemoveHandler m_windowForm.TextChanged, AddressOf HandleTextChanged
                    RemoveHandler m_windowForm.VisibleChanged, AddressOf HandleWindowVisibleChanged
                End If

                m_windowForm = value

                If Not m_windowForm Is Nothing Then
                    m_originalFormBorderStyle = m_windowForm.FormBorderStyle
                    m_originalBounds = m_windowForm.Bounds
                    m_originalStartupPosition = m_windowForm.StartPosition
                    AdjustFormProperties(True)

                    AddHandler m_windowForm.Activated, AddressOf HandleWindowActivated
                    AddHandler m_windowForm.Deactivate, AddressOf HandleWindowDeactivate
                    AddHandler m_windowForm.FormClosed, AddressOf HandleWindowClosed
                    AddHandler m_windowForm.FormClosing, AddressOf HandleWindowClosing
                    AddHandler m_windowForm.Enter, AddressOf HandleWindowEnter
                    AddHandler m_windowForm.Leave, AddressOf HandleWindowLeave
                    AddHandler m_windowForm.TextChanged, AddressOf HandleTextChanged
                    AddHandler m_windowForm.VisibleChanged, AddressOf HandleWindowVisibleChanged
                End If
            End If
        End Set

    End Property

    Public ReadOnly Property IsClosing() As Boolean

        Get
            Return m_closing
        End Get

    End Property

    Public ReadOnly Property IsClosed() As Boolean

        Get
            Return m_closed
        End Get

    End Property

    Friend Sub InitCustomSystemMenu()

        RemoveCustomSystemMenu()

        Try
            m_subclassedSystemMenu = New SubclassedSystemMenu(m_windowForm.Handle.ToInt32, "Return to Application Workspace")
        Catch
            'do nothing
        End Try

    End Sub

    Friend Sub RemoveCustomSystemMenu()

        If Not m_subclassedSystemMenu Is Nothing Then
            m_subclassedSystemMenu.Dispose()
            m_subclassedSystemMenu = Nothing
        End If

    End Sub

    Public Sub AdjustFormProperties(ByVal asWrappedWindow As Boolean)

        If Not m_windowForm Is Nothing Then
            If asWrappedWindow Then
                m_windowForm.FormBorderStyle = FormBorderStyle.None
                m_windowForm.StartPosition = FormStartPosition.Manual
            Else
                m_windowForm.Refresh() 'these refresh calls fix bugs with controls not placed right by the framework on AutoScroll forms
                m_windowForm.Bounds = m_originalBounds
                m_windowForm.Refresh() 'these refresh calls fix bugs with controls not placed right by the framework on AutoScroll forms
                m_windowForm.FormBorderStyle = m_originalFormBorderStyle
                m_windowForm.StartPosition = m_originalStartupPosition
            End If
        End If

    End Sub

    Protected Overridable Sub OnWindowActivated(ByVal e As EventArgs)

        RaiseEvent WindowActivated(Me, e)

    End Sub

    Protected Overridable Sub OnWindowDeactivate(ByVal e As EventArgs)

        RaiseEvent WindowDeactivate(Me, e)

    End Sub

    Protected Overridable Sub OnWindowClosing(ByVal e As System.Windows.Forms.FormClosingEventArgs)

        RaiseEvent WindowClosing(Me, e)

    End Sub

    Protected Overridable Sub OnWindowClosed(ByVal e As System.Windows.Forms.FormClosedEventArgs)

        RaiseEvent WindowClosed(Me, e)

    End Sub

    Protected Overridable Sub OnWindowEnter(ByVal e As EventArgs)

        RaiseEvent WindowEnter(Me, e)

    End Sub

    Protected Overridable Sub OnWindowLeave(ByVal e As EventArgs)

        RaiseEvent WindowLeave(Me, e)

    End Sub

    Protected Overridable Sub OnWindowTextChanged(ByVal e As EventArgs)

        RaiseEvent WindowTextChanged(Me, e)

    End Sub

    Protected Overridable Sub OnWindowVisibleChanged(ByVal e As EventArgs)

        RaiseEvent WindowVisibleChanged(Me, e)

    End Sub

    Protected Overridable Sub OnPopInRequested(ByVal e As System.ComponentModel.HandledEventArgs)

        RaiseEvent PopInRequested(Me, e)

    End Sub

    Private Sub HandleWindowClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs)

        m_closing = True

        If Not e.Cancel Then
            OnWindowClosing(e)
        End If

        If e.Cancel Then
            m_closing = False
        End If

    End Sub

    Private Sub HandleWindowClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs)

        m_closed = True

        Try
            m_windowForm.Visible = False
        Catch ex As Exception
            Debug.WriteLine("Error during WindowHide On WindowClose " + ex.ToString())
        End Try

        OnWindowClosed(e)

        Me.Dispose()

    End Sub

    'Private Sub HandleWindowPaint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    '    'UNDONE: do we really want to draw directly on the child window?

    '    If Not m_bPopout Then
    '        'Dim frm As Form = CType(sender, Form)

    '        'Dim rectDraw As New Rectangle(0, 0, 0, 0)
    '        'Dim bAdjust As Boolean

    '        'If frm.Width <= frm.AutoScrollMinSize.Width Then
    '        '    rectDraw.Width = frm.AutoScrollMinSize.Width
    '        '    bAdjust = True
    '        'Else
    '        '    rectDraw.Width = frm.Width
    '        'End If

    '        'If frm.Height <= frm.AutoScrollMinSize.Height Then
    '        '    rectDraw.Height = frm.AutoScrollMinSize.Height
    '        '    bAdjust = True
    '        'Else
    '        '    rectDraw.Height = frm.Height
    '        'End If

    '        'e.Graphics.TranslateTransform(frm.AutoScrollPosition.X, frm.AutoScrollPosition.Y)

    '        'System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, rectDraw, SystemColors.Control, ButtonBorderStyle.Inset)

    '        Dim frm As Form = CType(sender, Form)
    '        Dim rectDraw As Rectangle = New Rectangle(0, 0, frm.AutoScrollMinSize.Width, frm.AutoScrollMinSize.Height)
    '        Dim mx As New System.Drawing.Drawing2D.Matrix(1, 0, 0, 1, frm.AutoScrollPosition.X, frm.AutoScrollPosition.Y)

    '        e.Graphics.Transform = mx

    '        System.Windows.Forms.ControlPaint.DrawBorder(e.Graphics, rectDraw, SystemColors.Control, ButtonBorderStyle.Inset)

    '        'If m_bActive Then
    '        '    Dim rect As Rectangle = rectDraw
    '        '    rect.Inflate(-3, -3)
    '        '    System.Windows.Forms.ControlPaint.DrawFocusRectangle(e.Graphics, rect)
    '        'End If


    '        Dim cr As New Rectangle(frm.AutoScrollPosition.X, frm.AutoScrollPosition.Y, 100, 100)


    '        'e.Graphics.TranslateTransform(frm.AutoScrollPosition.X, frm.AutoScrollPosition.Y)

    '        'e.Graphics.FillRectangle(HB, cr)
    '        System.Windows.Forms.ControlPaint.DrawFocusRectangle(e.Graphics, cr)


    '    End If

    'End Sub

    Private Sub HandleWindowActivated(ByVal sender As Object, ByVal e As System.EventArgs)

        m_active = True
        OnWindowActivated(e)
        m_windowForm.Refresh()

    End Sub

    Private Sub HandleWindowDeactivate(ByVal sender As Object, ByVal e As System.EventArgs)

        m_active = False
        OnWindowDeactivate(e)
        m_windowForm.Refresh()

    End Sub

    Private Sub HandleWindowEnter(ByVal sender As Object, ByVal e As System.EventArgs)

        m_active = True
        OnWindowEnter(e)
        m_windowForm.Refresh()

    End Sub

    Private Sub HandleWindowLeave(ByVal sender As Object, ByVal e As System.EventArgs)

        m_active = False
        OnWindowLeave(e)
        m_windowForm.Refresh()

    End Sub

    Private Sub HandleTextChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        OnWindowTextChanged(e)

    End Sub

    Private Sub HandleWindowVisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        OnWindowVisibleChanged(e)

    End Sub

    Private Sub m_subclassedSystemMenu_LaunchDialog() Handles m_subclassedSystemMenu.LaunchDialog

        Dim eventargs As New System.ComponentModel.HandledEventArgs(False)

        OnPopInRequested(eventargs)

        If Not eventargs.Handled Then
            MsgBox("The window could not be returned to the application. Its parent window may have been closed.", MsgBoxStyle.Exclamation, System.Reflection.Assembly.GetEntryAssembly.GetName().Name)
        End If

    End Sub

End Class
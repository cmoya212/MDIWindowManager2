''' <summary>
''' Splitter class for WindowManagerPanel.
''' </summary>
''' <remarks>Allows for user repositioning of the borderless WindowManagerPanel.</remarks>
Friend Class WindowManagerSplitter
    Inherits System.Windows.Forms.UserControl

    Public Event NewSize As EventHandler(Of NewSizeEventArgs)
    Public Event BeforeSize As EventHandler(Of BeforeSizeEventArgs)

    Public Enum SplitterStyle
        Horizontal
        Vertical
    End Enum

    Private m_active As Boolean
    Private m_lastRect As Rectangle
    Private m_cancel As Boolean
    Private m_splitterStyle As SplitterStyle = SplitterStyle.Horizontal

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'UserControl overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'WindowManagerSplitter
        '
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.Cursor = System.Windows.Forms.Cursors.HSplit
        Me.Name = "WindowManagerSplitter"
        Me.Size = New System.Drawing.Size(380, 8)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public Property Style() As SplitterStyle

        Get
            Return m_splitterStyle
        End Get

        Set(ByVal value As SplitterStyle)
            m_splitterStyle = value

            Select Case m_splitterStyle
                Case SplitterStyle.Horizontal
                    Me.Cursor = Cursors.HSplit
                Case SplitterStyle.Vertical
                    Me.Cursor = Cursors.VSplit
            End Select
        End Set

    End Property

    Protected Overridable Sub OnNewSize(ByVal e As NewSizeEventArgs)

        RaiseEvent NewSize(Me, e)

    End Sub

    Protected Overridable Sub OnBeforeSize(ByVal e As BeforeSizeEventArgs)

        RaiseEvent BeforeSize(Me, e)

    End Sub

    Private Sub WindowManagerSplitter_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown

        m_cancel = False

        m_lastRect = Me.Parent.RectangleToScreen(Me.Bounds)

        Dim eventargs As New BeforeSizeEventArgs(m_lastRect)

        OnBeforeSize(eventargs)

        m_lastRect = eventargs.Rectangle

        m_active = True

        System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

    End Sub

    Private Sub WindowManagerSplitter_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp

        If Not m_cancel Then
            m_active = False

            System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

            If Not Rectangle.op_Equality(Me.Parent.RectangleToScreen(Me.Bounds), m_lastRect) Then
                Dim eventargs As New NewSizeEventArgs(m_lastRect)

                m_lastRect = Parent.RectangleToScreen(Me.Bounds)

                Try
                    OnNewSize(eventargs)
                Catch
                    'do nothing
                End Try
            End If
        Else
            m_cancel = False
        End If

    End Sub

    Private Sub WindowManagerSplitter_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove

        If m_active Then
            System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

            Select Case m_splitterStyle
                Case WindowManagerSplitter.SplitterStyle.Horizontal
                    m_lastRect.Y = PointToScreen(New Point(0, e.Y)).Y
                Case WindowManagerSplitter.SplitterStyle.Vertical
                    m_lastRect.X = PointToScreen(New Point(e.X, 0)).X
            End Select

            System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)
        End If

    End Sub

    Private Sub WindowManagerSplitter_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.DoubleClick

        m_active = False

        System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

        m_cancel = True

    End Sub

    'Private Sub WindowManagerSplitter_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint

    '    With Me.ClientRectangle
    '        Select Case m_splitterStyle
    '            Case WindowManagerSplitterStyle.Horizontal
    '                e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLight, .Left, .Top, .Right - 1, .Top)
    '                e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDark, .Left, .Bottom - 2, .Right - 1, .Bottom - 2)
    '            Case WindowManagerSplitterStyle.Vertical
    '                e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLight, .Left, .Top, .Left, .Bottom)
    '                e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDark, .Right - 2, .Top, .Right - 2, .Bottom)
    '        End Select
    '    End With

    'End Sub

End Class



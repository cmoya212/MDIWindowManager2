''' <summary>
''' Splitter class for WindowTabStrips.
''' </summary>
''' <remarks>Used to resize multiple WindowTabStrips (tiled windows).</remarks>
Friend Class WrappedWindowSplitter
    Inherits System.Windows.Forms.UserControl

    Public Event NewSize As EventHandler(Of SplitterEventArgs)

    Private m_active As Boolean
    Private m_lastRect As Rectangle

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
        'WrappedWindowSplitter
        '
        Me.Cursor = System.Windows.Forms.Cursors.VSplit
        Me.Name = "WrappedWindowSplitter"
        Me.Size = New System.Drawing.Size(8, 224)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub WrappedWindowSplitter_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown

        m_lastRect = Me.Parent.RectangleToScreen(Me.Bounds)

        m_active = True

        System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

    End Sub

    Private Sub WrappedWindowSplitter_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseUp

        m_active = False

        System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

        Dim eventargs As New SplitterEventArgs(m_lastRect, Me.Parent.RectangleToScreen(Me.Bounds))

        m_lastRect = Me.Parent.RectangleToScreen(Me.Bounds)

        Try
            OnNewSize(eventargs)
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub WrappedWindowSplitter_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove

        If m_active Then
            System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)

            m_lastRect.X = Me.PointToScreen(New Point(e.X, 0)).X

            System.Windows.Forms.ControlPaint.FillReversibleRectangle(m_lastRect, SystemColors.Highlight)
        End If

    End Sub

    Private Sub WrappedWindowSplitter_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint

        With Me.ClientRectangle
            e.Graphics.DrawLine(System.Drawing.SystemPens.ControlLight, .Left, .Top, .Left, .Bottom)
            e.Graphics.DrawLine(System.Drawing.SystemPens.ControlDark, .Right - 1, .Top, .Right - 1, .Bottom)
        End With

    End Sub

    Protected Sub OnNewSize(ByVal e As SplitterEventArgs)

        RaiseEvent NewSize(Me, e)

    End Sub

End Class
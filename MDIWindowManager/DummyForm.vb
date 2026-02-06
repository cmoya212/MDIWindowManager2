''' <summary>
''' Blank flat form.
''' </summary>
''' <remarks>Used to create a blank area on the sides of wrapped windows where the MDI background shows through.</remarks>
Friend Class DummyForm
    Inherits System.Windows.Forms.Form

    Public Event ResizePending As EventHandler(Of BeforeSizeEventArgs)
    Public Event ResizeRequested As EventHandler(Of NewSizeEventArgs)

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
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
        Me.WindowManagerSplitter1 = New MDIWindowManager.WindowManagerSplitter
        Me.SuspendLayout()
        '
        'WindowManagerSplitter1
        '
        Me.WindowManagerSplitter1.BackColor = System.Drawing.SystemColors.Control
        Me.WindowManagerSplitter1.Cursor = System.Windows.Forms.Cursors.VSplit
        Me.WindowManagerSplitter1.Dock = System.Windows.Forms.DockStyle.Left
        Me.WindowManagerSplitter1.Location = New System.Drawing.Point(0, 0)
        Me.WindowManagerSplitter1.Name = "WindowManagerSplitter1"
        Me.WindowManagerSplitter1.Size = New System.Drawing.Size(4, 205)
        Me.WindowManagerSplitter1.Style = MDIWindowManager.WindowManagerSplitter.SplitterStyle.Vertical
        Me.WindowManagerSplitter1.TabIndex = 0
        '
        'DummyForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(292, 205)
        Me.ControlBox = False
        Me.Controls.Add(Me.WindowManagerSplitter1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Name = "DummyForm"
        Me.ShowInTaskbar = False
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents WindowManagerSplitter1 As MDIWindowManager.WindowManagerSplitter

#End Region

    Private Sub WindowManagerSplitter1_BeforeSize(ByVal sender As Object, ByVal e As BeforeSizeEventArgs) Handles WindowManagerSplitter1.BeforeSize

        OnResizePending(e)

    End Sub

    Private Sub WindowManagerSplitter1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles WindowManagerSplitter1.DoubleClick

        OnDoubleClick(e)

    End Sub

    Private Sub WindowManagerSplitter1_NewSize(ByVal sender As Object, ByVal e As NewSizeEventArgs) Handles WindowManagerSplitter1.NewSize

        OnResizeRequested(e)

    End Sub

    Protected Overridable Sub OnResizePending(ByVal e As BeforeSizeEventArgs)

        RaiseEvent ResizePending(Me, e)

    End Sub

    Protected Overridable Sub OnResizeRequested(ByVal e As NewSizeEventArgs)

        RaiseEvent ResizeRequested(Me, e)

    End Sub

    Private Sub DummyForm_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.BackColorChanged

        Me.WindowManagerSplitter1.BackColor = Me.BackColor

    End Sub

End Class

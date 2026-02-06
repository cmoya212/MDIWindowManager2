<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class StandardTabsProvider
    Inherits TabsProviderBase

    'Control overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            HideToolTip(Me)
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Control Designer
    Private components As System.ComponentModel.IContainer

    ' NOTE: The following procedure is required by the Component Designer
    ' It can be modified using the Component Designer.  Do not modify it
    ' using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim WrappedWindow1 As WrappedWindow = New WrappedWindow()
        DrawAreaPanel = New DrawPanel()
        CloseTabButton = New Button()
        ScrollLeftButton = New WindowManagerButton()
        ScrollRightButton = New WindowManagerButton()
        FocusKludgeButton = New Button()
        ScrollTimer = New Timer(components)
        ToolTip1 = New ToolTip(components)
        TableLayoutPanel1 = New TableLayoutPanel()
        SpacerPanel1 = New Panel()
        DrawAreaPanel.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' DrawAreaPanel
        ' 
        DrawAreaPanel.AllowDrop = True
        DrawAreaPanel.Controls.Add(CloseTabButton)
        DrawAreaPanel.Dock = DockStyle.Fill
        DrawAreaPanel.Location = New Point(0, 0)
        DrawAreaPanel.Margin = New Padding(0)
        DrawAreaPanel.Name = "DrawAreaPanel"
        DrawAreaPanel.Size = New Size(807, 59)
        DrawAreaPanel.TabIndex = 4
        ' 
        ' CloseTabButton
        ' 
        CloseTabButton.BackColor = Color.MistyRose
        CloseTabButton.Font = New Font("Marlett", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(2))
        CloseTabButton.Location = New Point(0, 9)
        CloseTabButton.Margin = New Padding(0)
        CloseTabButton.Name = "CloseTabButton"
        CloseTabButton.Size = New Size(36, 34)
        CloseTabButton.TabIndex = 0
        CloseTabButton.Text = "r"
        ToolTip1.SetToolTip(CloseTabButton, "Close")
        CloseTabButton.UseVisualStyleBackColor = False
        CloseTabButton.Visible = False
        ' 
        ' ScrollLeftButton
        ' 
        ScrollLeftButton.BorderStyle = BorderStyle.FixedSingle
        ScrollLeftButton.Dock = DockStyle.Fill
        ScrollLeftButton.Font = New Font("Marlett", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(2))
        ScrollLeftButton.Location = New Point(807, 0)
        ScrollLeftButton.Margin = New Padding(0)
        ScrollLeftButton.Name = "ScrollLeftButton"
        ScrollLeftButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Fancy
        ScrollLeftButton.Size = New Size(43, 59)
        ScrollLeftButton.TabIndex = 5
        ScrollLeftButton.Text = "3"
        ScrollLeftButton.TextAlign = ContentAlignment.TopCenter
        ' 
        ' ScrollRightButton
        ' 
        ScrollRightButton.BorderStyle = BorderStyle.FixedSingle
        ScrollRightButton.Dock = DockStyle.Fill
        ScrollRightButton.Font = New Font("Marlett", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(2))
        ScrollRightButton.Location = New Point(850, 0)
        ScrollRightButton.Margin = New Padding(0)
        ScrollRightButton.Name = "ScrollRightButton"
        ScrollRightButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.RollOverClassic
        ScrollRightButton.Size = New Size(43, 59)
        ScrollRightButton.TabIndex = 6
        ScrollRightButton.Text = "4"
        ScrollRightButton.TextAlign = ContentAlignment.TopCenter
        ' 
        ' FocusKludgeButton
        ' 
        FocusKludgeButton.Location = New Point(-24, -26)
        FocusKludgeButton.Margin = New Padding(6, 7, 6, 7)
        FocusKludgeButton.Name = "FocusKludgeButton"
        FocusKludgeButton.Size = New Size(22, 24)
        FocusKludgeButton.TabIndex = 7
        ' 
        ' ScrollTimer
        ' 
        ScrollTimer.Interval = 400
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.ColumnCount = 3
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 43F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 43F))
        TableLayoutPanel1.Controls.Add(DrawAreaPanel, 0, 0)
        TableLayoutPanel1.Controls.Add(ScrollLeftButton, 1, 0)
        TableLayoutPanel1.Controls.Add(ScrollRightButton, 2, 0)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 5)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 100F))
        TableLayoutPanel1.Size = New Size(893, 59)
        TableLayoutPanel1.TabIndex = 8
        ' 
        ' SpacerPanel1
        ' 
        SpacerPanel1.Dock = DockStyle.Top
        SpacerPanel1.Location = New Point(0, 0)
        SpacerPanel1.Name = "SpacerPanel1"
        SpacerPanel1.Size = New Size(893, 5)
        SpacerPanel1.TabIndex = 9
        ' 
        ' StandardTabsProvider
        ' 
        AutoScaleDimensions = New SizeF(13F, 31F)
        Controls.Add(TableLayoutPanel1)
        Controls.Add(FocusKludgeButton)
        Controls.Add(SpacerPanel1)
        Margin = New Padding(0)
        Name = "StandardTabsProvider"
        WrappedWindow1.Window = Nothing
        SelectedWrappedWindowItem = WrappedWindow1
        Size = New Size(893, 64)
        DrawAreaPanel.ResumeLayout(False)
        TableLayoutPanel1.ResumeLayout(False)
        ResumeLayout(False)

    End Sub
    Friend WithEvents DrawAreaPanel As MDIWindowManager.DrawPanel
    Friend WithEvents ScrollLeftButton As WindowManagerButton
    Friend WithEvents ScrollRightButton As WindowManagerButton
    Friend WithEvents FocusKludgeButton As System.Windows.Forms.Button
    Friend WithEvents ScrollTimer As System.Windows.Forms.Timer
    Friend WithEvents CloseTabButton As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents SpacerPanel1 As Panel

End Class


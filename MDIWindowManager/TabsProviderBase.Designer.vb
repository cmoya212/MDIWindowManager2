<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TabsProviderBase
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        WindowsMenu = New ContextMenuStrip(components)
        WindowsNoWindowsMenuItem = New ToolStripMenuItem()
        WindowsMenu.SuspendLayout()
        SuspendLayout()
        ' 
        ' WindowsMenu
        ' 
        WindowsMenu.ImageScalingSize = New Size(31, 31)
        WindowsMenu.Items.AddRange(New ToolStripItem() {WindowsNoWindowsMenuItem})
        WindowsMenu.Name = "WindowsMenu"
        WindowsMenu.Size = New Size(233, 42)
        ' 
        ' WindowsNoWindowsMenuItem
        ' 
        WindowsNoWindowsMenuItem.Enabled = False
        WindowsNoWindowsMenuItem.Name = "WindowsNoWindowsMenuItem"
        WindowsNoWindowsMenuItem.Size = New Size(232, 38)
        WindowsNoWindowsMenuItem.Text = "(No Windows)"
        ' 
        ' TabsProviderBase
        ' 
        AutoScaleDimensions = New SizeF(13F, 31F)
        AutoScaleMode = AutoScaleMode.Font
        Margin = New Padding(6, 7, 6, 7)
        Name = "TabsProviderBase"
        Size = New Size(893, 48)
        WindowsMenu.ResumeLayout(False)
        ResumeLayout(False)

    End Sub
    Friend WithEvents WindowsMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents WindowsNoWindowsMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class

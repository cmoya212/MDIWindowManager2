Friend Class WindowManagerPanelMinModeBar
    Inherits System.Windows.Forms.UserControl

    Public Event MinModeButtonClick As EventHandler

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
    Friend WithEvents MinModeButton As ButtonR
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.MinModeButton = New MDIWindowManager.ButtonR
        Me.SuspendLayout()
        '
        'MinModeButton
        '
        Me.MinModeButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.MinModeButton.Location = New System.Drawing.Point(2, 2)
        Me.MinModeButton.Name = "MinModeButton"
        Me.MinModeButton.RotatedText = ""
        Me.MinModeButton.Size = New System.Drawing.Size(25, 98)
        Me.MinModeButton.TabIndex = 1
        '
        'WindowManagerPanelMinModeBar
        '
        Me.Controls.Add(Me.MinModeButton)
        Me.Name = "WindowManagerPanelMinModeBar"
        Me.Size = New System.Drawing.Size(30, 332)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub MinModeButton_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles MinModeButton.MouseDown

        OnMinModeButtonClick(EventArgs.Empty)

    End Sub

    Protected Overridable Sub OnMinModeButtonClick(ByVal e As System.EventArgs)

        RaiseEvent MinModeButtonClick(Me, e)

    End Sub

End Class

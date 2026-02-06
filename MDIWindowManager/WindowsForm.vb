''' <summary>
'''     Window List Dialog.
''' </summary>
''' <remarks>
'''     Used by WindowManager to allow the
'''     the display of a "Windows List" that can be manipulated directly by 
'''     the end-user.
''' </remarks>
Friend Class WindowsForm
    Inherits System.Windows.Forms.Form

    Public Enum UserAction
        ActivateWindow
        CloseWindows
    End Enum

    Public UserActionResult As UserAction

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
    Friend WithEvents ActivateWindowButton As System.Windows.Forms.Button
    Friend WithEvents CloseWindowsButton As System.Windows.Forms.Button
    Friend WithEvents CloseButton As System.Windows.Forms.Button
    Friend WithEvents WindowsListView As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.WindowsListView = New System.Windows.Forms.ListView
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ActivateWindowButton = New System.Windows.Forms.Button
        Me.CloseWindowsButton = New System.Windows.Forms.Button
        Me.CloseButton = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'WindowsListView
        '
        Me.WindowsListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.WindowsListView.FullRowSelect = True
        Me.WindowsListView.HideSelection = False
        Me.WindowsListView.Location = New System.Drawing.Point(4, 3)
        Me.WindowsListView.Name = "WindowsListView"
        Me.WindowsListView.Size = New System.Drawing.Size(275, 201)
        Me.WindowsListView.TabIndex = 0
        Me.WindowsListView.UseCompatibleStateImageBehavior = False
        Me.WindowsListView.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Name"
        Me.ColumnHeader1.Width = 250
        '
        'ActivateWindowButton
        '
        Me.ActivateWindowButton.Location = New System.Drawing.Point(285, 6)
        Me.ActivateWindowButton.Name = "ActivateWindowButton"
        Me.ActivateWindowButton.Size = New System.Drawing.Size(105, 24)
        Me.ActivateWindowButton.TabIndex = 1
        Me.ActivateWindowButton.Text = "Activate Window"
        '
        'CloseWindowsButton
        '
        Me.CloseWindowsButton.Location = New System.Drawing.Point(285, 36)
        Me.CloseWindowsButton.Name = "CloseWindowsButton"
        Me.CloseWindowsButton.Size = New System.Drawing.Size(105, 24)
        Me.CloseWindowsButton.TabIndex = 2
        Me.CloseWindowsButton.Text = "Close Window(s)"
        '
        'CloseButton
        '
        Me.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.CloseButton.Location = New System.Drawing.Point(285, 180)
        Me.CloseButton.Name = "CloseButton"
        Me.CloseButton.Size = New System.Drawing.Size(105, 24)
        Me.CloseButton.TabIndex = 3
        Me.CloseButton.Text = "OK"
        '
        'WindowsForm
        '
        Me.AcceptButton = Me.ActivateWindowButton
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.CancelButton = Me.CloseButton
        Me.ClientSize = New System.Drawing.Size(394, 210)
        Me.Controls.Add(Me.CloseButton)
        Me.Controls.Add(Me.CloseWindowsButton)
        Me.Controls.Add(Me.ActivateWindowButton)
        Me.Controls.Add(Me.WindowsListView)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "WindowsForm"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Windows"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Public ReadOnly Property WindowList() As ListView

        Get
            Return Me.WindowsListView
        End Get

    End Property

    Public Sub LoadWindowsList(ByVal wrappedWindows As WrappedWindowCollection)

        For Each wrappedWindow As WrappedWindow In wrappedWindows
            Dim item As New System.Windows.Forms.ListViewItem

            item.Text = wrappedWindow.Window.Text
            item.Tag = wrappedWindow

            Me.WindowsListView.Items.Add(item)
        Next wrappedWindow

    End Sub

    Private Sub WindowsListView_ColumnClick(ByVal sender As Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles WindowsListView.ColumnClick

        If Me.WindowsListView.Sorting = SortOrder.None Or Me.WindowsListView.Sorting = SortOrder.Descending Then
            Me.WindowsListView.Sorting = SortOrder.Ascending
        ElseIf Me.WindowsListView.Sorting = SortOrder.Ascending Then
            Me.WindowsListView.Sorting = SortOrder.Descending
        End If

        Me.WindowsListView.Sort()

    End Sub

    Private Sub ActivateWindowButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ActivateWindowButton.Click, WindowsListView.DoubleClick

        Me.UserActionResult = UserAction.ActivateWindow
        Me.DialogResult = DialogResult.OK

    End Sub

    Private Sub CloseWindowsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseWindowsButton.Click

        Me.UserActionResult = UserAction.CloseWindows
        Me.DialogResult = DialogResult.OK

    End Sub

End Class
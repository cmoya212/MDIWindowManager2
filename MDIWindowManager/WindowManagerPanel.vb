Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Reflection
Imports System.Linq

''' <summary>
''' Provides Tabbed MDI management for MDI Parent windows.
''' </summary>
''' <remarks></remarks>
<ToolboxItemAttribute(True), _
Description("Provides Tabbed MDI management for MDI Parent windows."), _
ToolboxBitmap(GetType(WindowManagerPanel))> _
Public Class WindowManagerPanel
    Inherits System.Windows.Forms.UserControl

#Region "Events"

    Private Declare Function PostMessage Lib "user32" Alias "PostMessageA" (ByVal hwnd As IntPtr, ByVal wMsg As Integer, ByVal wParam As Integer, ByVal lParam As String) As Integer

    Public Event WindowTiling As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowUnTiling As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowHTiling As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowPoppingOut As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowPoppingIn As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowPoppedOut As EventHandler(Of WrappedWindowEventArgs)
    Public Event WindowPoppedIn As EventHandler(Of WrappedWindowEventArgs)

    Public Event BeforeWrappedWindowAdded As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WrappedWindowAdded As EventHandler(Of WrappedWindowEventArgs)
    Public Event BeforeWrappedWindowRemoved As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WrappedWindowRemoved As EventHandler(Of WrappedWindowEventArgs)
    Public Event WrappedWindowsCleared As EventHandler
    Public Event WindowClosing As EventHandler(Of WrappedWindowClosingEventArgs)
    Public Event WindowClosed As EventHandler(Of WrappedWindowClosedEventArgs)
    Public Event WindowActivated As EventHandler(Of WrappedWindowEventArgs)
    Public Event WindowDeactivate As EventHandler(Of WrappedWindowEventArgs)
    Public Event WindowEnter As EventHandler(Of WrappedWindowEventArgs)
    Public Event WindowLeave As EventHandler(Of WrappedWindowEventArgs)
    Public Event WindowTextChanged As EventHandler(Of WrappedWindowEventArgs)
    Public Event WindowVisibleChanged As EventHandler(Of WrappedWindowEventArgs)

    Public Event NewCustomTabsProviderInstance As EventHandler(Of NewTabsProviderInstanceCreatedEventArgs)
    Public Event TabPaint As EventHandler(Of StandardTabsProvider.TabPaintEventArgs)

    Friend Event TempPanelDismissed As EventHandler
    Friend Event RequestChainRedoLayout As EventHandler(Of RedoLayoutEventArgs)
    Friend Event RequestChainGetTop As EventHandler(Of GetTopEventArgs)
    Friend Event RequestChainGetTopPanel As EventHandler(Of GetTopPanelEventArgs)
    Friend Event RequestMinModeChange As EventHandler(Of RequestMinModeChangeEventArgs)
    Friend Event IsActiveChanged As EventHandler

#End Region

#Region "Private Variables"

    Private m_parentForm As Form
    Private m_isLoaded As Boolean = False
    Private m_isUnloaded As Boolean = False
    Private m_style As TabStyle = TabStyle.AngledTabs
    Private m_enableAeroGlass As Boolean = False
    Private m_showIcons As Boolean = True
    Private m_orientation As WindowManagerOrientation = WindowManagerOrientation.Top
    Private m_allowUserVerticalRepositioning As Boolean = False
    Private m_autoHide As Boolean = False
    Private m_inAutoHideMode As Boolean = False
    Private m_sizeValueBeforeAutoHide As Integer
    Private m_minMode As Boolean = False
    Private m_topValueBeforeMinMode As Integer
    Private m_tabRenderMode As TabsProvider = TabsProvider.Standard
    Private m_buttonRenderMode As ButtonRenderMode = ButtonRenderMode.Standard
    Private m_customTabsProviderType As Type
    Private m_enableTabPaintEvent As Boolean = False
    Private m_disableCloseAction As Boolean
    Private m_disableTileAction As Boolean
    Private m_disableHTileAction As Boolean
    Private m_disablePopoutAction As Boolean
    Private m_tabForeColor As Color = SystemColors.ControlText
    Private m_tabBackColor As Color = SystemColors.Control
    Private m_selectedTabForeColor As Color = SystemColors.ControlText
    Private m_selectedTabBackColor As Color = SystemColors.Control
    Private m_emphasizeSelectedTab As Boolean = True
    Private m_wordWrap As Boolean = False

    Private WithEvents m_windowTabStrips As New WindowTabStripCollection
    Private m_suppressTabStripRemoveAbort As Boolean = False

    Private WithEvents m_poppedOutWindowsManager As PoppedOutWindowsManager = g_poppedOutWindowsManager

    Private WithEvents m_mdiClientControl As MdiClient
    Private m_originalMDIClientRect As Rectangle

    Private WithEvents m_blankAreaLeft As DummyForm
    Private WithEvents m_blankAreaRight As DummyForm
    Private WithEvents m_minModeBar As WindowManagerPanelMinModeBar

    Private WithEvents m_auxiliaryWindow As Form
    Private m_auxiliaryWindowOrigBorderStyle As System.Windows.Forms.FormBorderStyle
    Private m_auxiliaryWindowOrigSize As Size

    Private m_isActive As Boolean = False
    Private m_selectedTabStrip As WindowTabStrip

    Private m_autoDetectMdiChildWindows As Boolean = True

    'Private m_tmpOriginalBackColorCache1 As Color
    'Private m_tmpOriginalForeColorCache1 As Color
    'Private m_tmpOriginalBackColorCache2 As Color
    'Private m_tmpOriginalForeColorCache2 As Color

    Private WithEvents m_nextWindowManagerPanel As WindowManagerPanel
    Friend WithEvents FocusIndicatorPanel As System.Windows.Forms.Panel
    Private m_isTemporaryWindowManagerPanel As Boolean

    Private m_historyBack As New List(Of WrappedWindow)
    Private m_historyForward As New List(Of WrappedWindow)
    Friend WithEvents WindowManageButton As Button
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator

    Private WithEvents m_mdiWindowListItem As ToolStripMenuItem
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel

#If DEBUG Then
    Private m_showFocusIndicator As Boolean = False ' for debugging purposes... might be exposed as a feature someday
#Else
    Private m_showFocusIndicator As Boolean = False 
#End If

#End Region

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        InitializePrimaryTabStrip()

    End Sub

    'UserControl1 overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            HideToolTip(Me)
            RemoveParentHandlers()
            m_poppedOutWindowsManager = Nothing
            m_nextWindowManagerPanel = Nothing
            m_windowTabStrips = Nothing
            m_mdiClientControl = Nothing
            m_parentForm = Nothing

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
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents TabStripsContainer As System.Windows.Forms.Panel
    Friend WithEvents WindowManagerSplitter1 As MDIWindowManager.WindowManagerSplitter
    Friend WithEvents WindowManagerSplitter2 As MDIWindowManager.WindowManagerSplitter
    Friend WithEvents WindowButtonsContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents WindowButtonsToggleWindowButtonsMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents WindowContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents WindowHTileMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WindowTileWindowMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WindowPopoutWindowMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents WindowCloseMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleOptionsHTileMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleOptionsTileWindowMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleOptionsPopoutWindowMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleCloseWindowMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleNoWindowsMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TitleMinimizeMenuItem As System.Windows.Forms.ToolStripMenuItem

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        components = New Container()
        ToolTip1 = New ToolTip(components)
        TabStripsContainer = New Panel()
        TitleContextMenu = New ContextMenuStrip(components)
        TitleMinimizeMenuItem = New ToolStripMenuItem()
        ToolStripSeparator2 = New ToolStripSeparator()
        TitleOptionsHTileMenuItem = New ToolStripMenuItem()
        TitleOptionsTileWindowMenuItem = New ToolStripMenuItem()
        TitleOptionsPopoutWindowMenuItem = New ToolStripMenuItem()
        ToolStripSeparator3 = New ToolStripSeparator()
        TitleCloseWindowMenuItem = New ToolStripMenuItem()
        ToolStripSeparator4 = New ToolStripSeparator()
        TitleNoWindowsMenuItem = New ToolStripMenuItem()
        WindowButtonsContextMenu = New ContextMenuStrip(components)
        WindowButtonsToggleWindowButtonsMenuItem = New ToolStripMenuItem()
        WindowContextMenu = New ContextMenuStrip(components)
        WindowHTileMenuItem = New ToolStripMenuItem()
        WindowTileWindowMenuItem = New ToolStripMenuItem()
        WindowPopoutWindowMenuItem = New ToolStripMenuItem()
        ToolStripSeparator1 = New ToolStripSeparator()
        WindowCloseMenuItem = New ToolStripMenuItem()
        FocusIndicatorPanel = New Panel()
        WindowManagerSplitter2 = New WindowManagerSplitter()
        WindowManagerSplitter1 = New WindowManagerSplitter()
        WindowManageButton = New Button()
        TableLayoutPanel1 = New TableLayoutPanel()
        TitleContextMenu.SuspendLayout()
        WindowButtonsContextMenu.SuspendLayout()
        WindowContextMenu.SuspendLayout()
        TableLayoutPanel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' TabStripsContainer
        ' 
        TabStripsContainer.Dock = DockStyle.Fill
        TabStripsContainer.Location = New Point(0, 0)
        TabStripsContainer.Margin = New Padding(0)
        TabStripsContainer.Name = "TabStripsContainer"
        TabStripsContainer.Size = New Size(850, 52)
        TabStripsContainer.TabIndex = 9
        ' 
        ' TitleContextMenu
        ' 
        TitleContextMenu.ImageScalingSize = New Size(31, 31)
        TitleContextMenu.Items.AddRange(New ToolStripItem() {TitleMinimizeMenuItem, ToolStripSeparator2, TitleOptionsHTileMenuItem, TitleOptionsTileWindowMenuItem, TitleOptionsPopoutWindowMenuItem, ToolStripSeparator3, TitleCloseWindowMenuItem, ToolStripSeparator4, TitleNoWindowsMenuItem})
        TitleContextMenu.Name = "TitleContextMenu"
        TitleContextMenu.Size = New Size(391, 250)
        ' 
        ' TitleMinimizeMenuItem
        ' 
        TitleMinimizeMenuItem.Name = "TitleMinimizeMenuItem"
        TitleMinimizeMenuItem.Size = New Size(390, 38)
        TitleMinimizeMenuItem.Text = "Mi&nimize"
        ' 
        ' ToolStripSeparator2
        ' 
        ToolStripSeparator2.Name = "ToolStripSeparator2"
        ToolStripSeparator2.Size = New Size(387, 6)
        ' 
        ' TitleOptionsHTileMenuItem
        ' 
        TitleOptionsHTileMenuItem.Name = "TitleOptionsHTileMenuItem"
        TitleOptionsHTileMenuItem.Size = New Size(390, 38)
        TitleOptionsHTileMenuItem.Text = "New &Horizontal Tab Group"
        ' 
        ' TitleOptionsTileWindowMenuItem
        ' 
        TitleOptionsTileWindowMenuItem.Name = "TitleOptionsTileWindowMenuItem"
        TitleOptionsTileWindowMenuItem.Size = New Size(390, 38)
        TitleOptionsTileWindowMenuItem.Text = "&Tile or Untile Current Window"
        ' 
        ' TitleOptionsPopoutWindowMenuItem
        ' 
        TitleOptionsPopoutWindowMenuItem.Name = "TitleOptionsPopoutWindowMenuItem"
        TitleOptionsPopoutWindowMenuItem.Size = New Size(390, 38)
        TitleOptionsPopoutWindowMenuItem.Text = "&Pop Out Current Window"
        ' 
        ' ToolStripSeparator3
        ' 
        ToolStripSeparator3.Name = "ToolStripSeparator3"
        ToolStripSeparator3.Size = New Size(387, 6)
        ' 
        ' TitleCloseWindowMenuItem
        ' 
        TitleCloseWindowMenuItem.Name = "TitleCloseWindowMenuItem"
        TitleCloseWindowMenuItem.Size = New Size(390, 38)
        TitleCloseWindowMenuItem.Text = "&Close Current Window"
        ' 
        ' ToolStripSeparator4
        ' 
        ToolStripSeparator4.Name = "ToolStripSeparator4"
        ToolStripSeparator4.Size = New Size(387, 6)
        ' 
        ' TitleNoWindowsMenuItem
        ' 
        TitleNoWindowsMenuItem.Enabled = False
        TitleNoWindowsMenuItem.Name = "TitleNoWindowsMenuItem"
        TitleNoWindowsMenuItem.Size = New Size(390, 38)
        TitleNoWindowsMenuItem.Text = "(No Windows)"
        ' 
        ' WindowButtonsContextMenu
        ' 
        WindowButtonsContextMenu.ImageScalingSize = New Size(31, 31)
        WindowButtonsContextMenu.Items.AddRange(New ToolStripItem() {WindowButtonsToggleWindowButtonsMenuItem})
        WindowButtonsContextMenu.Name = "WindowButtonsContextMenu"
        WindowButtonsContextMenu.Size = New Size(311, 42)
        ' 
        ' WindowButtonsToggleWindowButtonsMenuItem
        ' 
        WindowButtonsToggleWindowButtonsMenuItem.Name = "WindowButtonsToggleWindowButtonsMenuItem"
        WindowButtonsToggleWindowButtonsMenuItem.Size = New Size(310, 38)
        WindowButtonsToggleWindowButtonsMenuItem.Text = "Hide Window &Buttons"
        ' 
        ' WindowContextMenu
        ' 
        WindowContextMenu.ImageScalingSize = New Size(31, 31)
        WindowContextMenu.Items.AddRange(New ToolStripItem() {WindowHTileMenuItem, WindowTileWindowMenuItem, WindowPopoutWindowMenuItem, ToolStripSeparator1, WindowCloseMenuItem})
        WindowContextMenu.Name = "WindowContextMenu"
        WindowContextMenu.Size = New Size(357, 162)
        ' 
        ' WindowHTileMenuItem
        ' 
        WindowHTileMenuItem.Name = "WindowHTileMenuItem"
        WindowHTileMenuItem.Size = New Size(356, 38)
        WindowHTileMenuItem.Text = "New &Horizontal Tab Group"
        ' 
        ' WindowTileWindowMenuItem
        ' 
        WindowTileWindowMenuItem.Name = "WindowTileWindowMenuItem"
        WindowTileWindowMenuItem.Size = New Size(356, 38)
        WindowTileWindowMenuItem.Text = "&Tile or Untile"
        ' 
        ' WindowPopoutWindowMenuItem
        ' 
        WindowPopoutWindowMenuItem.Name = "WindowPopoutWindowMenuItem"
        WindowPopoutWindowMenuItem.Size = New Size(356, 38)
        WindowPopoutWindowMenuItem.Text = "&Pop Out"
        ' 
        ' ToolStripSeparator1
        ' 
        ToolStripSeparator1.Name = "ToolStripSeparator1"
        ToolStripSeparator1.Size = New Size(353, 6)
        ' 
        ' WindowCloseMenuItem
        ' 
        WindowCloseMenuItem.Name = "WindowCloseMenuItem"
        WindowCloseMenuItem.Size = New Size(356, 38)
        WindowCloseMenuItem.Text = "&Close"
        ' 
        ' FocusIndicatorPanel
        ' 
        FocusIndicatorPanel.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left
        FocusIndicatorPanel.Location = New Point(0, 0)
        FocusIndicatorPanel.Name = "FocusIndicatorPanel"
        FocusIndicatorPanel.Size = New Size(4, 52)
        FocusIndicatorPanel.TabIndex = 16
        ' 
        ' WindowManagerSplitter2
        ' 
        WindowManagerSplitter2.BackColor = SystemColors.Control
        WindowManagerSplitter2.Cursor = Cursors.VSplit
        WindowManagerSplitter2.Location = New Point(0, 4)
        WindowManagerSplitter2.Name = "WindowManagerSplitter2"
        WindowManagerSplitter2.Size = New Size(4, 38)
        WindowManagerSplitter2.Style = WindowManagerSplitter.SplitterStyle.Vertical
        WindowManagerSplitter2.TabIndex = 12
        ' 
        ' WindowManagerSplitter1
        ' 
        WindowManagerSplitter1.BackColor = SystemColors.Control
        WindowManagerSplitter1.Cursor = Cursors.HSplit
        WindowManagerSplitter1.Dock = DockStyle.Top
        WindowManagerSplitter1.Location = New Point(0, 0)
        WindowManagerSplitter1.Name = "WindowManagerSplitter1"
        WindowManagerSplitter1.Size = New Size(886, 4)
        WindowManagerSplitter1.Style = WindowManagerSplitter.SplitterStyle.Horizontal
        WindowManagerSplitter1.TabIndex = 10
        ' 
        ' WindowManageButton
        ' 
        WindowManageButton.Dock = DockStyle.Fill
        WindowManageButton.FlatAppearance.BorderColor = SystemColors.Control
        WindowManageButton.FlatAppearance.BorderSize = 0
        WindowManageButton.FlatAppearance.MouseOverBackColor = Color.Silver
        WindowManageButton.Font = New Font("Marlett", 10.173913F, FontStyle.Bold, GraphicsUnit.Point, CByte(2))
        WindowManageButton.Location = New Point(851, 1)
        WindowManageButton.Margin = New Padding(1)
        WindowManageButton.Name = "WindowManageButton"
        WindowManageButton.Padding = New Padding(4, 0, 0, 0)
        WindowManageButton.Size = New Size(34, 50)
        WindowManageButton.TabIndex = 17
        WindowManageButton.Text = "7"
        WindowManageButton.TextAlign = ContentAlignment.BottomCenter
        WindowManageButton.UseVisualStyleBackColor = True
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.ColumnCount = 2
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 36.0F))
        TableLayoutPanel1.Controls.Add(WindowManageButton, 1, 0)
        TableLayoutPanel1.Controls.Add(TabStripsContainer, 0, 0)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 4)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 1
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        TableLayoutPanel1.Size = New Size(886, 52)
        TableLayoutPanel1.TabIndex = 18
        ' 
        ' WindowManagerPanel
        ' 
        AutoScaleDimensions = New SizeF(13.0F, 31.0F)
        AutoScaleMode = AutoScaleMode.Font
        Controls.Add(TableLayoutPanel1)
        Controls.Add(WindowManagerSplitter2)
        Controls.Add(WindowManagerSplitter1)
        Controls.Add(FocusIndicatorPanel)
        Margin = New Padding(0)
        Name = "WindowManagerPanel"
        Size = New Size(886, 56)
        TitleContextMenu.ResumeLayout(False)
        WindowButtonsContextMenu.ResumeLayout(False)
        WindowContextMenu.ResumeLayout(False)
        TableLayoutPanel1.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property IsDesignMode() As Boolean
        Get
            Return Me.DesignMode
        End Get
    End Property

    ''' <summary>
    ''' Default size of the panel.
    ''' </summary>
    Protected Overrides ReadOnly Property DefaultSize() As Size

        Get
            Return New Size(500, 53)
        End Get

    End Property

    ''' <summary>
    ''' Not supported.
    ''' </summary>
    <Category("Layout"),
    Description("Not supported."),
    Browsable(True),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Overrides Property Anchor() As System.Windows.Forms.AnchorStyles

        Get
            Return MyBase.Anchor
        End Get

        Set(ByVal value As System.Windows.Forms.AnchorStyles)
            If Not value = AnchorStyles.None Then
                Throw New System.Exception("Invalid property value. Anchor property is not supported or necessary.")
            Else
                MyBase.Anchor = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Not supported (See Orientation property).
    ''' </summary>
    <Category("Layout"),
    Description("Not supported (See Orientation property)."),
    Browsable(True),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)>
    Public Overrides Property Dock() As System.Windows.Forms.DockStyle

        Get
            Return MyBase.Dock
        End Get

        Set(ByVal value As System.Windows.Forms.DockStyle)
            If Not value = DockStyle.None Then
                Throw New System.Exception("Invalid property value. Please use the 'Orientation' property instead.")
            Else
                MyBase.Dock = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Placement of the panel on the MDI parent form.
    ''' </summary>
    <Category("Layout"),
    Description("Placement of the panel on the MDI parent form."),
    Browsable(True)>
    Public Property Orientation() As WindowManagerOrientation

        Get
            Return m_orientation
        End Get

        Set(ByVal value As WindowManagerOrientation)

            If Me.DesignMode AndAlso value = WindowManagerOrientation.Top AndAlso Me.DesignMode AndAlso Not Me.AuxiliaryWindow Is Nothing Then
                'I don't think we really need to enforce this
                'Throw New System.Exception("Orientiation.Top is not allowed when an AuxiliaryWindow is set.")
            Else
                Me.MinMode = False

                m_orientation = value

                'I don't think we really need to enforce this
                'If m_orientation = WindowManagerOrientation.Top Then
                '    Me.AllowUserVerticalRepositioning = False
                'End If

                If Me.DesignMode Then
                    If m_orientation = WindowManagerOrientation.Top OrElse m_orientation = WindowManagerOrientation.Left OrElse m_orientation = WindowManagerOrientation.Right Then
                        Me.AllowUserVerticalRepositioning = False
                    Else
                        Me.AllowUserVerticalRepositioning = True
                    End If
                End If

                SnapToSize()

                If Me.DesignMode Then
                    SnapToDesignSize()
                End If

                WindowManagerSplitter2.Visible = (m_orientation = WindowManagerOrientation.Left Or m_orientation = WindowManagerOrientation.Right)

                Select Case m_orientation
                    Case WindowManagerOrientation.Left
                        WindowManagerSplitter2.Dock = DockStyle.Right
                    Case WindowManagerOrientation.Right
                        WindowManagerSplitter2.Dock = DockStyle.Left
                End Select

                If Not Me.NextSubPanel Is Nothing Then
                    Me.NextSubPanel.Orientation = m_orientation
                    CopyAppearance(Me.NextSubPanel)
                End If
            End If

        End Set

    End Property

    ''' <summary>
    ''' Enable see-through Aero Glass effect on Vista and higher.
    ''' </summary>
    <Category("Appearance"),
    Description("Enable see-through Aero Glass effect on Vista and higher."),
    Browsable(False)>
    Public Property EnableAuroGlassEffect As Boolean
        Get
            Return m_enableAeroGlass
        End Get
        Set(value As Boolean)
            m_enableAeroGlass = value

            Try
                LayoutTabStrips()

                If Not m_enableAeroGlass Then
                    'Me.TileWindowButton.BackColor = Nothing
                    'Me.HTileButton.BackColor = Nothing
                    'Me.PopoutWindowButton.BackColor = Nothing
                    'Me.CloseWindowButton.BackColor = Nothing
                Else
                    Me.BackColor = Color.Transparent
                    'Me.TileWindowButton.BackColor = Color.Transparent
                    'Me.HTileButton.BackColor = Color.Transparent
                    'Me.PopoutWindowButton.BackColor = Color.Transparent
                    'Me.CloseWindowButton.BackColor = Color.Transparent
                End If
            Catch ex As Exception
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.EnableAuroGlassEffect = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property TabForeColor() As Color

        Get
            Return m_tabForeColor
        End Get

        Set(ByVal value As Color)
            m_tabForeColor = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.TabForeColor = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property TabBackColor() As Color

        Get
            Return m_tabBackColor
        End Get

        Set(ByVal value As Color)
            m_tabBackColor = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.TabBackColor = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property SelectedTabForeColor() As Color

        Get
            Return m_selectedTabForeColor
        End Get

        Set(ByVal value As Color)
            m_selectedTabForeColor = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.SelectedTabForeColor = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property SelectedTabBackColor() As Color

        Get
            Return m_selectedTabBackColor
        End Get

        Set(ByVal value As Color)
            m_selectedTabBackColor = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.SelectedTabBackColor = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property EmphasizeSelectedTab() As Boolean
        Get
            Return m_emphasizeSelectedTab
        End Get
        Set(ByVal value As Boolean)
            m_emphasizeSelectedTab = value
            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try
            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.EmphasizeSelectedTab = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property WordWrap() As Boolean

        Get
            Return m_wordWrap
        End Get

        Set(ByVal value As Boolean)
            m_wordWrap = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.WordWrap = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Look and feel of the tabs that represent the windows.
    ''' </summary>
    <Category("Appearance"),
    Description("Look and feel of the tabs that represent the windows."),
    Browsable(True)>
    Public Property Style() As TabStyle

        Get
            Return m_style
        End Get

        Set(ByVal value As TabStyle)
            m_style = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.Style = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Show the icon of the associated window in its corresponding tab.
    ''' </summary>
    <Category("Appearance"),
    Description("Show the icon of the associated window in its corresponding tab."),
    Browsable(True)>
    Public Property ShowIcons() As Boolean

        Get
            Return m_showIcons
        End Get

        Set(ByVal value As Boolean)
            m_showIcons = value

            Try
                LayoutTabStrips()
            Catch
                'do nothing
            End Try

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.ShowIcons = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Allow user to reposition (up/down) the panel.
    ''' </summary>
    <Category("Layout"),
    Description("Allow user to reposition (up/down) the panel."),
    Browsable(True)>
    Public Property AllowUserVerticalRepositioning() As Boolean

        Get
            Return m_allowUserVerticalRepositioning
        End Get

        Set(ByVal value As Boolean)
            If Me.DesignMode AndAlso value AndAlso Me.Orientation = WindowManagerOrientation.Top Then
                'I don't think we really need to enforce this, the panel will simply ignore the positioning if attempted
                'Throw New Exception("User repositioning is not allowed when Orientation=Top")
            Else
                m_allowUserVerticalRepositioning = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Allows the panel to disappear when no windows exist.
    ''' </summary>
    <Category("Layout"),
    Description("Allows the panel to disappear when no windows exist."),
    Browsable(True)>
    Public Property AutoHide() As Boolean

        Get
            Return m_autoHide
        End Get

        Set(ByVal value As Boolean)
            If value <> m_autoHide Then
                m_autoHide = value

                If m_isLoaded Then
                    Try
                        If m_autoHide Then
                            AttemptAutoHide()
                        Else
                            AttemptUndoAutoHide()
                        End If
                    Catch
                        'do nothing
                    End Try

                    If Not m_autoHide Then
                        Me.Visible = True
                        m_inAutoHideMode = False
                    End If
                End If
            End If
        End Set

    End Property

    ''' <summary>
    ''' Put the panel in a minimized mode.
    ''' </summary>
    <Browsable(False)>
    Public Property MinMode() As Boolean

        Get
            Return m_minMode
        End Get

        Set(ByVal value As Boolean)
            'todo: we're going to leave the bug where MinMode is not cancelled when Orientation=Left/Right 
            'and a window in a SUBPANEL is activated. This is caused because subpanels aren't *technically* in
            'minmode when the primary panel is Oriented on the Left or Right and therefore don't react to 
            'MinMode=False because of the following conditional. Changing this might mess some things up.
            'Need to find a workaround.
            If value <> m_minMode Then
                ToggleMinMode()
            End If
        End Set

    End Property

    ''' <summary>
    ''' Gets the collection of horizontally tiled strips that currently exist in the panel.
    ''' </summary>
    <Browsable(False)>
    Public ReadOnly Property TabStrips() As WindowTabStripCollection

        Get
            Return m_windowTabStrips
        End Get

    End Property

    ''' <summary>
    ''' Gets or sets the tabstrip with the focus.
    ''' </summary>
    <Browsable(False)>
    Public Property SelectedTabStrip() As WindowTabStrip

        Get
            If Not m_selectedTabStrip Is Nothing Then
                Return m_selectedTabStrip
            Else
                If m_windowTabStrips.Count <> 0 Then
                    Return m_windowTabStrips.Item(0)
                Else
                    Return Nothing
                End If
            End If
        End Get

        Set(ByVal value As WindowTabStrip)
            If Not value Is Nothing Then
                If m_windowTabStrips.Contains(value) Then
                    If Not m_selectedTabStrip Is Nothing Then
                        m_selectedTabStrip.IsActive = False
                    End If

                    If Not m_selectedTabStrip Is value Then
                        Me.MinMode = False
                    End If

                    m_selectedTabStrip = value

                    If m_isActive Then
                        m_selectedTabStrip.IsActive = True
                    End If
                Else
                    Throw New ArgumentException("Item not in collection.")
                End If
            Else
                m_selectedTabStrip = value
            End If
        End Set

    End Property

    Friend Property NextSubPanel() As WindowManagerPanel

        Get
            Return m_nextWindowManagerPanel
        End Get

        Set(ByVal value As WindowManagerPanel)
            If value Is Me Then
                Throw New System.InvalidOperationException("Cannot reference self.")
            Else
                m_nextWindowManagerPanel = value
            End If
        End Set

    End Property

    Private Property IsActive() As Boolean

        Get
            Return m_isActive
        End Get

        Set(ByVal value As Boolean)
            If value <> m_isActive Then
                m_isActive = value

                OnIsActiveChanged(System.EventArgs.Empty)

                'undone: do something pretty here to indicate focus
                If m_showFocusIndicator Then
                    If m_isActive Then
                        Me.WindowManagerSplitter2.BackColor = SystemColors.Highlight
                        Me.FocusIndicatorPanel.BackColor = SystemColors.Highlight
                    Else
                        Me.WindowManagerSplitter2.BackColor = Me.BackColor
                        Me.FocusIndicatorPanel.BackColor = Me.BackColor
                    End If
                End If
            End If

            If Not Me.SelectedTabStrip Is Nothing AndAlso Me.SelectedTabStrip.IsActive <> m_isActive Then
                Me.SelectedTabStrip.IsActive = m_isActive
            End If
        End Set

    End Property

    ''' <summary>
    ''' Gets or sets a window that will act as side-by-side pane.
    ''' </summary>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property AuxiliaryWindow() As Form
        Get
            Return m_auxiliaryWindow
        End Get

        Set(ByVal value As Form)
            If Not value Is Nothing And Me.Orientation = WindowManagerOrientation.Top Then
                Throw New Exception("AuxiliaryWindow is not supported when Orientation=Top")
            Else
                If Not m_auxiliaryWindow Is value Then
                    Dim origAuxiliaryWindow As Form = m_auxiliaryWindow

                    m_auxiliaryWindow = value

                    If Not m_auxiliaryWindow Is Nothing Then
                        If Not m_auxiliaryWindow.MdiParent Is GetMDIParent() Then
                            m_auxiliaryWindow.MdiParent = GetMDIParent()
                        End If

                        m_auxiliaryWindowOrigBorderStyle = m_auxiliaryWindow.FormBorderStyle
                        m_auxiliaryWindowOrigSize = m_auxiliaryWindow.Size

                        If m_auxiliaryWindow.FormBorderStyle <> FormBorderStyle.None Then
                            m_auxiliaryWindow.FormBorderStyle = FormBorderStyle.None
                        End If
                    End If

                    ReconcileAuxiliaryWindowSize()

                    If Not m_auxiliaryWindow Is Nothing Then
                        m_auxiliaryWindow.Show()
                        m_auxiliaryWindow.BringToFront()
                    End If

                    If Not origAuxiliaryWindow Is Nothing Then
                        origAuxiliaryWindow.Hide()
                        origAuxiliaryWindow.FormBorderStyle = m_auxiliaryWindowOrigBorderStyle
                        origAuxiliaryWindow.Size = m_auxiliaryWindowOrigSize
                    End If
                End If
            End If
        End Set

    End Property

    ''' <summary>
    ''' Allows MDIWindowManager to automatically take control of MDI child windows.
    ''' </summary>
    <Category("Behavior"),
    Description("Allows MDIWindowManager to automatically take control of MDI child windows."),
    Browsable(True)>
    Public Property AutoDetectMdiChildWindows() As Boolean

        Get
            Return m_autoDetectMdiChildWindows
        End Get

        Set(ByVal value As Boolean)
            m_autoDetectMdiChildWindows = value
        End Set

    End Property

    ''' <summary>
    ''' Allows you to select from a list of predefined TabProviders at design-time instead of using the CustomTabsProvider property.
    ''' </summary>
    <Category("Appearance"),
    Description("Allows you to select from a list of predefined TabProviders to change the look and feel of the tabs."),
    Browsable(False)>
    Public Property TabRenderMode() As TabsProvider

        Get
            Return m_tabRenderMode
        End Get

        Set(ByVal value As TabsProvider)
            If value <> m_tabRenderMode Then
                m_tabRenderMode = value
                InitializePrimaryTabStrip()
            End If
        End Set

    End Property

    ''' <summary>
    ''' Specifies the look and feel for the buttons.
    ''' </summary>
    <Category("Appearance"),
    Description("Specifies the look and feel for the buttons."),
    Browsable(False)>
    Public Property ButtonRenderMode() As ButtonRenderMode

        Get
            Return m_buttonRenderMode
        End Get

        Set(ByVal value As ButtonRenderMode)
            m_buttonRenderMode = value

            Select Case m_buttonRenderMode
                Case ButtonRenderMode.Standard
                    'Me.CloseWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.RollOverClassic
                    'Me.PopoutWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.RollOverClassic
                    'Me.TileWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.RollOverClassic
                    'Me.HTileButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.RollOverClassic
                Case ButtonRenderMode.System
                    'Me.CloseWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Standard
                    'Me.PopoutWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Standard
                    'Me.TileWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Standard
                    'Me.HTileButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Standard
                Case ButtonRenderMode.Professional
                    'Me.CloseWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Fancy
                    'Me.PopoutWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Fancy
                    'Me.TileWindowButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Fancy
                    'Me.HTileButton.RenderMode = WindowManagerButton.WindowManagerButtonRenderMode.Fancy
            End Select

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.ButtonRenderMode = value
            End If
        End Set

    End Property

    ''' <summary>
    ''' Allows you to specify a different provider for the look and feel of the tabs. (Set at design-time or app start only).
    ''' </summary>
    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property CustomTabsProviderType() As Type

        Get
            Return m_customTabsProviderType
        End Get

        Set(ByVal value As Type)
            If Not m_customTabsProviderType Is value Then
                m_customTabsProviderType = value
                InitializePrimaryTabStrip()
            End If
        End Set

    End Property

    ''' <summary>
    ''' Enables the TabPaint event to allow owner drawing of tabs.
    ''' </summary>
    <Category("Behavior"),
    Description("Enables the TabPaint event to allow owner drawing of tabs."),
    Browsable(True)>
    Public Property EnableTabPaintEvent() As Boolean

        Get
            Return m_enableTabPaintEvent
        End Get

        Set(ByVal value As Boolean)
            m_enableTabPaintEvent = value
            LayoutTabStrips()
        End Set

    End Property

    ''' <summary>
    ''' Disable the Close action button and menu items.
    ''' </summary>
    <Category("Appearance"),
    Description("Disable the Close action button and menu items."),
    Browsable(True)>
    Public Property DisableCloseAction() As Boolean

        Get
            Return m_disableCloseAction
        End Get

        Set(ByVal value As Boolean)
            m_disableCloseAction = value
            'Me.CloseWindowButton.Enabled = Not value
        End Set

    End Property

    ''' <summary>
    ''' Disable the Tile action button and menu items.
    ''' </summary>
    <Category("Appearance"),
    Description("Disable the Tile action button and menu items."),
    Browsable(True)>
    Public Property DisableTileAction() As Boolean
        Get
            Return m_disableTileAction
        End Get
        Set(ByVal value As Boolean)
            m_disableTileAction = value
            'Me.TileWindowButton.Enabled = Not value
        End Set
    End Property

    ''' <summary>
    ''' Disable the New Horizontral Group action button and menu items.
    ''' </summary>
    <Category("Appearance"),
    Description("Disable the New Horizontal Group action button and menu items."),
    Browsable(True)>
    Public Property DisableHTileAction() As Boolean
        Get
            Return m_disableHTileAction
        End Get
        Set(ByVal value As Boolean)
            m_disableHTileAction = value
            'Me.HTileButton.Enabled = Not value
        End Set
    End Property

    ''' <summary>
    ''' Disable the Popout Window action button and menu items.
    ''' </summary>
    <Category("Appearance"),
    Description("Disable the Popout Window action button and menu items."),
    Browsable(True)>
    Public Property DisablePopoutAction() As Boolean
        Get
            Return m_disablePopoutAction
        End Get
        Set(ByVal value As Boolean)
            m_disablePopoutAction = value
            'Me.PopoutWindowButton.Enabled = Not value
        End Set
    End Property

    ''' Summary:
    '''     Gets Or sets the System.Windows.Forms.ToolStripMenuItem that Is used to display
    '''     a list of Multiple-document interface (MDI) child forms.
    '''
    ''' Returns:
    '''     A System.Windows.Forms.ToolStripMenuItem that represents the menu item displaying
    '''     a list of MDI child forms that are open in the application.
    Public Property MdiWindowListItem As ToolStripMenuItem
        Get
            Return m_mdiWindowListItem
        End Get
        Set(value As ToolStripMenuItem)
            m_mdiWindowListItem = value
        End Set
    End Property


#End Region

#Region "Methods"

    Private Sub InitializePrimaryTabStrip()

        If m_windowTabStrips.Count > 1 OrElse GetAllWindows(False).Count > 0 Then
            Throw New InvalidOperationException("TabStrip Initialization not possible at this time.")
        Else
            If m_windowTabStrips.Count > 0 Then
                m_suppressTabStripRemoveAbort = True
                Try
                    Me.Controls.Remove(m_windowTabStrips.Item(0))
                    m_windowTabStrips.RemoveAt(0)
                Catch ex As Exception
                    Throw ex
                Finally
                    m_suppressTabStripRemoveAbort = False
                End Try
                'InitializePrimaryTabStrip will now be recalled by an event in m_windowTabStrips
            Else
                Dim windowTabStrip As WindowTabStrip = CreateWindowTabStrip()

                windowTabStrip.Width = Me.TabStripsContainer.ClientRectangle.Width
                m_windowTabStrips.Add(windowTabStrip)
            End If
        End If

    End Sub

    Private Function CreateWindowTabStrip() As WindowTabStrip

        Dim windowTabStrip As New WindowTabStrip

        If Not m_customTabsProviderType Is Nothing Then
            Dim newTabsProvider As ITabsProvider = CType(Activator.CreateInstance(m_customTabsProviderType), ITabsProvider)

            OnNewCustomTabsProviderInstance(New NewTabsProviderInstanceCreatedEventArgs(newTabsProvider))

            windowTabStrip.TabsProvider = newTabsProvider
        Else
            Select Case m_tabRenderMode
                Case TabsProvider.Standard
                    'do nothing
                    'Case TabsProvider.System
                    'windowTabStrip.TabsProvider = New SystemTabsProvider
            End Select
        End If

        Return windowTabStrip

    End Function

    Private Function GetMDIParent() As Form

        'Return CType(Me.Parent, Form)
        Return m_parentForm

    End Function

    Private Function GetMDIClient() As MdiClient

        If m_mdiClientControl Is Nothing Then
            m_mdiClientControl = ScanForMdiClientControl()

            If Not m_mdiClientControl Is Nothing Then
                m_originalMDIClientRect = m_mdiClientControl.Bounds
            End If
        End If

        Return m_mdiClientControl

    End Function

    Private Function ScanForMdiClientControl() As MdiClient

        Dim parentForm As Form = GetMDIParent()

        If Not parentForm Is Nothing Then
            For Each ctl As Control In parentForm.Controls
                If TypeOf ctl Is MdiClient Then
                    Return CType(ctl, MdiClient)
                End If
            Next ctl
        End If

        Return Nothing

    End Function

    Friend Sub SetAsTemporaryPanel(ByVal nextWindowManagerPanel As WindowManagerPanel)

        Me.NextSubPanel = nextWindowManagerPanel
        m_isTemporaryWindowManagerPanel = True

    End Sub

    ''' <summary>
    ''' Determine whether this is the currently focused WindowManagerPanel.
    ''' </summary>
    Public Function IsActivePanel() As Boolean

        Return Me.IsActive

    End Function

    ''' <summary>
    ''' Determine if this is the top primary panel on an MDI form.
    ''' </summary>
    Public Function IsPrimaryPanel() As Boolean

        Dim eventargs As New GetTopPanelEventArgs

        OnRequestChainGetTopPanel(eventargs)

        Return (eventargs.TopPanel Is Me OrElse eventargs.TopPanel Is Nothing)

    End Function

    ''' <summary>
    ''' (Reserved. Not intended for normal use. The Primary Panel is always the one created at design-time).
    ''' </summary>
    Public Function GetPrimaryPanel() As WindowManagerPanel

        Dim eventargs As New GetTopPanelEventArgs

        OnRequestChainGetTopPanel(eventargs)

        If Not eventargs.TopPanel Is Nothing Then
            Return eventargs.TopPanel
        Else
            Return Me
        End If

    End Function

    ''' <summary>
    ''' Get all the windows being managed.
    ''' </summary>
    Public Function GetAllWindows(Optional ByVal includeSubPanels As Boolean = True) As WrappedWindowCollection

        Dim wrappedWindows As New WrappedWindowCollection

        For Each windowTabStrip As WindowTabStrip In m_windowTabStrips
            For Each wrappedWindow As WrappedWindow In windowTabStrip.Items
                wrappedWindows.Add(wrappedWindow)
            Next wrappedWindow
        Next windowTabStrip

        If includeSubPanels Then
            If Not Me.NextSubPanel Is Nothing Then
                Dim chainWrappedWindows As WrappedWindowCollection = Me.NextSubPanel.GetAllWindows(includeSubPanels)

                For Each wrappedWindow As WrappedWindow In chainWrappedWindows
                    wrappedWindows.Add(wrappedWindow)
                Next wrappedWindow
            End If
        End If

        Return wrappedWindows

    End Function

    Private Function GetAllPanels() As List(Of WindowManagerPanel)

        Return GetPanels(includeOnlySubPanels:=False)

    End Function

    ''' <summary>
    ''' Returns a collection containing all subpanels.
    ''' </summary>
    Public Function GetAllSubPanels() As List(Of WindowManagerPanel)

        Return GetPanels(includeOnlySubPanels:=True)

    End Function

    Private Function GetPanels(ByVal includeOnlySubPanels As Boolean) As List(Of WindowManagerPanel)

        Dim panels As New List(Of WindowManagerPanel)

        If Not includeOnlySubPanels Then
            panels.Add(Me)
        End If

        Dim windowManagerPanel As WindowManagerPanel = Me.NextSubPanel

        Do Until windowManagerPanel Is Nothing
            panels.Add(windowManagerPanel)
            windowManagerPanel = windowManagerPanel.NextSubPanel
        Loop

        Return panels

    End Function

    ''' <summary>
    ''' Get the currently active panel.
    ''' </summary>
    Public Function GetActivePanel() As WindowManagerPanel

        Dim result As WindowManagerPanel
        Dim panels As List(Of WindowManagerPanel) = GetAllPanels()

        For Each panel As WindowManagerPanel In panels
            If panel.IsActivePanel() Then
                result = panel
                Exit For
            End If
        Next panel

        If result Is Nothing And Not Me.IsTemporaryPanel Then
            result = Me
        End If

        Return result

    End Function

    ''' <summary>
    ''' Add a window to the WindowManagerPanel.
    ''' </summary>
    Public Sub AddWindow(ByVal wrappedWindow As WrappedWindow)

        If Not m_autoDetectMdiChildWindows Then
            _AddWindow(wrappedWindow)
        Else
            Throw New InvalidOperationException("AddWindow not supported when AutoDetectMdiChildWindows is enabled.")
        End If

    End Sub

    Private Sub _AddWindow(ByVal wrappedWindow As WrappedWindow)

        AddWindow(m_windowTabStrips.Item(0), wrappedWindow)

    End Sub

    ''' <summary>
    ''' Add a window to the WindowManagerPanel.
    ''' </summary>
    Public Sub AddWindow(ByVal frm As Form)

        If Not m_autoDetectMdiChildWindows Then
            _AddWindow(frm)
        Else
            Throw New InvalidOperationException("AddWindow not supported when AutoDetectMdiChildWindows is enabled.")
        End If

    End Sub

    Private Sub _AddWindow(ByVal frm As Form)

        AddWindow(m_windowTabStrips.Item(0), frm)

    End Sub

    Friend Sub AddWindow(ByVal windowTabStrip As WindowTabStrip, ByVal frm As Form)

        If GetWrapperForWindow(frm) Is Nothing Then
            AddWindow(windowTabStrip, New WrappedWindow(frm))
        End If

    End Sub

    Friend Sub AddWindow(ByVal windowTabStrip As WindowTabStrip, ByVal wrappedWindow As WrappedWindow)

        If Not windowTabStrip Is Nothing And Not wrappedWindow Is Nothing Then
            'If Not windowTabStrip.WrappedWindows.Contains(wrappedWindow) Then
            If Not GetAllWindows().Contains(wrappedWindow) Then
                wrappedWindow.Window.MdiParent = GetMDIParent()
                wrappedWindow.Window.SetBounds(0 - wrappedWindow.Window.Width, 0 - wrappedWindow.Window.Height, 0, 0, BoundsSpecified.Location)
                wrappedWindow.Window.Show()

                Try
                    windowTabStrip.Items.Add(wrappedWindow)
                Catch
                    'do nothing
                End Try

                SetActiveWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Remove a window from the WindowManagerPanel.
    ''' </summary>
    Public Sub RemoveWindow(ByVal wrappedWindow As WrappedWindow)

        'since we're kinda enforcing the limitation that a window can only belong to one
        'tabstrip, this is mostly unecessary... but we're keeping it for now.
        Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow, False)

        Do Until windowTabStrip Is Nothing
            windowTabStrip.Items.Remove(wrappedWindow)
            windowTabStrip = GetWrappedWindowTabStrip(wrappedWindow, False)
        Loop

        If Not Me.NextSubPanel Is Nothing Then
            Me.NextSubPanel.RemoveWindow(wrappedWindow)
        End If

    End Sub

    ''' <summary>
    ''' Remove a window from the WindowManagerPanel.
    ''' </summary>
    Public Sub RemoveWindow(ByVal frm As Form)

        'since we're kinda enforcing the limitation that a window can only belong to one
        'tabstrip, this is mostly unecessary... but we're keeping it for now.
        For Each windowTabStrip As WindowTabStrip In m_windowTabStrips
            For Each wrappedWindow As WrappedWindow In windowTabStrip.Items
                If wrappedWindow.Window Is frm Then
                    windowTabStrip.Items.Remove(wrappedWindow)
                End If
            Next wrappedWindow
        Next windowTabStrip

        If Not Me.NextSubPanel Is Nothing Then
            Me.NextSubPanel.RemoveWindow(frm)
        End If

    End Sub

    ''' <summary>
    ''' Set the active window.
    ''' </summary>
    Public Sub SetActiveWindow(ByVal wrappedWindow As WrappedWindow)

        Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow)

        If Not windowTabStrip Is Nothing Then
            windowTabStrip.SelectedItem = wrappedWindow
            ActivateWindow(wrappedWindow.Window)
        End If

    End Sub

    ''' <summary>
    ''' Set the active window.
    ''' </summary>
    Public Sub SetActiveWindow(ByVal frm As Form)

        Dim wrappedWindow As WrappedWindow = GetWrapperForWindow(frm)

        If Not wrappedWindow Is Nothing Then
            SetActiveWindow(wrappedWindow)
        End If

    End Sub

    ''' <summary>
    ''' Set the active window.
    ''' </summary>
    Public Sub SetActiveWindow(ByVal index As Integer)

        SetActiveWindow(GetAllWindows().Item(index))

    End Sub

    ''' <summary>
    ''' Get the currently active window.
    ''' </summary>
    Public Function GetActiveWindow() As WrappedWindow

        Dim windowManagerPanel As WindowManagerPanel = GetActivePanel()

        If Not windowManagerPanel Is Nothing Then
            Return windowManagerPanel.GetSelectedWindow()
        Else
            Return Nothing
        End If

    End Function

    'this function returns the selected window in THIS tabstrip not the window that may be active in the MDI parent.
    'hidden from end-user to lesson confusion.
    Friend Function GetSelectedWindow() As WrappedWindow

        Dim windowTabStrip As WindowTabStrip = Me.SelectedTabStrip

        If Not windowTabStrip Is Nothing Then
            Return windowTabStrip.SelectedItem
        Else
            Return Nothing
        End If

    End Function

    Private Sub UserCloseActiveWindow()

        UserCloseWindow(GetActiveWindow())

    End Sub

    Private Sub UserCloseSelectedWindow()

        UserCloseWindow(GetSelectedWindow())

    End Sub

    ''' <summary>
    ''' Helper method to close a window as if done by the user in order to get the desired Reason code in the window's Unload events.
    ''' </summary>
    Public Sub UserCloseWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            UserCloseWindow(wrappedWindow.Window)
        End If

    End Sub

    ''' <summary>
    ''' Helper method to close a window as if done by the user in order to get the desired Reason code in the window's Unload events.
    ''' </summary>
    Public Sub UserCloseWindow(ByVal window As System.Windows.Forms.IWin32Window)

        If Not window Is Nothing Then
            'using PostMessage allows the window to receive the proper CloseReason in its Close events
            PostMessage(window.Handle, &H112, &HF060&, vbNullString) 'WM_SYSCOMMAND, SC_CLOSE
        End If

    End Sub

    ''' <summary>
    ''' Determine if this panel will be automatically be unloaded when all its windows are closed 
    ''' </summary>
    Public Function IsTemporaryPanel() As Boolean

        Return m_isTemporaryWindowManagerPanel

    End Function

    ''' <summary>
    ''' Find the wrapper object for a given window.
    ''' </summary>
    Public Function GetWrapperForWindow(ByVal frm As Form, Optional ByVal searchSubPanels As Boolean = True) As WrappedWindow

        Dim wrappedWindow As WrappedWindow = _GetWrapperForWindow(frm, searchSubPanels)

        If wrappedWindow Is Nothing Then
            Dim poppedWrappedWindow As WrappedWindow = g_poppedOutWindowsManager.FindWrapper(frm)

            If Not poppedWrappedWindow Is Nothing Then
                If g_poppedOutWindowsManager.GetWindowParent(poppedWrappedWindow) Is GetMDIParent() Then
                    wrappedWindow = poppedWrappedWindow
                End If
            End If
        End If

        Return wrappedWindow

    End Function

    Friend Function _GetWrapperForWindow(ByVal frm As Form, Optional ByVal searchSubPanels As Boolean = True) As WrappedWindow

        For Each windowTabStrip As WindowTabStrip In m_windowTabStrips
            For Each wrappedWindow As WrappedWindow In windowTabStrip.Items
                If wrappedWindow.Window Is frm Then
                    Return wrappedWindow
                End If
            Next wrappedWindow
        Next windowTabStrip

        If searchSubPanels AndAlso Not Me.NextSubPanel Is Nothing Then
            Return Me.NextSubPanel.GetWrapperForWindow(frm, searchSubPanels)
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' Search this panel and all subpanels to identify the panel that contains the specified wrapped window.
    ''' </summary>
    Public Function GetWrappedWindowPanel(ByVal wrappedWindow As WrappedWindow) As WindowManagerPanel

        If Not wrappedWindow Is Nothing Then
            If GetAllWindows(False).Contains(wrappedWindow) Then
                Return Me
            Else
                If Not Me.NextSubPanel Is Nothing Then
                    Return Me.NextSubPanel.GetWrappedWindowPanel(wrappedWindow)
                Else
                    Return Nothing
                End If
            End If
        End If

    End Function

    ''' <summary>
    ''' Search this panel and all subpanels to identify the tabstrip that contains the specified wrapped window.
    ''' </summary>
    Public Function GetWrappedWindowTabStrip(ByVal wrappedWindow As WrappedWindow, Optional ByVal searchSubPanels As Boolean = True) As WindowTabStrip

        If Not wrappedWindow Is Nothing Then
            If Not m_windowTabStrips Is Nothing Then
                For Each windowTabStrip As WindowTabStrip In m_windowTabStrips
                    If windowTabStrip.Items.Contains(wrappedWindow) Then
                        Return windowTabStrip
                    End If
                Next windowTabStrip
            End If

            If searchSubPanels AndAlso Not Me.NextSubPanel Is Nothing Then
                Return Me.NextSubPanel.GetWrappedWindowTabStrip(wrappedWindow, searchSubPanels)
            Else
                Return Nothing
            End If
        End If

    End Function

    ''' <summary>
    ''' Determine if a wrapped window is alone in a tabstrip.
    ''' </summary>
    Public Function IsWrappedWindowTiled(ByVal wrappedWindow As WrappedWindow) As Boolean

        Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow)

        If Not windowTabStrip Is Nothing Then
            Return (windowTabStrip.Items.Count = 1)
        Else
            Return False
        End If

    End Function

    ''' <summary>
    ''' Determine if a wrapped window is on a temporarily chained subpanel.
    ''' </summary>
    Public Function IsWrappedWindowHTiled(ByVal wrappedWindow As WrappedWindow) As Boolean

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = Me.GetWrappedWindowPanel(wrappedWindow)

            Return (Not windowManagerPanel Is Nothing AndAlso windowManagerPanel.IsTemporaryPanel)
        End If

    End Function

    ''' <summary>
    ''' Determine if a wrapped window is removed from the MDI parent and overlapping on the desktop.
    ''' </summary>
    Public Function IsWrappedWindowPoppedOut(ByVal wrappedWindow As WrappedWindow) As Boolean

        If Not wrappedWindow Is Nothing Then
            Return g_poppedOutWindowsManager.Contains(wrappedWindow)
        End If

    End Function

    ''' <summary>
    ''' Toggle tiling or untiling of a wrapped window.
    ''' </summary>
    Public Sub TileOrUntileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            If GetAllWindows(False).Contains(wrappedWindow) Then
                If IsWrappedWindowTiled(wrappedWindow) Then
                    UntileWrappedWindow(wrappedWindow)
                Else
                    TileWrappedWindow(wrappedWindow)
                End If
            Else
                If Not Me.NextSubPanel Is Nothing Then
                    Me.NextSubPanel.TileOrUntileWrappedWindow(wrappedWindow)
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Create a new tabstrip and put a wrapped window in it.
    ''' </summary>
    Public Sub TileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetWrappedWindowPanel(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                If windowManagerPanel Is Me Then
                    Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow, False)

                    If Not windowTabStrip Is Nothing Then
                        Dim eventargs As New WrappedWindowCancelEventArgs(wrappedWindow, windowTabStrip, Me)

                        OnWindowTiling(eventargs)

                        If Not eventargs.Cancel Then
                            Try
                                windowManagerPanel.Focus()
                            Catch
                                'do nothing
                            End Try

                            If Not windowTabStrip Is Nothing Then
                                windowTabStrip.Items.Remove(wrappedWindow)
                            End If

                            Dim newWindowTabStrip As WindowTabStrip = CreateWindowTabStrip()

                            m_windowTabStrips.Add(newWindowTabStrip)
                            Me.SelectedTabStrip = newWindowTabStrip
                            newWindowTabStrip.Items.Add(wrappedWindow)
                            newWindowTabStrip.SelectedItem = wrappedWindow
                            ActivateWindow(newWindowTabStrip.SelectedItem.Window)

                            LayoutTabStrips()
                        End If
                    End If
                Else
                    windowManagerPanel.TileWrappedWindow(wrappedWindow)
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Return the wrapped window to main tabstrip.
    ''' </summary>
    Public Sub UntileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetWrappedWindowPanel(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                If windowManagerPanel Is Me Then
                    Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow, False)

                    If Not windowTabStrip Is Nothing Then
                        Dim eventargs As New WrappedWindowCancelEventArgs(wrappedWindow, windowTabStrip, Me)

                        OnWindowUnTiling(eventargs)

                        If Not eventargs.Cancel Then
                            Try
                                windowManagerPanel.Focus()
                            Catch
                                'do nothing
                            End Try

                            Dim primaryWindowTabStrip As WindowTabStrip = m_windowTabStrips.Item(0)

                            If Not windowTabStrip Is primaryWindowTabStrip Then
                                If Not windowTabStrip Is Nothing Then
                                    windowTabStrip.Items.Remove(wrappedWindow)

                                    If windowTabStrip.Items.Count = 0 Then
                                        If m_windowTabStrips.Contains(windowTabStrip) Then
                                            m_windowTabStrips.Remove(windowTabStrip)
                                        End If
                                    End If
                                End If

                                primaryWindowTabStrip.Items.Add(wrappedWindow)
                                primaryWindowTabStrip.SelectedItem = wrappedWindow
                                ActivateWindow(primaryWindowTabStrip.SelectedItem.Window)

                                LayoutTabStrips()
                            End If
                        End If
                    End If
                Else
                    windowManagerPanel.UntileWrappedWindow(wrappedWindow)
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Add a new subpanel and put a wrapped window in it.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub HTileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            If GetAllWindows(False).Contains(wrappedWindow) Then
                Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow, False)

                If Not Me.NextSubPanel Is Nothing AndAlso Me.NextSubPanel.IsTemporaryPanel Then
                    Dim eventargs As New WrappedWindowCancelEventArgs(wrappedWindow, windowTabStrip, Me)

                    OnWindowHTiling(eventargs)

                    If Not eventargs.Cancel Then
                        Dim nextWindowManagerPanel As WindowManagerPanel = Me.NextSubPanel

                        Try
                            nextWindowManagerPanel.Focus()
                        Catch
                            'do nothing
                        End Try

                        windowTabStrip.Items.Remove(wrappedWindow)
                        nextWindowManagerPanel.TabStrips.Item(0).Items.Add(wrappedWindow)
                        nextWindowManagerPanel.SetActiveWindow(wrappedWindow)
                        ActivateWindow(wrappedWindow.Window)
                    End If
                Else
                    Dim eventargs As New WrappedWindowCancelEventArgs(wrappedWindow, windowTabStrip, Me)

                    OnWindowHTiling(eventargs)

                    If Not eventargs.Cancel Then
                        If GetAllWindows(False).Count > 1 Then
                            Dim newWindowManagerPanel As New WindowManagerPanel

                            'newWindowManagerPanel.Style = Me.Style

                            If Me.Orientation = WindowManagerOrientation.Top Then
                                newWindowManagerPanel.Orientation = WindowManagerOrientation.Bottom
                            Else
                                newWindowManagerPanel.Orientation = Me.Orientation
                            End If

                            'newWindowManagerPanel.ShowTitle = Me.ShowTitle
                            'newWindowManagerPanel.ShowLayoutButtons = Me.ShowLayoutButtons
                            CopyAppearance(newWindowManagerPanel)

                            newWindowManagerPanel.AllowUserVerticalRepositioning = True

                            Dim mdiParentForm As Form = GetMDIParent()
                            Dim mdiClientControl As MdiClient = GetMDIClient()
                            Dim correctionNeeded As Boolean = False
                            Dim bottomBarrier As Integer = Me.Top + Me.Height + 50 + newWindowManagerPanel.Height + 50

                            correctionNeeded = False

                            If NextSubPanel Is Nothing Then
                                If mdiClientControl.PointToClient(mdiParentForm.PointToScreen(New Point(Me.Left, bottomBarrier))).Y > mdiClientControl.ClientRectangle.Bottom Then
                                    bottomBarrier -= mdiClientControl.PointToClient(mdiParentForm.PointToScreen(New Point(Me.Left, bottomBarrier))).Y - mdiClientControl.ClientRectangle.Bottom
                                    correctionNeeded = True
                                End If
                            Else
                                If bottomBarrier > Me.NextSubPanel.Top Then
                                    bottomBarrier -= bottomBarrier - Me.NextSubPanel.Top
                                    correctionNeeded = True
                                End If
                            End If

                            Dim okToContinue As Boolean = False

                            If correctionNeeded Then
                                Dim eventargs2 As RedoLayoutEventArgs

                                bottomBarrier = bottomBarrier - 50 - newWindowManagerPanel.Height - 50 - Me.Height

                                eventargs2 = New RedoLayoutEventArgs(Me, mdiClientControl, True)
                                eventargs2.BottomBarrier = bottomBarrier
                                OnRequestChainRedoLayout(eventargs2)

                                If eventargs2.Cancel Then
                                    If Me.NextSubPanel Is Nothing Then
                                        bottomBarrier = mdiClientControl.ClientRectangle.Bottom - 50 - newWindowManagerPanel.Height - 50 - Me.Height
                                    Else
                                        bottomBarrier = NextSubPanel.Top - 50 - newWindowManagerPanel.Height - 50 - Me.Height
                                    End If

                                    eventargs2 = New RedoLayoutEventArgs(Me, mdiClientControl, True)
                                    eventargs2.BottomBarrier = bottomBarrier
                                    OnRequestChainRedoLayout(eventargs2)
                                End If

                                okToContinue = Not eventargs2.Cancel
                            Else
                                If Me.NextSubPanel Is Nothing Then
                                    bottomBarrier = CInt((mdiClientControl.ClientRectangle.Bottom - (Me.Top)) / 2)
                                Else
                                    bottomBarrier = CInt((Me.NextSubPanel.Top - (Me.Top)) / 2)
                                End If

                                okToContinue = True
                            End If

                            If Not okToContinue Then
                                MsgBox("Not enough room to open a new horizontal tab group.", MsgBoxStyle.Exclamation, System.Reflection.Assembly.GetEntryAssembly.GetName().Name)
                            Else
                                Dim eventargs2 As New RedoLayoutEventArgs(Me, mdiClientControl, False)

                                eventargs2.BottomBarrier = bottomBarrier
                                OnRequestChainRedoLayout(eventargs2)

                                windowTabStrip.Items.Remove(wrappedWindow)

                                Me.Parent.Controls.Add(newWindowManagerPanel)
                                'Me.Parent.Refresh()

                                Dim oldNextWindowManagerPanel As WindowManagerPanel = Me.NextSubPanel

                                Me.NextSubPanel = Nothing

                                If correctionNeeded Then
                                    Me.Top = bottomBarrier
                                End If

                                newWindowManagerPanel.CustomTabsProviderType = Me.CustomTabsProviderType

                                OnNewCustomTabsProviderInstance(New NewTabsProviderInstanceCreatedEventArgs(newWindowManagerPanel.TabStrips.Item(0).TabsProvider))

                                newWindowManagerPanel.SetAsTemporaryPanel(oldNextWindowManagerPanel)

                                Me.NextSubPanel = newWindowManagerPanel

                                newWindowManagerPanel.Visible = True

                                If correctionNeeded Then
                                    newWindowManagerPanel.SetBounds(Me.Location.X, Me.Top + Me.Height + 50, Me.Size.Width, Me.Size.Height)
                                Else
                                    newWindowManagerPanel.SetBounds(Me.Location.X, bottomBarrier, Me.Size.Width, Me.Size.Height)
                                End If

                                Try
                                    newWindowManagerPanel.Focus()
                                Catch
                                    'do nothing
                                End Try

                                newWindowManagerPanel.TabStrips.Item(0).Items.Add(wrappedWindow)
                                newWindowManagerPanel.SetActiveWindow(wrappedWindow)
                                ActivateWindow(wrappedWindow.Window)
                            End If
                        End If
                    End If
                End If
            Else
                If Not Me.NextSubPanel Is Nothing Then
                    Me.NextSubPanel.HTileWrappedWindow(wrappedWindow)
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Remove the wrapped window from the panel and place it on the desktop as an overlapping window.
    ''' </summary>
    Public Sub PopOutWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow)

        If Not windowTabStrip Is Nothing Then
            Dim eventargs As New WrappedWindowCancelEventArgs(wrappedWindow, windowTabStrip, Me)

            OnWindowPoppingOut(eventargs)

            If Not eventargs.Cancel Then
                If (GetPrimaryPanel() Is Me) Then
                    windowTabStrip = GetWrappedWindowTabStrip(wrappedWindow)

                    'get and save these references because the WindowManagerPanel may clear them
                    'if the form being popped out is that last remaining form in the panel
                    Dim parentForm As Form = Me.GetMDIParent()
                    Dim poppedOutWindowsManager As PoppedOutWindowsManager = m_poppedOutWindowsManager
                    Dim originalAutoHideValue As Boolean = Me.AutoHide
                    Dim originalAutoDetectMdiChildWindowsValue As Boolean = Me.AutoDetectMdiChildWindows

                    Me.AutoDetectMdiChildWindows = False
                    Me.AutoHide = False

                    Try
                        windowTabStrip.Items.Remove(wrappedWindow)
                    Catch ex As WindowTabStripCollectionException
                        'do nothing
                    End Try

                    Try
                        wrappedWindow.AdjustFormProperties(False)
                        wrappedWindow.Window.MdiParent = Nothing
                        wrappedWindow.InitCustomSystemMenu()

                        poppedOutWindowsManager.AddWindow(wrappedWindow, parentForm)
                    Catch ex As Exception
                        Throw ex
                    Finally
                        Me.AutoHide = originalAutoHideValue
                        Me.AutoDetectMdiChildWindows = originalAutoDetectMdiChildWindowsValue
                    End Try

                    ActivateWindow(wrappedWindow.Window)

                    OnWindowPoppedOut(New WrappedWindowEventArgs(eventargs.WrappedWindow, eventargs.WindowTabStrip, eventargs.WindowManagerPanel))
                Else
                    MoveWindowToMainPanelAndPopOut(wrappedWindow)
                End If
            End If
        End If

    End Sub

    ''' <summary>
    ''' Return an overlapping wrapped window to WindowManagerPanel.
    ''' </summary>
    Public Sub PopInWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim eventargs As New WrappedWindowCancelEventArgs(wrappedWindow, Nothing, Me)

            OnWindowPoppingIn(eventargs)

            If Not eventargs.Cancel Then
                wrappedWindow.Window.WindowState = FormWindowState.Normal

                Try
                    m_poppedOutWindowsManager.RemoveWindow(wrappedWindow)
                Catch
                    'do nothing
                End Try

                Dim windowTabStrip As WindowTabStrip = m_windowTabStrips.Item(0)
                Dim mdiParentForm As Form = GetMDIParent()

                wrappedWindow.Window.Visible = False
                wrappedWindow.Window.MdiParent = mdiParentForm
                wrappedWindow.RemoveCustomSystemMenu()
                wrappedWindow.AdjustFormProperties(True)

                windowTabStrip.Items.Add(wrappedWindow)
                windowTabStrip.SelectedItem = wrappedWindow
                ActivateWindow(windowTabStrip.SelectedItem.Window)

                OnWindowPoppedIn(New WrappedWindowEventArgs(eventargs.WrappedWindow, eventargs.WindowTabStrip, eventargs.WindowManagerPanel))
            End If
        End If

    End Sub

    ''' <summary>
    ''' Close all windows being managed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseAllWindows(Optional ByVal includeSubPanels As Boolean = True)

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows(includeSubPanels)

        For count As Integer = wrappedWindows.Count - 1 To 0 Step -1
            Try
                ShowWindowWithFocus(wrappedWindows.Item(count).Window)
                wrappedWindows.Item(count).Window.Close()
            Catch
                'do nothing
            End Try
        Next count

    End Sub

    ''' <summary>
    ''' Helper function to easily get the adjusted MDIClient area bounds of the parent form.
    ''' </summary>
    Public Function GetMDIClientAreaBounds() As Rectangle

        Dim mdiParentForm As Form = Me.GetMDIParent
        Dim mdiClientControl As MdiClient = GetMDIClient()

        If Not mdiClientControl Is Nothing Then
            Return mdiParentForm.RectangleToClient(mdiClientControl.RectangleToScreen(mdiClientControl.ClientRectangle))
        Else
            Return New Rectangle(0, 0, 0, 0)
        End If

    End Function

    ''' <summary>
    ''' Minimize the WindowManagerPanel.
    ''' </summary>
    Public Sub ToggleMinMode()

        If Not Me.IsTemporaryPanel Then
            Select Case Me.Orientation
                Case WindowManagerOrientation.Bottom
                    ToggleMinModeForBottom()
                Case WindowManagerOrientation.Left, WindowManagerOrientation.Right
                    ToggleMinModeForSides()
            End Select
        Else
            ToggleMinModeForBottom()
        End If


    End Sub

    ''' <summary>
    ''' Collapse the WindowManagerPanel to its smallest vertical size.
    ''' </summary>
    Public Sub SnapToMinimumSize(Optional ByVal includeSubPanels As Boolean = False)

        If includeSubPanels AndAlso Not Me.NextSubPanel Is Nothing Then
            Me.NextSubPanel.SnapToMinimumSize(includeSubPanels)
        End If

        If m_allowUserVerticalRepositioning Then
            If Not Me.NextSubPanel Is Nothing Then
                Me.Top = Me.NextSubPanel.Top - Me.Height
            Else
                Dim mdiClientControl As MdiClient = GetMDIClient()

                Me.Top = mdiClientControl.Bottom - Me.Height
            End If

            LayoutWindows()
        End If

    End Sub

    Private Sub UnloadWrappedWindowMenuItems()

        For index As Integer = Me.TitleContextMenu.Items.Count - 1 To 0 Step -1
            If (TypeOf Me.TitleContextMenu.Items.Item(index) Is ToolStripMenuItem) Then
                Dim mnu As ToolStripMenuItem = Me.TitleContextMenu.Items.Item(index)

                If TypeOf mnu Is WrappedWindowMenuItem Then
                    RemoveHandler mnu.Click, AddressOf HandleWrappedWindowMenuItemClick
                    Me.TitleContextMenu.Items.Remove(mnu)
                    mnu.Dispose()
                End If
            End If
        Next index

    End Sub

    Private Sub CopyAppearance(ByVal toWindowManagerPanel As WindowManagerPanel)

        If Not toWindowManagerPanel Is Nothing Then
            With toWindowManagerPanel
                .EnableTabPaintEvent = Me.EnableTabPaintEvent
                .DisableCloseAction = Me.DisableCloseAction
                .DisableTileAction = Me.DisableTileAction
                .DisableHTileAction = Me.DisableHTileAction
                .DisablePopoutAction = Me.DisablePopoutAction
                .ButtonRenderMode = Me.ButtonRenderMode
                .BackColor = Me.BackColor
                .ForeColor = Me.ForeColor
                .Style = Me.Style
                .EnableAuroGlassEffect = Me.EnableAuroGlassEffect
                .ShowIcons = Me.ShowIcons
                .TabForeColor = Me.TabForeColor
                .TabBackColor = Me.TabBackColor
                .SelectedTabForeColor = Me.SelectedTabForeColor
                .SelectedTabBackColor = Me.SelectedTabBackColor
                .EmphasizeSelectedTab = Me.EmphasizeSelectedTab
                .Height = Me.Height
            End With
        End If

    End Sub

    ''' <summary>
    ''' Display a dialog that displays all the windows in all panels.
    ''' </summary>
    Public Function ShowAllWindowsDialog() As DialogResult

        Return ShowAllWindowsDialog(Nothing)

    End Function

    ''' <summary>
    ''' Display a dialog that displays all the windows in all panels.
    ''' </summary>
    Public Function ShowAllWindowsDialog(ByVal owner As System.Windows.Forms.IWin32Window) As DialogResult

        Dim frm As New WindowsForm

        frm.CloseWindowsButton.Visible = Not m_disableCloseAction

        frm.LoadWindowsList(GetAllWindows())

        Dim result As DialogResult = frm.ShowDialog(owner)

        'all this code used to be in WindowsForm itself... but bugs in the way MDI focuses child windows
        'let to putting all the work here after the dialog is dismissed.
        If result = DialogResult.OK Then
            Select Case frm.UserActionResult
                Case WindowsForm.UserAction.ActivateWindow
                    If frm.WindowList.SelectedItems.Count > 0 Then
                        ActivateWindow(CType(frm.WindowList.SelectedItems.Item(0).Tag, WrappedWindow).Window)
                    End If
                Case WindowsForm.UserAction.CloseWindows
                    If frm.WindowList.SelectedItems.Count > 0 Then
                        For Each item As ListViewItem In frm.WindowList.SelectedItems
                            Dim wrappedWindow As WrappedWindow = CType(item.Tag, WrappedWindow)

                            With wrappedWindow
                                ShowWindowWithFocus(.Window)
                                .Window.Refresh()
                                .Window.Close()
                            End With
                        Next item
                    End If
            End Select
        End If

        frm.Dispose()

        Return result

    End Function

    ''' <summary>
    ''' Retrieve an array of menu items representing all open windows.
    ''' </summary>
    Public Function GetAllWindowsMenu(Optional ByVal includeTopNineAccelerators As Boolean = False) As ToolStripMenuItem()

        Return GetAllWindowsMenu(0, includeTopNineAccelerators)

    End Function

    ''' <summary>
    ''' Retrieve an array of menu items representing all open windows.
    ''' </summary>
    Public Function GetAllWindowsMenu(ByVal limit As Integer, Optional ByVal includeTopNineAccelerators As Boolean = False) As ToolStripMenuItem()

        Dim menuItems() As WrappedWindowMenuItem
        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        If wrappedWindows.Count > 0 Then
            Dim activePanel As WindowManagerPanel = GetActivePanel()
            Dim activeWrappedWindow As WrappedWindow

            If Not activePanel Is Nothing Then
                activeWrappedWindow = activePanel.GetActiveWindow()
            End If

            ReDim menuItems(wrappedWindows.Count - 1)
            Dim i As Integer = 0

            For Each wrappedWindow As WrappedWindow In wrappedWindows
                Dim mnu As New WrappedWindowMenuItem

                If Not activeWrappedWindow Is Nothing AndAlso activeWrappedWindow Is wrappedWindow Then
                    'mnu.RadioCheck = True
                    mnu.Checked = True
                End If

                If includeTopNineAccelerators AndAlso i < 9 Then
                    mnu.TextPrefix = "&" + (i + 1).ToString("")
                End If

                mnu.WrappedWindow = wrappedWindow
                AddHandler mnu.Click, AddressOf HandleWindowMenuItemClick
                menuItems(i) = mnu

                i = i + 1

                If limit > 0 AndAlso i = limit Then
                    Exit For
                End If
            Next wrappedWindow

            ReDim Preserve menuItems(i - 1)
        End If

        Return menuItems

    End Function

    Private Sub CleanUp()

        m_isUnloaded = True

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        For Each wrappedWindow As WrappedWindow In wrappedWindows
            wrappedWindow.Dispose()
        Next wrappedWindow

        UnloadLeftBlankArea()
        UnloadRightBlankArea()

    End Sub

    Private Sub AddParentHandlers()

        If Not m_parentForm Is Nothing Then
            AddHandler m_parentForm.MdiChildActivate, AddressOf HandleParentMdiChildActivate
            AddHandler m_parentForm.FormClosed, AddressOf HandleParentFormClosed
        End If

    End Sub

    Private Sub RemoveParentHandlers()

        If Not m_parentForm Is Nothing Then
            RemoveHandler m_parentForm.MdiChildActivate, AddressOf HandleParentMdiChildActivate
            RemoveHandler m_parentForm.FormClosed, AddressOf HandleParentFormClosed
        End If

    End Sub

    Private Sub AddWindowToHistory(ByVal wrappedWindow As WrappedWindow)

        If m_historyBack.Count = 0 OrElse Not (m_historyBack.Count > 0 AndAlso m_historyBack.Item(m_historyBack.Count - 1) Is wrappedWindow) Then
            m_historyBack.Add(wrappedWindow)

            If m_historyBack.Count > 20 Then
                m_historyBack.RemoveAt(0)
            End If
        End If

    End Sub

    Private Sub RemoveWindowFromHistory(ByVal wrappedWindow As WrappedWindow)

        For index As Integer = m_historyBack.Count - 1 To 0 Step -1
            If m_historyBack.Item(index) Is wrappedWindow Then
                m_historyBack.RemoveAt(index)
            End If
        Next index

    End Sub

    Friend Function GetWindowHistoryBack() As System.Collections.ObjectModel.ReadOnlyCollection(Of WrappedWindow)

        Return New System.Collections.ObjectModel.ReadOnlyCollection(Of WrappedWindow)(m_historyBack)

    End Function

    Friend Function GetTabStripPreviousWrappedWindow(ByVal windowTabStrip As WindowTabStrip) As WrappedWindow

        Dim result As WrappedWindow
        Dim tabStripWrappedWindows As WrappedWindowCollection = windowTabStrip.Items
        Dim historyBack As System.Collections.ObjectModel.ReadOnlyCollection(Of WrappedWindow) = GetPrimaryPanel().GetWindowHistoryBack()

        For index As Integer = historyBack.Count - 1 To 0 Step -1
            Dim wrappedWindow As WrappedWindow = historyBack.Item(index)

            If tabStripWrappedWindows.Contains(wrappedWindow) Then
                result = wrappedWindow
                Exit For
            End If
        Next index

        'If result Is Nothing AndAlso historyBack.Count > 0 Then
        'result = m_historyBack.Item(historyBack.Count - 1)
        'End If

        Return result

    End Function

    Private Sub TrackActiveMdiChild()

        If Not m_parentForm Is Nothing Then
            Dim activeWindow As Form = m_parentForm.ActiveMdiChild()

            If Not activeWindow Is Nothing Then
                Dim panels As List(Of WindowManagerPanel) = GetPrimaryPanel().GetAllPanels()
                Dim foundPanel As WindowManagerPanel
                Dim foundTabStrip As WindowTabStrip
                Dim found As Boolean

                For Each panel As WindowManagerPanel In panels
                    For Each tabstrip As WindowTabStrip In panel.TabStrips
                        For Each wrappedWindow As WrappedWindow In tabstrip.Items
                            If wrappedWindow.Window Is activeWindow Then
                                foundPanel = panel
                                foundTabStrip = tabstrip
                                found = True
                                Exit For
                            End If
                        Next wrappedWindow
                    Next tabstrip
                Next panel

                If found Then
                    If Not foundPanel.SelectedTabStrip Is foundTabStrip Then
                        foundPanel.SelectedTabStrip = foundTabStrip
                    End If

                    For Each panel As WindowManagerPanel In panels
                        panel.IsActive = (panel Is foundPanel)
                    Next panel
                End If
            End If
        End If

    End Sub

    Private Sub ActivateWindow(ByVal frm As Form)

        ShowWindowWithFocus(frm)
        TrackActiveMdiChild()

    End Sub

#End Region

#Region "Internal Window Layout Routines"

    Private Sub LayoutTabStrips(Optional ByVal dontFitTabStrips As Boolean = False)

        If m_windowTabStrips.Count > 0 Then
            'If Me.WindowButtonsPanel.Visible Then
            '    Me.TabStripsContainer.Width = Me.WindowButtonsPanel.Left - Me.TabStripsContainer.Left - 5
            'Else
            '    Me.TabStripsContainer.Width = Me.WindowButtonsPanel.Right - Me.TabStripsContainer.Left
            'End If

            If Not dontFitTabStrips Then
                SetBestTabStripWidths(m_windowTabStrips, Me.TabStripsContainer.ClientRectangle.Width - 1)
            End If

            Dim currentX As Integer = 0

            For Each windowTabStrip As WindowTabStrip In m_windowTabStrips
                With windowTabStrip
                    .Style = m_style
                    .EnableAeroGlassEffect = m_enableAeroGlass
                    .EnableTabPaintEvent = m_enableTabPaintEvent
                    .ShowIcons = m_showIcons
                    .TabForeColor = m_tabForeColor
                    .TabBackColor = m_tabBackColor
                    .SelectedTabForeColor = m_selectedTabForeColor
                    .SelectedTabBackColor = m_selectedTabBackColor
                    .EmphasizeSelectedTab = m_emphasizeSelectedTab
                    .WordWrap = m_wordWrap
                    .SetBounds(currentX, 0, .Width, Me.TabStripsContainer.ClientRectangle.Height)
                    .Visible = True
                    .Refresh()

                    currentX += .Width
                End With
            Next windowTabStrip
        End If

    End Sub

    Private Sub LayoutWindows()

        If m_isUnloaded Then Exit Sub

        Dim mdiParentForm As Form = GetMDIParent()
        Dim mdiClientControl As MdiClient = GetMDIClient()

        If Not mdiClientControl Is Nothing Then
            Dim myAdjustedMDIBounds As Rectangle = mdiClientControl.RectangleToClient(mdiParentForm.RectangleToScreen(Me.Bounds))
            Dim tabStripAdjustedMDIBounds As Rectangle
            Dim windowTabStrip As WindowTabStrip
            Dim bottomBarrier As Integer

            If m_nextWindowManagerPanel Is Nothing Then
                bottomBarrier = mdiClientControl.ClientRectangle.Bottom
            Else
                bottomBarrier = mdiClientControl.RectangleToClient(mdiParentForm.RectangleToScreen(m_nextWindowManagerPanel.Bounds)).Top
            End If

            If Not mdiClientControl Is Nothing Then
                If m_windowTabStrips.Count > 1 Then
                    For Each windowTabStrip In m_windowTabStrips
                        For Each wrappedWindow As WrappedWindow In windowTabStrip.Items
                            tabStripAdjustedMDIBounds = mdiClientControl.RectangleToClient(Me.TabStripsContainer.RectangleToScreen(windowTabStrip.Bounds))

                            wrappedWindow.Window.SetBounds(tabStripAdjustedMDIBounds.Left, myAdjustedMDIBounds.Bottom, tabStripAdjustedMDIBounds.Width - 3, bottomBarrier - myAdjustedMDIBounds.Bottom)

                            If Not wrappedWindow.Window.Visible Then
                                wrappedWindow.Window.Visible = True
                            End If
                        Next wrappedWindow

                        mdiParentForm.Controls.Add(windowTabStrip.ResizeSplitter)

                        Dim splitterBounds As New Rectangle(tabStripAdjustedMDIBounds.Left + tabStripAdjustedMDIBounds.Width - 3, myAdjustedMDIBounds.Bottom, 3, bottomBarrier - myAdjustedMDIBounds.Bottom)

                        splitterBounds = mdiParentForm.RectangleToClient(mdiClientControl.RectangleToScreen(splitterBounds))
                        windowTabStrip.ResizeSplitter.Bounds = splitterBounds
                        windowTabStrip.ResizeSplitter.BackColor = Me.BackColor
                        windowTabStrip.ResizeSplitter.Visible = True
                    Next windowTabStrip

                    LayoutSideAreas(True, mdiParentForm, mdiClientControl, myAdjustedMDIBounds, bottomBarrier)
                Else
                    If m_windowTabStrips.Count > 0 Then
                        If mdiParentForm.Controls.Contains(m_windowTabStrips.Item(0).ResizeSplitter) Then
                            m_windowTabStrips.Item(0).ResizeSplitter.Visible = False
                            mdiParentForm.Controls.Remove(m_windowTabStrips.Item(0).ResizeSplitter)
                        End If

                        For Each wrappedWindow As WrappedWindow In m_windowTabStrips.Item(0).Items
                            If Me.Orientation = WindowManagerOrientation.Right Then
                                wrappedWindow.Window.SetBounds(myAdjustedMDIBounds.Left + 5, myAdjustedMDIBounds.Bottom, myAdjustedMDIBounds.Width - 5, bottomBarrier - myAdjustedMDIBounds.Bottom)
                            ElseIf Me.Orientation = WindowManagerOrientation.Left Then
                                wrappedWindow.Window.SetBounds(myAdjustedMDIBounds.Left, myAdjustedMDIBounds.Bottom, myAdjustedMDIBounds.Width - 5, bottomBarrier - myAdjustedMDIBounds.Bottom)
                            Else
                                wrappedWindow.Window.SetBounds(myAdjustedMDIBounds.Left, myAdjustedMDIBounds.Bottom, myAdjustedMDIBounds.Width, bottomBarrier - myAdjustedMDIBounds.Bottom)
                            End If

                            'If Not wrappedWindow.Window.Visible Then
                            '    wrappedWindow.Window.Visible = True
                            'End If
                        Next wrappedWindow

                        LayoutSideAreas(False, mdiParentForm, mdiClientControl, myAdjustedMDIBounds, bottomBarrier)
                    End If
                End If
            End If
        End If

    End Sub

    Private Sub LayoutSideAreas(ByVal multipleTabStrips As Boolean, ByVal mdiParentForm As Form, ByVal mdiClientControl As MdiClient, ByVal myAdjustedMDIBounds As Rectangle, ByVal bottomBarrier As Integer)

        If multipleTabStrips Or (Me.Orientation = WindowManagerOrientation.Right Or Me.Orientation = WindowManagerOrientation.Left) Then
            Dim windowTabStrip As WindowTabStrip
            Dim tabStripAdjustedMDIBounds As Rectangle

            If multipleTabStrips Or (Me.Orientation = WindowManagerOrientation.Bottom Or Me.Orientation = WindowManagerOrientation.Top) Or Me.Orientation = WindowManagerOrientation.Right Then
                If m_blankAreaLeft Is Nothing Then
                    m_blankAreaLeft = New DummyForm

                    With m_blankAreaLeft
                        .MdiParent = mdiParentForm
                        .BackColor = Me.BackColor
                        .Visible = True
                    End With
                End If

                If Me.Orientation = WindowManagerOrientation.Right Then
                    m_blankAreaLeft.WindowManagerSplitter1.Dock = DockStyle.Left
                    m_blankAreaLeft.Enabled = True
                Else
                    m_blankAreaLeft.Enabled = False
                End If

                windowTabStrip = m_windowTabStrips.Item(0)
                tabStripAdjustedMDIBounds = mdiClientControl.RectangleToClient(Me.TabStripsContainer.RectangleToScreen(windowTabStrip.Bounds))

                If multipleTabStrips Or (Me.Orientation = WindowManagerOrientation.Bottom Or Me.Orientation = WindowManagerOrientation.Top) Then
                    m_blankAreaLeft.SetBounds(myAdjustedMDIBounds.Left, myAdjustedMDIBounds.Bottom, tabStripAdjustedMDIBounds.Left - myAdjustedMDIBounds.Left, bottomBarrier - myAdjustedMDIBounds.Bottom)
                ElseIf Me.Orientation = WindowManagerOrientation.Right Then
                    m_blankAreaLeft.SetBounds(myAdjustedMDIBounds.Left, myAdjustedMDIBounds.Bottom, 5, bottomBarrier - myAdjustedMDIBounds.Bottom)
                End If
            Else
                UnloadLeftBlankArea()
            End If

            '---------------------------

            If multipleTabStrips Or (Me.Orientation = WindowManagerOrientation.Bottom Or Me.Orientation = WindowManagerOrientation.Top) Or Me.Orientation = WindowManagerOrientation.Left Then
                If m_blankAreaRight Is Nothing Then
                    m_blankAreaRight = New DummyForm
                    With m_blankAreaRight
                        .MdiParent = mdiParentForm
                        .BackColor = Me.BackColor
                        .Visible = True
                        .Enabled = False
                    End With
                End If

                If Me.Orientation = WindowManagerOrientation.Left Then
                    m_blankAreaRight.WindowManagerSplitter1.Dock = DockStyle.Right
                    m_blankAreaRight.Enabled = True
                Else
                    m_blankAreaRight.Enabled = False
                End If

                windowTabStrip = m_windowTabStrips.Item(m_windowTabStrips.Count - 1)
                tabStripAdjustedMDIBounds = mdiClientControl.RectangleToClient(Me.TabStripsContainer.RectangleToScreen(windowTabStrip.Bounds))

                If multipleTabStrips Or (Me.Orientation = WindowManagerOrientation.Bottom Or Me.Orientation = WindowManagerOrientation.Top) Then
                    m_blankAreaRight.SetBounds(tabStripAdjustedMDIBounds.Right, myAdjustedMDIBounds.Bottom, myAdjustedMDIBounds.Right - tabStripAdjustedMDIBounds.Right, bottomBarrier - myAdjustedMDIBounds.Bottom)
                ElseIf Me.Orientation = WindowManagerOrientation.Left Then
                    m_blankAreaRight.SetBounds(myAdjustedMDIBounds.Right - 5, myAdjustedMDIBounds.Bottom, 5, bottomBarrier - myAdjustedMDIBounds.Bottom)
                End If
            Else
                UnloadRightBlankArea()
            End If
        Else
            UnloadLeftBlankArea()
            UnloadRightBlankArea()
        End If

    End Sub

    Private Sub UnloadLeftBlankArea()

        If Not m_blankAreaLeft Is Nothing Then
            m_blankAreaLeft.Dispose()
            m_blankAreaLeft = Nothing
        End If

    End Sub

    Private Sub UnloadRightBlankArea()

        If Not m_blankAreaRight Is Nothing Then
            m_blankAreaRight.Dispose()
            m_blankAreaRight = Nothing
        End If

    End Sub

    Private Sub AttemptAutoHide(Optional ByVal force As Boolean = False)

        If Me.Site Is Nothing OrElse Not Me.Site.DesignMode Then
            If m_autoHide AndAlso Not m_inAutoHideMode AndAlso m_nextWindowManagerPanel Is Nothing Then
                If GetAllWindows(False).Count = 0 Or force Then
                    Select Case m_orientation
                        Case WindowManagerOrientation.Bottom, WindowManagerOrientation.Top
                            m_sizeValueBeforeAutoHide = Me.Height
                            Me.Visible = False 'Me.Height = 0
                            m_inAutoHideMode = True
                        Case WindowManagerOrientation.Left, WindowManagerOrientation.Right
                            m_sizeValueBeforeAutoHide = Me.Width
                            Me.Visible = False 'Me.Width = 0
                            m_inAutoHideMode = True
                    End Select

                    ReconcileAuxiliaryWindowSize()
                End If
            End If
        End If

    End Sub

    Private Sub AttemptUndoAutoHide(Optional ByVal force As Boolean = False)

        If m_autoHide AndAlso m_inAutoHideMode Then
            If GetAllWindows(False).Count > 0 Or force Then
                Select Case m_orientation
                    Case WindowManagerOrientation.Bottom, WindowManagerOrientation.Top
                        Me.Visible = True 'Me.Height = m_iSizeValueBeforeShrink
                        m_inAutoHideMode = False
                    Case WindowManagerOrientation.Left, WindowManagerOrientation.Right
                        Me.Visible = True 'Me.Width = m_iSizeValueBeforeShrink
                        m_inAutoHideMode = False
                End Select

                ReconcileAuxiliaryWindowSize()
            End If
        End If

    End Sub

    Private Sub SetBestTabStripWidths(ByVal windowTabStrips As WindowTabStripCollection, ByVal canvasWidth As Integer)

        Dim totalWidth As Integer
        Dim correctForZeroWidths As Boolean = True

        For Each windowTabStrip As WindowTabStrip In windowTabStrips
            If windowTabStrip.Width = 0 Then
                correctForZeroWidths = True
            End If
        Next windowTabStrip

        If correctForZeroWidths Then
            For Each windowTabStrip As WindowTabStrip In windowTabStrips
                windowTabStrip.Width = CInt(Me.TabStripsContainer.ClientRectangle.Width / m_windowTabStrips.Count)
            Next windowTabStrip
        End If

        For Each windowTabStrip As WindowTabStrip In windowTabStrips
            totalWidth += windowTabStrip.Width
        Next windowTabStrip

        If totalWidth > 0 Then
            For Each windowTabStrip As WindowTabStrip In windowTabStrips
                windowTabStrip.Width = CInt(canvasWidth * (windowTabStrip.Width / totalWidth))
            Next windowTabStrip
        End If

    End Sub

    Private Sub SnapToSize()

        Try
            Dim parentForm As Form = GetMDIParent()
            Dim mdiClientControl As MdiClient = GetMDIClient()

            If Not mdiClientControl Is Nothing Then
                Dim mdiClientAdjustedBounds As Rectangle = parentForm.RectangleToClient(mdiClientControl.RectangleToScreen(mdiClientControl.ClientRectangle))

                Select Case m_orientation
                    Case WindowManagerOrientation.Bottom
                        Me.SetBounds(mdiClientAdjustedBounds.Left, 0, mdiClientAdjustedBounds.Width, 0, BoundsSpecified.X Or BoundsSpecified.Width)
                    Case WindowManagerOrientation.Top
                        Me.SetBounds(mdiClientAdjustedBounds.Left, mdiClientAdjustedBounds.Top, mdiClientAdjustedBounds.Width, 0, BoundsSpecified.X Or BoundsSpecified.Width Or BoundsSpecified.Y)
                    Case WindowManagerOrientation.Right
                        Me.Left = mdiClientAdjustedBounds.Right - Me.Width
                    Case WindowManagerOrientation.Left
                        Me.Left = mdiClientAdjustedBounds.Left
                End Select

                LayoutWindows()
            End If
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub AdjustForClientTop()

        Try
            Dim mdiClientControl As MdiClient = GetMDIClient()

            If Not mdiClientControl Is Nothing Then
                If Not m_minMode Then
                    If mdiClientControl.Location.Y <> m_originalMDIClientRect.Location.Y Then
                        Dim topDelta As Integer = mdiClientControl.Location.Y - m_originalMDIClientRect.Location.Y

                        Me.Top += topDelta
                    End If
                Else
                    If m_orientation = WindowManagerOrientation.Bottom AndAlso mdiClientControl.Height <> m_originalMDIClientRect.Height Then
                        Dim topDelta As Integer = mdiClientControl.Height - m_originalMDIClientRect.Height

                        Me.Top += topDelta
                    End If
                End If
            End If
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub AdjustForClientLeft()

        Try
            Dim mdiClientControl As MdiClient = GetMDIClient()

            If Not mdiClientControl Is Nothing Then
                If mdiClientControl.Location.X <> m_originalMDIClientRect.Location.X Then
                    Dim leftDelta As Integer = mdiClientControl.Location.X - m_originalMDIClientRect.Location.X

                    Me.Width -= leftDelta
                End If
            End If
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub AdjustForClientWidth()

        Dim mdiClientControl As MdiClient = GetMDIClient()

        If Not mdiClientControl Is Nothing Then
            Select Case m_orientation
                Case WindowManagerOrientation.Right, WindowManagerOrientation.Left
                    If m_originalMDIClientRect.Width <> mdiClientControl.Width Then
                        Dim delta As Integer = mdiClientControl.Width - m_originalMDIClientRect.Width

                        If Me.Width + delta < 40 Then
                            If Not m_minMode Then
                                Me.Width = 40
                            Else
                                m_topValueBeforeMinMode = 40
                            End If
                        Else
                            If Not m_minMode Then
                                Me.Width += delta
                            Else
                                m_topValueBeforeMinMode += delta
                            End If
                        End If
                    End If
            End Select
        End If

    End Sub

    Private Sub AdjustForClientHeight()

        Dim mdiClientControl As MdiClient = GetMDIClient()

        If Me.Top + Me.Height > mdiClientControl.Height Then
            Dim bottomBarrier As Integer = mdiClientControl.Height - Me.Height

            If Not bottomBarrier < mdiClientControl.Top Then
                Dim eventargs As RedoLayoutEventArgs

                eventargs = New RedoLayoutEventArgs(Me, mdiClientControl, True)
                eventargs.BottomBarrier = bottomBarrier

                OnRequestChainRedoLayout(eventargs)

                If Not eventargs.Cancel Then
                    Dim eventargs2 As RedoLayoutEventArgs

                    eventargs2 = New RedoLayoutEventArgs(Me, mdiClientControl, False)
                    eventargs2.BottomBarrier = bottomBarrier

                    OnRequestChainRedoLayout(eventargs2)

                    Me.Top = bottomBarrier
                End If
            End If
        End If

    End Sub

    Friend Sub ToggleMinModeForBottom()

        If Not m_minMode Then
            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.MinMode = True
            End If

            m_topValueBeforeMinMode = Me.Top
            SnapToMinimumSize(includeSubPanels:=False)

            If Me.Orientation <> WindowManagerOrientation.Left And Me.Orientation <> WindowManagerOrientation.Right Then
                m_minMode = True
            End If
        Else
            RaiseEvent RequestMinModeChange(Me, New RequestMinModeChangeEventArgs(False))

            m_minMode = False

            Me.Top = m_topValueBeforeMinMode
            LayoutWindows()

            If Not Me.NextSubPanel Is Nothing Then
                Me.NextSubPanel.MinMode = False
            End If
        End If

    End Sub

    Friend Sub ToggleMinModeForSides()

        If Me.Orientation = WindowManagerOrientation.Left Or Me.Orientation = WindowManagerOrientation.Right Then
            Dim eventargs As New GetTopPanelEventArgs

            OnRequestChainGetTopPanel(eventargs)

            If Not eventargs.TopPanel Is Nothing Then
                eventargs.TopPanel.ToggleMinModeForSides()
            Else
                Dim mdiParentForm As Form = GetMDIParent()
                Dim mdiClientControl As MdiClient = GetMDIClient()

                If Not m_minMode Then
                    m_minMode = True

                    Dim ctl As New WindowManagerPanelMinModeBar

                    ctl.MinModeButton.RotatedText = Me.Text
                    mdiParentForm.Controls.Add(ctl)
                    ctl.BringToFront()

                    Select Case Me.Orientation
                        Case WindowManagerOrientation.Right
                            ctl.Location = New Point(mdiClientControl.Right - ctl.Size.Width, Me.Location.Y)
                            ctl.Anchor = AnchorStyles.Right Or AnchorStyles.Top Or AnchorStyles.Bottom
                        Case WindowManagerOrientation.Left
                            ctl.Location = New Point(mdiClientControl.Left, Me.Location.Y)
                            ctl.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Bottom
                    End Select

                    ctl.Height = mdiClientControl.Bottom - Me.Location.Y

                    m_minModeBar = ctl

                    m_topValueBeforeMinMode = Me.Width
                    Me.Width = ctl.Width - 5

                    ctl.BringToFront()
                Else
                    m_minMode = False

                    If Not m_minModeBar Is Nothing Then
                        If mdiParentForm.Controls.Contains(m_minModeBar) Then
                            mdiParentForm.Controls.Remove(m_minModeBar)
                        End If

                        m_minModeBar.Dispose()
                        m_minModeBar = Nothing
                    End If

                    Me.Width = m_topValueBeforeMinMode
                End If
            End If
        End If

    End Sub

    Private Sub SnapToDesignSize()

        If Me.Orientation = WindowManagerOrientation.Left Or Me.Orientation = WindowManagerOrientation.Right Then
            Dim mdiClientControl As MdiClient = GetMDIClient()

            If Not mdiClientControl Is Nothing Then
                Dim mdiClientAdjustedBounds As Rectangle = ParentForm.RectangleToClient(mdiClientControl.RectangleToScreen(mdiClientControl.ClientRectangle))

                Me.Top = mdiClientAdjustedBounds.Top
                Me.Width = CInt(mdiClientAdjustedBounds.Width / 4)
            End If
        ElseIf Me.Orientation = WindowManagerOrientation.Top Then
            Dim mdiClientControl As MdiClient = GetMDIClient()

            If Not mdiClientControl Is Nothing Then
                Dim mdiClientAdjustedBounds As Rectangle = ParentForm.RectangleToClient(mdiClientControl.RectangleToScreen(mdiClientControl.ClientRectangle))

                'Dim tempVal As Boolean = m_allowUserVerticalRepositioning

                'm_allowUserVerticalRepositioning = True
                Me.Top = mdiClientAdjustedBounds.Top
                'm_allowUserVerticalRepositioning = tempVal
            End If
        End If

    End Sub

    Private Sub ReconcileAuxiliaryWindowSize()

        Dim mdiParentForm As Form = GetMDIParent()
        Dim mdiClientControl As MdiClient = GetMDIClient()

        If Not mdiClientControl Is Nothing Then
            If Not m_auxiliaryWindow Is Nothing Then
                Dim myAdjustedMDIBounds As Rectangle = mdiClientControl.RectangleToClient(mdiParentForm.RectangleToScreen(Me.Bounds))

                Select Case m_orientation
                    Case WindowManagerOrientation.Bottom
                        If Not m_inAutoHideMode Then
                            m_auxiliaryWindow.SetBounds(0, 0, mdiClientControl.ClientSize.Width, myAdjustedMDIBounds.Top)
                        Else
                            m_auxiliaryWindow.SetBounds(0, 0, mdiClientControl.ClientSize.Width, mdiClientControl.ClientSize.Height)
                        End If
                    Case WindowManagerOrientation.Right
                        m_auxiliaryWindow.SetBounds(0, 0, myAdjustedMDIBounds.Left, mdiClientControl.ClientSize.Height)
                    Case WindowManagerOrientation.Left
                        m_auxiliaryWindow.SetBounds(myAdjustedMDIBounds.Width, 0, mdiClientControl.ClientSize.Width - myAdjustedMDIBounds.Width, mdiClientControl.ClientSize.Height)
                    Case WindowManagerOrientation.Top
                        'not doing anything special because we're not expecting an Aux Window when orientation=top
                        'm_auxiliaryWindow.SetBounds(myAdjustedMDIBounds.Width, 0, mdiClientControl.ClientSize.Width - myAdjustedMDIBounds.Width, mdiClientControl.ClientSize.Height)
                End Select
            End If
        End If

    End Sub

    Private Sub ArrangeWindowActionButtons()

        'Dim buttons As Control() = {Me.CloseWindowButton, Me.PopoutWindowButton, Me.TileWindowButton, Me.HTileButton}
        'Dim pos As Integer = Me.WindowButtonsPanel.ClientSize.Width
        'Dim buttonsWidth As Integer = 0

        'Me.WindowButtonsPanel.Visible = True 'must do this because for some dumb reason the buttons' visible property isn't accurately reflected by the framework otherwise

        'Me.CloseWindowButton.Visible = m_windowCloseButtonVisible
        'Me.PopoutWindowButton.Visible = m_windowLayoutButtonsVisible
        'Me.TileWindowButton.Visible = m_windowLayoutButtonsVisible
        'Me.HTileButton.Visible = m_windowLayoutButtonsVisible

        'For Each button As Control In buttons
        '    If button.Visible Then
        '        button.Left = pos - button.Width
        '        buttonsWidth += button.Width
        '        pos -= button.Width
        '    End If
        'Next button

        'Me.WindowButtonsPanel.Width = buttonsWidth
        'Me.WindowButtonsPanel.Left = Me.TitlePanel.Right - Me.WindowButtonsPanel.Width - 1

        'Me.WindowButtonsPanel.Visible = Not (buttonsWidth = 0)

    End Sub

#End Region

#Region "Protected"

    Protected Overridable Sub OnWindowTiling(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent WindowTiling(Me, e)

    End Sub

    Protected Overridable Sub OnWindowUnTiling(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent WindowUnTiling(Me, e)

    End Sub

    Protected Overridable Sub OnWindowHTiling(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent WindowHTiling(Me, e)

    End Sub

    Protected Overridable Sub OnWindowPoppingOut(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent WindowPoppingOut(Me, e)

    End Sub

    Protected Overridable Sub OnWindowPoppedOut(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowPoppedOut(Me, e)

    End Sub

    Protected Overridable Sub OnWindowPoppingIn(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent WindowPoppingIn(Me, e)

    End Sub

    Protected Overridable Sub OnWindowPoppedIn(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowPoppedIn(Me, e)

    End Sub

    Protected Overridable Sub OnBeforeWrappedWindowAdded(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent BeforeWrappedWindowAdded(Me, e)

    End Sub

    Protected Overridable Sub OnWrappedWindowAdded(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WrappedWindowAdded(Me, e)

    End Sub

    Protected Overridable Sub OnBeforeWrappedWindowRemoved(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent BeforeWrappedWindowRemoved(Me, e)

    End Sub

    Protected Overridable Sub OnWrappedWindowRemoved(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WrappedWindowRemoved(Me, e)

    End Sub

    Protected Overridable Sub OnWrappedWindowsCleared(ByVal e As System.EventArgs)

        RaiseEvent WrappedWindowsCleared(Me, e)

    End Sub

    Protected Overridable Sub OnWindowClosing(ByVal e As WrappedWindowClosingEventArgs)

        RaiseEvent WindowClosing(Me, e)

    End Sub

    Protected Overridable Sub OnWindowClosed(ByVal e As WrappedWindowClosedEventArgs)

        If Not Not m_isTemporaryWindowManagerPanel Then
            RemoveWindowFromHistory(e.WrappedWindow)
        End If

        RaiseEvent WindowClosed(Me, e)

    End Sub

    Protected Overridable Sub OnWindowActivated(ByVal e As WrappedWindowEventArgs)

        If Not m_isTemporaryWindowManagerPanel Then
            AddWindowToHistory(e.WrappedWindow)
        End If

        RaiseEvent WindowActivated(Me, e)

    End Sub

    Protected Overridable Sub OnWindowDeactivate(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowDeactivate(Me, e)

    End Sub

    Protected Overridable Sub OnWindowEnter(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowEnter(Me, e)

    End Sub

    Protected Overridable Sub OnWindowLeave(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowLeave(Me, e)

    End Sub

    Protected Overridable Sub OnWindowTextChanged(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowTextChanged(Me, e)

    End Sub

    Protected Overridable Sub OnWindowVisibleChanged(ByVal e As WrappedWindowEventArgs)

        RaiseEvent WindowVisibleChanged(Me, e)

    End Sub

    Protected Overridable Sub OnTempPanelDismissed(ByVal e As System.EventArgs)

        RaiseEvent TempPanelDismissed(Me, e)

    End Sub

    Protected Overridable Sub OnRequestChainRedoLayout(ByVal e As RedoLayoutEventArgs)

        RaiseEvent RequestChainRedoLayout(Me, e)

    End Sub

    Protected Overridable Sub OnRequestChainGetTop(ByVal e As GetTopEventArgs)

        RaiseEvent RequestChainGetTop(Me, e)

    End Sub

    Protected Overridable Sub OnRequestChainGetTopPanel(ByVal e As GetTopPanelEventArgs)

        RaiseEvent RequestChainGetTopPanel(Me, e)

    End Sub

    Protected Overridable Sub OnIsActiveChanged(ByVal e As System.EventArgs)

        RaiseEvent IsActiveChanged(Me, e)

    End Sub

    Protected Overridable Sub OnNewCustomTabsProviderInstance(ByVal e As NewTabsProviderInstanceCreatedEventArgs)

        RaiseEvent NewCustomTabsProviderInstance(Me, e)

    End Sub

#End Region

#Region "Event Handlers"

    Private Sub WindowManagerPanel_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        m_isLoaded = True

        If m_autoHide Then
            AttemptAutoHide()
        Else
            SnapToSize()
        End If

        'DesignerNote1Label.Visible = Me.DesignMode

        If m_enableAeroGlass Then
            'WinAPI.SetAeroGlass(Me)
        End If

    End Sub

    Private Sub WindowManagerPanel_BackColorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.BackColorChanged

        Me.WindowManagerSplitter1.BackColor = Me.BackColor
        Me.WindowManagerSplitter2.BackColor = Me.BackColor

    End Sub

    Private Sub WindowManagerPanel_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize

        'I don't think we really need to enforce this
        'If Me.Width < MINIMUM_PANEL_WIDTH Then
        'Me.Width = MINIMUM_PANEL_WIDTH
        'End If

        Dim mdiParentForm As Form = GetMDIParent()

        If Not mdiParentForm Is Nothing Then
            If mdiParentForm.WindowState <> FormWindowState.Minimized Then
                SnapToSize()
                ReconcileAuxiliaryWindowSize()
            End If
        End If

        'Me.TabStripsContainer.SetBounds(0, 0, Me.ClientSize.Width - Me.WindowManageButton.Width, Me.ClientSize.Height)
        Me.TabStripsContainer.Width = Me.ClientSize.Width - Me.WindowManageButton.Width

    End Sub

    Private Sub WindowManagerPanel_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.LocationChanged

        SnapToSize()
        ReconcileAuxiliaryWindowSize()

    End Sub

    Private Sub WindowManagerPanel_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.SizeChanged

        Try
            If Not m_nextWindowManagerPanel Is Nothing Then
                m_nextWindowManagerPanel.Size = Me.Size
            End If
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub WindowManagerPanel_Enter(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Enter

        'todo: set focus to selected window here perhaps?
        'Me.IsActive = True
        TrackActiveMdiChild()

    End Sub

    Private Sub WindowManagerPanel_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Leave

        'Me.IsActive = False
        TrackActiveMdiChild()

    End Sub

    Private Sub WindowManagerPanel_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.VisibleChanged

        Try
            If Not m_blankAreaLeft Is Nothing Then
                m_blankAreaLeft.Visible = Me.Visible
            End If

            If Not m_blankAreaRight Is Nothing Then
                m_blankAreaRight.Visible = Me.Visible
            End If

            If Me.Visible Then
                SnapToSize()
            End If
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub WindowManagerPanel_ParentChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.ParentChanged

        UnloadLeftBlankArea()
        UnloadRightBlankArea()

        RemoveParentHandlers()

        If Not Me.Parent Is Nothing Then
            If TypeOf Me.Parent Is Form Then
                m_parentForm = DirectCast(Me.Parent, Form)
                AddParentHandlers()
            Else
                Throw New System.NotSupportedException("Parent must be a Windows Form class.")
            End If
        End If

    End Sub

    Private Sub HandleWindowTabStripItemsFirstAdded(ByVal sender As Object, ByVal e As System.EventArgs)

        AttemptUndoAutoHide()

    End Sub

    Private Sub HandleWindowTabStripItemsCleared(ByVal sender As Object, ByVal e As System.EventArgs)

        m_windowTabStrips.Remove(CType(sender, WindowTabStrip))

        AttemptAutoHide()

    End Sub

    Private Sub HandleWindowTabStripEnter(ByVal sender As Object, ByVal e As System.EventArgs)

        'Me.IsActive = True
        Me.SelectedTabStrip = CType(sender, WindowTabStrip)
        TrackActiveMdiChild()

    End Sub

    Private Sub HandleWindowTabStripLeave(ByVal sender As Object, ByVal e As System.EventArgs)

        'Me.IsActive = False
        'Me.ActiveWindowTabStrip = Nothing 'let's not do this. I think we'd like to remember the last one for now
        TrackActiveMdiChild()

    End Sub

    Private Sub HandleWindowTabStripNewSplitSize(ByVal sender As Object, ByVal e As WindowTabStrip.WindowTabStripNewSplitSizeEventArgs)

        Dim windowTabStrip As WindowTabStrip = CType(sender, WindowTabStrip)
        Dim windowTabStripIndex As Integer = m_windowTabStrips.IndexOf(windowTabStrip)

        If windowTabStripIndex <> m_windowTabStrips.Count - 1 Then
            Dim windowTabStrip2 As WindowTabStrip = m_windowTabStrips.Item(windowTabStripIndex + 1)

            If windowTabStrip.Width + e.NewSizeDelta > windowTabStrip.ResizeSplitter.Width And windowTabStrip2.Width - e.NewSizeDelta > windowTabStrip2.ResizeSplitter.Width Then
                windowTabStrip.Width += e.NewSizeDelta
                windowTabStrip2.Width -= e.NewSizeDelta
            End If

            LayoutTabStrips(dontFitTabStrips:=True)
            LayoutWindows()
        End If

    End Sub

    Private Sub HandleWindowTabStripSelectedItemChanged(ByVal sender As Object, ByVal e As SelectedWrappedWindowChangedEventArgs)

        If Not e.SelectedWrappedWindow Is Nothing AndAlso Not e.SelectedWrappedWindow.Window Is Nothing AndAlso Not e.SelectedWrappedWindow.Window.IsDisposed Then
            Dim mdiParentForm As Form = GetMDIParent()

            If Not e.SelectedWrappedWindow.Window.MdiParent Is mdiParentForm Then
                If Not e.SelectedWrappedWindow.Window.MdiParent Is mdiParentForm Then
                    e.SelectedWrappedWindow.Window.MdiParent = mdiParentForm
                End If
            End If

            'e.SelectedWrappedWindow.Window.SetBounds(0 - e.SelectedWrappedWindow.Window.Width, 0 - e.SelectedWrappedWindow.Window.Height, 0, 0, BoundsSpecified.Location)
            CType(sender, WindowTabStrip).BeginUpdate()
            ActivateWindow(e.SelectedWrappedWindow.Window)
            LayoutWindows()
            CType(sender, WindowTabStrip).EndUpdate()
        End If

        Me.MinMode = False

    End Sub

    Private Sub HandleWindowTabStripSelectedItemReselected(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim wrappedWindow As WrappedWindow = CType(sender, WindowTabStrip).SelectedItem

        If Not wrappedWindow Is Nothing AndAlso Not wrappedWindow.Window Is Nothing AndAlso Not wrappedWindow.Window.IsDisposed Then
            CType(sender, WindowTabStrip).BeginUpdate()
            ActivateWindow(wrappedWindow.Window)
            LayoutWindows()
            CType(sender, WindowTabStrip).EndUpdate()
        End If

        Me.MinMode = False

    End Sub

    Private Sub HandleWindowTabStripItemTabDoubleClicked(ByVal sender As Object, ByVal e As WrappedWindowItemEventArgs)

        PopOutWrappedWindow(e.WrappedWindow)

    End Sub

    Private Sub HandleWindowTabStripRequestPreviousWindowSelect(ByVal sender As Object, ByVal e As WrappedWindowItemEventArgs)

        Dim windowTabStrip As WindowTabStrip = CType(sender, WindowTabStrip)

        If windowTabStrip.Items.Count > 0 Then
            Dim wrappedWindow As WrappedWindow = Me.GetTabStripPreviousWrappedWindow(windowTabStrip)

            If Not wrappedWindow Is Nothing Then
                windowTabStrip.SelectedItem = wrappedWindow
            Else
                windowTabStrip.SelectedItem = windowTabStrip.Items.Item(0)
                'todo: IN ADDITION we may want to set focus on next window in back history
            End If
        Else
            'todo: we may want to set focus on next window in back history
        End If

    End Sub

    Private Sub HandleWindowTabStripTabPaint(ByVal sender As Object, ByVal e As StandardTabsProvider.TabPaintEventArgs)

        RaiseEvent TabPaint(Me, e)

    End Sub

    Private Sub MDIClient_RepositionAndResize(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_mdiClientControl.Resize, m_mdiClientControl.LocationChanged

        Dim mdiParentForm As Form = GetMDIParent()

        If Not mdiParentForm Is Nothing Then
            If mdiParentForm.WindowState <> FormWindowState.Minimized Then
                'adjustments must occur in this order
                AdjustForClientWidth()
                SnapToSize()
                AdjustForClientTop()
                AdjustForClientLeft()
                AdjustForClientHeight()
                ReconcileAuxiliaryWindowSize()

                Dim mdiClient As MdiClient = GetMDIClient()

                If Not mdiClient Is Nothing Then
                    m_originalMDIClientRect = mdiClient.Bounds
                End If
            End If
        End If

    End Sub

    Private Sub HandleWindowTabStripBeginDragItem(ByVal sender As Object, ByVal e As WrappedWindowItemEventArgs)

        Dim windowTabStrip As WindowTabStrip = CType(sender, WindowTabStrip)
        Dim data As New WrappedWindowDragDropData(Me, windowTabStrip, e.WrappedWindow)

        windowTabStrip.DoDragDrop(data, DragDropEffects.Move)

    End Sub

    Public Sub MoveWindowToMainPanelAndPopOut(ByVal wrappedWindow As WrappedWindow)

        Dim mainWindowManagerPanel As WindowManagerPanel = GetPrimaryPanel()

        If Not mainWindowManagerPanel Is Nothing Then
            Dim sourceWindowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(wrappedWindow)

            If Not sourceWindowTabStrip Is Nothing Then
                Dim targetWindowTabStrip As WindowTabStrip = mainWindowManagerPanel.TabStrips.Item(0)

                If targetWindowTabStrip IsNot Nothing Then
                    sourceWindowTabStrip.Items.Remove(wrappedWindow)
                    targetWindowTabStrip.Items.Add(wrappedWindow)
                    'mainWindowManagerPanel.SetActiveWindow(wrappedWindow)
                    mainWindowManagerPanel.PopOutWrappedWindow(wrappedWindow)
                End If
            End If
        End If

    End Sub

    Private Sub HandleWindowTabStripItemDropped(ByVal sender As Object, ByVal e As DroppedWrappedWindowEventArgs)

        Dim windowTabStrip As WindowTabStrip = CType(sender, WindowTabStrip)
        Dim wrappedWindow As WrappedWindow = e.Data.WrappedWindow
        Dim sourceWindowTabStrip As WindowTabStrip = e.Data.WindowTabStrip

        sourceWindowTabStrip.Items.Remove(wrappedWindow)
        windowTabStrip.Items.Add(wrappedWindow)

        SetActiveWindow(wrappedWindow)

    End Sub

    Private Sub HandleWindowTabStripShowWindowMenuRequested(ByVal sender As Object, ByVal e As System.EventArgs)

        If Not (m_disableTileAction AndAlso m_disableHTileAction AndAlso m_disablePopoutAction AndAlso m_disableCloseAction) Then
            WindowContextMenu.Show(Me, Me.PointToClient(System.Windows.Forms.Cursor.Position))
        End If

    End Sub

    Private Sub WindowTabStrips_TabStripAdded(ByVal sender As Object, ByVal e As WindowTabStripCollection.ItemsEventArgs) Handles m_windowTabStrips.TabStripAdded

        Me.TabStripsContainer.Controls.Add(e.WindowTabStrip)

        With e.WindowTabStrip
            .Width = CInt(Me.TabStripsContainer.ClientRectangle.Width / m_windowTabStrips.Count)

            AddHandler .Enter, AddressOf HandleWindowTabStripEnter
            AddHandler .Leave, AddressOf HandleWindowTabStripLeave
            AddHandler .SelectedItemChanged, AddressOf HandleWindowTabStripSelectedItemChanged
            AddHandler .SelectedItemReselected, AddressOf HandleWindowTabStripSelectedItemReselected
            AddHandler .ItemsFirstAdded, AddressOf HandleWindowTabStripItemsFirstAdded
            AddHandler .ItemsCleared, AddressOf HandleWindowTabStripItemsCleared
            AddHandler .NewSplitSize, AddressOf HandleWindowTabStripNewSplitSize
            AddHandler .BeginDragItem, AddressOf HandleWindowTabStripBeginDragItem
            AddHandler .ItemDropped, AddressOf HandleWindowTabStripItemDropped
            AddHandler .ShowWindowMenuRequested, AddressOf HandleWindowTabStripShowWindowMenuRequested
            AddHandler .ItemTabDoubleClicked, AddressOf HandleWindowTabStripItemTabDoubleClicked
            AddHandler .RequestPreviousWindowSelect, AddressOf HandleWindowTabStripRequestPreviousWindowSelect
            AddHandler .TabPaint, AddressOf HandleWindowTabStripTabPaint

            AddHandler .Items.BeforeWrappedWindowAdded, AddressOf HandleBeforeWrappedWindowAdded
            AddHandler .Items.BeforeWrappedWindowRemoved, AddressOf HandleBeforeWrappedWindowRemoved
            AddHandler .Items.WrappedWindowAdded, AddressOf HandleWrappedWindowAdded
            AddHandler .Items.WrappedWindowRemoved, AddressOf HandleWrappedWindowRemoved
            AddHandler .Items.WrappedWindowsCleared, AddressOf HandleWrappedWindowsCleared
            AddHandler .Items.WindowActivated, AddressOf HandleWindowActivated
            AddHandler .Items.WindowClosed, AddressOf HandleWindowClosed
            AddHandler .Items.WindowClosing, AddressOf HandleWindowClosing
            AddHandler .Items.WindowDeactivate, AddressOf HandleWindowDeactivate
            AddHandler .Items.WindowEnter, AddressOf HandleWindowEnter
            AddHandler .Items.WindowLeave, AddressOf HandleWindowLeave
            AddHandler .Items.WindowTextChanged, AddressOf HandleWindowTextChanged
            AddHandler .Items.WindowVisibleChanged, AddressOf HandleWindowVisibleChanged
        End With

        LayoutTabStrips()

    End Sub

    Private Sub WindowTabStrips_BeforeTabStripRemoved(ByVal sender As Object, ByVal e As WindowTabStripCollection.ItemsCancelEventArgs) Handles m_windowTabStrips.BeforeTabStripRemoved

        If m_windowTabStrips.Count = 1 And Not m_isTemporaryWindowManagerPanel Then
            e.Cancel = True
        End If

    End Sub

    Private Sub WindowTabStrips_TabStripRemoved(ByVal sender As Object, ByVal e As WindowTabStripCollection.ItemsEventArgs) Handles m_windowTabStrips.TabStripRemoved, m_windowTabStrips.TabStripClearWarn

        With e.WindowTabStrip
            RemoveHandler .Enter, AddressOf HandleWindowTabStripEnter
            RemoveHandler .Leave, AddressOf HandleWindowTabStripLeave
            RemoveHandler .SelectedItemChanged, AddressOf HandleWindowTabStripSelectedItemChanged
            RemoveHandler .SelectedItemReselected, AddressOf HandleWindowTabStripSelectedItemReselected
            RemoveHandler .ItemsFirstAdded, AddressOf HandleWindowTabStripItemsFirstAdded
            RemoveHandler .ItemsCleared, AddressOf HandleWindowTabStripItemsCleared
            RemoveHandler .NewSplitSize, AddressOf HandleWindowTabStripNewSplitSize
            RemoveHandler .BeginDragItem, AddressOf HandleWindowTabStripBeginDragItem
            RemoveHandler .ItemDropped, AddressOf HandleWindowTabStripItemDropped
            RemoveHandler .ShowWindowMenuRequested, AddressOf HandleWindowTabStripShowWindowMenuRequested
            RemoveHandler .ItemTabDoubleClicked, AddressOf HandleWindowTabStripItemTabDoubleClicked
            RemoveHandler .RequestPreviousWindowSelect, AddressOf HandleWindowTabStripRequestPreviousWindowSelect
            RemoveHandler .TabPaint, AddressOf HandleWindowTabStripTabPaint

            RemoveHandler .Items.BeforeWrappedWindowAdded, AddressOf HandleBeforeWrappedWindowAdded
            RemoveHandler .Items.BeforeWrappedWindowRemoved, AddressOf HandleBeforeWrappedWindowRemoved
            RemoveHandler .Items.WrappedWindowAdded, AddressOf HandleWrappedWindowAdded
            RemoveHandler .Items.WrappedWindowRemoved, AddressOf HandleWrappedWindowRemoved
            RemoveHandler .Items.WrappedWindowsCleared, AddressOf HandleWrappedWindowsCleared
            RemoveHandler .Items.WindowActivated, AddressOf HandleWindowActivated
            RemoveHandler .Items.WindowClosed, AddressOf HandleWindowClosed
            RemoveHandler .Items.WindowClosing, AddressOf HandleWindowClosing
            RemoveHandler .Items.WindowDeactivate, AddressOf HandleWindowDeactivate
            RemoveHandler .Items.WindowEnter, AddressOf HandleWindowEnter
            RemoveHandler .Items.WindowLeave, AddressOf HandleWindowLeave
            RemoveHandler .Items.WindowTextChanged, AddressOf HandleWindowTextChanged
            RemoveHandler .Items.WindowVisibleChanged, AddressOf HandleWindowVisibleChanged

            Me.TabStripsContainer.Controls.Remove(e.WindowTabStrip)

            Dim mdiParentForm As Form = GetMDIParent()

            If Not mdiParentForm Is Nothing Then
                If mdiParentForm.Controls.Contains(.ResizeSplitter) Then
                    .ResizeSplitter.Visible = False
                    mdiParentForm.Controls.Remove(.ResizeSplitter)
                End If
            End If
        End With

        If Me.SelectedTabStrip Is e.WindowTabStrip Then
            If Not Me.TabStrips.Count = 0 Then
                Me.SelectedTabStrip = Me.TabStrips.Item(0)
            Else
                Me.SelectedTabStrip = Nothing
            End If
        End If

        LayoutTabStrips()
        LayoutWindows()

    End Sub

    Private Sub WindowTabStrips_TabStripsCleared(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_windowTabStrips.TabStripsCleared

        Me.TabStripsContainer.Controls.Clear()
        LayoutTabStrips()
        LayoutWindows()

        If m_isTemporaryWindowManagerPanel Then
            OnTempPanelDismissed(System.EventArgs.Empty)
            Me.Parent.Controls.Remove(Me)
            Me.Dispose()
        Else
            InitializePrimaryTabStrip()
        End If

    End Sub

    Private Sub HandleBeforeWrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsCancelEventArgs)

        If Not e.Cancel Then
            Dim wrappedWindow As WrappedWindow = GetWrapperForWindow(e.WrappedWindow.Window)

            If Not Me.IsTemporaryPanel AndAlso Not wrappedWindow Is Nothing Then
                'todo: why is this here?
                'this is a check to make sure to make sure the window isn't already being handled
                'maybe we should throw an exception instead?
                e.Cancel = True
            Else
                Dim eventargs As New WrappedWindowCancelEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow, False), Me)
                OnBeforeWrappedWindowAdded(eventargs)
                e.Cancel = eventargs.Cancel
            End If
        End If

    End Sub

    Private Sub HandleWrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWrappedWindowAdded(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleBeforeWrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsCancelEventArgs)

        If Not e.Cancel Then
            Dim eventargs As New WrappedWindowCancelEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me)

            OnBeforeWrappedWindowRemoved(eventargs)

            e.Cancel = eventargs.Cancel
        End If

    End Sub

    Private Sub HandleWrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWrappedWindowRemoved(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWrappedWindowsCleared(ByVal sender As Object, ByVal e As System.EventArgs)

        OnWrappedWindowsCleared(EventArgs.Empty)

    End Sub

    Private Sub HandleWindowClosing(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsClosingEventArgs)

        If Not e.Cancel Then
            Dim eventargs As New WrappedWindowClosingEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me, e.CloseReason)

            OnWindowClosing(eventargs)

            e.Cancel = eventargs.Cancel
        End If

    End Sub

    Private Sub HandleWindowClosed(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsClosedEventArgs)

        OnWindowClosed(New WrappedWindowClosedEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me, e.CloseReason))

    End Sub

    Private Sub HandleWindowActivated(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWindowActivated(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWindowDeactivate(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWindowDeactivate(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWindowEnter(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWindowEnter(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWindowLeave(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWindowLeave(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWindowTextChanged(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWindowTextChanged(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWindowVisibleChanged(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs)

        OnWindowVisibleChanged(New WrappedWindowEventArgs(e.WrappedWindow, GetWrappedWindowTabStrip(e.WrappedWindow), Me))

    End Sub

    Private Sub HandleWindowMenuItemClick(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim mnu As WrappedWindowMenuItem = CType(sender, WrappedWindowMenuItem)

        If Not mnu.WrappedWindow Is Nothing Then
            If Not mnu.WrappedWindow.Window Is Nothing Then
                ActivateWindow(mnu.WrappedWindow.Window)
            End If
        End If

    End Sub

    Private Sub HandleParentMdiChildActivate(ByVal sender As Object, ByVal e As System.EventArgs)

        'todo: We might also achieve the same functionality and reduce flicker if we 
        'use events of the hidden magical MDIClient control (such as moving the window
        'out of the visible area as soon as its added and before its shown). However,
        'this seems kludgy to me and subject to future incompatabilities. Will investigate further.

        If Not m_isTemporaryWindowManagerPanel Then
            If m_autoDetectMdiChildWindows Then
                Try
                    Dim mdiChildForm As Form = m_parentForm.ActiveMdiChild

                    If Not mdiChildForm Is Nothing AndAlso Not mdiChildForm.IsDisposed AndAlso Not mdiChildForm Is Me.AuxiliaryWindow AndAlso Not TypeOf mdiChildForm Is DummyForm Then
                        Dim wrappedWindow As WrappedWindow = GetWrapperForWindow(mdiChildForm)

                        If wrappedWindow Is Nothing Then
                            wrappedWindow = New WrappedWindow(mdiChildForm)
                            mdiChildForm.SetBounds(0 - mdiChildForm.Width, 0 - mdiChildForm.Height, 0, 0, BoundsSpecified.Location)
                            _AddWindow(wrappedWindow)
                            '_AddWindow(mdiChildForm)
                            SetActiveWindow(mdiChildForm)
                        Else
                            SetActiveWindow(wrappedWindow)
                        End If
                    End If
                Catch
                    'do nothing
                End Try
            End If

            TrackActiveMdiChild()
        End If

    End Sub

    Private Sub HandleParentFormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs)

        CleanUp()

    End Sub

    Private Sub WindowManagerSplitter1_NewSize(ByVal sender As System.Object, ByVal e As NewSizeEventArgs) Handles WindowManagerSplitter1.NewSize

        If m_allowUserVerticalRepositioning Then
            Dim mdiParentForm As Form = GetMDIParent()
            Dim mdiClientControl As MdiClient = GetMDIClient()
            Dim adjustedBounds As Rectangle = mdiParentForm.RectangleToClient(e.Rectangle)

            With adjustedBounds
                If .Y < mdiClientControl.Top Then
                    .Y = mdiClientControl.Top
                End If

                Dim bottomBarrier As Integer

                If m_nextWindowManagerPanel Is Nothing Then
                    bottomBarrier = mdiClientControl.Bottom
                Else
                    bottomBarrier = m_nextWindowManagerPanel.Top
                End If

                If .Y + Me.Height > bottomBarrier Then
                    .Y = bottomBarrier - Me.Height
                End If

                If Me.Top <> .Y Then
                    m_minMode = False

                    Me.Top = .Y

                    LayoutWindows()
                End If
            End With
        End If

    End Sub

    Private Sub TabStripsContainer_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabStripsContainer.Resize

        Dim mdiParentForm As Form = GetMDIParent()

        If Not mdiParentForm Is Nothing Then
            If mdiParentForm.WindowState <> FormWindowState.Minimized Then
                LayoutTabStrips()
                LayoutWindows()
            End If
        End If

    End Sub

    Private Sub m_nextWindowManagerPanel_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_nextWindowManagerPanel.LocationChanged

        If m_nextWindowManagerPanel.Top < Me.Bottom Then
            m_nextWindowManagerPanel.Top = Me.Bottom
        End If

        LayoutWindows()

    End Sub

    Private Sub m_nextWindowManagerPanel_TempPanelDismissed(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_nextWindowManagerPanel.TempPanelDismissed

        Me.NextSubPanel = m_nextWindowManagerPanel.NextSubPanel
        LayoutTabStrips()
        LayoutWindows()

        If Not m_isTemporaryWindowManagerPanel Then
            AttemptAutoHide()
        End If

        If Not Me.GetActiveWindow() Is Nothing Then
            ActivateWindow(Me.GetActiveWindow().Window)
        End If

    End Sub

    Private Sub m_nextWindowManagerPanel_RequestChainGetTop(ByVal sender As Object, ByVal e As GetTopEventArgs) Handles m_nextWindowManagerPanel.RequestChainGetTop

        e.AbsoluteTop = Me.Top

        OnRequestChainGetTop(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_SizeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_nextWindowManagerPanel.SizeChanged

        Me.Size = m_nextWindowManagerPanel.Size

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowActivated(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowActivated

        OnWindowActivated(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowClosed(ByVal sender As Object, ByVal e As WrappedWindowClosedEventArgs) Handles m_nextWindowManagerPanel.WindowClosed

        OnWindowClosed(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowClosing(ByVal sender As Object, ByVal e As WrappedWindowClosingEventArgs) Handles m_nextWindowManagerPanel.WindowClosing

        OnWindowClosing(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowDeactivate(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowDeactivate

        OnWindowDeactivate(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowEnter(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowEnter

        OnWindowEnter(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowHTiling(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.WindowHTiling

        OnWindowHTiling(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowLeave(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowLeave

        OnWindowLeave(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowPoppingIn(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.WindowPoppingIn

        OnWindowPoppingIn(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowPoppedIn(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowPoppedIn

        OnWindowPoppedIn(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowPoppingOut(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.WindowPoppingOut

        OnWindowPoppingOut(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowPoppedOut(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowPoppedOut

        OnWindowPoppedOut(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowTextChanged(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowTextChanged

        OnWindowTextChanged(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowTiling(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.WindowTiling

        OnWindowTiling(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowUnTiling(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.WindowUnTiling

        OnWindowUnTiling(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WindowVisibleChanged(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WindowVisibleChanged

        OnWindowVisibleChanged(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WrappedWindowAdded

        OnWrappedWindowAdded(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowEventArgs) Handles m_nextWindowManagerPanel.WrappedWindowRemoved

        OnWrappedWindowRemoved(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_WrappedWindowsCleared(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_nextWindowManagerPanel.WrappedWindowsCleared

        OnWrappedWindowsCleared(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_BeforeWrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.BeforeWrappedWindowRemoved

        OnBeforeWrappedWindowRemoved(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_BeforeWrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs) Handles m_nextWindowManagerPanel.BeforeWrappedWindowAdded

        OnBeforeWrappedWindowAdded(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_NewTabsProviderInstanceCreated(ByVal sender As Object, ByVal e As NewTabsProviderInstanceCreatedEventArgs) Handles m_nextWindowManagerPanel.NewCustomTabsProviderInstance

        OnNewCustomTabsProviderInstance(e)

    End Sub

    Private Sub m_nextWindowManagerPanel_TabPaint(ByVal sender As Object, ByVal e As StandardTabsProvider.TabPaintEventArgs) Handles m_nextWindowManagerPanel.TabPaint

        'simply bubble up the event
        RaiseEvent TabPaint(Me, e)

    End Sub

    Private Sub m_nextWindowManagerPanel_RequestChainRedoLayout(ByVal sender As Object, ByVal e As RedoLayoutEventArgs) Handles m_nextWindowManagerPanel.RequestChainRedoLayout

        Dim newTop As Integer = Me.Top

        If Me.Top + Me.Height > e.BottomBarrier Then
            newTop = e.BottomBarrier - Me.Height
        End If

        If e.MDIClient.PointToClient(Me.Parent.PointToScreen(New Point(Me.Left, newTop))).Y < -2 Then
            e.Cancel = True
        Else
            e.BottomBarrier = newTop
            OnRequestChainRedoLayout(e)

            If Not e.Test Then
                Me.Top = newTop
            End If
        End If

    End Sub

    Private Sub m_nextWindowManagerPanel_RequestMinModeChange(ByVal sender As Object, ByVal e As RequestMinModeChangeEventArgs) Handles m_nextWindowManagerPanel.RequestMinModeChange

        Me.MinMode = e.MinModeValue

    End Sub

    Private Sub OrientationSplitters_ResizePending(ByVal sender As Object, ByVal e As BeforeSizeEventArgs) Handles m_blankAreaLeft.ResizePending, m_blankAreaRight.ResizePending, WindowManagerSplitter2.BeforeSize

        Dim eventargs As New GetTopEventArgs

        eventargs.AbsoluteTop = Me.Top
        OnRequestChainGetTop(eventargs)

        Dim point As Point = GetMDIParent.PointToScreen(New Point(0, eventargs.AbsoluteTop))
        point.X = e.Rectangle.X

        Dim mdiClientControl As MdiClient = GetMDIClient()

        Dim rect As New Rectangle(point, New Size(e.Rectangle.Width, mdiClientControl.Parent.PointToScreen(New Point(0, mdiClientControl.Bottom)).Y - point.Y))

        e.Rectangle = rect

    End Sub

    Private Sub OrientationSplitters_ResizeRequested(ByVal sender As Object, ByVal e As NewSizeEventArgs) Handles m_blankAreaLeft.ResizeRequested, m_blankAreaRight.ResizeRequested, WindowManagerSplitter2.NewSize

        Dim mdiClientControl As MdiClient = GetMDIClient()

        If Not mdiClientControl Is Nothing Then
            Dim mdiParentForm As Form = GetMDIParent()
            Dim mdiClientAdjustedBounds As Rectangle = mdiParentForm.RectangleToClient(mdiClientControl.RectangleToScreen(mdiClientControl.ClientRectangle))
            Dim adjustedBounds As Rectangle = mdiParentForm.RectangleToClient(e.Rectangle)

            Select Case Me.Orientation
                Case WindowManagerOrientation.Left
                    Dim newLeft As Integer

                    If adjustedBounds.Left <= mdiClientAdjustedBounds.Right Then
                        newLeft = adjustedBounds.Left
                    Else
                        newLeft = mdiClientAdjustedBounds.Right
                    End If

                    If newLeft - Me.Left >= MINIMUM_PANEL_WIDTH Then
                        Me.Width = newLeft - Me.Left
                    Else
                        Me.Width = MINIMUM_PANEL_WIDTH
                    End If
                Case WindowManagerOrientation.Right
                    Dim newRight As Integer

                    If adjustedBounds.Right >= mdiClientAdjustedBounds.Left Then
                        newRight = adjustedBounds.Left
                    Else
                        newRight = mdiClientAdjustedBounds.Left
                    End If

                    If Me.Right - newRight >= MINIMUM_PANEL_WIDTH Then
                        Me.Width = Me.Right - newRight
                    Else
                        Me.Width = MINIMUM_PANEL_WIDTH
                    End If
            End Select
        End If

    End Sub

    Private Sub WindowContextMenu_Popup(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WindowContextMenu.Opening

        If m_disableTileAction AndAlso m_disableHTileAction AndAlso m_disablePopoutAction Then
            Me.WindowTileWindowMenuItem.Visible = False
            Me.WindowHTileMenuItem.Visible = False
            Me.WindowPopoutWindowMenuItem.Visible = False
            Me.ToolStripSeparator1.Visible = False
        Else
            Me.WindowTileWindowMenuItem.Visible = True
            Me.WindowHTileMenuItem.Visible = True
            Me.WindowPopoutWindowMenuItem.Visible = True
            Me.ToolStripSeparator1.Visible = True
        End If

        Me.WindowTileWindowMenuItem.Enabled = Not m_disableTileAction
        Me.WindowHTileMenuItem.Enabled = Not m_disableHTileAction
        Me.WindowPopoutWindowMenuItem.Enabled = Not m_disablePopoutAction
        Me.WindowPopoutWindowMenuItem.Visible = Not m_disablePopoutAction

        Me.WindowCloseMenuItem.Enabled = Not m_disableCloseAction
        Me.WindowCloseMenuItem.Visible = Not m_disableCloseAction

        If Me.ToolStripSeparator1.Visible Then
            Me.ToolStripSeparator1.Visible = Not m_disableCloseAction
        End If

    End Sub

    Private Sub TitleContextMenu_Popup(ByVal sender As Object, ByVal e As System.EventArgs) Handles TitleContextMenu.Opening

        Me.TitleOptionsTileWindowMenuItem.Enabled = Not m_disableTileAction
        Me.TitleOptionsHTileMenuItem.Enabled = Not m_disableHTileAction
        Me.TitleOptionsPopoutWindowMenuItem.Enabled = Not m_disablePopoutAction

        If m_disableTileAction AndAlso m_disableHTileAction AndAlso m_disablePopoutAction Then
            Me.TitleOptionsTileWindowMenuItem.Visible = False
            Me.TitleOptionsHTileMenuItem.Visible = False
            Me.TitleOptionsPopoutWindowMenuItem.Visible = False
            Me.ToolStripSeparator2.Visible = False
        Else
            Me.TitleOptionsTileWindowMenuItem.Visible = True
            Me.TitleOptionsHTileMenuItem.Visible = True
            Me.TitleOptionsPopoutWindowMenuItem.Visible = True
            Me.ToolStripSeparator2.Visible = True
        End If

        Me.TitleOptionsTileWindowMenuItem.Enabled = Not m_disableTileAction
        Me.TitleOptionsHTileMenuItem.Enabled = Not m_disableHTileAction
        Me.TitleOptionsPopoutWindowMenuItem.Enabled = Not m_disablePopoutAction
        Me.TitleOptionsPopoutWindowMenuItem.Visible = Not m_disablePopoutAction

        Me.TitleCloseWindowMenuItem.Enabled = Not m_disableCloseAction
        Me.TitleCloseWindowMenuItem.Visible = Not m_disableCloseAction
        Me.ToolStripSeparator3.Visible = Not m_disableCloseAction

        Me.TitleMinimizeMenuItem.Checked = Me.MinMode
        'Me.TitleOptionsToggleWindowButtonsMenuItem.Checked = Not Me.ShowLayoutButtons

        UnloadWrappedWindowMenuItems()

        Dim count As Integer = 0

        For Each windowTabStrip As WindowTabStrip In m_windowTabStrips
            For Each wrappedWindow As WrappedWindow In windowTabStrip.Items
                Dim mnu As New WrappedWindowMenuItem

                'mnu.RadioCheck = True
                mnu.WrappedWindow = wrappedWindow
                AddHandler mnu.Click, AddressOf HandleWrappedWindowMenuItemClick

                If windowTabStrip Is Me.SelectedTabStrip Then
                    If wrappedWindow Is windowTabStrip.SelectedItem Then
                        mnu.Checked = True
                    End If
                End If

                Me.TitleContextMenu.Items.Add(mnu)
                mnu.Visible = True

                count += 1
            Next wrappedWindow
        Next windowTabStrip

        Me.TitleNoWindowsMenuItem.Visible = (count = 0)

    End Sub

    Private Sub HandleWrappedWindowMenuItemClick(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim mnu As WrappedWindowMenuItem = CType(sender, WrappedWindowMenuItem)

        Dim windowTabStrip As WindowTabStrip = GetWrappedWindowTabStrip(mnu.WrappedWindow, False)

        If Not windowTabStrip Is Nothing Then
            Me.SelectedTabStrip = windowTabStrip
            windowTabStrip.SelectedItem = mnu.WrappedWindow
            ActivateWindow(windowTabStrip.SelectedItem.Window)
        End If

    End Sub

    Private Sub TitleOptionsHTileMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TitleOptionsHTileMenuItem.Click

        HTileWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub TitleOptionsTileWindowMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TitleOptionsTileWindowMenuItem.Click

        TileOrUntileWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub TitleOptionsPopoutWindowMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TitleOptionsPopoutWindowMenuItem.Click

        PopOutWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub TitleCloseWindowMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TitleCloseWindowMenuItem.Click

        UserCloseSelectedWindow()

    End Sub

    Private Sub WindowHTileMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WindowHTileMenuItem.Click

        HTileWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub WindowTileWindowMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WindowTileWindowMenuItem.Click

        TileOrUntileWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub WindowPopoutWindowMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WindowPopoutWindowMenuItem.Click

        PopOutWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub WindowCloseMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WindowCloseMenuItem.Click

        UserCloseSelectedWindow()

    End Sub

    Private Sub WindowManagerSplitter1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles WindowManagerSplitter1.DoubleClick

        ToggleMinModeForBottom()

    End Sub

    Private Sub WindowManagerSplitter2_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles WindowManagerSplitter2.DoubleClick, m_blankAreaLeft.DoubleClick, m_blankAreaRight.DoubleClick

        'this is confusing to the user. let them use the Minimize menu option instead.
        'ToggleMinModeForSides()

    End Sub

    Private Sub m_nextWindowManagerPanel_RequestChainGetTopPanel(ByVal sender As Object, ByVal e As GetTopPanelEventArgs) Handles m_nextWindowManagerPanel.RequestChainGetTopPanel

        OnRequestChainGetTopPanel(e)

        If e.TopPanel Is Nothing Then
            e.TopPanel = Me
        End If

    End Sub

    Private Sub m_minModeBar_MinModeButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_minModeBar.MinModeButtonClick

        ToggleMinModeForSides()

    End Sub

    Private Sub m_auxiliaryWindow_LocationChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_auxiliaryWindow.LocationChanged

        ReconcileAuxiliaryWindowSize()

    End Sub

    Private Sub m_auxiliaryWindow_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_auxiliaryWindow.Resize

        ReconcileAuxiliaryWindowSize()

    End Sub

    Private Sub TitleMinimizeMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TitleMinimizeMenuItem.Click

        ToggleMinMode()

    End Sub

    Private Sub m_poppedOutWindowsManager_PopInRequested(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_poppedOutWindowsManager.PopInRequested

        If Not e.Handled Then
            If Not Me.IsTemporaryPanel Then
                Try
                    If m_poppedOutWindowsManager.GetWindowParent(e.WrappedWindow) Is GetMDIParent() Then
                        PopInWrappedWindow(e.WrappedWindow)
                        e.Handled = True
                    End If
                Catch
                    'do nothing
                End Try
            End If
        End If

    End Sub

    Private Sub MDIWinManHTileItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        HTileWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub MDIWinManTileItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        TileOrUntileWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub MDIWinManPopOutItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        PopOutWrappedWindow(GetSelectedWindow())

    End Sub

    Private Sub MDIWinManCloseAllItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        CloseAllWindows()

    End Sub

    Private Sub MDIWinManWindowItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim mnu As ToolStripMenuItem = CType(sender, ToolStripMenuItem)

        If mnu IsNot Nothing Then
            Dim wrappedWindow As WrappedWindow = CType(mnu.Tag, WrappedWindow)

            If wrappedWindow IsNot Nothing Then
                ShowWindowWithFocus(wrappedWindow.Window)
            End If
        End If

    End Sub

    Private Sub MDIWinManWindowsItem_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        ShowAllWindowsDialog()

    End Sub

    Private Sub m_mdiWindowListItem_DropDownOpening(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_mdiWindowListItem.DropDownOpening

        If Me.DesignMode OrElse m_mdiWindowListItem Is Nothing Then
            Return
        End If

        Dim htileMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                             Where TypeOf item Is ToolStripMenuItem AndAlso DirectCast(item, ToolStripMenuItem).Name = "MDIWinManHTileItem"
                             Select DirectCast(item, ToolStripMenuItem)).FirstOrDefault()
        Dim tileMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                            Where TypeOf item Is ToolStripMenuItem AndAlso DirectCast(item, ToolStripMenuItem).Name = "MDIWinManTileItem"
                            Select DirectCast(item, ToolStripMenuItem)).FirstOrDefault()
        Dim popoutMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                              Where TypeOf item Is ToolStripMenuItem AndAlso DirectCast(item, ToolStripMenuItem).Name = "MDIWinManPopOutItem"
                              Select DirectCast(item, ToolStripMenuItem)).FirstOrDefault()
        Dim toolSepMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                               Where TypeOf item Is ToolStripSeparator AndAlso DirectCast(item, ToolStripSeparator).Name = "MDIWinManToolSep"
                               Select DirectCast(item, ToolStripSeparator)).FirstOrDefault()
        Dim closeAllMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                                Where TypeOf item Is ToolStripMenuItem AndAlso DirectCast(item, ToolStripMenuItem).Name = "MDIWinManCloseAllItem"
                                Select DirectCast(item, ToolStripMenuItem)).FirstOrDefault()
        Dim windowsSepMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                                  Where TypeOf item Is ToolStripSeparator AndAlso DirectCast(item, ToolStripSeparator).Name = "MDIWinManWinListSep"
                                  Select DirectCast(item, ToolStripSeparator)).FirstOrDefault()

        If htileMenuItem Is Nothing Then
            htileMenuItem = New ToolStripMenuItem
            htileMenuItem.Name = "MDIWinManHTileItem"
            htileMenuItem.Text = "New &Horizontal Tab Group"
            AddHandler htileMenuItem.Click, AddressOf MDIWinManHTileItem_Click
            m_mdiWindowListItem.DropDownItems.Add(htileMenuItem)
        End If

        If tileMenuItem Is Nothing Then
            tileMenuItem = New ToolStripMenuItem
            tileMenuItem.Name = "MDIWinManTileItem"
            tileMenuItem.Text = "&Tile Or Untile"
            AddHandler tileMenuItem.Click, AddressOf MDIWinManTileItem_Click
            m_mdiWindowListItem.DropDownItems.Add(tileMenuItem)
        End If

        If popoutMenuItem Is Nothing Then
            popoutMenuItem = New ToolStripMenuItem
            popoutMenuItem.Name = "MDIWinManPopOutItem"
            popoutMenuItem.Text = "&Pop Out"
            AddHandler popoutMenuItem.Click, AddressOf MDIWinManPopOutItem_Click
            m_mdiWindowListItem.DropDownItems.Add(popoutMenuItem)
        End If

        If toolSepMenuItem Is Nothing Then
            toolSepMenuItem = New ToolStripSeparator
            toolSepMenuItem.Name = "MDIWinManToolSep"
            m_mdiWindowListItem.DropDownItems.Add(toolSepMenuItem)
        End If

        If closeAllMenuItem Is Nothing Then
            closeAllMenuItem = New ToolStripMenuItem
            closeAllMenuItem.Name = "MDIWinManCloseAllItem"
            closeAllMenuItem.Text = "Close All"
            AddHandler closeAllMenuItem.Click, AddressOf MDIWinManCloseAllItem_Click
            m_mdiWindowListItem.DropDownItems.Add(closeAllMenuItem)
        End If

        htileMenuItem.Enabled = Not m_disableHTileAction
        tileMenuItem.Enabled = Not m_disableTileAction
        popoutMenuItem.Enabled = Not m_disablePopoutAction
        popoutMenuItem.Visible = Not m_disablePopoutAction
        closeAllMenuItem.Enabled = Not m_disableCloseAction

        For index As Integer = m_mdiWindowListItem.DropDownItems.Count - 1 To 0 Step -1
            If TypeOf m_mdiWindowListItem.DropDownItems(index) Is ToolStripMenuItem Then
                Dim mnu As ToolStripMenuItem = DirectCast(m_mdiWindowListItem.DropDownItems(index), ToolStripMenuItem)
                If TypeOf mnu.Tag Is WrappedWindow Then
                    m_mdiWindowListItem.DropDownItems.RemoveAt(index)
                End If
            End If
        Next index

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        If wrappedWindows.Count > 0 Then
            Dim activePanel As WindowManagerPanel = GetActivePanel()
            Dim activeWrappedWindow As WrappedWindow

            If Not activePanel Is Nothing Then
                activeWrappedWindow = activePanel.GetActiveWindow()
            End If

            If windowsSepMenuItem Is Nothing Then
                windowsSepMenuItem = New ToolStripSeparator()
                windowsSepMenuItem.Name = "MDIWinManWinListSep"
                m_mdiWindowListItem.DropDownItems.Add(windowsSepMenuItem)
            End If

            windowsSepMenuItem.Visible = True

            For index As Integer = 0 To Math.Min(8, wrappedWindows.Count - 1)
                Dim wrappedWindow As WrappedWindow = wrappedWindows(index)
                Dim mnu As New ToolStripMenuItem

                If Not activeWrappedWindow Is Nothing AndAlso activeWrappedWindow Is wrappedWindow Then
                    mnu.Checked = True
                End If

                mnu.Text = $"&{index + 1} {wrappedWindow.Window.Text}"
                mnu.Tag = wrappedWindow
                AddHandler mnu.Click, AddressOf MDIWinManWindowItem_Click

                m_mdiWindowListItem.DropDownItems.Add(mnu)
            Next index

            Dim showWindowsMenuItem = (From item In m_mdiWindowListItem.DropDownItems
                                       Where TypeOf item Is ToolStripMenuItem AndAlso (DirectCast(item, ToolStripMenuItem)).Name = "MDIWinManWindowsItem"
                                       Select DirectCast(item, ToolStripMenuItem)).FirstOrDefault()

            If showWindowsMenuItem Is Nothing Then
                showWindowsMenuItem = New ToolStripMenuItem
                showWindowsMenuItem.Name = "MDIWinManWindowsItem"
                showWindowsMenuItem.Text = "Windows..."
                AddHandler showWindowsMenuItem.Click, AddressOf MDIWinManWindowsItem_Click
                m_mdiWindowListItem.DropDownItems.Add(showWindowsMenuItem)
            Else
                m_mdiWindowListItem.DropDownItems.Remove(showWindowsMenuItem)
                m_mdiWindowListItem.DropDownItems.Add(showWindowsMenuItem)
            End If
        Else
            If windowsSepMenuItem IsNot Nothing Then
                windowsSepMenuItem.Visible = False
            End If
        End If

    End Sub

#End Region

#Region "Nested Classes"

    Protected Friend Class TempPanelDismissedEventArgs
        Inherits System.EventArgs

        Private m_nextWindowTabStrip As WindowTabStrip

        Public Sub New(ByVal nextWindowTabStrip As WindowTabStrip)

            m_nextWindowTabStrip = nextWindowTabStrip

        End Sub

        Public ReadOnly Property WindowTabStrip() As WindowTabStrip

            Get
                Return m_nextWindowTabStrip
            End Get

        End Property

    End Class

    Protected Friend Class RedoLayoutEventArgs
        Inherits System.ComponentModel.CancelEventArgs

        Private m_originalWindowManagerPanel As WindowManagerPanel
        Private m_bottomBarrier As Integer
        Private m_test As Boolean = False
        Private m_mdiClientControl As MdiClient

        Public Sub New(ByVal originalWindowManagerPanel As WindowManagerPanel, ByVal mdiClientControl As MdiClient, ByVal test As Boolean)

            m_originalWindowManagerPanel = originalWindowManagerPanel
            m_mdiClientControl = mdiClientControl
            m_test = test

        End Sub

        Public ReadOnly Property OriginalWindowManagerPanel() As WindowManagerPanel

            Get
                Return m_originalWindowManagerPanel
            End Get

        End Property

        Public ReadOnly Property Test() As Boolean

            Get
                Return m_test
            End Get

        End Property

        Public Property BottomBarrier() As Integer

            Get
                Return m_bottomBarrier
            End Get

            Set(ByVal value As Integer)
                m_bottomBarrier = value
            End Set

        End Property

        Public ReadOnly Property MDIClient() As MdiClient

            Get
                Return m_mdiClientControl
            End Get

        End Property

    End Class

    Protected Friend Class GetTopEventArgs
        Inherits System.EventArgs

        Private m_absoluteTop As Integer

        Public Property AbsoluteTop() As Integer

            Get
                Return m_absoluteTop
            End Get

            Set(ByVal value As Integer)
                m_absoluteTop = value
            End Set

        End Property

    End Class

    Protected Friend Class GetTopPanelEventArgs
        Inherits System.EventArgs

        Private m_topPanel As WindowManagerPanel

        Public Property TopPanel() As WindowManagerPanel

            Get
                Return m_topPanel
            End Get

            Set(ByVal value As WindowManagerPanel)
                m_topPanel = value
            End Set

        End Property

    End Class

    Protected Friend Class RequestMinModeChangeEventArgs
        Inherits System.EventArgs

        Private m_minMode As Boolean

        Public Sub New()

        End Sub

        Public Sub New(ByVal minModeValue As Boolean)

            m_minMode = minModeValue

        End Sub

        Public Property MinModeValue() As Boolean

            Get
                Return m_minMode
            End Get

            Set(ByVal value As Boolean)
                m_minMode = value
            End Set

        End Property

    End Class

    Private Sub WindowManageButton_Click(sender As Object, e As EventArgs) Handles WindowManageButton.Click

        Me.TitleContextMenu.Show(Me.WindowManageButton, 0, Me.WindowManageButton.Height)

    End Sub

#End Region

End Class
Imports System.ComponentModel

''' <summary>
''' Tabbed Windows Strip 
''' </summary>
''' <remarks>Facilitates vertical "tiling." Used by WindowManagerPanel to present multiple sets of tabbed windows.</remarks>
<ToolboxItemAttribute(False)> _
Public Class WindowTabStrip
    Inherits System.Windows.Forms.UserControl

    Public Event BeginDragItem As EventHandler(Of WrappedWindowItemEventArgs)
    Friend Event ItemDropped As EventHandler(Of DroppedWrappedWindowEventArgs)
    Public Event SelectedItemChanged As EventHandler(Of SelectedWrappedWindowChangedEventArgs)
    Public Event SelectedItemReselected As EventHandler
    Public Event ItemsFirstAdded As EventHandler
    Public Event NewSplitSize As EventHandler(Of WindowTabStripNewSplitSizeEventArgs)
    Public Event ShowWindowMenuRequested As EventHandler
    Public Event ItemTabDoubleClicked As EventHandler(Of WrappedWindowItemEventArgs)
    Public Event RequestPreviousWindowSelect As EventHandler(Of WrappedWindowItemEventArgs)
    Public Event ItemsCleared As EventHandler
    'todo: see where to go with this... make the control offload everything to WindowManagerPanel
    'including deciding which tab to select or not select
    'Public Event TabClick As EventHandler(Of WindowTabStripEventArgs)

    Public Event TabPaint As EventHandler(Of StandardTabsProvider.TabPaintEventArgs)

    Private WithEvents m_items As New WrappedWindowCollection
    Private m_style As TabStyle
    Private m_enableAeroGlass As Boolean = False
    Private m_showIcons As Boolean
    Private m_tabBackColor As Color
    Private m_tabForeColor As Color
    Private m_selectedTabBackColor As Color
    Private m_selectedTabForeColor As Color
    Private m_emphasizeSelectedTab As Boolean = True
    Private m_wordWrap As Boolean
    Private m_selectedItem As WrappedWindow
    Private m_selectableUserControlFixer As SelectableUserControlFixer
    Private WithEvents m_wrappedWindowSplitter As New WrappedWindowSplitter
    Private m_isActive As Boolean

    'both of these will refer to the same object
    Private WithEvents m_tabsProvider As ITabsProvider
    Private WithEvents m_standardTabsProvider As StandardTabsProvider

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        m_selectableUserControlFixer = New SelectableUserControlFixer(Me)
        m_wrappedWindowSplitter.Width = 3
        InitializeTabs(New StandardTabsProvider)

    End Sub

    'UserControl overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            m_selectableUserControlFixer.Dispose()
            m_selectableUserControlFixer = Nothing

            m_wrappedWindowSplitter = Nothing
            m_items = Nothing

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
        SuspendLayout()
        ' 
        ' WindowTabStrip
        ' 
        Margin = New Padding(0)
        Name = "WindowTabStrip"
        Size = New Size(412, 20)
        ResumeLayout(False)

    End Sub

#End Region

    Private Sub InitializeTabs(ByVal tabsProvider As ITabsProvider)

        If tabsProvider Is Nothing Then
            Throw New InvalidOperationException("Tabs Provider cannot be null.")
        ElseIf m_tabsProvider Is tabsProvider Then
            'do nothing
        Else
            If Not m_tabsProvider Is Nothing Then
                Dim oldTabsProviderControl As Control = CType(m_tabsProvider, Control)

                Me.Controls.Remove(oldTabsProviderControl)
                oldTabsProviderControl.Dispose()
                m_tabsProvider = Nothing
                m_standardTabsProvider = Nothing
            End If

            m_tabsProvider = tabsProvider

            If TypeOf m_tabsProvider Is StandardTabsProvider Then
                m_standardTabsProvider = CType(m_tabsProvider, StandardTabsProvider)
            Else
                m_standardTabsProvider = Nothing
            End If

            Dim newTabsProviderControl As Control = CType(m_tabsProvider, Control)

            Me.Controls.Add(newTabsProviderControl)
            'newTabsProviderControl.SetBounds(0, 0, Me.ClientRectangle.Width, Me.ClientRectangle.Height)
            ''newTabsProviderControl.SetBounds(0, 0, Me.Width, Me.Height)
            'newTabsProviderControl.Anchor = AnchorStyles.Left Or AnchorStyles.Top Or AnchorStyles.Right Or AnchorStyles.Bottom
            newTabsProviderControl.Dock = DockStyle.Fill

            m_tabsProvider.WrappedWindowsCollection = m_items
        End If

    End Sub

    Friend Property TabsProvider() As ITabsProvider

        Get
            Return m_tabsProvider
        End Get

        Set(ByVal value As ITabsProvider)
            InitializeTabs(value)
        End Set

    End Property

    Public ReadOnly Property Items() As WrappedWindowCollection

        Get
            Return m_items
        End Get

    End Property

    Public Property Style() As TabStyle

        Get
            Return m_style
        End Get

        Set(ByVal value As TabStyle)
            m_style = value

            If Not m_standardTabsProvider Is Nothing Then
                m_standardTabsProvider.Style = m_style
            End If
        End Set

    End Property

    Public Property EnableAeroGlassEffect As Boolean

        Get
            Return m_enableAeroGlass
        End Get

        Set(value As Boolean)
            m_enableAeroGlass = value

            If Not m_standardTabsProvider Is Nothing Then
                m_standardTabsProvider.EnableAeroGlassEffect = m_enableAeroGlass
            End If
        End Set

    End Property

    Public Property EnableTabPaintEvent() As Boolean

        Get
            If Not m_standardTabsProvider Is Nothing Then
                Return m_standardTabsProvider.EnabledOwnerPaint
            Else
                Return False
            End If
        End Get

        Set(ByVal value As Boolean)
            If Not m_standardTabsProvider Is Nothing Then
                m_standardTabsProvider.EnabledOwnerPaint = value
            End If
        End Set

    End Property

    Public Property ShowIcons() As Boolean

        Get
            Return m_showIcons
        End Get

        Set(ByVal value As Boolean)
            m_showIcons = value
            m_tabsProvider.ShowIcons = m_showIcons
        End Set

    End Property

    Public Property TabBackColor() As Color
        Get
            Return m_tabBackColor
        End Get
        Set(ByVal value As Color)
            m_tabBackColor = value
            m_tabsProvider.TabBackColor = m_tabBackColor
        End Set
    End Property

    Public Property TabForeColor() As Color
        Get
            Return m_tabForeColor
        End Get
        Set(ByVal value As Color)
            m_tabForeColor = value
            m_tabsProvider.TabForeColor = m_tabForeColor
        End Set
    End Property

    Public Property SelectedTabBackColor() As Color
        Get
            Return m_selectedTabBackColor
        End Get
        Set(ByVal value As Color)
            m_selectedTabBackColor = value
            m_tabsProvider.SelectedTabBackColor = m_selectedTabBackColor
        End Set
    End Property

    Public Property SelectedTabForeColor() As Color
        Get
            Return m_selectedTabForeColor
        End Get
        Set(ByVal value As Color)
            m_selectedTabForeColor = value
            m_tabsProvider.SelectedTabForeColor = m_selectedTabForeColor
        End Set
    End Property

    Public Property EmphasizeSelectedTab() As Boolean
        Get
            Return m_emphasizeSelectedTab
        End Get
        Set(ByVal value As Boolean)
            m_emphasizeSelectedTab = value
            m_tabsProvider.EmphasizeSelectedTab = m_emphasizeSelectedTab
        End Set
    End Property

    Public Property WordWrap() As Boolean
        Get
            Return m_wordWrap
        End Get
        Set(ByVal value As Boolean)
            m_wordWrap = value
            m_tabsProvider.WordWrap = m_wordWrap
        End Set
    End Property

    Public Property SelectedItem() As WrappedWindow

        Get
            Return m_selectedItem
        End Get

        Set(ByVal value As WrappedWindow)
            Dim previouslySelectedWrappedWindow As WrappedWindow = m_selectedItem
            m_selectedItem = value
            m_tabsProvider.SetSelectedWrappedWindowItemDirect(value, True)
            OnSelectedItemChanged(New SelectedWrappedWindowChangedEventArgs(m_selectedItem, previouslySelectedWrappedWindow))
        End Set

    End Property

    Friend ReadOnly Property ResizeSplitter() As WrappedWindowSplitter

        Get
            Return m_wrappedWindowSplitter
        End Get

    End Property

    Friend Property IsActive() As Boolean

        Get
            Return m_isActive
        End Get

        Set(ByVal value As Boolean)
            m_isActive = value
            m_tabsProvider.EmphasizeSelectedTab = value
        End Set

    End Property

    Public Sub BeginUpdate()

        m_tabsProvider.BeginUpdate()

    End Sub

    Public Sub EndUpdate()

        m_tabsProvider.EndUpdate()

    End Sub

    Private Sub m_items_WrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_items.WrappedWindowAdded

        If m_items.Count = 1 Then
            OnItemsFirstAdded(EventArgs.Empty)
        End If

    End Sub

    Private Sub m_items_WrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_items.WrappedWindowRemoved

        If Me.SelectedItem Is e.WrappedWindow Then
            Me.SelectedItem = Nothing
            OnRequestPreviousWindowSelect(New WrappedWindowItemEventArgs(e.WrappedWindow))
        End If

        If m_items.Count = 0 Then
            OnItemsCleared(EventArgs.Empty)
        End If

    End Sub

    Private Sub m_items_WindowActivated(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_items.WindowActivated

        'undone: do we really need this if we're doing it in WindowEnter instead?

        OnEnter(EventArgs.Empty)

        If Not Me.SelectedItem Is e.WrappedWindow Then
            Me.SelectedItem = e.WrappedWindow
            m_tabsProvider.SetSelectedWrappedWindowItemDirect(Me.SelectedItem, True)
        End If

    End Sub

    Private Sub m_items_WindowEnter(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_items.WindowEnter

        'undone: do we really need this if we're doing it in WindowActivated instead?

        OnEnter(EventArgs.Empty)

        If Not Me.SelectedItem Is e.WrappedWindow Then
            Me.SelectedItem = e.WrappedWindow
            m_tabsProvider.SetSelectedWrappedWindowItemDirect(Me.SelectedItem, True)
        End If

    End Sub

    Private Sub m_wrappedWindows_WindowLeave(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_items.WindowLeave

        OnLeave(EventArgs.Empty)

    End Sub

    Protected Overridable Sub OnBeginDragItem(ByVal e As WrappedWindowItemEventArgs)

        RaiseEvent BeginDragItem(Me, e)

    End Sub

    Friend Overridable Sub OnItemDropped(ByVal e As DroppedWrappedWindowEventArgs)

        RaiseEvent ItemDropped(Me, e)

    End Sub

    Protected Overridable Sub OnSelectedItemChanged(ByVal e As SelectedWrappedWindowChangedEventArgs)

        RaiseEvent SelectedItemChanged(Me, e)

    End Sub

    Protected Overridable Sub OnSelectedItemReselected(ByVal e As EventArgs)

        RaiseEvent SelectedItemReselected(Me, e)

    End Sub

    Protected Overridable Sub OnItemsFirstAdded(ByVal e As EventArgs)

        RaiseEvent ItemsFirstAdded(Me, e)

    End Sub

    Protected Overridable Sub OnNewSplitSize(ByVal e As WindowTabStripNewSplitSizeEventArgs)

        RaiseEvent NewSplitSize(Me, e)

    End Sub

    Protected Overridable Sub OnShowWindowMenuRequested(ByVal e As EventArgs)

        RaiseEvent ShowWindowMenuRequested(Me, e)

    End Sub

    Protected Overridable Sub OnItemTabDoubleClicked(ByVal e As WrappedWindowItemEventArgs)

        RaiseEvent ItemTabDoubleClicked(Me, e)

    End Sub

    Protected Overridable Sub OnItemsCleared(ByVal e As EventArgs)

        RaiseEvent ItemsCleared(Me, e)

    End Sub

    Protected Overridable Sub OnRequestPreviousWindowSelect(ByVal e As WrappedWindowItemEventArgs)

        RaiseEvent RequestPreviousWindowSelect(Me, e)

    End Sub

    Private Sub m_wrappedWindowSplitter_NewSize(ByVal sender As Object, ByVal e As SplitterEventArgs) Handles m_wrappedWindowSplitter.NewSize

        OnNewSplitSize(New WindowTabStripNewSplitSizeEventArgs(e.NewRectangle.Right - e.OldRectangle.Right))

    End Sub

    Private Sub m_tabsProvider_BeginDragTabItems(ByVal sender As Object, ByVal e As WrappedWindowItemEventArgs) Handles m_tabsProvider.BeginDragTabItem

        OnBeginDragItem(e)

    End Sub

    Private Sub m_tabsProvider_SelectedTabItemChanged(ByVal sender As Object, ByVal e As SelectedWrappedWindowChangedEventArgs) Handles m_tabsProvider.SelectedTabItemChanged

        Me.SelectedItem = e.SelectedWrappedWindow

    End Sub

    Private Sub m_tabsProvider_SelectedTabItemReselected(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tabsProvider.SelectedTabItemReselected

        OnSelectedItemReselected(e)

    End Sub

    Private Sub m_tabsProvider_ShowWindowCmdMenuRequested(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_tabsProvider.ShowWindowCmdMenuRequested

        OnShowWindowMenuRequested(e)

    End Sub

    Private Sub m_tabsProvider_TabItemDropped(ByVal sender As Object, ByVal e As DroppedWrappedWindowEventArgs) Handles m_tabsProvider.TabItemDropped

        OnItemDropped(e)

    End Sub

    Private Sub m_tabsProvider_TabItemDoubleClicked(ByVal sender As Object, ByVal e As WrappedWindowItemEventArgs) Handles m_tabsProvider.TabItemDoubleClicked

        OnItemTabDoubleClicked(e)

    End Sub

    Private Sub m_standardTabsProvider_TabPaint(ByVal sender As Object, ByVal e As StandardTabsProvider.TabPaintEventArgs) Handles m_standardTabsProvider.TabPaint

        'simply bubble up the paint event
        RaiseEvent TabPaint(Me, e)

    End Sub

    Public Class WindowTabStripNewSplitSizeEventArgs
        Inherits EventArgs

        Private m_newSizeDelta As Integer

        Public Sub New(ByVal newSizeDelta As Integer)

            m_newSizeDelta = newSizeDelta

        End Sub

        Public ReadOnly Property NewSizeDelta() As Integer

            Get
                Return m_newSizeDelta
            End Get

        End Property

    End Class

    Private Sub WindowTabStrip_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        'Stop
    End Sub
End Class
Imports System.ComponentModel

''' <summary>
''' Provides a starting point and contains a lot of the plumbing for TabsProviders.
''' </summary>
''' <remarks>By inheriting from this class instead of simply implementing ITabsProvider 
''' all a TabsProvider has to to do is drop some Tab control in the designer and 
''' respond to a couple of events.</remarks>
<ToolboxItemAttribute(False)> _
Public Class TabsProviderBase
    Implements ITabsProvider

    Public Event BeginDragTabItem As EventHandler(Of WrappedWindowItemEventArgs) Implements ITabsProvider.BeginDragTabItem
    Public Event TabItemDropped As EventHandler(Of DroppedWrappedWindowEventArgs) Implements ITabsProvider.TabItemDropped
    Public Event SelectedTabItemChanged As EventHandler(Of SelectedWrappedWindowChangedEventArgs) Implements ITabsProvider.SelectedTabItemChanged
    Public Event SelectedTabItemReselected As EventHandler Implements ITabsProvider.SelectedTabItemReselected
    Public Event ShowWindowCmdMenuRequested As EventHandler Implements ITabsProvider.ShowWindowCmdMenuRequested
    Public Event TabItemDoubleClicked As EventHandler(Of WrappedWindowItemEventArgs) Implements ITabsProvider.TabItemDoubleClicked

    Friend WithEvents m_wrappedWindowItems As New WrappedWindowCollection
    Private m_selectedWrappedWindowItem As WrappedWindow
    Private m_updating As Boolean = False
    Private m_emphasizeSelectedTab As Boolean = False
    Private m_showIcons As Boolean = True
    Private m_tabForeColor As Color = SystemColors.ControlText
    Private m_tabBackColor As Color = SystemColors.Control
    Private m_selectedTabForeColor As Color = SystemColors.ControlText
    Private m_selectedTabBackColor As Color = SystemColors.Control
    Private m_wordWrap As Boolean = False
    Private m_suppressSelectionChangeEvents As Boolean = False

    Public ReadOnly Property WrappedWindowItems() As System.Collections.ObjectModel.ReadOnlyCollection(Of WrappedWindow) Implements ITabsProvider.WrappedWindowItems

        Get
            Return New System.Collections.ObjectModel.ReadOnlyCollection(Of WrappedWindow)(DirectCast(m_wrappedWindowItems, System.Collections.Generic.IList(Of WrappedWindow)))
        End Get

    End Property

    Friend Property WrappedWindowsCollection() As WrappedWindowCollection Implements ITabsProvider.WrappedWindowsCollection

        Get
            Return m_wrappedWindowItems
        End Get

        Set(ByVal value As WrappedWindowCollection)
            m_wrappedWindowItems = value
        End Set

    End Property

    Public Property SelectedWrappedWindowItem() As WrappedWindow Implements ITabsProvider.SelectedWrappedWindowItem

        Get
            Return m_selectedWrappedWindowItem
        End Get

        Set(ByVal value As WrappedWindow)
            SetSelectedWrappedWindowItemDirect(value, False)
        End Set

    End Property

    Friend Sub SetSelectedWrappedWindowItemDirect(ByVal value As WrappedWindow, ByVal suppressEvents As Boolean) Implements ITabsProvider.SetSelectedWrappedWindowItemDirect

        If suppressEvents Then
            m_suppressSelectionChangeEvents = True
        End If

        If Not value Is m_selectedWrappedWindowItem OrElse suppressEvents Then
            Dim previouslySelectedWrappedWindow As WrappedWindow = m_selectedWrappedWindowItem

            m_selectedWrappedWindowItem = value

            OnSelectedTabItemChanged(New SelectedWrappedWindowChangedEventArgs(m_selectedWrappedWindowItem, previouslySelectedWrappedWindow))
        Else
            If Not value Is Nothing Then
                OnSelectedTabItemReselected(EventArgs.Empty)
            End If
        End If

        If suppressEvents Then
            m_suppressSelectionChangeEvents = False
        End If

    End Sub

    Public ReadOnly Property IsUpdating() As Boolean Implements ITabsProvider.IsUpdating

        Get
            Return m_updating
        End Get

    End Property

    Protected Friend Property EmphasizeSelectedTab() As Boolean Implements ITabsProvider.EmphasizeSelectedTab

        Get
            Return m_emphasizeSelectedTab
        End Get

        Set(ByVal value As Boolean)
            If m_emphasizeSelectedTab <> value Then
                m_emphasizeSelectedTab = value
                ProcessEmphasizeSelectedTabChanged()
            End If
        End Set

    End Property

    Protected Friend Property WordWrap() As Boolean Implements ITabsProvider.WordWrap

        Get
            Return m_wordWrap
        End Get

        Set(ByVal value As Boolean)
            If m_wordWrap <> value Then
                m_wordWrap = value
            End If
        End Set

    End Property

    Public Property ShowIcons() As Boolean Implements ITabsProvider.ShowIcons

        Get
            Return m_showIcons
        End Get

        Set(ByVal value As Boolean)
            If m_showIcons <> value Then
                m_showIcons = value
                ProcessShowIconsChanged()
            End If
        End Set

    End Property

    Public Property TabForeColor As Color Implements ITabsProvider.TabForeColor
        Get
            Return m_tabForeColor
        End Get
        Set(value As Color)
            If m_tabForeColor <> value Then
                m_tabForeColor = value
                ProcessTabColorsChanged()
            End If
        End Set
    End Property

    Public Property TabBackColor As Color Implements ITabsProvider.TabBackColor
        Get
            Return m_tabBackColor
        End Get
        Set(value As Color)
            If m_tabBackColor <> value Then
                m_tabBackColor = value
                ProcessTabColorsChanged()
            End If
        End Set
    End Property

    Public Property SelectedTabForeColor As Color Implements ITabsProvider.SelectedTabForeColor
        Get
            Return m_selectedTabForeColor
        End Get
        Set(value As Color)
            If m_selectedTabForeColor <> value Then
                m_selectedTabForeColor = value
                ProcessTabColorsChanged()
            End If
        End Set
    End Property

    Public Property SelectedTabBackColor As Color Implements ITabsProvider.SelectedTabBackColor
        Get
            Return m_selectedTabBackColor
        End Get
        Set(value As Color)
            If m_selectedTabBackColor <> value Then
                m_selectedTabBackColor = value
                ProcessTabColorsChanged()
            End If
        End Set
    End Property

    Public Overridable Sub BeginUpdate() Implements ITabsProvider.BeginUpdate

        m_updating = True

    End Sub

    Public Overridable Sub EndUpdate() Implements ITabsProvider.EndUpdate

        m_updating = False

    End Sub

    Public Overridable Sub EnsureVisible() Implements ITabsProvider.EnsureVisible

        'to be implemented by inherited classes

    End Sub

    Public Overridable Sub ShowWindowsMenu() Implements ITabsProvider.ShowWindowsMenu

        ShowWindowsMenu(Me.PointToClient(System.Windows.Forms.Cursor.Position))

    End Sub

    Public Overridable Sub ShowWindowsMenu(ByVal pos As Point) Implements ITabsProvider.ShowWindowsMenu

        Me.WindowsMenu.Show(Me, pos)

    End Sub

    Private Sub m_wrappedWindowItems_WindowTextChanged(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_wrappedWindowItems.WindowTextChanged

        ProcessWindowItemTextChanged(e.WrappedWindow)

    End Sub

    Private Sub m_wrappedWindowItems_WrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_wrappedWindowItems.WrappedWindowAdded

        ProcessWindowItemAdded(e.WrappedWindow)

    End Sub

    Private Sub m_wrappedWindowItems_WrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_wrappedWindowItems.WrappedWindowRemoved

        ProcessWindowItemRemoved(e.WrappedWindow)

    End Sub

    Private Sub m_wrappedWindowItems_WrappedWindowsCleared(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_wrappedWindowItems.WrappedWindowsCleared

        ProcessWindowItemsCleared()

    End Sub

    Private Sub WindowsMenu_Popup(ByVal sender As Object, ByVal e As System.EventArgs) Handles WindowsMenu.Opening

        UnloadWrappedWindowMenuItems()

        Dim count As Integer = 0

        For Each wrappedWindow As WrappedWindow In m_wrappedWindowItems
            Dim mnu As New WrappedWindowMenuItem

            'mnu.RadioCheck = True
            mnu.WrappedWindow = wrappedWindow
            AddHandler mnu.Click, AddressOf HandleWrappedWindowMenuItemClick

            If wrappedWindow Is Me.SelectedWrappedWindowItem Then
                mnu.Checked = True
            End If

            Me.WindowsMenu.Items.Add(mnu)
            mnu.Visible = True

            count += 1
        Next wrappedWindow

        Me.WindowsNoWindowsMenuItem.Visible = (count = 0)

    End Sub

    Private Sub UnloadWrappedWindowMenuItems()

        For index As Integer = WindowsMenu.Items.Count - 1 To 0 Step -1
            Dim mnu As ToolStripMenuItem = Me.WindowsMenu.Items.Item(index)

            If TypeOf mnu Is WrappedWindowMenuItem Then
                RemoveHandler mnu.Click, AddressOf HandleWrappedWindowMenuItemClick
                Me.WindowsMenu.Items.Remove(mnu)
                mnu.Dispose()
            End If
        Next index

    End Sub

    Private Sub HandleWrappedWindowMenuItemClick(ByVal sender As System.Object, ByVal e As System.EventArgs)

        Dim mnu As WrappedWindowMenuItem = CType(sender, WrappedWindowMenuItem)

        If m_wrappedWindowItems.Contains(mnu.WrappedWindow) Then
            Me.SelectedWrappedWindowItem = mnu.WrappedWindow
        End If

    End Sub

    Protected Overridable Sub ProcessWindowItemAdded(ByVal wrappedWindow As WrappedWindow)

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub ProcessWindowItemRemoved(ByVal wrappedWindow As WrappedWindow)

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub ProcessWindowItemsCleared()

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub ProcessWindowItemTextChanged(ByVal wrappedWindow As WrappedWindow)

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub ProcessEmphasizeSelectedTabChanged()

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub ProcessShowIconsChanged()

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub ProcessTabColorsChanged()

        'to be implemented by inherited classes

    End Sub

    Protected Overridable Sub OnBeginDragTabItem(ByVal e As WrappedWindowItemEventArgs)

        RaiseEvent BeginDragTabItem(Me, e)

    End Sub

    Protected Overridable Sub OnTabItemDropped(ByVal e As DroppedWrappedWindowEventArgs)

        RaiseEvent TabItemDropped(Me, e)

    End Sub

    Protected Overridable Sub OnSelectedTabItemChanged(ByVal e As SelectedWrappedWindowChangedEventArgs)

        If Not m_suppressSelectionChangeEvents Then
            RaiseEvent SelectedTabItemChanged(Me, e)
        End If

        EnsureVisible()

    End Sub

    Protected Overridable Sub OnSelectedTabItemReselected(ByVal e As EventArgs)

        If Not m_suppressSelectionChangeEvents Then
            RaiseEvent SelectedTabItemReselected(Me, e)
        End If

        EnsureVisible()

    End Sub

    Protected Overridable Sub OnShowWindowCmdMenuRequested(ByVal e As EventArgs)

        RaiseEvent ShowWindowCmdMenuRequested(Me, e)

    End Sub

    Protected Overridable Sub OnTabItemDoubleClicked(ByVal e As WrappedWindowItemEventArgs)

        RaiseEvent TabItemDoubleClicked(Me, e)

    End Sub

    Private Sub TabsProviderBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
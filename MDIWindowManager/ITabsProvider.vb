''' <summary>
''' Interface that custom tab providers must implement.
''' </summary>
''' <remarks>See TabsProviderBase for a starting point to creating TabProviders.</remarks>
Public Interface ITabsProvider

    Event BeginDragTabItem As EventHandler(Of WrappedWindowItemEventArgs)
    Event TabItemDropped As EventHandler(Of DroppedWrappedWindowEventArgs)
    Event SelectedTabItemChanged As EventHandler(Of SelectedWrappedWindowChangedEventArgs)
    Event SelectedTabItemReselected As EventHandler
    Event ShowWindowCmdMenuRequested As EventHandler
    Event TabItemDoubleClicked As EventHandler(Of WrappedWindowItemEventArgs)

    ReadOnly Property WrappedWindowItems() As System.Collections.ObjectModel.ReadOnlyCollection(Of WrappedWindow)
    Property WrappedWindowsCollection() As WrappedWindowCollection
    Property SelectedWrappedWindowItem() As WrappedWindow
    ReadOnly Property IsUpdating() As Boolean
    Property EmphasizeSelectedTab() As Boolean
    Property ShowIcons() As Boolean
    Property TabForeColor() As Color
    Property TabBackColor() As Color
    Property SelectedTabForeColor() As Color
    Property SelectedTabBackColor() As Color
    Property WordWrap As Boolean

    Sub BeginUpdate()
    Sub EndUpdate()
    Sub EnsureVisible()
    Sub ShowWindowsMenu()
    Sub ShowWindowsMenu(ByVal pos As Point)

    Sub SetSelectedWrappedWindowItemDirect(ByVal value As WrappedWindow, ByVal suppressEvents As Boolean)

End Interface

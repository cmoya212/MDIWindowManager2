Imports System.ComponentModel

'This class is fully documented to demonstrate how to create
'custom tab providers.

'To get started create a new UserControl. Show All Files in Solution
'Explorer and go to the code-behind partial class and change 
'"Inherits System.Windows.Forms.UserControl" to "Inherits 
'TabsProviderBase."

''' <summary>
''' Alternative TabsProvider that uses the intrinsic .NET TabControl.
''' </summary>
''' <remarks>This class inherits from TabsProviderBase</remarks>
<ToolboxItemAttribute(False)> _
Public Class SystemTabsProvider

#Region "Variables and Properties"

    'used to allow the user a margin of error before initiating a drag on a tab
    Private m_mouseDownStartPoint As Point
    'used to prevent a cascade of events when a tab is selected
    Private m_ignoreTabControlIndexChangedEvent As Boolean = False
    Private m_ignoreTabControlDragOver As Boolean = False

    'we're exposing the Appearance property of the underlying TabControl
    'this will allow consumers of the control to change the appearance 
    'if they wish.
    Public Property TabAppearance() As System.Windows.Forms.TabAppearance

        Get
            Return Me.TabControl1.Appearance
        End Get

        Set(ByVal value As System.Windows.Forms.TabAppearance)
            Me.TabControl1.Appearance = value
        End Set

    End Property

#End Region

#Region "Base Class Overrides"

    'A window has been added to the collection. Update the TabControl.
    Protected Overrides Sub ProcessWindowItemAdded(ByVal wrappedWindow As WrappedWindow)

        Dim tabPage As New TabPage

        tabPage.Text = GetTruncatedTabText(wrappedWindow.Window.Text)
        tabPage.Tag = wrappedWindow

        Dim imageKey As String = Guid.NewGuid().ToString()

        Me.ImageList1.Images.Add(imageKey, wrappedWindow.Window.Icon)
        Me.TabControl1.TabPages.Add(tabPage)

        tabPage.ImageKey = imageKey 'for some reason this must be set *after* the tabpage is added to the tabcontrol

    End Sub

    'A window has been removed from the collection. Update the TabControl.
    Protected Overrides Sub ProcessWindowItemRemoved(ByVal wrappedWindow As WrappedWindow)

        Dim deadTabPage As TabPage = FindTabPage(wrappedWindow)

        If Not deadTabPage Is Nothing Then
            Me.TabControl1.TabPages.Remove(deadTabPage)
            Me.ImageList1.Images.RemoveByKey(deadTabPage.ImageKey)
            ReapplyTabPageImageKeys()
        End If

    End Sub

    'All the windows have been removed. Clean up.
    Protected Overrides Sub ProcessWindowItemsCleared()

        Me.TabControl1.TabPages.Clear()
        Me.ImageList1.Images.Clear()

    End Sub

    'The title text of a window has changed. Update it's corresponding tab in the TabControl.
    Protected Overrides Sub ProcessWindowItemTextChanged(ByVal wrappedWindow As WrappedWindow)

        Dim tabPage As TabPage = FindTabPage(wrappedWindow)

        If Not tabPage Is Nothing Then
            tabPage.Text = GetTruncatedTabText(wrappedWindow.Window.Text)
        End If

    End Sub

    'We're being told to emphasize the selected window because There may be different tabstrips on 
    'the parent MDI window due to tiling and the user should be able to tell what is the ONE 
    'truly "active" window with focus in the container.
    Protected Overrides Sub ProcessEmphasizeSelectedTabChanged()

        ApplySelectedTabEmphasis()

    End Sub

    'We're being told to hide or show icons in the tabs.
    Protected Overrides Sub ProcessShowIconsChanged()

        If Me.ShowIcons Then
            If Me.TabControl1.ImageList Is Nothing Then
                Me.TabControl1.ImageList = Me.ImageList1
                ReapplyTabPageImageKeys()
            End If
        Else
            If Not Me.TabControl1.ImageList Is Nothing Then
                Me.TabControl1.ImageList = Nothing
                ReapplyTabPageImageKeys()
            End If
        End If

    End Sub

    'The selected window has changed. Update the TabControl.
    Protected Overrides Sub OnSelectedTabItemChanged(ByVal e As SelectedWrappedWindowChangedEventArgs)

        MyBase.OnSelectedTabItemChanged(e)

        Dim tabPage As TabPage = FindTabPage(Me.SelectedWrappedWindowItem)

        m_ignoreTabControlIndexChangedEvent = True
        TabControl1.SelectedTab = tabPage
        m_ignoreTabControlIndexChangedEvent = False

        ApplySelectedTabEmphasis()

    End Sub

    'The selected tab has been reselected... take this change to update the TabControl.
    Protected Overrides Sub OnSelectedTabItemReselected(ByVal e As System.EventArgs)

        MyBase.OnSelectedTabItemReselected(e)

        ApplySelectedTabEmphasis()

    End Sub

#End Region

#Region "Helper Methods"

    'kludge: due to the way TabControl accesses ImageList items.
    Private Sub ReapplyTabPageImageKeys()

        'discovered on a whim... ImageKey for all tabitems has to be reset when an item is removed
        'from the imagelist. Apparently, the tabcontrol tracks imagelist items by index internally 
        'even when you specify an imagekey
        For Each tabPage As TabPage In TabControl1.TabPages
            tabPage.ImageKey = tabPage.ImageKey 'just set to same imagekey to knock sense into the tabcontrol
        Next tabPage

    End Sub

    Private Sub ApplySelectedTabEmphasis()

        'apparently I typed all this for no reason because the intrinsic system tab 
        'control doesn't seem to support individual tabs being bolded
        If Not Me.SelectedWrappedWindowItem Is Nothing Then
            Dim tabPage As TabPage = FindTabPage(Me.SelectedWrappedWindowItem)

            If Not tabPage Is Nothing Then
                If tabPage.Font.Bold <> Me.EmphasizeSelectedTab Then
                    If Me.EmphasizeSelectedTab Then
                        tabPage.Font = New Font(tabPage.Font, FontStyle.Bold)
                    Else
                        tabPage.Font = New Font(tabPage.Font, FontStyle.Regular)
                    End If
                End If
            End If
        End If

    End Sub

    'find the tab that corresponds to the WrappedWindow
    Private Function FindTabPage(ByVal wrappedWindow As WrappedWindow) As TabPage

        Dim result As TabPage

        For Each tabPage As TabPage In Me.TabControl1.TabPages
            If tabPage.Tag Is wrappedWindow Then
                result = tabPage
                Exit For
            End If
        Next tabPage

        Return result

    End Function

    'select a tab by screen coordinates
    'we're using this for 'Right-click' because the intrinsic TabControl doesn't
    'respond to right-clicking natively.
    Private Sub SetSelectedTabItemByPoint(ByVal pos As Point)

        'I have no idea if the TabControl supports select by right-click... but
        'it was faster writing this than trying to find out
        Dim index As Integer = GetTabPageIndexAtPoint(pos)

        If index <> -1 Then
            Me.TabControl1.SelectedIndex = index
        End If

    End Sub

    Private Function GetTabPageIndexAtPoint(ByVal pos As Point) As Integer

        'I know GetChildAtPoint() exists... not bothering with it right now
        For index As Integer = 0 To Me.TabControl1.TabPages.Count - 1
            If Me.TabControl1.GetTabRect(index).Contains(pos) Then
                Return index
            End If
        Next index

        Return -1

    End Function

    'used to truncate window title that may be overly long.
    Private Function GetTruncatedTabText(ByVal s As String) As String

        If Len(s) <= 20 Then
            Return s
        Else
            Return Microsoft.VisualBasic.Left(s, 30) + "..."
        End If
        'Yes I like the VB.Classic text manipulation functions. 
        'They're way more elegant than .NET's, which need a bunch of conditional checks
        'to make sure you're not dealing with a NothingString. 

    End Function

#End Region

#Region "TabControl Event Handlers"

    Private Sub TabControl1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.SelectedIndexChanged

        'The user has selected a new tab. If the tab was changed by the parent tab strip (see 
        'OnSelectedTabItemChanged) then we do nothing since this may trigger a cascade (such as 
        'if the parent tabstrip rejects the change and tries to switch back to a different tab.
        If Not m_ignoreTabControlIndexChangedEvent Then
            If Not Me.TabControl1.SelectedTab Is Nothing Then
                Me.SelectedWrappedWindowItem = CType(Me.TabControl1.SelectedTab.Tag, WrappedWindow)
            Else
                Me.SelectedWrappedWindowItem = Nothing
            End If
        End If

    End Sub

    Private Sub TabControl1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.Click

        'the click event of the TabControl is the only way to determine if the user clicks on an already selected tab
        If Not Me.TabControl1.SelectedTab Is Nothing AndAlso Me.TabControl1.SelectedTab.Tag Is Me.SelectedWrappedWindowItem Then
            Me.SelectedWrappedWindowItem = Me.SelectedWrappedWindowItem 'this will trigger a reselect (see OnSelectedTabItemReselected) 
        End If

    End Sub

    Private Sub TabControl1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TabControl1.MouseDown

        'this will allow us to give the user a little margin of movement before initiating a drag
        m_mouseDownStartPoint = e.Location

        'right-click- select the tab at the mouse location and request that the WindowManagerPanel menu be shown
        If e.Button = MouseButtons.Right Then
            SetSelectedTabItemByPoint(e.Location)
            If Not Me.SelectedWrappedWindowItem Is Nothing Then
                OnShowWindowCmdMenuRequested(EventArgs.Empty)
            End If
        End If

    End Sub

    Private Sub TabControl1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles TabControl1.MouseMove

        'initiating a drag of a tab is easy... and handled mostly by TabsProviderBase
        If e.Button = MouseButtons.Left AndAlso Not Me.SelectedWrappedWindowItem Is Nothing Then
            'doing this check to provide the user with a little leeway when clicking on tabs
            'before initiating a drag
            If Math.Abs(e.Location.X - m_mouseDownStartPoint.X) >= 3 OrElse Math.Abs(e.Location.Y - m_mouseDownStartPoint.Y) >= 3 Then
                OnBeginDragTabItem(New WrappedWindowItemEventArgs(Me.SelectedWrappedWindowItem))
            End If
        End If

    End Sub

    Private Sub TabControl1_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles TabControl1.DragOver

        'determine if we're interested in the data being dragged
        If Not e.Data.GetDataPresent(GetType(WrappedWindowDragDropData)) OrElse m_ignoreTabControlDragOver Then
            e.Effect = DragDropEffects.None
        Else
            Dim data As WrappedWindowDragDropData = CType(e.Data.GetData(GetType(WrappedWindowDragDropData)), WrappedWindowDragDropData)

            If data.WindowTabStrip Is Me.Parent Then
                'user is reordering the tabs
                Dim index As Integer = GetTabPageIndexAtPoint(Me.TabControl1.PointToClient(New Point(e.X, e.Y)))

                If index <> -1 AndAlso index <> Me.TabControl1.SelectedIndex Then
                    Dim tabPage As TabPage = Me.TabControl1.SelectedTab

                    m_ignoreTabControlIndexChangedEvent = True
                    m_ignoreTabControlDragOver = True
                    Me.TabControl1.SelectedTab = Nothing
                    Me.TabControl1.TabPages.Remove(tabPage)
                    Me.TabControl1.TabPages.Insert(index, tabPage)
                    Me.TabControl1.SelectedTab = tabPage
                    m_ignoreTabControlDragOver = False
                    m_ignoreTabControlIndexChangedEvent = False
                    Me.TabControl1.Refresh()
                End If

                e.Effect = DragDropEffects.Move
            Else
                'this tab comes from a different tab strip
                e.Effect = DragDropEffects.Move
            End If
        End If

    End Sub

    Private Sub TabControl1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles TabControl1.DragDrop

        'determine if we're interested in the data being dragged
        If Not e.Data.GetDataPresent(GetType(WrappedWindowDragDropData)) Then
            e.Effect = DragDropEffects.None
        Else
            Dim data As WrappedWindowDragDropData = CType(e.Data.GetData(GetType(WrappedWindowDragDropData)), WrappedWindowDragDropData)

            If data.WindowTabStrip Is Me.Parent Then
                'essentially do nothing... we handled tab rearranging in real-time during the dragover itself
                e.Effect = DragDropEffects.Move
            Else
                'a new tab was dropped... offload the work to the TabsProviderBase
                OnTabItemDropped(New DroppedWrappedWindowEventArgs(data))
            End If
        End If

    End Sub

    'this is to allow drag/dropping over parts that the TabControl doesn't cover
    'see SystemTabsProvider_DragDrop
    Private Sub SystemTabsProvider_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragOver

        If Not e.Data.GetDataPresent(GetType(WrappedWindowDragDropData)) Then
            e.Effect = DragDropEffects.None
        Else
            e.Effect = DragDropEffects.Move
        End If

    End Sub

    'this is to allow drag/dropping over parts that the TabControl doesn't cover
    'see SystemTabsProvider_DragDrop
    Private Sub SystemTabsProvider_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles Me.DragDrop

        If Not e.Data.GetDataPresent(GetType(WrappedWindowDragDropData)) Then
            e.Effect = DragDropEffects.None
        Else
            Dim data As WrappedWindowDragDropData = CType(e.Data.GetData(GetType(WrappedWindowDragDropData)), WrappedWindowDragDropData)

            OnTabItemDropped(New DroppedWrappedWindowEventArgs(data))
        End If

    End Sub

#End Region

End Class
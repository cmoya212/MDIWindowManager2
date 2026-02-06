Imports System.ComponentModel

'<ToolboxItemAttribute(True), _
'Description("Provides a tabbed MDI management for MDI child windows."), _
'ToolboxBitmap(GetType(WindowManager))> _


''' <summary>
''' Provides a tabbed MDI management for MDI child windows.
''' </summary>
''' <remarks></remarks>
Friend Class WindowManager
    Inherits System.ComponentModel.Component

    Public Event WindowTiling As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowUnTiling As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowHTiling As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowPoppingOut As EventHandler(Of WrappedWindowCancelEventArgs)
    Public Event WindowPoppingIn As EventHandler(Of WrappedWindowCancelEventArgs)

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

    Private m_parentForm As Form
    Private m_windowManagerPanels As New System.Collections.Generic.List(Of WindowManagerPanel)
    Private m_lastWindowManagerPanel As WindowManagerPanel
    Private m_managerPanelsDetectEnabled As Boolean = True
    Private m_autoDetectMdiChildWindows As Boolean = True

#Region " Component Designer generated code "

    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        container.Add(Me)
    End Sub

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Component overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            RemoveParentHandlers()
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
            'GC.SuppressFinalize(Me)
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container

    End Sub

#End Region

    <Browsable(False), _
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Overrides Property Site() As System.ComponentModel.ISite

        Get
            Return MyBase.Site
        End Get

        Set(ByVal value As System.ComponentModel.ISite)
            MyBase.Site = value

            'Automatically add a WindowManagerPanel to the form when the 
            'WindowManager component is dropped onto the form.
            If Not Me.Site Is Nothing Then
                If Me.Site.DesignMode Then
                    Dim dh As Design.IDesignerHost = DirectCast(Me.GetService(GetType(Design.IDesignerHost)), Design.IDesignerHost)

                    If Not dh Is Nothing Then
                        Dim rc As Object = dh.RootComponent

                        If Not rc Is Nothing Then
                            If TypeOf rc Is Form Then
                                Dim frm As Form = DirectCast(rc, Form)

                                If Not dh.Loading AndAlso Not ContainsWindowManagerPanels(frm) Then
                                    Dim windowManagerPanel As WindowManagerPanel = CType(dh.CreateComponent(GetType(WindowManagerPanel)), WindowManagerPanel)

                                    frm.Controls.Add(windowManagerPanel)
                                End If

                                Me.Parent = frm
                            Else
                                Throw New System.NotSupportedException("WindowManager must be hosted directly by a Form.")
                            End If
                        End If
                    End If
                End If
            End If
        End Set

    End Property

    <Browsable(False)> _
    Public Property Parent() As Form

        Get
            Return m_parentForm
        End Get

        Set(ByVal value As Form)
            If Not value Is m_parentForm Then
                RemoveParentHandlers()

                m_parentForm = value

                If Not value Is Nothing Then
                    AddParentHandlers()

                    If m_managerPanelsDetectEnabled Then
                        ScanForWindowManagerPanels()
                    End If
                End If
            End If
        End Set

    End Property

    ''' <summary>
    ''' Allows WindowManager to automatically take control of MDI child windows.
    ''' </summary>
    <Category("Behavior"), _
    Description("Allows the WindowManagerPanel to automatically take control of MDI child windows."), _
    Browsable(True)> _
    Public Property AutoDetectMdiChildWindows() As Boolean

        Get
            Return m_autoDetectMdiChildWindows
        End Get

        Set(ByVal value As Boolean)
            m_autoDetectMdiChildWindows = value
        End Set

    End Property

    ''' <summary>
    ''' Gets or sets a window that will act as side-by-side pane.
    ''' </summary>
    <Description("Gets or sets a window that will act as side-by-side pane."), _
    Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)> _
    Public Property AuxiliaryWindow() As Form

        Get
            Dim windowManagerPanel As WindowManagerPanel = GetPrimaryWindowManagerPanel()

            If Not windowManagerPanel Is Nothing Then
                Return windowManagerPanel.AuxiliaryWindow
            Else
                Return Nothing
            End If
        End Get

        Set(ByVal value As Form)
            GetPrimaryWindowManagerPanel().AuxiliaryWindow = value
        End Set

    End Property

    ''' <summary>
    ''' Get the primary (top) WindowManagerPanel.
    ''' </summary>
    Public Function GetPrimaryWindowManagerPanel() As WindowManagerPanel

        'it's not really necessary to do all this since the primary should always be 
        '(0) in the collection... but this may change in the future
        For Each windowManagerPanel As WindowManagerPanel In m_windowManagerPanels
            If Not windowManagerPanel.IsTemporaryPanel() Then
                Return windowManagerPanel
            End If
        Next windowManagerPanel

        Return Nothing

    End Function

    ''' <summary>
    ''' Get the currently active WindowManagerPanel.
    ''' </summary>
    Public Function GetActiveWindowManagerPanel() As WindowManagerPanel

        Dim result As WindowManagerPanel

        If Not m_lastWindowManagerPanel Is Nothing Then
            result = m_lastWindowManagerPanel
        Else
            For Each windowManagerPanel As WindowManagerPanel In m_windowManagerPanels
                If Not windowManagerPanel.GetActiveWindow() Is Nothing Then
                    result = windowManagerPanel
                    Exit For
                End If
            Next windowManagerPanel
        End If

        If result Is Nothing Then
            Return GetPrimaryWindowManagerPanel()
        Else
            Return result
        End If

    End Function

    Private Sub ScanForWindowManagerPanels()

        If Not m_parentForm Is Nothing Then
            DoScanForWindowManagerPanels(m_parentForm)
        End If

    End Sub

    Private Sub DoScanForWindowManagerPanels(ByVal control As Control)

        For Each childControl As Control In control.Controls
            If TypeOf childControl Is WindowManagerPanel Then
                m_windowManagerPanels.Add(DirectCast(childControl, WindowManagerPanel))
            ElseIf childControl.Controls.Count > 0 Then
                DoScanForWindowManagerPanels(childControl)
            End If
        Next childControl

    End Sub

    Private Function ContainsWindowManagerPanels(ByVal control As Control) As Boolean

        Dim result As Boolean = False

        If Not control Is Nothing Then
            For Each childControl As Control In control.Controls
                If TypeOf childControl Is WindowManagerPanel Then
                    result = True
                    Exit For
                End If
            Next childControl
        End If

        Return result

    End Function

    Private Sub AddWindowManagerPanel(ByVal windowManagerPanel As WindowManagerPanel)

        m_windowManagerPanels.Add(windowManagerPanel)
        AddWindowManagerPanelHandlers(windowManagerPanel)

    End Sub

    Private Sub RemoveWindowManagerPanel(ByVal windowManagerPanel As WindowManagerPanel)

        RemoveWindowManagerPanelHandlers(windowManagerPanel)
        m_windowManagerPanels.Remove(windowManagerPanel)

    End Sub

    Private Sub AddParentHandlers()

        If Not m_parentForm Is Nothing Then
            AddHandler m_parentForm.ControlAdded, AddressOf HandleParentControlAdded
            AddHandler m_parentForm.ControlRemoved, AddressOf HandleParentControlRemoved
            AddHandler m_parentForm.MdiChildActivate, AddressOf HandleParentMdiChildActivate
            AddHandler m_parentForm.FormClosed, AddressOf HandleParentFormClosed
        End If

    End Sub

    Private Sub RemoveParentHandlers()

        If Not m_parentForm Is Nothing Then
            RemoveHandler m_parentForm.ControlAdded, AddressOf HandleParentControlAdded
            RemoveHandler m_parentForm.ControlRemoved, AddressOf HandleParentControlRemoved
            RemoveHandler m_parentForm.MdiChildActivate, AddressOf HandleParentMdiChildActivate
            RemoveHandler m_parentForm.FormClosed, AddressOf HandleParentFormClosed
        End If

    End Sub

    Private Sub AddWindowManagerPanelHandlers(ByVal windowManagerPanel As WindowManagerPanel)

        With windowManagerPanel
            AddHandler .IsActiveChanged, AddressOf HandleWindowManagerPanelIsActiveChanged

            AddHandler .WindowTiling, AddressOf HandleWindowManagerPanelWindowTiling
            AddHandler .WindowUnTiling, AddressOf HandleWindowManagerPanelWindowUnTiling
            AddHandler .WindowHTiling, AddressOf HandleWindowManagerPanelWindowHTiling
            AddHandler .WindowPoppingOut, AddressOf HandleWindowManagerPanelWindowPoppingOut
            AddHandler .WindowPoppingIn, AddressOf HandleWindowManagerPanelWindowPoppingIn

            AddHandler .BeforeWrappedWindowAdded, AddressOf HandleWindowManagerPanelBeforeWrappedWindowAdded
            AddHandler .BeforeWrappedWindowRemoved, AddressOf HandleWindowManagerPanelBeforeWrappedWindowRemoved
            AddHandler .WrappedWindowAdded, AddressOf HandleWindowManagerPanelWrappedWindowAdded
            AddHandler .WrappedWindowRemoved, AddressOf HandleWindowManagerPanelWrappedWindowRemoved
            AddHandler .WrappedWindowsCleared, AddressOf HandleWindowManagerPanelWrappedWindowsCleared
            AddHandler .WindowActivated, AddressOf HandleWindowManagerPanelWindowActivated
            AddHandler .WindowClosed, AddressOf HandleWindowManagerPanelWindowClosed
            AddHandler .WindowClosing, AddressOf HandleWindowManagerPanelWindowClosing
            AddHandler .WindowDeactivate, AddressOf HandleWindowManagerPanelWindowDeactivate
            AddHandler .WindowEnter, AddressOf HandleWindowManagerPanelWindowEnter
            AddHandler .WindowLeave, AddressOf HandleWindowManagerPanelWindowLeave
            AddHandler .WindowTextChanged, AddressOf HandleWindowManagerPanelWindowTextChanged
            AddHandler .WindowVisibleChanged, AddressOf HandleWindowManagerPanelWindowVisibleChanged
            AddHandler .TempPanelDismissed, AddressOf HandleWindowManagerPanelTempPanelDismissed
        End With

    End Sub

    Private Sub RemoveWindowManagerPanelHandlers(ByVal windowManagerPanel As WindowManagerPanel)

        With windowManagerPanel
            RemoveHandler .IsActiveChanged, AddressOf HandleWindowManagerPanelIsActiveChanged

            RemoveHandler .WindowTiling, AddressOf HandleWindowManagerPanelWindowTiling
            RemoveHandler .WindowUnTiling, AddressOf HandleWindowManagerPanelWindowUnTiling
            RemoveHandler .WindowHTiling, AddressOf HandleWindowManagerPanelWindowHTiling
            RemoveHandler .WindowPoppingOut, AddressOf HandleWindowManagerPanelWindowPoppingOut
            RemoveHandler .WindowPoppingIn, AddressOf HandleWindowManagerPanelWindowPoppingIn

            RemoveHandler .BeforeWrappedWindowAdded, AddressOf HandleWindowManagerPanelBeforeWrappedWindowAdded
            RemoveHandler .BeforeWrappedWindowRemoved, AddressOf HandleWindowManagerPanelBeforeWrappedWindowRemoved
            RemoveHandler .WrappedWindowAdded, AddressOf HandleWindowManagerPanelWrappedWindowAdded
            RemoveHandler .WrappedWindowRemoved, AddressOf HandleWindowManagerPanelWrappedWindowRemoved
            RemoveHandler .WrappedWindowsCleared, AddressOf HandleWindowManagerPanelWrappedWindowsCleared
            RemoveHandler .WindowActivated, AddressOf HandleWindowManagerPanelWindowActivated
            RemoveHandler .WindowClosed, AddressOf HandleWindowManagerPanelWindowClosed
            RemoveHandler .WindowClosing, AddressOf HandleWindowManagerPanelWindowClosing
            RemoveHandler .WindowDeactivate, AddressOf HandleWindowManagerPanelWindowDeactivate
            RemoveHandler .WindowEnter, AddressOf HandleWindowManagerPanelWindowEnter
            RemoveHandler .WindowLeave, AddressOf HandleWindowManagerPanelWindowLeave
            RemoveHandler .WindowTextChanged, AddressOf HandleWindowManagerPanelWindowTextChanged
            RemoveHandler .WindowVisibleChanged, AddressOf HandleWindowManagerPanelWindowVisibleChanged
            RemoveHandler .TempPanelDismissed, AddressOf HandleWindowManagerPanelTempPanelDismissed
        End With

    End Sub

    ''' <summary>
    ''' Gets the currently active window in the parent form.
    ''' </summary>
    ''' <remarks>This result normally is the same as ActiveMdiChild but might not be under some sitations (such as when an Aux Window is set).</remarks>
    Public Function GetActiveWindow() As WrappedWindow

        Dim windowManagerPanel As WindowManagerPanel = GetActiveWindowManagerPanel()

        If Not windowManagerPanel Is Nothing Then
            Return windowManagerPanel.GetActiveWindow()
        End If

        Return Nothing

    End Function

    ''' <summary>
    ''' Retrieve the wrapper object for a window.
    ''' </summary>
    ''' <param name="frm"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetWrapperForWindow(ByVal frm As Form) As WrappedWindow

        If Not frm Is Nothing Then
            Dim result As WrappedWindow = GetPrimaryWindowManagerPanel().GetWrapperForWindow(frm, True)

            If result Is Nothing Then
                result = g_poppedOutWindowsManager.FindWrapper(frm)
            End If

            Return result
        End If

    End Function

    ''' <summary>
    ''' Get the panel a wrapped window is currently in.
    ''' </summary>
    Public Function GetPanelForWindow(ByVal wrappedWindow As WrappedWindow) As WindowManagerPanel

        If Not wrappedWindow Is Nothing Then
            For Each windowManagerPanel As WindowManagerPanel In m_windowManagerPanels
                If windowManagerPanel.GetAllWindows().Contains(wrappedWindow) Then
                    Return windowManagerPanel
                End If
            Next windowManagerPanel
        End If

    End Function

    ''' <summary>
    ''' Get all WindowManagerPanels on the form.
    ''' </summary>
    Public Function GetAllWindowManagerPanels() As System.Collections.Generic.List(Of WindowManagerPanel)

        Dim windowManagerPanels As New System.Collections.Generic.List(Of WindowManagerPanel)

        For Each windowManagerPanel As WindowManagerPanel In m_windowManagerPanels
            windowManagerPanels.Add(windowManagerPanel)
        Next windowManagerPanel

        Return windowManagerPanels

    End Function

    ''' <summary>
    ''' Set the active window.
    ''' </summary>
    Public Sub SetActiveWindow(ByVal index As Integer)

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        If wrappedWindows.Count > 0 Then
            SetActiveWindow(wrappedWindows.Item(index))
        End If

    End Sub

    ''' <summary>
    ''' Set the active window.
    ''' </summary>
    Public Sub SetActiveWindow(ByVal wrappedWindow As WrappedWindow)

        Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

        windowManagerPanel.SetActiveWindow(wrappedWindow)

    End Sub

    ''' <summary>
    ''' Set the active window.
    ''' </summary>
    Public Sub SetActiveWindow(ByVal frm As Form)

        Dim wrappedWindow As WrappedWindow = GetWrapperForWindow(frm)

        SetActiveWindow(wrappedWindow)

    End Sub

    ''' <summary>
    ''' Get all windows from all the WindowManagerPanels.
    ''' </summary>
    Public Function GetAllWindows() As WrappedWindowCollection

        Dim allWrappedWindows As New WrappedWindowCollection

        For Each windowManagerPanel As WindowManagerPanel In m_windowManagerPanels
            Dim wrappedWindows As WrappedWindowCollection = windowManagerPanel.GetAllWindows()

            For Each wrappedWindow As WrappedWindow In wrappedWindows
                allWrappedWindows.Add(wrappedWindow)
            Next wrappedWindow
        Next windowManagerPanel

        Return allWrappedWindows

    End Function

    ''' <summary>
    ''' Add a window to the primary WindowManagerPanel.
    ''' </summary>
    Public Sub AddWindow(ByVal frm As Form)

        GetPrimaryWindowManagerPanel().AddWindow(frm)

    End Sub

    ''' <summary>
    ''' Add a window to the primary WindowManagerPanel.
    ''' </summary>
    Public Sub AddWindow(ByVal wrappedWindow As WrappedWindow)

        GetPrimaryWindowManagerPanel().AddWindow(wrappedWindow)

    End Sub

    ''' <summary>
    ''' Remove a window from WindowManager's control.
    ''' </summary>
    Public Sub RemoveWindow(ByVal frm As Form)

        If Not frm Is Nothing Then
            Dim wrappedWindow As WrappedWindow = GetWrapperForWindow(frm)

            If Not wrappedWindow Is Nothing Then
                RemoveWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Remove a window from WindowManager's control.
    ''' </summary>
    Public Sub RemoveWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                windowManagerPanel.RemoveWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Determine if a wrapped window is on a temporarily chained WindowManagerPanel.
    ''' </summary>
    Public Function IsWrappedWindowHTiled(ByVal wrappedWindow As WrappedWindow) As Boolean

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            Return (Not windowManagerPanel Is Nothing AndAlso windowManagerPanel.IsTemporaryPanel)
        End If

    End Function

    ''' <summary>
    ''' Determine if a wrapped window is alone in a tab strip.
    ''' </summary>
    Public Function IsWrappedWindowTiled(ByVal wrappedWindow As WrappedWindow) As Boolean

        If Not wrappedWindow Is Nothing Then
            Dim windowTabStrip As WindowTabStrip = GetPrimaryWindowManagerPanel().GetWrappedWindowTabStrip(wrappedWindow, True)

            If Not windowTabStrip Is Nothing Then
                Return (windowTabStrip.WrappedWindows.Count = 1)
            Else
                Return False
            End If
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
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                windowManagerPanel.TileOrUntileWrappedWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Create a new tab strip and put a wrapped window in it.
    ''' </summary>
    Public Sub TileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                windowManagerPanel.TileWrappedWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Return the wrapped window to main tab strip on the panel.
    ''' </summary>
    Public Sub UntileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                windowManagerPanel.UntileWrappedWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Add a new WindowManagerPanel to the chain and put a wrapped window in it.
    ''' </summary>
    Public Sub HTileWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                windowManagerPanel.HTileWrappedWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Remove the wrapped window from WindowManagerPanel and place it on the desktop as an overlapping window.
    ''' </summary>
    Public Sub PopOutWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If Not windowManagerPanel Is Nothing Then
                windowManagerPanel.PopOutWrappedWindow(wrappedWindow)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Return an overlapping wrapped window to the primary WindowManagerPanel.
    ''' </summary>
    Public Sub PopInWrappedWindow(ByVal wrappedWindow As WrappedWindow)

        If Not wrappedWindow Is Nothing Then
            Dim windowManagerPanel As WindowManagerPanel = GetPanelForWindow(wrappedWindow)

            If windowManagerPanel Is Nothing Then
                windowManagerPanel.PopInWrappedWindow(wrappedWindow)
            End If
        End If

    End Sub

    Public Function CreatePoppedOutWrappedWindow(ByVal frm As Form) As WrappedWindow

        If Not frm Is Nothing Then
            If Not m_parentForm Is Nothing Then
                If Not GetWrapperForWindow(frm) Is Nothing OrElse Not g_poppedOutWindowsManager.FindWrapper(frm) Is Nothing Then
                    Throw New Exception("WrappedWindow already exists.")
                Else
                    Dim wrappedWindow As New WrappedWindow(frm)

                    wrappedWindow.AdjustFormProperties(False)
                    g_poppedOutWindowsManager.AddWindow(wrappedWindow, m_parentForm)

                    Return wrappedWindow
                End If
            End If
        End If

    End Function

    ''' <summary>
    ''' CTRL+TAB Forward.
    ''' </summary>
    Public Sub SetFocusOnNextWindow()

        Dim windowManagerPanel As WindowManagerPanel = m_lastWindowManagerPanel 'GetWindowManagerPanelWithFocus()

        If Not windowManagerPanel Is Nothing Then
            Dim wrappedWindow As WrappedWindow = GetNextWindow(windowManagerPanel.GetActiveWindow())

            If Not wrappedWindow Is Nothing Then
                ShowWindowWithFocus(wrappedWindow.Window)
            End If
        End If

    End Sub

    ''' <summary>
    ''' CTRL+TAB Backward.
    ''' </summary>
    Public Sub SetFocusOnPreviousWindow()

        Dim windowManagerPanel As WindowManagerPanel = m_lastWindowManagerPanel 'GetWindowManagerPanelWithFocus()

        If Not windowManagerPanel Is Nothing Then
            Dim wrappedWindow As WrappedWindow = GetPreviousWindow(windowManagerPanel.GetActiveWindow())

            If Not wrappedWindow Is Nothing Then
                ShowWindowWithFocus(wrappedWindow.Window)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Get the next window immediately after the specified window.
    ''' </summary>
    Public Function GetNextWindow(ByVal wrappedWindow As WrappedWindow) As WrappedWindow

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        If wrappedWindows.Count > 0 Then
            If Not wrappedWindow Is Nothing Then
                Dim index As Integer = wrappedWindows.IndexOf(wrappedWindow)

                If index < wrappedWindows.Count - 1 Then
                    Return wrappedWindows.Item(index + 1)
                Else
                    Return wrappedWindows.Item(0)
                End If
            Else
                Return wrappedWindows.Item(0)
            End If
        Else
            Return Nothing
        End If

    End Function

    ''' <summary>
    ''' Get the next window immediately preceeding the specified window.
    ''' </summary>
    Public Function GetPreviousWindow(ByVal wrappedWindow As WrappedWindow) As WrappedWindow

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        If wrappedWindows.Count > 0 Then
            If Not wrappedWindow Is Nothing Then
                Dim index As Integer = wrappedWindows.IndexOf(wrappedWindow)

                If index > 0 Then
                    Return wrappedWindows.Item(index - 1)
                Else
                    Return wrappedWindows.Item(wrappedWindows.Count - 1)
                End If
            Else
                Return wrappedWindows.Item(0)
            End If
        Else
            Return Nothing
        End If

    End Function

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

        frm.LoadWindowsList(GetAllWindows())

        Return frm.ShowDialog(owner)

    End Function

    ''' <summary>
    ''' Retrieve an array of menu items representing open windows.
    ''' </summary>
    Public Function GetAllWindowsMenu(Optional ByVal includeTopNineAccelerators As Boolean = False) As ToolStripMenuItem()

        Return GetAllWindowsMenu(0, includeTopNineAccelerators)

    End Function

    ''' <summary>
    ''' Retrieve an array of menu items representing open windows.
    ''' </summary>
    Public Function GetAllWindowsMenu(ByVal limit As Integer, Optional ByVal includeTopNineAccelerators As Boolean = False) As ToolStripMenuItem()

        Dim menuItems() As WrappedWindowMenuItem
        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        If wrappedWindows.Count > 0 Then
            Dim activePanel As WindowManagerPanel = GetActiveWindowManagerPanel()
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

    ''' <summary>
    ''' Close all windows being managed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseAllWindows()

        Dim wrappedWindows As MDIWindowManager.WrappedWindowCollection = GetAllWindows()

        For count As Integer = wrappedWindows.Count - 1 To 0 Step -1
            Try
                ShowWindowWithFocus(wrappedWindows.Item(count).Window)
                wrappedWindows.Item(count).Window.Close()
            Catch
                'do nothing
            End Try
        Next count

    End Sub

    Private Sub CleanUpWrappedWindows()

        Dim wrappedWindows As WrappedWindowCollection = GetAllWindows()

        For Each wrappedWindow As WrappedWindow In wrappedWindows
            wrappedWindow.Dispose()
        Next wrappedWindow

    End Sub

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

    Protected Overridable Sub OnWindowPoppingIn(ByVal e As WrappedWindowCancelEventArgs)

        RaiseEvent WindowPoppingIn(Me, e)

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

    Protected Overridable Sub OnWrappedWindowsCleared(ByVal e As EventArgs)

        RaiseEvent WrappedWindowsCleared(Me, e)

    End Sub

    Protected Overridable Sub OnWindowClosing(ByVal e As WrappedWindowClosingEventArgs)

        RaiseEvent WindowClosing(Me, e)

    End Sub

    Protected Overridable Sub OnWindowClosed(ByVal e As WrappedWindowClosedEventArgs)

        RaiseEvent WindowClosed(Me, e)

    End Sub

    Protected Overridable Sub OnWindowActivated(ByVal e As WrappedWindowEventArgs)

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

    Private Sub HandleParentControlAdded(ByVal sender As Object, ByVal e As System.Windows.Forms.ControlEventArgs)

        If m_managerPanelsDetectEnabled Then
            If TypeOf e.Control Is WindowManagerPanel AndAlso Not m_windowManagerPanels.Contains(DirectCast(e.Control, WindowManagerPanel)) Then
                AddWindowManagerPanel(CType(e.Control, WindowManagerPanel))
            End If
        End If

    End Sub

    Private Sub HandleParentControlRemoved(ByVal sender As Object, ByVal e As System.Windows.Forms.ControlEventArgs)

        If m_managerPanelsDetectEnabled Then
            If TypeOf e.Control Is WindowManagerPanel AndAlso m_windowManagerPanels.Contains(DirectCast(e.Control, WindowManagerPanel)) Then
                RemoveWindowManagerPanel(CType(e.Control, WindowManagerPanel))
            End If
        End If

    End Sub

    Private Sub HandleParentMdiChildActivate(ByVal sender As Object, ByVal e As System.EventArgs)

        If m_autoDetectMdiChildWindows Then
            Dim windowManagerPanel As WindowManagerPanel = GetActiveWindowManagerPanel()

            If Not windowManagerPanel Is Nothing Then
                Try
                    Dim mdiChildForm As Form = m_parentForm.ActiveMdiChild

                    If Not mdiChildForm Is Nothing AndAlso Not mdiChildForm.IsDisposed AndAlso Not mdiChildForm Is windowManagerPanel.AuxiliaryWindow AndAlso Not TypeOf mdiChildForm Is DummyForm Then
                        Dim wrappedWindow As WrappedWindow = GetWrapperForWindow(mdiChildForm)

                        If wrappedWindow Is Nothing Then
                            windowManagerPanel.AddWindow(mdiChildForm)
                            windowManagerPanel.SetActiveWindow(mdiChildForm)
                        Else
                            windowManagerPanel.SetActiveWindow(wrappedWindow)
                        End If
                    End If
                Catch
                    'do nothing
                End Try
            End If
        End If

    End Sub

    Private Sub HandleParentFormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs)

        CleanUpWrappedWindows()

    End Sub

    Private Sub HandleWindowManagerPanelWindowTiling(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnWindowTiling(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowUnTiling(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnWindowUnTiling(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowHTiling(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnWindowHTiling(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowPoppingOut(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnWindowPoppingOut(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowPoppingIn(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnWindowPoppingIn(e)

    End Sub

    Private Sub HandleWindowManagerPanelIsActiveChanged(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim windowManagerPanel As WindowManagerPanel = CType(sender, WindowManagerPanel)

        If windowManagerPanel.IsActivePanel Then
            m_lastWindowManagerPanel = windowManagerPanel
        Else
            If windowManagerPanel Is m_lastWindowManagerPanel Then
                'do nothing... I think we need to remember the last active panel
            End If
        End If

    End Sub

    Private Sub HandleWindowManagerPanelTempPanelDismissed(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim windowManagerPanel As WindowManagerPanel = CType(sender, WindowManagerPanel)

        If m_lastWindowManagerPanel Is windowManagerPanel Then
            m_lastWindowManagerPanel = Nothing
        End If

    End Sub

    Private Sub HandleWindowMenuItemClick(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim mnu As WrappedWindowMenuItem = CType(sender, WrappedWindowMenuItem)

        If Not mnu.WrappedWindow Is Nothing Then
            If Not mnu.WrappedWindow.Window Is Nothing Then
                ShowWindowWithFocus(mnu.WrappedWindow.Window)
            End If
        End If

    End Sub

    Private Sub HandleWindowManagerPanelBeforeWrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnBeforeWrappedWindowAdded(e)

    End Sub

    Private Sub HandleWindowManagerPanelWrappedWindowAdded(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWrappedWindowAdded(e)

    End Sub

    Private Sub HandleWindowManagerPanelBeforeWrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCancelEventArgs)

        OnBeforeWrappedWindowRemoved(e)

    End Sub

    Private Sub HandleWindowManagerPanelWrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWrappedWindowRemoved(e)

    End Sub

    Private Sub HandleWindowManagerPanelWrappedWindowsCleared(ByVal sender As Object, ByVal e As System.EventArgs)

        OnWrappedWindowsCleared(EventArgs.Empty)

    End Sub

    Private Sub HandleWindowManagerPanelWindowClosing(ByVal sender As Object, ByVal e As WrappedWindowClosingEventArgs)

        OnWindowClosing(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowClosed(ByVal sender As Object, ByVal e As WrappedWindowClosedEventArgs)

        OnWindowClosed(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowActivated(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWindowActivated(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowDeactivate(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWindowDeactivate(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowEnter(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWindowEnter(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowLeave(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWindowLeave(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowTextChanged(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWindowTextChanged(e)

    End Sub

    Private Sub HandleWindowManagerPanelWindowVisibleChanged(ByVal sender As Object, ByVal e As WrappedWindowEventArgs)

        OnWindowVisibleChanged(e)

    End Sub

End Class
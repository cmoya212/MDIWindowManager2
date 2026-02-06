Public Class WrappedWindowEventArgs
    Inherits WrappedWindowCollection.ItemsEventArgs

    Private m_windowTabStrip As WindowTabStrip
    Private m_windowManagerPanel As WindowManagerPanel

    Public Sub New(ByVal wrappedWindow As WrappedWindow, ByVal windowTabStrip As WindowTabStrip, ByVal windowManagerPanel As WindowManagerPanel)

        MyBase.New(wrappedWindow)

        m_windowTabStrip = windowTabStrip
        m_windowManagerPanel = windowManagerPanel

    End Sub

    Public ReadOnly Property WindowTabStrip() As WindowTabStrip

        Get
            Return m_windowTabStrip
        End Get

    End Property

    Public ReadOnly Property WindowManagerPanel() As WindowManagerPanel

        Get
            Return m_windowManagerPanel
        End Get

    End Property

    Public Shared Shadows ReadOnly Property Empty() As WrappedWindowEventArgs

        Get
            Return New WrappedWindowEventArgs(Nothing, Nothing, Nothing)
        End Get

    End Property

End Class

Public Class WrappedWindowClosedEventArgs
    Inherits WrappedWindowEventArgs

    Private m_closeReason As System.Windows.Forms.CloseReason = CloseReason.None

    Public Sub New(ByVal wrappedWindow As WrappedWindow, ByVal windowTabStrip As WindowTabStrip, ByVal windowManagerPanel As WindowManagerPanel, ByVal closeReason As System.Windows.Forms.CloseReason)

        MyBase.New(wrappedWindow, windowTabStrip, windowManagerPanel)

        m_closeReason = closeReason

    End Sub

    Public ReadOnly Property CloseReason() As System.Windows.Forms.CloseReason

        Get
            Return m_closeReason
        End Get

    End Property

    Public Shared Shadows ReadOnly Property Empty() As WrappedWindowClosedEventArgs

        Get
            Return New WrappedWindowClosedEventArgs(Nothing, Nothing, Nothing, CloseReason.None)
        End Get

    End Property

End Class

Public Class WrappedWindowCancelEventArgs
    Inherits WrappedWindowCollection.ItemsCancelEventArgs

    Private m_windowTabStrip As WindowTabStrip
    Private m_windowManagerPanel As WindowManagerPanel

    Public Sub New(ByVal wrappedWindow As WrappedWindow, ByVal windowTabStrip As WindowTabStrip, ByVal windowManagerPanel As WindowManagerPanel)

        MyBase.New(wrappedWindow)

        m_windowTabStrip = windowTabStrip
        m_windowManagerPanel = windowManagerPanel

    End Sub

    Public ReadOnly Property WindowTabStrip() As WindowTabStrip

        Get
            Return m_windowTabStrip
        End Get

    End Property

    Public ReadOnly Property WindowManagerPanel() As WindowManagerPanel

        Get
            Return m_windowManagerPanel
        End Get

    End Property

    Public Shared Shadows ReadOnly Property Empty() As WrappedWindowCancelEventArgs

        Get
            Return New WrappedWindowCancelEventArgs(Nothing, Nothing, Nothing)
        End Get

    End Property

End Class

Public Class WrappedWindowClosingEventArgs
    Inherits WrappedWindowCancelEventArgs

    Private m_closeReason As System.Windows.Forms.CloseReason = CloseReason.None

    Public Sub New(ByVal wrappedWindow As WrappedWindow, ByVal windowTabStrip As WindowTabStrip, ByVal windowManagerPanel As WindowManagerPanel, ByVal closeReason As System.Windows.Forms.CloseReason)

        MyBase.New(wrappedWindow, windowTabStrip, windowManagerPanel)

        m_closeReason = closeReason

    End Sub

    Public ReadOnly Property CloseReason() As System.Windows.Forms.CloseReason

        Get
            Return m_closeReason
        End Get

    End Property

    Public Shared Shadows ReadOnly Property Empty() As WrappedWindowCancelEventArgs

        Get
            Return New WrappedWindowClosingEventArgs(Nothing, Nothing, Nothing, CloseReason.None)
        End Get

    End Property

End Class

Public Class SelectedWrappedWindowChangedEventArgs
    Inherits EventArgs

    Private m_selectedWrappedWindow As WrappedWindow
    Private m_previouslySelectedWrappedWindow As WrappedWindow

    Public Sub New(ByVal selectedWrappedWindow As WrappedWindow, ByVal previouslySelectedWrappedWindow As WrappedWindow)

        m_selectedWrappedWindow = selectedWrappedWindow
        m_previouslySelectedWrappedWindow = previouslySelectedWrappedWindow

    End Sub

    Public ReadOnly Property SelectedWrappedWindow() As WrappedWindow

        Get
            Return m_selectedWrappedWindow
        End Get

    End Property

    Public ReadOnly Property PreviouslySelectedWrappedWindow() As WrappedWindow

        Get
            Return m_previouslySelectedWrappedWindow
        End Get

    End Property

End Class

Public Class WrappedWindowItemEventArgs
    Inherits EventArgs

    Private m_wrappedWindow As WrappedWindow

    Public Sub New(ByVal wrappedWindow As WrappedWindow)

        m_wrappedWindow = wrappedWindow

    End Sub

    Public ReadOnly Property WrappedWindow() As WrappedWindow

        Get
            Return m_wrappedWindow
        End Get

    End Property

End Class

Public Class DroppedWrappedWindowEventArgs
    Inherits EventArgs

    Private m_data As WrappedWindowDragDropData

    Public Sub New(ByVal data As WrappedWindowDragDropData)

        m_data = data

    End Sub

    Public ReadOnly Property Data() As WrappedWindowDragDropData

        Get
            Return m_data
        End Get

    End Property

End Class

Public Class NewSizeEventArgs
    Inherits System.EventArgs

    Private m_rect As Rectangle

    Public Sub New(ByVal rect As Rectangle)

        m_rect = rect

    End Sub

    Public ReadOnly Property Rectangle() As Rectangle

        Get
            Return m_rect
        End Get

    End Property

End Class

Public Class BeforeSizeEventArgs
    Inherits System.EventArgs

    Private m_rect As Rectangle

    Public Sub New(ByVal rect As Rectangle)

        m_rect = rect

    End Sub

    Public Property Rectangle() As Rectangle

        Get
            Return m_rect
        End Get

        Set(ByVal value As Rectangle)
            m_rect = value
        End Set

    End Property

End Class

Public Class SplitterEventArgs
    Inherits System.EventArgs

    Private m_newRect As Rectangle
    Private m_oldRect As Rectangle

    Public Sub New(ByVal newRect As Rectangle, ByVal oldRect As Rectangle)

        m_newRect = newRect
        m_oldRect = oldRect

    End Sub

    Public ReadOnly Property NewRectangle() As Rectangle

        Get
            Return m_newRect
        End Get

    End Property

    Public ReadOnly Property OldRectangle() As Rectangle

        Get
            Return m_oldRect
        End Get

    End Property

End Class

Public Class NewTabsProviderInstanceCreatedEventArgs
    Inherits System.EventArgs

    Private m_tabsProvider As ITabsProvider

    Public Sub New(ByVal tabsProvider As ITabsProvider)

        m_tabsProvider = tabsProvider

    End Sub

    Public ReadOnly Property TabsProvider() As ITabsProvider

        Get
            Return m_tabsProvider
        End Get

    End Property

End Class
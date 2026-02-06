Public Class WrappedWindowDragDropData

    Private m_windowManagerPanel As WindowManagerPanel
    Private m_windowTabStrip As WindowTabStrip
    Private m_wrappedWindow As WrappedWindow

    Public Sub New(ByVal windowManagerPanel As WindowManagerPanel, ByVal windowTabStrip As WindowTabStrip, ByVal wrappedWindow As WrappedWindow)

        m_windowManagerPanel = windowManagerPanel
        m_windowTabStrip = windowTabStrip
        m_wrappedWindow = wrappedWindow

    End Sub

    Public ReadOnly Property WindowManagerPanel() As WindowManagerPanel

        Get
            Return m_windowManagerPanel
        End Get

    End Property

    Public ReadOnly Property WindowTabStrip() As WindowTabStrip

        Get
            Return m_windowTabStrip
        End Get

    End Property

    Public ReadOnly Property WrappedWindow() As WrappedWindow

        Get
            Return m_wrappedWindow
        End Get

    End Property

End Class
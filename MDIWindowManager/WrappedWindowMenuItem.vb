''' <summary>
''' Custom MenuItem provided to help display wrapped windows in menus
''' </summary>
''' <remarks></remarks>
Public Class WrappedWindowMenuItem
    Inherits ToolStripMenuItem

    Private WithEvents m_wrappedWindow As WrappedWindow
    Private m_textPrefix As String = String.Empty

    Public Property WrappedWindow() As WrappedWindow

        Get
            Return m_wrappedWindow
        End Get

        Set(ByVal value As WrappedWindow)
            m_wrappedWindow = value

            SyncWithWindowText()
        End Set

    End Property

    Public Property TextPrefix() As String

        Get
            Return m_textPrefix
        End Get

        Set(ByVal value As String)
            m_textPrefix = value
        End Set

    End Property

    Private Sub m_wrappedWindow_WindowTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_wrappedWindow.WindowTextChanged

        SyncWithWindowText()

    End Sub

    Private Sub SyncWithWindowText()

        If Not m_wrappedWindow Is Nothing Then
            If Not m_wrappedWindow.Window Is Nothing Then
                If m_textPrefix <> String.Empty Then
                    MyBase.Text = m_textPrefix + " " + m_wrappedWindow.Window.Text
                Else
                    MyBase.Text = m_wrappedWindow.Window.Text
                End If
            Else
                MyBase.Text = String.Empty
            End If
        Else
            MyBase.Text = String.Empty
        End If

    End Sub

End Class
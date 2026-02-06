Imports System.Collections.Generic

''' <summary>
''' Used to keep track of windows that are overlapping on the desktop and not directly managed by WindowManagerPanel.
''' </summary>
''' <remarks></remarks>
Friend Class PoppedOutWindowsManager

    Public Event PopInRequested As EventHandler(Of WrappedWindowCollection.ItemsEventArgs)

    Private WithEvents m_poppedOutWindows As New WrappedWindowCollection
    Private m_poppedOutWindowsParents As New Dictionary(Of WrappedWindow, Form)

    Public Sub AddWindow(ByVal wrappedWindow As WrappedWindow, ByVal parentForm As Form)

        m_poppedOutWindows.Add(wrappedWindow)
        m_poppedOutWindowsParents.Add(wrappedWindow, parentForm)

    End Sub

    Public Sub RemoveWindow(ByVal wrappedWindow As WrappedWindow)

        m_poppedOutWindows.Remove(wrappedWindow)
        'event will remove item from parents collection also

    End Sub

    Public Function Contains(ByVal wrappedWindow As WrappedWindow) As Boolean

        Return m_poppedOutWindows.Contains(wrappedWindow)

    End Function

    Public Function GetWindowParent(ByVal wrappedWindow As WrappedWindow) As Form

        Try
            Return m_poppedOutWindowsParents.Item(wrappedWindow)
        Catch
            Return Nothing
        End Try

    End Function

    Public Function FindWrapper(ByVal frm As Form) As WrappedWindow

        For Each wrappedWindow As WrappedWindow In m_poppedOutWindows
            If wrappedWindow.Window Is frm Then
                Return wrappedWindow
            End If
        Next wrappedWindow

        Return Nothing

    End Function

    Protected Overridable Sub OnPopInRequested(ByVal e As WrappedWindowCollection.ItemsEventArgs)

        RaiseEvent PopInRequested(Me, e)

    End Sub

    Private Sub m_poppedOutWindows_PopInRequested(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_poppedOutWindows.PopInRequested

        OnPopInRequested(e)

    End Sub

    Private Sub m_poppedOutWindows_WrappedWindowRemoved(ByVal sender As Object, ByVal e As WrappedWindowCollection.ItemsEventArgs) Handles m_poppedOutWindows.WrappedWindowRemoved

        Try
            m_poppedOutWindowsParents.Remove(e.WrappedWindow)
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub m_poppedOutWindows_WrappedWindowsCleared(ByVal sender As Object, ByVal e As System.EventArgs) Handles m_poppedOutWindows.WrappedWindowsCleared

        m_poppedOutWindowsParents.Clear()

    End Sub

End Class
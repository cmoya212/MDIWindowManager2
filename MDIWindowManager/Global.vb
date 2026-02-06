Imports System.Runtime.CompilerServices

''' <summary>
''' Global helper procs and instantiated objects.
''' </summary>
''' <remarks></remarks>
Friend Module [Global]

    Public g_toolTip As New ToolTip
    Public g_balloonTip As New ToolTip

    Public g_poppedOutWindowsManager As New PoppedOutWindowsManager

    Public Const MINIMUM_TAB_WIDTH_FLOOR As Integer = 50
    Public Const MINIMUM_PANEL_WIDTH As Integer = 150

    Public Sub ShowWindowWithFocus(ByVal frm As Form)

        'Each of these those something slightly different. For instance an Mdi child could be 
        '"active" but not have focus because the focus is held by a control on the Mdi parent.
        'Depending on the situation, the results are unintuitive. Calling Activate on an Mdi
        'child that is the ActiveMdiChild but does NOT have focus does nothing! 
        'Ultimately... the only way to truly "REfocus on a window that is active is to focus
        'on its ActiveControl property.

        If Not frm Is Nothing Then
            frm.Show()

            Try
                frm.BringToFront() 'with mdichildren sometimes Show() just ain't enough

                Try
                    frm.Activate()
                Catch
                    'do nothing
                    Debug.WriteLine("Could not activate.")
                End Try

                Try
                    frm.Focus()
                Catch ex As Exception
                    'do nothing
                    Debug.WriteLine("Could not focus.")
                End Try

                Try
                    If Not frm.ActiveControl Is Nothing Then
                        frm.ActiveControl.Focus()
                    End If
                Catch ex As Exception
                    'do nothing
                    Debug.WriteLine("Could not focus.")
                End Try
            Catch ex As Exception
                'do nothing
                Debug.WriteLine("Could not bring to front. " + ex.ToString())
            End Try
        End If

    End Sub

    Public Sub ShowToolTip(ByVal text As String, ByVal window As System.Windows.Forms.IWin32Window)

        Try
            g_toolTip.Show(text, window)
        Catch
            'do nothing
        End Try

    End Sub

    Public Sub ShowToolTip(ByVal text As String, ByVal window As System.Windows.Forms.IWin32Window, ByVal point As Point)

        Try
            g_toolTip.Show(text, window, point)
        Catch
            'do nothing
        End Try

    End Sub

    Public Sub HideToolTip(ByVal window As System.Windows.Forms.IWin32Window)

        Try
            g_toolTip.Hide(window)
        Catch
            'do nothing
        End Try

    End Sub

    Public Sub ShowBalloonTip(ByVal text As String, ByVal window As System.Windows.Forms.IWin32Window, Optional ByVal toolTipIcon As System.Windows.Forms.ToolTipIcon = ToolTipIcon.Info, Optional ByVal x As Integer = 0, Optional ByVal y As Integer = 0)

        g_balloonTip.IsBalloon = True
        g_balloonTip.ToolTipIcon = toolTipIcon
        g_balloonTip.IsBalloon = True
        g_balloonTip.Show(text, window, x, y)

    End Sub

End Module

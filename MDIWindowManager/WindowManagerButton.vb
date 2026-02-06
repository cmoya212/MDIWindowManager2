Imports System.Drawing

''' <summary>
''' Custom drawn button used by MDIWindowManager.
''' </summary>
''' <remarks></remarks>
Friend Class WindowManagerButton

    Public Enum WindowManagerButtonRenderMode
        Standard
        RollOverClassic
        Fancy
    End Enum

    Private m_borderStyle As BorderStyle = BorderStyle.None
    Private m_renderMode As WindowManagerButtonRenderMode = WindowManagerButtonRenderMode.RollOverClassic
    Private m_toolStripProRenderer As New ToolStripProfessionalRenderer
    Private m_isMouseOver As Boolean = False
    Private m_isMouseDown As Boolean = False

    Public Property BorderStyle() As BorderStyle

        Get
            Return m_borderStyle
        End Get

        Set(ByVal value As BorderStyle)
            m_borderStyle = value
            Invalidate()
        End Set

    End Property

    Public Property RenderMode() As WindowManagerButtonRenderMode

        Get
            Return m_renderMode
        End Get

        Set(ByVal value As WindowManagerButtonRenderMode)
            m_renderMode = value
            Invalidate()
        End Set

    End Property

    Private Function IsNonStandardMode() As Boolean

        Return Not (m_renderMode = WindowManagerButtonRenderMode.Standard)

    End Function

    Protected Overrides Sub OnPaint(ByVal pe As System.Windows.Forms.PaintEventArgs)

        If IsNonStandardMode() Then
            Dim g As Graphics = pe.Graphics
            Dim rect As Rectangle = Me.ClientRectangle
            Dim rect2 As New Rectangle(rect.X, rect.Y, rect.Width - 1, rect.Height - 1)
            Dim rect3 As New Rectangle(rect2.X, rect2.Y + 1, rect2.Width, rect2.Height)
            Dim stringFormat As New StringFormat

            stringFormat.Alignment = StringAlignment.Center
            stringFormat.LineAlignment = StringAlignment.Center

            g.Clear(Me.BackColor)

            If Me.Enabled Then
                If m_isMouseOver OrElse m_isMouseDown Then
                    If m_isMouseDown Then
                        Select Case m_renderMode
                            Case WindowManagerButtonRenderMode.RollOverClassic
                                ButtonRenderer.DrawButton(g, rect, VisualStyles.PushButtonState.Pressed)
                            Case WindowManagerButtonRenderMode.Fancy
                                g.FillRectangle(New Drawing2D.LinearGradientBrush(rect, m_toolStripProRenderer.ColorTable.ButtonPressedGradientBegin, m_toolStripProRenderer.ColorTable.ButtonPressedGradientEnd, Drawing2D.LinearGradientMode.ForwardDiagonal), rect)
                        End Select
                    Else
                        Select Case m_renderMode
                            Case WindowManagerButtonRenderMode.RollOverClassic
                                ButtonRenderer.DrawButton(g, rect, VisualStyles.PushButtonState.Hot)
                            Case WindowManagerButtonRenderMode.Fancy
                                g.FillRectangle(New Drawing2D.LinearGradientBrush(rect, m_toolStripProRenderer.ColorTable.ButtonSelectedGradientBegin, m_toolStripProRenderer.ColorTable.ButtonSelectedGradientEnd, Drawing2D.LinearGradientMode.ForwardDiagonal), rect)
                        End Select
                    End If

                    PaintImage(g)

                    g.DrawString(Me.Text, Me.Font, New SolidBrush(Me.ForeColor), rect3, stringFormat)

                    Select Case m_renderMode
                        Case WindowManagerButtonRenderMode.RollOverClassic
                            If m_borderStyle <> BorderStyle.None Then
                                g.DrawRectangle(SystemPens.ControlDark, rect2)
                            End If
                        Case WindowManagerButtonRenderMode.Fancy
                            g.DrawRectangle(New Pen(m_toolStripProRenderer.ColorTable.MenuItemBorder), rect2)
                    End Select
                Else
                    PaintImage(g)

                    g.DrawString(Me.Text, Me.Font, New SolidBrush(Me.ForeColor), rect3, stringFormat)

                    If m_borderStyle <> BorderStyle.None Then
                        g.DrawRectangle(SystemPens.ControlDark, rect2)
                    End If

                    If Me.Focused Then
                        ControlPaint.DrawFocusRectangle(g, rect2)
                    End If
                End If
            Else
                PaintImage(g)

                g.DrawString(Me.Text, Me.Font, SystemBrushes.GrayText, rect3, stringFormat)

                If m_borderStyle <> BorderStyle.None Then
                    g.DrawRectangle(SystemPens.ControlDark, rect2)
                End If
            End If
        Else
            MyBase.OnPaint(pe)
        End If

    End Sub

    Private Sub PaintImage(ByVal g As Graphics)

        If Not Me.Image Is Nothing Then
            Dim pos1 As PointF
            Dim clientRect As Rectangle = Me.ClientRectangle
            Dim imageSize As Size = Me.Image.Size

            Select Case Me.ImageAlign
                Case ContentAlignment.BottomCenter
                    pos1 = New PointF(Convert.ToSingle((clientRect.Width / 2) - (imageSize.Width / 2)), clientRect.Height - imageSize.Height - 1)
                Case ContentAlignment.BottomLeft
                    pos1 = New PointF(1, clientRect.Height - imageSize.Height - 1)
                Case ContentAlignment.BottomRight
                    pos1 = New PointF(clientRect.Width - imageSize.Width - 1, clientRect.Height - imageSize.Height - 1)
                Case ContentAlignment.MiddleCenter
                    pos1 = New PointF(Convert.ToSingle((clientRect.Width / 2) - (imageSize.Width / 2)), Convert.ToSingle((clientRect.Height / 2) - (imageSize.Height / 2)))
                Case ContentAlignment.MiddleLeft
                    pos1 = New PointF(1, Convert.ToSingle((clientRect.Height / 2) - (imageSize.Height / 2)))
                Case ContentAlignment.MiddleRight
                    pos1 = New PointF(clientRect.Width - imageSize.Width - 1, Convert.ToSingle((clientRect.Height / 2) - (imageSize.Height / 2)))
                Case ContentAlignment.TopCenter
                    pos1 = New PointF(Convert.ToSingle((clientRect.Width / 2) - (imageSize.Width / 2)), 1)
                Case ContentAlignment.TopLeft
                    pos1 = New PointF(1, 1)
                Case ContentAlignment.TopRight
                    pos1 = New PointF(clientRect.Width - imageSize.Width - 1, 1)
            End Select

            Dim pos2 As Point = Point.Round(pos1)

            If Me.Enabled Then
                If Me.ImageList Is Nothing Then
                    g.DrawImage(Me.Image, pos2)
                Else
                    Me.ImageList.Draw(g, pos2, Me.ImageIndex)
                End If
            Else
                ControlPaint.DrawImageDisabled(g, Me.Image, pos2.X, pos2.Y, Me.BackColor)
            End If
        End If

    End Sub

    Protected Overrides Sub OnMouseEnter(ByVal e As System.EventArgs)

        MyBase.OnMouseEnter(e)

        If IsNonStandardMode() Then
            m_isMouseOver = True
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnMouseMove(ByVal mevent As System.Windows.Forms.MouseEventArgs)

        MyBase.OnMouseMove(mevent)

        If IsNonStandardMode() Then
            m_isMouseOver = (mevent.Location.X >= 0 And mevent.Location.Y >= 0 And mevent.Location.X <= Me.Width And mevent.Location.Y <= Me.Height)
            m_isMouseDown = ((mevent.Button And MouseButtons.Left) = MouseButtons.Left)
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnMouseDown(ByVal mevent As System.Windows.Forms.MouseEventArgs)

        MyBase.OnMouseDown(mevent)

        If IsNonStandardMode() Then
            m_isMouseDown = ((mevent.Button And MouseButtons.Left) = MouseButtons.Left)
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnMouseUp(ByVal mevent As System.Windows.Forms.MouseEventArgs)

        MyBase.OnMouseUp(mevent)

        If IsNonStandardMode() Then
            m_isMouseDown = False
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As System.EventArgs)

        MyBase.OnMouseLeave(e)

        If IsNonStandardMode() Then
            m_isMouseOver = False
            m_isMouseDown = False
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnKeyDown(ByVal kevent As System.Windows.Forms.KeyEventArgs)

        MyBase.OnKeyDown(kevent)

        If IsNonStandardMode() Then
            m_isMouseDown = (kevent.KeyValue = Keys.Space)
            Invalidate()
        End If

    End Sub

    Protected Overrides Sub OnKeyUp(ByVal kevent As System.Windows.Forms.KeyEventArgs)

        MyBase.OnKeyUp(kevent)

        If IsNonStandardMode() Then
            m_isMouseDown = False
            Invalidate()
        End If

    End Sub

End Class
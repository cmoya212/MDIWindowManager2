Imports System.ComponentModel

''' <summary>
''' Built-in default tabs provider.
''' </summary>
''' <remarks>Inherits from TabsProviderBase.</remarks>
<ToolboxItemAttribute(False)> _
Public Class StandardTabsProvider

    Public Event TabPaint As EventHandler(Of TabPaintEventArgs)

    Private m_designMode As Boolean = False
    Private m_style As TabStyle
    Private m_enableAeroGlass As Boolean = False
    Private m_tabPadding As Integer = 3
    Private m_currentScrollPos As Integer = 0
    Private m_ensureVisibleActive As Boolean = False
    Private m_suppressFinalScroll As Boolean = False
    Private m_scrollRepeatDirection As Integer = 0
    Private m_mouseDownActive As Boolean = False
    Private m_mouseDownStartPoint As Point
    Private m_currentDragOverTabItem As WrappedWindow
    Private m_ignoreTabControlIndexChangedEvent As Boolean = False
    Private m_enableOwnerPaint As Boolean = False
    Private m_tabPaintEventArgs As New TabPaintEventArgs(Nothing, New TabPaintOptions)
    Private m_tabWidth As Integer = MINIMUM_TAB_WIDTH_FLOOR

    Private Structure TabPositionStruct
        Public WrappedWindow As WrappedWindow
        Public Rectangle As Rectangle
        Public CaptionTruncated As Boolean
    End Structure

    Private m_tabPositions As TabPositionStruct() = {}

    Public Property Style() As TabStyle

        Get
            Return m_style
        End Get

        Set(ByVal value As TabStyle)
            m_style = value
            RefreshTabs()
        End Set

    End Property

    Public Property EnableAeroGlassEffect As Boolean

        Get
            Return m_enableAeroGlass
        End Get

        Set(value As Boolean)
            m_enableAeroGlass = value
            'If Not value Then
            '    Me.BackColor = Nothing
            '    Me.ScrollLeftButton.BackColor = Nothing
            '    Me.ScrollRightButton.BackColor = Nothing
            'Else
            '    Me.BackColor = Color.Transparent
            '    Me.ScrollLeftButton.BackColor = SystemColors.Control
            '    Me.ScrollRightButton.BackColor = SystemColors.Control
            'End If
            RefreshTabs()
        End Set

    End Property

    Public Property EnabledOwnerPaint() As Boolean

        Get
            Return m_enableOwnerPaint
        End Get

        Set(ByVal value As Boolean)
            m_enableOwnerPaint = value
        End Set

    End Property

    Public Sub RefreshTabs()

        If Not Me.IsUpdating Then
            Me.DrawAreaPanel.Refresh()
        End If

    End Sub

    Public Sub ForceRefreshTabs()

        If Not Me.IsUpdating Then
            DrawTabs(Nothing)
        End If

    End Sub

    Public Sub CalcRefreshTabs()

        DrawTabs(Nothing, True)

    End Sub

    Private Sub DrawTabs(ByVal g As Graphics, Optional ByVal calcOnly As Boolean = False)

        Select Case m_style
            Case TabStyle.ClassicTabs, TabStyle.ModernTabs, TabStyle.FlatTabs, TabStyle.AngledTabs
                DrawNormalTabs(g, m_wrappedWindowItems, Me.SelectedWrappedWindowItem, m_style, calcOnly)
            Case Else
                DrawNormalTabs(g, m_wrappedWindowItems, Me.SelectedWrappedWindowItem, TabStyle.ClassicTabs, calcOnly)
        End Select

    End Sub

    Private Sub DrawNormalTabs(ByVal g As Graphics, ByVal wrappedWindows As WrappedWindowCollection, ByVal selectedWrappedWindow As WrappedWindow, ByVal style As TabStyle, ByVal calcOnly As Boolean)

        Dim currentX As Integer = m_currentScrollPos
        Dim currentY As Integer = 1
        Dim scrollLeftRequired As Boolean = False
        Dim scrollRightRequired As Boolean = False
        Dim backColorBrush As SolidBrush
        Dim textColorBrush As SolidBrush
        Dim focusBackColorBrush As SolidBrush
        Dim focusTextColorBrush As SolidBrush
        Dim charactersFitted As Integer
        Dim linesFitted As Integer
        Dim stringFormat As New StringFormat
        Dim captionTruncated As Boolean = False
        Dim draw3D As Boolean = False
        Dim fancyOffset1 As Integer = 0
        Dim fillPadding As Integer
        Dim forceGraphics As Boolean = False
        Dim tabpaintOptions As TabPaintOptions = m_tabPaintEventArgs.TabPaintOptions
        Dim g2 As Graphics
        Dim bmp As Bitmap

        'using this to gauge how often tabs are redrawm during certain events
        'Console.WriteLine("I'm painting tabs!!! " + Now().ToString())

        Try
            If g Is Nothing Then
                forceGraphics = True
                If Me.DrawAreaPanel.ClientRectangle.Width > 0 And Me.DrawAreaPanel.ClientRectangle.Height > 0 Then
                    bmp = New Bitmap(Me.DrawAreaPanel.ClientRectangle.Width, Me.DrawAreaPanel.ClientRectangle.Height)
                Else
                    bmp = New Bitmap(10, 10)
                End If
                g = Graphics.FromImage(bmp)
            End If

            ClearTabPositions()

            If m_designMode Then
                If wrappedWindows.Count = 0 Then
                    For i As Integer = 1 To 2
                        Dim ww As New WrappedWindow(Nothing)
                        ww.Window = New Form
                        ww.Window.Text = "Document " & i.ToString
                        wrappedWindows.Add(ww)
                    Next
                End If
                selectedWrappedWindow = wrappedWindows(0)
            End If

            If Not Me.WordWrap Then
                stringFormat.FormatFlags = StringFormatFlags.NoWrap
            End If
            'stringFormat.FormatFlags = StringFormatFlags.NoWrap
            stringFormat.Trimming = StringTrimming.EllipsisCharacter

            Select Case style
                'Case TabStyle.ClassicTabs
                '    If Not m_enableAeroGlass Then
                '        backColorBrush = New SolidBrush(Me.DrawAreaPanel.BackColor)
                '        focusBackColorBrush = New SolidBrush(Me.DrawAreaPanel.BackColor)
                '    Else
                '        backColorBrush = New SolidBrush(SystemColors.Control)
                '        focusBackColorBrush = New SolidBrush(SystemColors.Control)
                '    End If
                '    textColorBrush = New SolidBrush(Me.DrawAreaPanel.ForeColor)
                '    focusTextColorBrush = New SolidBrush(Me.DrawAreaPanel.ForeColor)
                '    draw3D = True
                '    fancyOffset1 = 0
                'Case TabStyle.ModernTabs
                '    backColorBrush = New SolidBrush(System.Windows.Forms.ControlPaint.Light(Me.DrawAreaPanel.BackColor, 0.5))
                '    textColorBrush = New SolidBrush(System.Windows.Forms.ControlPaint.Light(Me.DrawAreaPanel.ForeColor, 0.5))
                '    focusBackColorBrush = New SolidBrush(Me.DrawAreaPanel.BackColor)
                '    focusTextColorBrush = New SolidBrush(Me.DrawAreaPanel.ForeColor)
                '    draw3D = True
                '    fancyOffset1 = 0
                'Case TabStyle.FlatHiliteTabs
                '    If Not m_enableAeroGlass Then
                '        backColorBrush = New SolidBrush(Me.DrawAreaPanel.BackColor)
                '    Else
                '        backColorBrush = New SolidBrush(Color.Linen)
                '    End If
                '    textColorBrush = New SolidBrush(Me.DrawAreaPanel.ForeColor)
                '    focusBackColorBrush = New SolidBrush(SystemColors.Window)
                '    focusTextColorBrush = New SolidBrush(SystemColors.WindowText)
                '    draw3D = False
                '    fancyOffset1 = 0
                'Case TabStyle.AngledHiliteTabs
                '    If Not m_enableAeroGlass Then
                '        'backColorBrush = New SolidBrush(Me.DrawAreaPanel.BackColor)
                '        backColorBrush = New SolidBrush(Me.TabBackColor)
                '    Else
                '        backColorBrush = New SolidBrush(Color.Linen)
                '    End If
                '    'textColorBrush = New SolidBrush(Me.DrawAreaPanel.ForeColor)
                '    textColorBrush = New SolidBrush(Me.TabForeColor)
                '    'focusBackColorBrush = New SolidBrush(SystemColors.Window)
                '    'focusTextColorBrush = New SolidBrush(SystemColors.WindowText)
                '    focusBackColorBrush = New SolidBrush(Me.SelectedTabBackColor)
                '    focusTextColorBrush = New SolidBrush(Me.SelectedTabForeColor)
                '    draw3D = False
                '    fancyOffset1 = 18
                Case TabStyle.ClassicTabs
                    If Not m_enableAeroGlass Then
                        backColorBrush = New SolidBrush(Me.TabBackColor)
                    Else
                        backColorBrush = New SolidBrush(SystemColors.Control)
                        focusBackColorBrush = New SolidBrush(SystemColors.Control)
                    End If
                    textColorBrush = New SolidBrush(Me.TabForeColor)
                    focusBackColorBrush = New SolidBrush(Me.SelectedTabBackColor)
                    focusTextColorBrush = New SolidBrush(Me.SelectedTabForeColor)
                    draw3D = True
                    fancyOffset1 = 0
                Case TabStyle.ModernTabs
                    backColorBrush = New SolidBrush(Me.TabBackColor)
                    textColorBrush = New SolidBrush(Me.TabForeColor)
                    focusBackColorBrush = New SolidBrush(Me.SelectedTabBackColor)
                    focusTextColorBrush = New SolidBrush(Me.SelectedTabForeColor)
                    draw3D = True
                    fancyOffset1 = 0
                Case TabStyle.FlatTabs
                    If Not m_enableAeroGlass Then
                        backColorBrush = New SolidBrush(Me.TabBackColor)
                    Else
                        backColorBrush = New SolidBrush(Color.Linen)
                    End If
                    textColorBrush = New SolidBrush(Me.TabForeColor)
                    focusBackColorBrush = New SolidBrush(Me.SelectedTabBackColor)
                    focusTextColorBrush = New SolidBrush(Me.SelectedTabForeColor)
                    draw3D = False
                    fancyOffset1 = 0
                Case TabStyle.AngledTabs
                    If Not m_enableAeroGlass Then
                        'backColorBrush = New SolidBrush(Me.DrawAreaPanel.BackColor)
                        backColorBrush = New SolidBrush(Me.TabBackColor)
                    Else
                        backColorBrush = New SolidBrush(Color.Linen)
                    End If
                    textColorBrush = New SolidBrush(Me.TabForeColor)
                    focusBackColorBrush = New SolidBrush(Me.SelectedTabBackColor)
                    focusTextColorBrush = New SolidBrush(Me.SelectedTabForeColor)
                    draw3D = False
                    fancyOffset1 = 18
                Case Else
                    Throw New InvalidOperationException("Unknown WindowManagerStyle.")
            End Select

            With g
                If fancyOffset1 <> 0 Then
                    .SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                End If

                If Not calcOnly Then
                    m_tabPaintEventArgs.Counter = 0

                    If Not m_enableAeroGlass Then
                        '.FillRectangle(backColorBrush, Me.DrawAreaPanel.ClientRectangle)
                    End If

                    If draw3D Then
                        .DrawLine(SystemPens.ControlLightLight, 0, Me.DrawAreaPanel.ClientRectangle.Bottom - 2, Me.DrawAreaPanel.ClientRectangle.Right - 1, Me.DrawAreaPanel.ClientRectangle.Bottom - 2)
                        .DrawLine(SystemPens.ControlDarkDark, 0, Me.DrawAreaPanel.ClientRectangle.Bottom - 1, Me.DrawAreaPanel.ClientRectangle.Right - 1, Me.DrawAreaPanel.ClientRectangle.Bottom - 1)
                    Else
                        .DrawLine(SystemPens.ControlDarkDark, 0, Me.DrawAreaPanel.ClientRectangle.Bottom - 1, Me.DrawAreaPanel.ClientRectangle.Right - 1, Me.DrawAreaPanel.ClientRectangle.Bottom - 1)
                    End If
                End If

                Dim tabLineHeight As Integer = 0

                If wrappedWindows.Count > 0 Then
                    Dim tabTestSize = g.MeasureString("XXXXX XXXXX XXXXX XXXXX", Me.Font)
                    m_tabWidth = tabTestSize.Width
                    tabLineHeight = tabTestSize.Height
                End If

                For Each wrappedWindow As WrappedWindow In wrappedWindows
                    tabpaintOptions.TabRect = New Rectangle(currentX, currentY, m_tabWidth, Me.DrawAreaPanel.ClientRectangle.Height - currentY - 2)

                    captionTruncated = False

                    If tabpaintOptions.TabRect.Right > 0 AndAlso tabpaintOptions.TabRect.Left < Me.DrawAreaPanel.ClientRectangle.Right Then
                        tabpaintOptions.Text = wrappedWindow.Window.Text
                        tabpaintOptions.Font = Me.Font

                        Dim drawBottom As Boolean = False

                        If wrappedWindow Is selectedWrappedWindow Then
                            m_tabPaintEventArgs.IsSelectedValue = True

                            If Me.EmphasizeSelectedTab Then
                                tabpaintOptions.Font = New Font(tabpaintOptions.Font, FontStyle.Bold)
                            End If

                            tabpaintOptions.BackColorBrush = focusBackColorBrush
                            tabpaintOptions.ForeColorBrush = focusTextColorBrush

                            drawBottom = False
                        Else
                            m_tabPaintEventArgs.IsSelectedValue = False

                            'tabpaintOptions.BackColorBrush = Nothing
                            tabpaintOptions.BackColorBrush = backColorBrush
                            tabpaintOptions.ForeColorBrush = textColorBrush

                            drawBottom = True
                        End If

                        If Me.ShowIcons Then
                            'Dim iconRect As New Rectangle(currentX + 4 + fancyOffset1, 3, 16, 16)

                            Dim iconRect As New Rectangle(currentX + 4 + fancyOffset1, 3, tabLineHeight, tabLineHeight)


                            'If tabpaintOptions.TabRect.Height - 2 < 15 Then
                            'iconRect.Height = iconRect.Height + (tabpaintOptions.TabRect.Height - 2 - 16)
                            'End If

                            tabpaintOptions.IconRect = iconRect
                            tabpaintOptions.Icon = wrappedWindow.Window.Icon
                            tabpaintOptions.TextRect = New RectangleF(tabpaintOptions.IconRect.Left + tabpaintOptions.IconRect.Width, 3, tabpaintOptions.TabRect.Width - tabpaintOptions.IconRect.Width - 5 - fancyOffset1, tabpaintOptions.TabRect.Height - 2)
                        Else
                            tabpaintOptions.Icon = Nothing
                            tabpaintOptions.TextRect = New RectangleF(currentX + 4 + fancyOffset1, 3, tabpaintOptions.TabRect.Width - 5 - fancyOffset1, tabpaintOptions.TabRect.Height - 2)
                        End If


                        '--------------------------

                        If m_enableOwnerPaint And Not calcOnly Then
                            m_tabPaintEventArgs.GraphicsObject = g
                            m_tabPaintEventArgs.Handled = False
                            m_tabPaintEventArgs.WrappedWindowItemObject = wrappedWindow
                            m_tabPaintEventArgs.Counter += 1
                            OnTabPaint(m_tabPaintEventArgs)
                        End If

                        '--------------------------


                        If (Not m_enableOwnerPaint OrElse Not m_tabPaintEventArgs.Handled) AndAlso Not calcOnly Then

                            .FillRectangle(New SolidBrush(Me.BackColor), (tabpaintOptions.TabRect.Left - m_tabPadding) + 1, tabpaintOptions.TabRect.Top - 1, tabpaintOptions.TabRect.Width + m_tabPadding, tabpaintOptions.TabRect.Height)

                            If draw3D Then
                                'undone: fancy offset currently not supported when drawing in 3d
                                .DrawLine(SystemPens.ControlLightLight, tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Bottom - 1, tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Top)
                                .DrawLine(SystemPens.ControlLightLight, tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Top, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Top)
                                .DrawLine(SystemPens.ControlDarkDark, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Top + 1, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Bottom - 1)

                                .DrawLine(SystemPens.ControlLight, tabpaintOptions.TabRect.Left + 1, tabpaintOptions.TabRect.Bottom - 1, tabpaintOptions.TabRect.Left + 1, tabpaintOptions.TabRect.Top + 1)
                                .DrawLine(SystemPens.ControlLight, tabpaintOptions.TabRect.Left + 1, tabpaintOptions.TabRect.Top + 1, tabpaintOptions.TabRect.Right - 1, tabpaintOptions.TabRect.Top + 1)
                                .DrawLine(SystemPens.ControlDark, tabpaintOptions.TabRect.Right - 1, tabpaintOptions.TabRect.Top + 2, tabpaintOptions.TabRect.Right - 1, tabpaintOptions.TabRect.Bottom - 1)
                            Else
                                '.DrawLines(SystemPens.ControlDarkDark, New Point() {New Point(tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Bottom - 1), New Point(tabpaintOptions.TabRect.Left + fancyOffset1, tabpaintOptions.TabRect.Top), New Point(tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Top), New Point(tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Bottom - 1)})
                                'keeping the drawing in separate calls will allow us to specify different colors for the different sides
                                .DrawLine(SystemPens.ControlDarkDark, tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Bottom - 1, tabpaintOptions.TabRect.Left + fancyOffset1, tabpaintOptions.TabRect.Top)
                                .DrawLine(SystemPens.ControlDarkDark, tabpaintOptions.TabRect.Left + fancyOffset1, tabpaintOptions.TabRect.Top, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Top)
                                .DrawLine(SystemPens.ControlDarkDark, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Top + 1, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Bottom - 1)
                            End If

                            If Not tabpaintOptions.BackColorBrush Is Nothing Then
                                If draw3D Then
                                    fillPadding = 2
                                Else
                                    fillPadding = 1
                                End If

                                If fancyOffset1 = 0 Then
                                    .FillRectangle(tabpaintOptions.BackColorBrush, tabpaintOptions.TabRect.Left + fillPadding, tabpaintOptions.TabRect.Top + fillPadding, tabpaintOptions.TabRect.Width - (fillPadding + 1), tabpaintOptions.TabRect.Height + 2)
                                Else
                                    .FillPolygon(tabpaintOptions.BackColorBrush, New Point() {New Point(tabpaintOptions.TabRect.Left + fillPadding - 1, tabpaintOptions.TabRect.Bottom + 2), New Point(tabpaintOptions.TabRect.Left + fillPadding + fancyOffset1, tabpaintOptions.TabRect.Top + fillPadding), New Point(tabpaintOptions.TabRect.Right - fillPadding, tabpaintOptions.TabRect.Top + fillPadding), New Point(tabpaintOptions.TabRect.Right - fillPadding, tabpaintOptions.TabRect.Bottom + 2), New Point(tabpaintOptions.TabRect.Left + fillPadding - 1, tabpaintOptions.TabRect.Bottom + 2)})
                                End If
                            End If

                            If Me.ShowIcons AndAlso Not tabpaintOptions.Icon Is Nothing Then
                                .DrawIcon(tabpaintOptions.Icon, tabpaintOptions.IconRect)
                            End If

                            .DrawString(tabpaintOptions.Text, tabpaintOptions.Font, tabpaintOptions.ForeColorBrush, tabpaintOptions.TextRect, stringFormat)

                            If drawBottom Then
                                .DrawLine(SystemPens.ControlDarkDark, tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Bottom, tabpaintOptions.TabRect.Right, tabpaintOptions.TabRect.Bottom)
                            End If
                        End If

                        .MeasureString(tabpaintOptions.Text, tabpaintOptions.Font, tabpaintOptions.TextRect.Size, stringFormat, charactersFitted, linesFitted)

                        If charactersFitted <> tabpaintOptions.Text.Length Then
                            captionTruncated = True
                        End If

                        If Not calcOnly Then
                            If wrappedWindow Is m_currentDragOverTabItem Then
                                .FillPolygon(textColorBrush, New PointF() {New PointF(tabpaintOptions.TabRect.Left - 7, tabpaintOptions.TabRect.Top), New PointF(tabpaintOptions.TabRect.Left, tabpaintOptions.TabRect.Top + CInt((tabpaintOptions.TabRect.Bottom - tabpaintOptions.TabRect.Top) / 2)), New PointF(tabpaintOptions.TabRect.Left + 7, tabpaintOptions.TabRect.Top), New PointF(tabpaintOptions.TabRect.Left - 5, tabpaintOptions.TabRect.Top)})
                            End If
                        End If

                        If currentX + m_tabWidth > Me.DrawAreaPanel.ClientRectangle.Right Then
                            scrollRightRequired = True
                        End If

                        If currentX < 0 Then
                            scrollLeftRequired = True
                        End If
                    Else
                        If currentX + m_tabWidth <= 0 Then
                            scrollLeftRequired = True
                        End If

                        If currentX >= Me.DrawAreaPanel.ClientRectangle.Right Then
                            scrollRightRequired = True
                            'Exit For 'we're going to continue because in the future we're going to allow diff tab sizing
                        End If
                    End If

                    AddTabPosition(wrappedWindow, captionTruncated, tabpaintOptions.TabRect)

                    currentX += (m_tabWidth + m_tabPadding)
                Next wrappedWindow
            End With

            ScrollRightButton.Enabled = scrollRightRequired
            ScrollLeftButton.Enabled = scrollLeftRequired

            If (Not calcOnly) Then
                g.DrawLine(SystemPens.ControlDarkDark, tabpaintOptions.TabRect.Right + 1, tabpaintOptions.TabRect.Top, tabpaintOptions.TabRect.Right + 1, tabpaintOptions.TabRect.Bottom - 1)

            End If

            If Not calcOnly Then
                g.Flush()

                If forceGraphics AndAlso Not calcOnly Then
                    g2 = DrawAreaPanel.CreateGraphics()

                    Try
                        If m_enableAeroGlass Then
                            g2.Clear(Color.Black)
                        End If
                        g2.DrawImageUnscaled(bmp, 0, 0)
                    Catch
                        'do nothing
                    Finally
                        g2.Dispose()
                    End Try
                End If
            End If
        Catch
            'do nothing
        Finally
            If forceGraphics Then
                Try
                    bmp.Dispose()
                Catch
                    'do nothing
                End Try
                Try
                    g.Dispose()
                Catch
                    'do nothing
                End Try
            End If
        End Try

    End Sub

    Private Sub AddTabPosition(ByVal wrappedWindow As WrappedWindow, ByVal captionTruncated As Boolean, ByVal tabRect As Rectangle)

        Dim tabPosition As TabPositionStruct


        If wrappedWindow.Window.Text = "Message item 3XXXXXXXXXXX" And captionTruncated = False Then
        End If

        tabPosition.WrappedWindow = wrappedWindow
        tabPosition.CaptionTruncated = captionTruncated
        tabPosition.Rectangle = tabRect

        ReDim Preserve m_tabPositions(m_tabPositions.Length)

        m_tabPositions(m_tabPositions.Length - 1) = tabPosition

    End Sub

    Private Function HitTest(ByVal point As PointF) As WrappedWindow

        Return HitTest(CInt(point.X), CInt(point.Y), Nothing)

    End Function

    Private Function HitTest(ByVal point As Point) As WrappedWindow

        Return HitTest(point.X, point.Y, Nothing)

    End Function

    Private Function HitTest(ByVal point As PointF, ByRef structRetTabPosition As TabPositionStruct) As WrappedWindow

        Return HitTest(CInt(point.X), CInt(point.Y), structRetTabPosition)

    End Function

    Private Function HitTest(ByVal point As Point, ByRef structRetTabPosition As TabPositionStruct) As WrappedWindow

        Return HitTest(point.X, point.Y, structRetTabPosition)

    End Function

    Private Function HitTest(ByVal x As Integer, ByVal y As Integer) As WrappedWindow

        Return HitTest(x, y, Nothing)

    End Function

    Private Function HitTest(ByVal x As Integer, ByVal y As Integer, ByRef returnTabPosition As TabPositionStruct) As WrappedWindow

        Dim tabPosition As TabPositionStruct

        For Each tabPosition In m_tabPositions
            With tabPosition.Rectangle
                If x >= .Left AndAlso y >= .Top AndAlso x <= .Right AndAlso y <= .Bottom Then
                    returnTabPosition = tabPosition
                    Return tabPosition.WrappedWindow
                End If
            End With
        Next tabPosition

        Return Nothing

    End Function

    Private Sub ClearTabPositions()

        m_tabPositions = New TabPositionStruct() {}

    End Sub

    Public Overrides Sub EnsureVisible()

        _EnsureVisible(Me.SelectedWrappedWindowItem)

    End Sub

    Private Shadows Sub _EnsureVisible(ByVal wrappedWindow As WrappedWindow, Optional ByVal disallowRecurse As Boolean = False)

        CalcRefreshTabs()

        m_ensureVisibleActive = True

        Dim tabPosition As TabPositionStruct = FindWrappedWindowTabLocation(wrappedWindow)

        If Not tabPosition.WrappedWindow Is Nothing Then
            If Me.DrawAreaPanel.ClientRectangle.Width > 0 Then
                If tabPosition.Rectangle.Right >= Me.DrawAreaPanel.ClientRectangle.Right Then
                    Dim lastRight As Integer

                    Do
                        lastRight = tabPosition.Rectangle.Right
                        ScrollRight(noRefocus:=True)
                        tabPosition = FindWrappedWindowTabLocation(wrappedWindow)
                    Loop Until tabPosition.Rectangle.Right < Me.DrawAreaPanel.ClientRectangle.Right Or tabPosition.Rectangle.Right = lastRight

                    If Not disallowRecurse Then
                        _EnsureVisible(wrappedWindow, True) 'come back to leftmost state
                    End If

                    m_ensureVisibleActive = True 'reset flag in case the recurse changed it
                ElseIf tabPosition.Rectangle.Left < 0 Then
                    Dim lastLeft As Integer

                    Do
                        lastLeft = tabPosition.Rectangle.Left
                        ScrollLeft(noRefocus:=True)
                        tabPosition = FindWrappedWindowTabLocation(wrappedWindow)
                    Loop Until tabPosition.Rectangle.Left >= 0 Or tabPosition.Rectangle.Left = lastLeft
                End If
            End If
        End If

        m_ensureVisibleActive = False

    End Sub

    Protected Overridable Sub OnTabPaint(ByVal e As TabPaintEventArgs)

        RaiseEvent TabPaint(Me, e)

    End Sub

    Public Overrides Sub EndUpdate()

        MyBase.EndUpdate()

        RefreshTabs()

    End Sub

    Private Function FindWrappedWindowTabLocation(ByVal wrappedWindow As WrappedWindow) As TabPositionStruct

        Dim tabPosition As TabPositionStruct

        For Each tabPosition In m_tabPositions
            If tabPosition.WrappedWindow Is wrappedWindow Then
                Return tabPosition
            End If
        Next tabPosition

        Return Nothing

    End Function

    Private Sub ScrollToStart()

        Do While Me.ScrollLeftButton.Enabled
            ScrollLeft(noRefocus:=True)
        Loop

    End Sub

    Private Sub ScrollToEnd()

        Do While Me.ScrollRightButton.Enabled
            ScrollRight(noRefocus:=True)
        Loop

    End Sub

    Private Sub ScrollLeft(Optional ByVal noRefocus As Boolean = False)

        m_currentScrollPos += CInt(m_tabWidth / 2)

        ForceRefreshTabs()

        If Not noRefocus Then
            SetSpecialFocus()
        End If

    End Sub

    Private Sub ScrollRight(Optional ByVal noRefocus As Boolean = False)

        m_currentScrollPos -= CInt(m_tabWidth / 2)

        ForceRefreshTabs()

        If Not noRefocus Then
            SetSpecialFocus()
        End If

    End Sub

    Private Sub SetSpecialFocus()

        Try
            FocusKludgeButton.Focus()
        Catch
            'do nothing
        End Try

    End Sub

    Private Sub ScrollLeftButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScrollLeftButton.Click

        If Not m_suppressFinalScroll Then
            If CBool(Control.ModifierKeys And Keys.Control) Then
                ScrollToStart()
            Else
                ScrollLeft()
            End If
        End If

    End Sub

    Private Sub ScrollRightButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ScrollRightButton.Click

        If Not m_suppressFinalScroll Then
            If CBool(Control.ModifierKeys And Keys.Control) Then
                ScrollToEnd()
            Else
                ScrollRight()
            End If
        End If

    End Sub

    Protected Overrides Sub ProcessWindowItemAdded(ByVal wrappedWindow As WrappedWindow)

        RefreshTabs()

    End Sub

    Protected Overrides Sub ProcessEmphasizeSelectedTabChanged()

        RefreshTabs()

    End Sub

    Protected Overrides Sub ProcessShowIconsChanged()

        RefreshTabs()

    End Sub

    Protected Overrides Sub ProcessWindowItemRemoved(ByVal wrappedWindow As WrappedWindow)

        RefreshTabs()

    End Sub

    Protected Overrides Sub ProcessWindowItemsCleared()

        RefreshTabs()

    End Sub

    Protected Overrides Sub ProcessWindowItemTextChanged(ByVal wrappedWindow As WrappedWindow)

        RefreshTabs()

    End Sub

    Private Sub DrawAreaPanel_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles DrawAreaPanel.Paint

        DrawTabs(e.Graphics)

    End Sub

    Private Sub DrawAreaPanel_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles DrawAreaPanel.MouseDown

        If m_designMode Then
            Return
        End If

        If (e.X >= Me.CloseTabButton.Left AndAlso e.X <= Me.CloseTabButton.Right) AndAlso (e.Y >= Me.CloseTabButton.Top AndAlso e.Y <= Me.CloseTabButton.Bottom) Then
            Dim wrappedWindow As WrappedWindow = HitTest(Me.CloseTabButton.Location.X, Me.CloseTabButton.Location.Y)

            If Not wrappedWindow Is Nothing Then
                Me.SelectedWrappedWindowItem = wrappedWindow
                wrappedWindow.Window.Close()
            End If

        Else
            HideToolTip(Me)

            m_mouseDownActive = True
            m_mouseDownStartPoint = e.Location

            Dim wrappedWindow As WrappedWindow = HitTest(e.X, e.Y)

            If Not wrappedWindow Is Nothing Then
                Me.SelectedWrappedWindowItem = wrappedWindow

                If e.Button = MouseButtons.Right Then
                    OnShowWindowCmdMenuRequested(EventArgs.Empty)
                End If
            End If

            m_mouseDownActive = False
        End If

    End Sub

    Private Sub StandardTabsProvider_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Resize

        If Me.ClientRectangle.Height > 25 Then
            Me.ScrollLeftButton.TextAlign = ContentAlignment.MiddleCenter
            Me.ScrollRightButton.TextAlign = ContentAlignment.MiddleCenter
        Else
            Me.ScrollLeftButton.TextAlign = ContentAlignment.TopCenter
            Me.ScrollRightButton.TextAlign = ContentAlignment.TopCenter
        End If

    End Sub

    Private Sub DrawAreaPanel_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles DrawAreaPanel.MouseMove

        If e.Button = MouseButtons.Left AndAlso Not Me.SelectedWrappedWindowItem Is Nothing Then
            Me.CloseTabButton.Visible = False
            HideToolTip(Me)
            'doing this check to provide the user with a little leeway when clicking on tabs
            'before initiating a drag
            If Math.Abs(e.Location.X - m_mouseDownStartPoint.X) >= 3 OrElse Math.Abs(e.Location.Y - m_mouseDownStartPoint.Y) >= 3 Then
                OnBeginDragTabItem(New WrappedWindowItemEventArgs(Me.SelectedWrappedWindowItem))
            End If
        Else
            Dim tabPosition As TabPositionStruct
            Dim wrappedWindow As WrappedWindow = HitTest(e.X, e.Y, tabPosition)

            If Not wrappedWindow Is Nothing Then
                Dim newCloseButtonLocation As Point = New Point((tabPosition.Rectangle.Right - Me.CloseTabButton.Width) - 2, tabPosition.Rectangle.Top + 2)
                If Me.CloseTabButton.Location.X <> newCloseButtonLocation.X OrElse Me.CloseTabButton.Location.Y <> newCloseButtonLocation.Y Then
                    Me.CloseTabButton.Location = newCloseButtonLocation
                    Debug.Print("Button moved.")
                End If
                If Not Me.CloseTabButton.Visible Then
                    Debug.Print(Me.CloseTabButton.Visible)
                    Me.CloseTabButton.Visible = True
                    Debug.Print($"Button shown. Button:{Me.CloseTabButton.Location} New:{newCloseButtonLocation}")
                End If
                If tabPosition.CaptionTruncated Then
                    ShowToolTip(wrappedWindow.Window.Text, Me, New Point(tabPosition.Rectangle.Left, tabPosition.Rectangle.Bottom + 10))
                End If
            Else
                Me.CloseTabButton.Visible = False
                Debug.Print("Button hidden.")
                HideToolTip(Me)
            End If
        End If

    End Sub

    Private Sub DrawAreaPanel_DragOver(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles DrawAreaPanel.DragOver

        If Not e.Data.GetDataPresent(GetType(WrappedWindowDragDropData)) Then
            e.Effect = DragDropEffects.None
        Else
            Dim data As WrappedWindowDragDropData = CType(e.Data.GetData(GetType(WrappedWindowDragDropData)), WrappedWindowDragDropData)

            If data.WindowTabStrip Is Me.Parent Then
                Dim dragPoint As Point = New Point(e.X, e.Y)

                If Me.DrawAreaPanel.PointToClient(dragPoint).X <= 15 Then
                    StartAutoScroll(0, True)
                ElseIf Me.DrawAreaPanel.PointToClient(dragPoint).X >= Me.DrawAreaPanel.ClientRectangle.Right - 15 Then
                    StartAutoScroll(1, True)
                Else
                    StopAutoScroll()
                End If

                Dim wrappedWindow As WrappedWindow = HitTest(Me.DrawAreaPanel.PointToClient(dragPoint))

                If Not wrappedWindow Is Nothing Then
                    If Not m_currentDragOverTabItem Is wrappedWindow Then
                        m_currentDragOverTabItem = wrappedWindow
                        RefreshTabs()
                    End If
                    e.Effect = DragDropEffects.Move
                Else
                    If Not m_currentDragOverTabItem Is Nothing Then
                        m_currentDragOverTabItem = Nothing
                        RefreshTabs()
                    End If
                    e.Effect = DragDropEffects.None
                End If
            Else
                e.Effect = DragDropEffects.Move
            End If
        End If

    End Sub

    Private Sub DrawAreaPanel_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles DrawAreaPanel.DragDrop

        m_currentDragOverTabItem = Nothing
        RefreshTabs()

        If Not e.Data.GetDataPresent(GetType(WrappedWindowDragDropData)) Then
            e.Effect = DragDropEffects.None
        Else
            Dim data As WrappedWindowDragDropData = CType(e.Data.GetData(GetType(WrappedWindowDragDropData)), WrappedWindowDragDropData)

            If data.WindowTabStrip Is Me.Parent Then
                Dim wrappedWindow As WrappedWindow = HitTest(Me.DrawAreaPanel.PointToClient(New Point(e.X, e.Y)))

                If Not wrappedWindow Is Nothing AndAlso Not wrappedWindow Is Me.SelectedWrappedWindowItem Then
                    Dim wrappedWindowToMove As WrappedWindow = Me.SelectedWrappedWindowItem

                    Me.BeginUpdate()
                    m_wrappedWindowItems.Remove(wrappedWindowToMove)
                    m_wrappedWindowItems.Insert(m_wrappedWindowItems.IndexOf(wrappedWindow), wrappedWindowToMove)
                    m_currentDragOverTabItem = Nothing
                    Me.SelectedWrappedWindowItem = wrappedWindowToMove
                    Me.EndUpdate()

                    e.Effect = DragDropEffects.Move
                Else
                    e.Effect = DragDropEffects.None
                End If
            Else
                Dim eventargs As New DroppedWrappedWindowEventArgs(data)

                OnTabItemDropped(eventargs)
            End If
        End If

    End Sub

    Private Sub DrawAreaPanel_DragLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles DrawAreaPanel.DragLeave

        StopAutoScroll()
        m_currentDragOverTabItem = Nothing
        RefreshTabs()

    End Sub

    Private Function IsCloseButtonEntered() As Boolean

        Dim mousePos As Point = Me.DrawAreaPanel.PointToClient(Cursor.Position)
        Return (mousePos.X >= Me.CloseTabButton.Left AndAlso mousePos.X <= Me.CloseTabButton.Right) AndAlso (mousePos.Y >= Me.CloseTabButton.Top AndAlso mousePos.Y <= Me.CloseTabButton.Bottom)

    End Function

    Private Sub DrawAreaPanel_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles DrawAreaPanel.MouseLeave

        If Not IsCloseButtonEntered() Then
            Me.CloseTabButton.Visible = False
        End If

        HideToolTip(Me)

    End Sub

    Private Sub DrawAreaPanel_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles DrawAreaPanel.DoubleClick

        If Not Me.SelectedWrappedWindowItem Is Nothing AndAlso Not m_ensureVisibleActive AndAlso Not m_mouseDownActive Then
            OnTabItemDoubleClicked(New WrappedWindowItemEventArgs(Me.SelectedWrappedWindowItem))
        End If

    End Sub

    Private Sub ScrollTimer_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ScrollTimer.Tick

        If Not m_suppressFinalScroll Then
            m_suppressFinalScroll = True
            Me.ScrollTimer.Interval = 250
        End If

        Select Case m_scrollRepeatDirection
            Case 0
                If Me.ScrollLeftButton.Enabled Then
                    ScrollLeft()
                Else
                    StopAutoScroll()
                End If
            Case 1
                If Me.ScrollRightButton.Enabled Then
                    ScrollRight()
                Else
                    StopAutoScroll()
                End If
        End Select

    End Sub

    Private Sub ScrollButtons_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ScrollLeftButton.MouseDown, ScrollRightButton.MouseDown

        If e.Button = MouseButtons.Right Then
            Me.WindowsMenu.Show(Me, Me.PointToClient(System.Windows.Forms.Cursor.Position))
        Else
            If sender Is ScrollLeftButton Then
                StartAutoScroll(0)
                Application.DoEvents()
            ElseIf sender Is ScrollRightButton Then
                StartAutoScroll(1)
                Application.DoEvents()
            End If
        End If

    End Sub

    Private Sub ScrollButtons_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles ScrollLeftButton.MouseLeave, ScrollRightButton.MouseLeave

        StopAutoScroll()

    End Sub

    Private Sub ScrollButtons_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles ScrollLeftButton.MouseUp, ScrollRightButton.MouseUp

        StopAutoScroll()

    End Sub

    Private Sub StartAutoScroll(ByVal direction As Integer, Optional ByVal noDelay As Boolean = False)

        If Not Me.ScrollTimer.Enabled Or direction <> m_scrollRepeatDirection Then
            m_scrollRepeatDirection = direction

            If noDelay Then
                Me.ScrollTimer.Interval = 10
            Else
                Me.ScrollTimer.Interval = 550
            End If

            Me.ScrollTimer.Enabled = True
        End If

    End Sub

    Private Sub StopAutoScroll()

        Me.ScrollTimer.Enabled = False
        m_suppressFinalScroll = False

    End Sub

#Region "TabPaint Classes"

    Public Class TabPaintEventArgs
        Inherits System.ComponentModel.HandledEventArgs

        Private m_graphics As Graphics
        Private m_wrappedWindowItem As WrappedWindow
        Private m_isSelected As Boolean
        Private m_tabPaintOptions As TabPaintOptions
        Private m_counter As Integer

        Public Sub New(ByVal wrappedWindow As WrappedWindow, ByVal tabPaintOptions As TabPaintOptions)

            m_wrappedWindowItem = wrappedWindow
            m_tabPaintOptions = tabPaintOptions

        End Sub

        Public ReadOnly Property Graphics() As Graphics

            Get
                Return m_graphics
            End Get

        End Property

        Friend Property GraphicsObject() As Graphics

            Get
                Return m_graphics
            End Get

            Set(ByVal value As Graphics)
                m_graphics = value
            End Set

        End Property

        Public ReadOnly Property WrappedWindowItem() As WrappedWindow

            Get
                Return m_wrappedWindowItem
            End Get

        End Property

        Friend Property WrappedWindowItemObject() As WrappedWindow

            Get
                Return m_wrappedWindowItem
            End Get

            Set(ByVal value As WrappedWindow)
                m_wrappedWindowItem = value
            End Set

        End Property

        Public ReadOnly Property IsSelected() As Boolean

            Get
                Return m_isSelected
            End Get

        End Property

        Friend Property IsSelectedValue() As Boolean
            Get
                Return m_isSelected
            End Get
            Set(ByVal value As Boolean)
                m_isSelected = value
            End Set
        End Property

        Public ReadOnly Property TabPaintOptions() As TabPaintOptions

            Get
                Return m_tabPaintOptions
            End Get

        End Property

        Public Property Counter() As Integer

            Get
                Return m_counter
            End Get

            Set(ByVal value As Integer)
                m_counter = value
            End Set

        End Property

    End Class

    'Public Structure TabPaintOptions
    '    Public TabRect As Rectangle
    '    Public Font As Font
    '    Public Icon As Icon
    '    Public Text As String
    '    Public BackColorBrush As Brush
    '    Public ForeColorBrush As Brush
    '    Public IconRect As Rectangle
    '    Public TextRect As RectangleF
    'End Structure

    Public Class TabPaintOptions

        Private m_tabRect As Rectangle
        Public Property TabRect() As Rectangle
            Get
                Return m_tabRect
            End Get
            Set(ByVal value As Rectangle)
                m_tabRect = value
            End Set
        End Property

        Private m_font As Font
        Public Property Font() As Font
            Get
                Return m_font
            End Get
            Set(ByVal value As Font)
                m_font = value
            End Set
        End Property

        Private m_icon As Icon
        Public Property Icon() As Icon
            Get
                Return m_icon
            End Get
            Set(ByVal value As Icon)
                m_icon = value
            End Set
        End Property

        Private m_text As String
        Public Property Text() As String
            Get
                Return m_text
            End Get
            Set(ByVal value As String)
                m_text = value
            End Set
        End Property

        Private m_backColorBrush As Brush
        Public Property BackColorBrush() As Brush
            Get
                Return m_backColorBrush
            End Get
            Set(ByVal value As Brush)
                m_backColorBrush = value
            End Set
        End Property

        Private m_foreColorBrush As Brush
        Public Property ForeColorBrush() As Brush
            Get
                Return m_foreColorBrush
            End Get
            Set(ByVal value As Brush)
                m_foreColorBrush = value
            End Set
        End Property

        Private m_iconRect As Rectangle
        Public Property IconRect() As Rectangle
            Get
                Return m_iconRect
            End Get
            Set(ByVal value As Rectangle)
                m_iconRect = value
            End Set
        End Property

        Private m_textRect As RectangleF
        Public Property TextRect() As RectangleF
            Get
                Return m_textRect
            End Get
            Set(ByVal value As RectangleF)
                m_textRect = value
            End Set
        End Property

    End Class

    Private Sub StandardTabsProvider_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Dim parent As Control = Me.Parent

        While (parent IsNot Nothing AndAlso TypeOf parent IsNot WindowManagerPanel)
            parent = parent.Parent
        End While

        If parent IsNot Nothing Then
            m_designMode = DirectCast(parent, WindowManagerPanel).IsDesignMode
        End If

    End Sub

    Private Sub CloseTabButton_Click(sender As Object, e As EventArgs) Handles CloseTabButton.Click

        Dim wrappedWindow As WrappedWindow = HitTest(Me.CloseTabButton.Location.X, Me.CloseTabButton.Location.Y)

        If Not wrappedWindow Is Nothing Then
            Me.SelectedWrappedWindowItem = wrappedWindow
            wrappedWindow.Window.Close()
            Me.CloseTabButton.Visible = False
        End If

    End Sub

#End Region

End Class

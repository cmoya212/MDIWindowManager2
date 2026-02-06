Imports System.Drawing
Imports System.ComponentModel

''' <summary>
''' Rotated button control.
''' </summary>
''' <remarks>Used by WindowManagerPanel when in minimized mode.</remarks>
Friend Class ButtonR
    Inherits System.Windows.Forms.Button

    Private m_rotatedText As String

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property RotatedText() As String

        Get
            Return m_rotatedText
        End Get

        Set(ByVal value As String)
            m_rotatedText = value
            Refresh()
        End Set

    End Property

    Private Sub ButtonR_Paint(ByVal sender As Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles MyBase.Paint

        Dim itemRect As Rectangle = e.ClipRectangle
        Dim rotateAngle As Single = 90
        Dim cp As New PointF(itemRect.Left + (itemRect.Width \ 2), itemRect.Top + (itemRect.Height \ 2))

        e.Graphics.TranslateTransform(cp.X, cp.Y)
        e.Graphics.RotateTransform(rotateAngle)

        itemRect = New Rectangle(-(itemRect.Height \ 2), -(itemRect.Width \ 2), itemRect.Height, itemRect.Width)

        Dim stringFormat As New StringFormat

        Select Case Me.TextAlign
            Case ContentAlignment.MiddleCenter
                stringFormat.Alignment = StringAlignment.Center
                stringFormat.LineAlignment = StringAlignment.Center
            Case ContentAlignment.MiddleLeft
                stringFormat.Alignment = StringAlignment.Near
                stringFormat.LineAlignment = StringAlignment.Center
            Case ContentAlignment.MiddleRight
                stringFormat.Alignment = StringAlignment.Far
                stringFormat.LineAlignment = StringAlignment.Center
            Case ContentAlignment.TopCenter
                stringFormat.Alignment = StringAlignment.Center
                stringFormat.LineAlignment = StringAlignment.Near
            Case ContentAlignment.TopLeft
                stringFormat.Alignment = StringAlignment.Near
                stringFormat.LineAlignment = StringAlignment.Near
            Case ContentAlignment.TopRight
                stringFormat.Alignment = StringAlignment.Far
                stringFormat.LineAlignment = StringAlignment.Near
            Case ContentAlignment.BottomCenter
                stringFormat.Alignment = StringAlignment.Center
                stringFormat.LineAlignment = StringAlignment.Far
            Case ContentAlignment.BottomLeft
                stringFormat.Alignment = StringAlignment.Near
                stringFormat.LineAlignment = StringAlignment.Far
            Case ContentAlignment.BottomRight
                stringFormat.Alignment = StringAlignment.Far
                stringFormat.LineAlignment = StringAlignment.Far
        End Select

        e.Graphics.DrawString(Me.RotatedText, Me.Font, New SolidBrush(Me.ForeColor), RectangleF.op_Implicit(itemRect), stringFormat)

    End Sub

End Class

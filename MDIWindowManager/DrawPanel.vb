''' <summary>
''' Drawing canvas preinitialized with double-buffering, etc.
''' </summary>
''' <remarks></remarks>
Friend Class DrawPanel
    Inherits Panel

    Public Sub New()

        MyBase.New()

        'SetStyle(ControlStyles.UserPaint, True)
        'SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        'SetStyle(ControlStyles.OptimizedDoubleBuffer, True)

        Me.DoubleBuffered = True

    End Sub

End Class
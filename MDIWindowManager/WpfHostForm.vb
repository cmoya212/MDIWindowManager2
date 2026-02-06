Imports System.Windows.Forms.Integration

Public Class WpfHostForm

    Private m_elementHost As ElementHost

    Public ReadOnly Property WpfElement As ElementHost

        Get
            Return m_elementHost
        End Get

    End Property

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    Public Sub New(wpfControl As System.Windows.Controls.UserControl, Optional titleFallback As String = Nothing)
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        Dim elementHost As New ElementHost()

        elementHost.Dock = DockStyle.Fill

        Dim titleElementValue As String = Nothing
        Dim titleElement = CType(wpfControl.FindName("WindowHostTitle"), System.Windows.Controls.Label)

        If (titleElement IsNot Nothing) Then
            titleElementValue = titleElement.Content
        End If

        If Not String.IsNullOrEmpty(titleElementValue) Then
            Me.Text = titleElementValue
        ElseIf Not String.IsNullOrEmpty(titleFallback) Then
            Me.Text = titleFallback
        Else
            Me.Text = wpfControl.Name
        End If

        elementHost.Child = wpfControl

        Me.Controls.Add(elementHost)

        m_elementHost = elementHost

    End Sub

End Class
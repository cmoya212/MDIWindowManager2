'////////////////////////////////////////////////////////////
'//  [AUTHOR]:      C. Moya                                //
'//  [DESCRIPTION]                                         //
'//                 Strongly Typed Collection Template     //
'//  [/DESCRIPTION]                                        //
'//  [COMMENTS]                                            //
'//                 Substitue xOBJECTx placeholder with    //
'//                 real types.                            //
'//  [/COMMENTS]                                           //
'////////////////////////////////////////////////////////////
Option Strict Off
Option Explicit On 

' typename of object:     WindowTabStrip
' typename of collection: WindowTabStripCollection
' name of enumerator:     WindowTabStripCollectionEnumerator

'test edit

Imports System
Imports System.Collections

Public Class WindowTabStripCollectionException
    Inherits System.Exception

    Public Sub New()

        MyBase.New()

    End Sub

    Public Sub New(ByVal message As String)

        MyBase.New(message)

    End Sub

End Class

Public Class WindowTabStripCollection
    Inherits System.Collections.CollectionBase

    Public Event BeforeTabStripAdded As EventHandler(Of WindowTabStripCollection.ItemsCancelEventArgs)
    Public Event TabStripAdded As EventHandler(Of WindowTabStripCollection.ItemsEventArgs)
    Public Event BeforeTabStripRemoved As EventHandler(Of WindowTabStripCollection.ItemsCancelEventArgs)
    Public Event TabStripRemoved As EventHandler(Of WindowTabStripCollection.ItemsEventArgs)
    Public Event TabStripClearWarn As EventHandler(Of WindowTabStripCollection.ItemsEventArgs)
    Public Event TabStripsCleared As EventHandler
    'Public Event TabStripGotFocus As EventHandler(Of WindowTabStripCollection.ItemsEventArgs)

    Public Class ItemsEventArgs
        Inherits System.EventArgs

        Private m_windowTabStrip As WindowTabStrip

        Public Sub New(ByVal windowTabStrip As WindowTabStrip)

            m_windowTabStrip = windowTabStrip

        End Sub

        Public ReadOnly Property WindowTabStrip() As WindowTabStrip

            Get
                Return m_windowTabStrip
            End Get

        End Property

    End Class

    Public Class ItemsCancelEventArgs
        Inherits System.ComponentModel.CancelEventArgs

        Private m_windowTabStrip As WindowTabStrip

        Public Sub New(ByVal windowTabStrip As WindowTabStrip)

            m_windowTabStrip = windowTabStrip

        End Sub

        Public ReadOnly Property WindowTabStrip() As WindowTabStrip

            Get
                Return m_windowTabStrip
            End Get

        End Property

    End Class

    Public Sub New()

        MyBase.New()

    End Sub

    Default Public Property Item(ByVal index As Integer) As WindowTabStrip
        Get
            Return CType(Me.List(index), WindowTabStrip)
        End Get
        Set(ByVal value As WindowTabStrip)
            Me.List(index) = Value
        End Set
    End Property

    Public Function Add(ByVal value As WindowTabStrip) As Integer

        Return Me.List.Add(value)

    End Function

    Public Function Contains(ByVal value As WindowTabStrip) As Boolean

        Return Me.List.Contains(value)

    End Function

    Public Function IndexOf(ByVal value As WindowTabStrip) As Integer

        Return Me.List.IndexOf(value)

    End Function

    Public Sub Remove(ByVal value As WindowTabStrip)

        Me.List.Remove(value)

    End Sub

    Public Shadows Function GetEnumerator() As WindowTabStripCollectionEnumerator

        Return New WindowTabStripCollectionEnumerator(Me)

    End Function

    Public Sub Insert(ByVal index As Integer, ByVal value As WindowTabStrip)

        Me.List.Insert(index, value)

    End Sub

    Protected Overrides Sub OnClear()

        For Each windowTabStrip As WindowTabStrip In Me.List
            Dim cancelEventArgs As New WindowTabStripCollection.ItemsCancelEventArgs(windowTabStrip)

            cancelEventArgs.Cancel = False
            RaiseEvent BeforeTabStripRemoved(Me, cancelEventArgs)

            If cancelEventArgs.Cancel Then
                Throw New WindowTabStripCollectionException("Clear aborted.")
                Exit For
            End If

            Try
                RaiseEvent TabStripClearWarn(Me, New WindowTabStripCollection.ItemsEventArgs(windowTabStrip))
            Catch
                'do nothing
            End Try
        Next windowTabStrip

        MyBase.OnClear()

    End Sub

    Protected Overrides Sub OnClearComplete()

        RaiseEvent TabStripsCleared(Me, System.EventArgs.Empty)

        MyBase.OnClearComplete()

    End Sub

    Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)

        Dim cancelEventArgs As New WindowTabStripCollection.ItemsCancelEventArgs(value)

        RaiseEvent BeforeTabStripAdded(Me, cancelEventArgs)

        If cancelEventArgs.Cancel Then
            Throw New WindowTabStripCollectionException("Window add aborted.")
        End If

        MyBase.OnInsert(index, value)

    End Sub

    Protected Overrides Sub OnInsertComplete(ByVal index As Integer, ByVal value As Object)

        RaiseEvent TabStripAdded(Me, New WindowTabStripCollection.ItemsEventArgs(value))

        MyBase.OnInsertComplete(index, value)

    End Sub

    Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)

        Dim cancelEventArgs As New WindowTabStripCollection.ItemsCancelEventArgs(value)

        RaiseEvent BeforeTabStripRemoved(Me, cancelEventArgs)

        If cancelEventArgs.Cancel Then
            'Throw New WindowTabStripCollectionException("Window remove aborted.")
        End If

        MyBase.OnRemove(index, value)

    End Sub

    Protected Overrides Sub OnRemoveComplete(ByVal index As Integer, ByVal value As Object)

        RaiseEvent TabStripRemoved(Me, New WindowTabStripCollection.ItemsEventArgs(value))

        If Me.List.Count = 0 Then
            RaiseEvent TabStripsCleared(Me, System.EventArgs.Empty)
        End If

        MyBase.OnRemoveComplete(index, value)

    End Sub

    Public Class WindowTabStripCollectionEnumerator

        Implements System.Collections.IEnumerator

        Private m_index As Integer

        Private m_currentElement As WindowTabStrip

        Private m_items As WindowTabStripCollection

        Friend Sub New(ByVal collection As WindowTabStripCollection)

            MyBase.New()
            m_index = -1
            m_items = collection

        End Sub

        Public ReadOnly Property Current() As Object Implements System.Collections.IEnumerator.Current
            Get
                If ((m_index = -1) OrElse (m_index >= m_items.Count)) Then
                    Throw New System.IndexOutOfRangeException("Enumerator not started.")
                Else
                    Return m_currentElement
                End If
            End Get
        End Property

        Public Function MoveNext() As Boolean Implements System.Collections.IEnumerator.MoveNext

            If (m_index < (m_items.Count - 1)) Then
                m_index = (m_index + 1)
                m_currentElement = m_items(m_index)
                Return True
            End If
            m_index = m_items.Count
            Return False

        End Function

        Public Sub Reset() Implements System.Collections.IEnumerator.Reset

            m_index = -1
            m_currentElement = Nothing

        End Sub

    End Class

End Class
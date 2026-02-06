Imports System.ComponentModel

''' <summary>
''' Implemented by child windows that need or want to be aware of the MDI Window Manager. (Optional)
''' </summary>
''' <remarks></remarks>
Public Interface IWrappedWindow

    'Not really needed right now... but could be expanded upon in the future.

    Function PlaceHolder() As Boolean

End Interface

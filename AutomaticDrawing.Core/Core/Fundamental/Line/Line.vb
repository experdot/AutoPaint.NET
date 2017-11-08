Imports System.Drawing
Imports AutomaticDrawing.Core
''' <summary>
''' 线条
''' </summary>
Public Class Line
    Implements ILine
    ''' <summary>
    ''' 点集
    ''' </summary>
    Public Property Vertices As New List(Of Vertex) Implements ILine.Vertices

End Class

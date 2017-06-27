Imports System.Drawing
''' <summary>
''' 线条
''' </summary>
Public Class Line
    ''' <summary>
    ''' 点集
    ''' </summary>
    Public Property Vertices As New List(Of Vertex)
    ''' <summary>
    ''' 计算画笔大小
    ''' </summary>
    Public Sub CalcSize()
        If Vertices.Count > 0 Then
            Dim mid As Single
            For i = 0 To Vertices.Count - 1
                mid = CSng(Math.Abs(i - Vertices.Count / 2))
                Vertices(i).Size = CSng(Math.Log10(mid)) / 2
            Next
        End If
    End Sub
    ''' <summary>
    ''' 计算画笔颜色
    ''' </summary>
    Public Sub CalcAverageColor()
        If Vertices.Count > 0 Then
            Dim R, G, B As Integer
            For Each SubPoint In Vertices
                R += SubPoint.Color.R
                G += SubPoint.Color.G
                B += SubPoint.Color.B
            Next
            Dim tempCol As Color = Color.FromArgb(255, CByte(R / Vertices.Count),
                                                       CByte(G / Vertices.Count),
                                                       CByte(B / Vertices.Count))
            For Each SubPoint In Vertices
                SubPoint.Color = tempCol
            Next
        End If
    End Sub
End Class

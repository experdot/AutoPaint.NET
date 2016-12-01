Imports System.Drawing
''' <summary>
''' 线条
''' </summary>
Public Class Line
    ''' <summary>
    ''' 点集
    ''' </summary>
    Public Points As New List(Of Point)
    ''' <summary>
    ''' 计算画笔大小
    ''' </summary>
    Public Sub CalcSize()
        If Points.Count > 0 Then
            Dim Mid As Single
            For i = 0 To Points.Count - 1
                Mid = CSng(Math.Abs(i - Points.Count / 2))
                Points(i).Size = CSng(Math.Log10(Mid)) / 2
            Next
        End If
    End Sub
    ''' <summary>
    ''' 计算画笔大小
    ''' </summary>
    Public Sub CalcAverageColor()
        If Points.Count > 0 Then
            Dim R, G, B As Integer
            For Each SubPoint In Points
                R += SubPoint.Color.R
                G += SubPoint.Color.G
                B += SubPoint.Color.B
            Next
            Dim tempCol As Color = Color.FromArgb(255, CByte(R / Points.Count),
                                                       CByte(G / Points.Count),
                                                       CByte(B / Points.Count))
            For Each SubPoint In Points
                SubPoint.Color = tempCol
            Next
        End If
    End Sub
End Class

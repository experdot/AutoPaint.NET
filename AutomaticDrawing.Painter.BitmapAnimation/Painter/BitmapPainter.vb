Imports System.Drawing
Imports AutomaticDrawing.Core
''' <summary>
''' 提供位图动画的绘制器
''' </summary>
Public Class BitmapPainter
    Implements IPainter
    Public Event UpdatePaint As IPainter.UpdatePaintEventHandler Implements IPainter.UpdatePaint
    ''' <summary>
    ''' 视图
    ''' </summary>
    Public Property View As Bitmap
    ''' <summary>
    ''' 是否绘制原始数据
    ''' </summary>
    Public Property IsPaintRaw As Boolean

    Shared Rnd As New Random

    ''' <summary>
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(view As Bitmap, Optional isPaintRaw As Boolean = True)
        Me.View = view
        Me.IsPaintRaw = isPaintRaw
    End Sub

    Public Sub Start(lines As List(Of ILine)) Implements IPainter.Start
        If IsPaintRaw Then
            PaintRaw(lines)
        Else
            PaintColorful(lines)
        End If
    End Sub

    Public Sub Pause() Implements IPainter.Pause
        Throw New NotImplementedException()
    End Sub
    Public Sub [Stop]() Implements IPainter.Stop
        Throw New NotImplementedException()
    End Sub

    Private Sub PaintRaw(lines As List(Of ILine))
        Using pg As Graphics = Graphics.FromImage(View)
            pg.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Dim totalCount As Integer = lines.SelectMany(Function(e As ILine) e.Vertices).Count
            Dim current As Integer = 0
            For Each SubSequence In lines
                For Each SubVertex In SubSequence.Vertices
                    Dim penSize As Single = SubVertex.Size
                    Dim brush As New SolidBrush(SubVertex.Color)
                    pg.FillRectangle(brush, New RectangleF(SubVertex.X - penSize / 2, SubVertex.Y - penSize / 2, penSize, penSize))
                    'pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                    current += 1
                    RaiseEvent UpdatePaint(Me, New UpdatePaintEventArgs(SubVertex, current / totalCount))
                Next
            Next
        End Using
    End Sub
    Private Sub PaintColorful(lines As List(Of ILine))
        Dim tempColor As Color = Color.FromArgb(255, 0, 0, 0)
        Using pg As Graphics = Graphics.FromImage(View)
            pg.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Dim totalCount As Integer = lines.SelectMany(Function(e As ILine) e.Vertices).Count
            Dim current As Integer = 0
            For Each SubSequence In lines
                tempColor = Color.FromArgb(255, Rnd.NextDouble * 255, Rnd.NextDouble * 255, Rnd.NextDouble * 255)
                For Each SubPoint In SubSequence.Vertices
                    Dim penSize As Single = SubPoint.Size
                    Dim mypen As New Pen(tempColor, 1 + penSize)
                    pg.DrawEllipse(mypen, New RectangleF(SubPoint.X - penSize / 2, SubPoint.Y - penSize / 2, penSize, penSize))
                    'pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                    current += 1
                    RaiseEvent UpdatePaint(Me, New UpdatePaintEventArgs(SubPoint, current / totalCount))
                Next
            Next
        End Using
    End Sub

End Class

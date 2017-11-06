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
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(view As Bitmap)
        Me.View = view
    End Sub

    Public Sub Start(lines As List(Of ILine)) Implements IPainter.Start
        Static Rnd As New Random
        Dim tempColor As Color = Color.FromArgb(255, 0, 0, 0)
        Using pg As Graphics = Graphics.FromImage(View)
            pg.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            Dim totalCount As Integer = lines.SelectMany(Function(e As ILine)
                                                             Return e.Vertices
                                                         End Function).Count
            Dim current As Integer = 0
            For Each SubSequence In lines
                Dim seqIndex As Integer = lines.IndexOf(SubSequence)
                Dim TempR = Rnd.NextDouble * 255
                Dim TempG = Rnd.NextDouble * 255
                Dim TempB = Rnd.NextDouble * 255
                For Each SubPoint In SubSequence.Vertices
                    Dim Index As Single = SubSequence.Vertices.IndexOf(SubPoint)
                    Dim alpha As Integer = 255 - Index * 255 / SubSequence.Vertices.Count
                    If alpha < 1 Then alpha = 1
                    tempColor = Color.FromArgb(alpha, 0, 0, 0)
                    'tempColor = Color.FromArgb(alpha, TempR, TempG, TempB)
                    Dim Count As Single = SubSequence.Vertices.Count
                    'Dim penWidth As Single = 0.01 + (Count / 2 - Math.Abs(Index - Count / 2)) / 20
                    Dim penWidth As Single = 0.5 + Math.Abs(Index - Count) / 30
                    Dim maxWidth As Single = 2
                    If penWidth > maxWidth Then penWidth = maxWidth
                    If penWidth > 2 AndAlso CInt(Index) Mod (maxWidth + 2 - CInt(penWidth)) = 0 Then Continue For
                    Dim mypen As New Pen(tempColor, 1 + penWidth)
                    pg.DrawEllipse(mypen, New RectangleF(SubPoint.X - penWidth / 2, SubPoint.Y - penWidth / 2, penWidth, penWidth))
                    'pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                    current += 1
                    RaiseEvent UpdatePaint(Me, New UpdatePaintEventArgs(SubPoint, current / totalCount))
                Next
            Next
        End Using
    End Sub
    Public Sub Pause() Implements IPainter.Pause
        Throw New NotImplementedException()
    End Sub
    Public Sub [Stop]() Implements IPainter.Stop
        Throw New NotImplementedException()
    End Sub

End Class

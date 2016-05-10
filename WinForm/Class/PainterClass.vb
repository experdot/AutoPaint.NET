Imports System.Runtime.InteropServices

Public Class PainterClass
    Private Declare Sub mouse_event Lib "user32" (ByVal dwFlags As Int32, ByVal dx As Int32, ByVal dy As Int32, ByVal cButtons As Int32, ByVal dwExtraInfo As Int32)
    Private Declare Function SetCursorPos Lib "user32" (ByVal x As Integer, ByVal y As Integer) As Integer
    Public SleepTime As Integer = 1
    Public BoardX As Integer
    Public BoardY As Integer
    Public Event UpdatePreviewImage(ePoint As PointF, ePen As Pen)
    ''' <summary>
    ''' 绘制
    ''' </summary>
    ''' <param name="sBitmap"></param>
    ''' <param name="bx"></param>
    ''' <param name="by"></param>
    Public Sub StartPaint(sBitmap As Bitmap, bx As Integer, by As Integer)
        BoardX = bx
        BoardY = by
        Dim TempBolArr(,) = GetImageBol(sBitmap)
        Painting(New SequenceManagerClass(TempBolArr))
    End Sub
    ''' <summary>
    ''' 预览
    ''' </summary>
    ''' <param name="sBitmap"></param>
    Public Sub StartPreview(ByRef sBitmap As Bitmap, ByRef ViewBitmap As Bitmap)
        Dim TempBolArr(,) = GetImageBol(sBitmap)
        Previewing(New SequenceManagerClass(TempBolArr), ViewBitmap)
    End Sub
    ''' <summary>
    ''' 模拟鼠标左键按下或弹起
    ''' </summary>
    ''' <param name="dx"></param>
    ''' <param name="dy"></param>
    ''' <param name="type"></param>
    Private Sub MouseDownUp(ByVal dx As Integer, ByVal dy As Integer, ByVal type As Boolean)
        If type Then '按下
            mouse_event(&H2, 0, 0, 0, IntPtr.Zero)
        Else '弹起
            mouse_event(&H4, 0, 0, 0, IntPtr.Zero)
        End If
        System.Threading.Thread.Sleep(SleepTime)
    End Sub
    ''' <summary>
    ''' 模拟鼠标移动
    ''' </summary>
    ''' <param name="dx"></param>
    ''' <param name="dy"></param>
    Private Sub MouseMove(ByVal dx As Integer, ByVal dy As Integer)
        Cursor.Position = New Point(dx, dy)
        System.Threading.Thread.Sleep(SleepTime)
    End Sub
    ''' <summary>
    ''' 返回指定图像的二值化数据
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Private Function GetImageBol(ByVal gBitmap As Bitmap) As Integer(,)
        Dim ResultArr(gBitmap.Width - 1, gBitmap.Height - 1) As Integer
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If gBitmap.GetPixel(i, j).Equals(Color.FromArgb(255, 255, 255)) Then
                    ResultArr(i, j) = 0
                Else
                    ResultArr(i, j) = 1
                End If
            Next
        Next
        Return ResultArr
    End Function
    Private Sub Painting(SequenceManager As SequenceManagerClass)
        For Each SubSequence In SequenceManager.SequenceList
            MouseMove(SubSequence.PointList.First.X + BoardX, SubSequence.PointList.First.Y + BoardY)
            MouseDownUp(SubSequence.PointList.First.X + BoardX, SubSequence.PointList.First.Y + BoardY, True)
            For Each SubPoint In SubSequence.PointList
                MouseMove(SubPoint.X + BoardX, SubPoint.Y + BoardY)
            Next
            MouseDownUp(SubSequence.PointList.Last.X + BoardX, SubSequence.PointList.Last.Y + BoardY, False)
        Next
    End Sub
    Private Sub Previewing(SequenceManager As SequenceManagerClass, ByRef DepthBitmap As Bitmap)
        Dim TempColor As Color = Color.FromArgb(255, 0, 0, 0)
        Dim rnd As New Random
        Using pg As Graphics = Graphics.FromImage(DepthBitmap)
            pg.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            For Each SubSequence In SequenceManager.SequenceList
                Dim TempR = rnd.NextDouble * SubSequence.PointList.Count
                Dim TempG = rnd.NextDouble * SubSequence.PointList.Count
                Dim TempB = rnd.NextDouble * SubSequence.PointList.Count
                For Each SubPoint In SubSequence.PointList
                    Dim Index As Single = SubSequence.PointList.IndexOf(SubPoint)
                    Dim alpha As Integer = 255 - Index
                    If alpha < 1 Then alpha = 1
                    TempColor = Color.FromArgb(alpha, 0, 0, 0)
                    'TempColor = Color.FromArgb(255 - Index, TempR, TempG, TempB)
                    Dim Count As Single = SubSequence.PointList.Count
                    'Dim penWidth As Single = 0.01 + (Count / 2 - Math.Abs(Index - Count / 2)) / 20
                    Dim penWidth As Single = 0.5 + Math.Abs(Index - Count) / 40
                    Dim maxWidth As Single = 3
                    If penWidth > maxWidth Then penWidth = maxWidth
                    If penWidth > 2 AndAlso CInt(Index) Mod (maxWidth + 2 - CInt(penWidth)) = 0 Then Continue For
                    Dim mypen As New Pen(TempColor, 1 + penWidth)
                    pg.DrawEllipse(mypen, New RectangleF(SubPoint.X - penWidth / 2, SubPoint.Y - penWidth / 2, penWidth, penWidth))
                    'pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                    RaiseEvent UpdatePreviewImage(SubPoint, mypen)
                Next
            Next
        End Using
    End Sub
End Class

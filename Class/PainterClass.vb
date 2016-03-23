Public Class PainterClass
    Private Declare Sub mouse_event Lib "user32" (ByVal dwFlags As Int32, ByVal dx As Int32, ByVal dy As Int32, ByVal cButtons As Int32, ByVal dwExtraInfo As Int32)
    Private Declare Function SetCursorPos Lib "user32" (ByVal x As Integer, ByVal y As Integer) As Integer
    Public SleepTime As Integer = 1
    Public BoardX As Integer
    Public BoardY As Integer
    Public Sub StartPaint(sBitmap As Bitmap, bx As Integer, by As Integer)
        BoardX = bx
        BoardY = by
        Dim TempBolArr(,) = GetImageBol(sBitmap)
        Painting(New SequenceManagerClass(TempBolArr))
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
    End Sub
    ''' <summary>
    ''' 模拟鼠标移动
    ''' </summary>
    ''' <param name="dx"></param>
    ''' <param name="dy"></param>
    Private Sub MouseMove(ByVal dx As Integer, ByVal dy As Integer)
        Cursor.Position = New Point(dx, dy)
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
                If gBitmap.GetPixel(i, j).Equals(Color.FromArgb(0, 0, 0)) = True Then
                    ResultArr(i, j) = 1
                Else
                    ResultArr(i, j) = 0
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
                System.Threading.Thread.Sleep(SleepTime)
            Next
            MouseDownUp(SubSequence.PointList.Last.X + BoardX, SubSequence.PointList.Last.Y + BoardY, False)
        Next
    End Sub
End Class

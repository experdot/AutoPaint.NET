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
        Painting(TempBolArr)
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
        Dim AbsX = BoardX
        Dim AbsY = BoardY
        'Cursor.Position = New Point(AbsX + dx, AbsY + dy)
        SetCursorPos(AbsX + dx, AbsY + dy)
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
                If gBitmap.GetPixel(i, j).Equals(Color.FromArgb(0, 0, 0)) = True Then
                    ResultArr(i, j) = 1
                Else
                    ResultArr(i, j) = 0
                End If
            Next
        Next
        Return ResultArr
    End Function
    ''' <summary>
    ''' 检查一个点是否被包围
    ''' </summary>
    ''' <param name="BolArr"></param>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <returns></returns>
    Private Function CheckPointAround(BolArr As Integer(,), ByVal x As Integer, ByVal y As Integer) As Boolean
        If Not (x > 0 And y > 0 And x < BolArr.GetUpperBound(0) And y < BolArr.GetUpperBound(1)) Then Return False
        If BolArr(x - 1, y) = 1 And BolArr(x + 1, y) = 1 And BolArr(x, y - 1) = 1 And BolArr(x, y + 1) = 1 Then
            Return True '当前点为实体内部
        Else
            Return False '当前点为实体边缘
        End If
    End Function

    Dim xArray() As Short = {-1, 0, 1, 1, 1, 0, -1, -1}
    Dim yArray() As Short = {-1, -1, -1, 0, 1, 1, 1, 0}
    Dim NewStart As Boolean
    '检查移动
    Private Sub CheckMove(ByRef BolArr(,) As Integer, ByVal x As Integer, ByVal y As Integer, ByVal StepNum As Integer)
        If StepNum > 100 Then Exit Sub
        Dim dx, dy As Integer
        For i = 0 To 7
            dx = x + xArray(i) : dy = y + yArray(i)
            If Not (dx > 0 And dy > 0 And dx < BolArr.GetUpperBound(0) And dy < BolArr.GetUpperBound(1)) Then MouseDownUp(dx, dy, False) : NewStart = True : Exit Sub
            If CheckPointAround(BolArr, dx, dy) = False Then
                If BolArr(dx, dy) = 1 Then
                    BolArr(dx, dy) = 0
                    MouseMove(dx, dy)
                    If NewStart = True Then MouseDownUp(dx, dy, True) : NewStart = False
                    CheckMove(BolArr, dx, dy, StepNum + 1)
                    MouseDownUp(dx, dy, False)
                    NewStart = True
                End If
            Else
                BolArr(dx, dy) = 0
            End If
        Next
        Application.DoEvents()
    End Sub

    Private Sub Painting(BolArr(,) As Integer)
        Dim xCount As Integer = BolArr.GetUpperBound(0)
        Dim yCount As Integer = BolArr.GetUpperBound(1)
        Dim CP As New Point(xCount / 2, yCount / 2)
        Dim R As Integer = 0
        For R = 0 To If(xCount > yCount, xCount, yCount)
            For Theat = 0 To Math.PI * 2 Step 1 / R
                Dim dx As Integer = CP.X + R * Math.Cos(Theat)
                Dim dy As Integer = CP.Y + R * Math.Sin(Theat)
                If Not (dx > 0 And dy > 0 And dx < xCount And dy < yCount) Then Continue For
                'NewStart = True
                If BolArr(dx, dy) = 1 Then
                    BolArr(dx, dy) = 0
                    MouseMove(dx, dy)
                    MouseDownUp(dx, dy, True)
                    CheckMove(BolArr, dx, dy, 0)
                    MouseDownUp(dx, dy, False)
                End If
            Next
        Next
    End Sub
End Class

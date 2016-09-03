''' <summary>
''' 提供对位图图像和颜色的一系列操作的对象
''' </summary>
Public Class ImageProcess
    ''' <summary> 
    ''' 基于RGB根据指定阈值判断两个颜色是否相近
    ''' </summary> 
    Public Function CompareRGB(ByVal Color1 As Color, ByVal Color2 As Color, ByVal Distance As Single) As Boolean
        Dim r As Integer = Int(Color1.R) - Int(Color2.R)
        Dim g As Integer = Int(Color1.G) - Int(Color2.G)
        Dim b As Integer = Int(Color1.B) - Int(Color2.B)
        Dim absDis As Integer = Math.Sqrt(r * r + g * g + b * b)
        If absDis < Distance Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary> 
    ''' 基于HSB根据指定阈值判断两个颜色是否相近
    ''' </summary> 
    Public Function CompareHSB(ByVal Color1 As Color, ByVal Color2 As Color, ByVal Distance As Single) As Boolean
        '向量距离
        'Dim h As Single = (Color1.GetHue - Color2.GetHue) / 360
        'Dim s As Single = Color1.GetSaturation - Color2.GetSaturation
        'Dim b As Single = Color1.GetBrightness - Color2.GetBrightness
        'Dim absDis As Single = Math.Sqrt(h * h + s * s + b * b)
        'If absDis < Distance Then
        '    Return True
        'Else
        '    Return False
        'End If
        '向量夹角
        Dim h1 As Single = Color1.GetHue / 360
        Dim s1 As Single = Color1.GetSaturation
        Dim b1 As Single = Color1.GetBrightness
        Dim h2 As Single = Color2.GetHue / 360
        Dim s2 As Single = Color2.GetSaturation
        Dim b2 As Single = Color2.GetBrightness
        Dim absDis As Single = (h1 * h2 + s1 * s2 + b1 * b2) / (Math.Sqrt(h1 * h1 + s1 * s1 + b1 * b1) * Math.Sqrt(h2 * h2 + s2 * s2 + b2 * b2))
        If absDis > Distance / 5 + 0.8 Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary> 
    ''' 返回指定颜色的中值
    ''' </summary> 
    Public Function gethHD(ByVal color1 As Color)
        Dim HD, r, g, b As Integer
        r = color1.R
        g = color1.G
        b = color1.B
        HD = (r + g + b) / 3
        Return HD
    End Function
    ''' <summary>
    ''' 返回指定位图的颜色数组
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Public Function GetColorArr(ByRef gBitmap As Bitmap) As Color(,)
        Dim TempArr(gBitmap.Width - 1, gBitmap.Height - 1) As Color
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                TempArr(i, j) = gBitmap.GetPixel(i, j)
            Next
        Next
        Return TempArr
    End Function
    ''' <summary>
    ''' 返回指定矩形区域的屏幕图像
    ''' </summary>
    ''' <param name="rect">指定的矩形区域</param>
    ''' <returns></returns>
    Public Function GetScreenImage(ByVal rect As Rectangle) As Bitmap
        Dim resultBmp As New Bitmap(rect.Width, rect.Height)
        Using pg As Graphics = Graphics.FromImage(resultBmp)
            pg.CopyFromScreen(rect.X, rect.Y, 0, 0, New Size(rect.Width, rect.Height))
        End Using
        Return resultBmp
    End Function
    ''' <summary>
    ''' 返回指定文字生成的位图
    ''' </summary>
    ''' <param name="text">文本</param>
    ''' <param name="font">字体</param>
    ''' <param name="width">位图宽度</param>
    ''' <param name="height">位图高度</param>
    ''' <returns></returns>
    Public Function GetTextImage(ByVal text As String, ByVal font As Font, ByVal width As Integer, ByVal height As Integer) As Bitmap
        Dim resultBmp As New Bitmap(width, height)
        Using pg = Graphics.FromImage(resultBmp)
            pg.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias '抗锯齿
            pg.DrawString(text, font, Brushes.Black, 0, 0)
        End Using
        Return resultBmp
    End Function
    ''' <summary>
    ''' 返回指定图位图的二值化图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <param name="gSplitNum"></param>
    ''' <returns></returns>
    Public Function GetThresholdImage(ByVal gBitmap As Bitmap, ByVal gSplitNum As Single, Optional IsHSB As Boolean = False) As Bitmap
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim ColorArr(,) = GetColorArr(gBitmap)
        Dim TempHD As Integer
        Dim IsOverThreshold = Function(ByVal C1 As Color, ByVal gNum As Single)
                                  TempHD = gethHD(C1)
                                  Return (If(IsHSB, (C1.GetHue / 360 + C1.GetSaturation + C1.GetBrightness) / 3 < gNum,
                                  TempHD < gNum))
                              End Function
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                ResultBitmap.SetPixel(i, j, If(IsOverThreshold(ColorArr(i, j), gSplitNum), Color.Black, Color.White))
            Next
        Next

        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定位图的轮廓图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <param name="gDistance"></param>
    ''' <returns></returns>
    Public Function GetOutLineImage(ByVal gBitmap As Bitmap, ByVal gDistance As Single, Optional IsHSB As Boolean = False) As Bitmap
        Dim xArray2() As Short = {0, 1, 0, -1}
        Dim yArray2() As Short = {-1, 0, 1, 0}
        'Dim ResultBitmap As New Bitmap(gBitmap) '在原图的基础上绘图
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim Color1, Color2 As Color
        Dim CompareColor = Function(ByVal C1 As Color, ByVal C2 As Color, ByVal Distance As Single)
                               Return If(IsHSB,
                               CompareHSB(Color1, Color2, Distance),
                               CompareRGB(Color1, Color2, Distance))
                           End Function
        Dim CompareColorExtra = Function(ByVal C1 As Color, ByVal C2 As Color)
                                    Return If(IsHSB,
                                    Color1.GetBrightness - Color2.GetBrightness > 0,
                                    gethHD(Color1) - gethHD(Color2) > 0)
                                End Function
        Dim ColorArr(,) = GetColorArr(gBitmap)
        For i = 1 To gBitmap.Width - 2
            For j = 1 To gBitmap.Height - 2
                ResultBitmap.SetPixel(i, j, Color.White)
                Color1 = ColorArr(i, j)
                For p = 0 To 3
                    Color2 = ColorArr(i + xArray2(p), j + yArray2(p))
                    If Not CompareColor(Color1, Color2, gDistance) And CompareColorExtra(Color1, Color2) Then
                        ResultBitmap.SetPixel(i, j, Color.Black)
                        ' ResultBitmap.SetPixel(i, j, ColorArr(i, j))
                    End If
                Next
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定位图的空心图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Public Function GetAroundImage(gBitmap As Bitmap)
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim ImageBolArr(,) As Integer = GetImageBol(gBitmap)
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If ImageBolArr(i, j) = 1 AndAlso CheckPointAround(ImageBolArr, i, j) = False Then
                    ResultBitmap.SetPixel(i, j, Color.Black)
                Else
                    ResultBitmap.SetPixel(i, j, Color.White)
                End If
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定位图的反相图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Public Function GetInvertImage(gBitmap As Bitmap)
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim ImageBolArr(,) As Integer = GetImageBol(gBitmap)
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If ImageBolArr(i, j) = 1 Then
                    ResultBitmap.SetPixel(i, j, Color.White)
                Else
                    ResultBitmap.SetPixel(i, j, Color.Black)
                End If
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定位图的色块图像
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Public Function GetLumpImage(gBitmap As Bitmap, Optional Range As Integer = 10)
        Dim ResultBitmap As New Bitmap(gBitmap.Width, gBitmap.Height)
        Dim ColorArr(,) = GetColorArr(gBitmap)
        Dim R, G, B As Integer
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                R = Int(ColorArr(i, j).R / Range) * Range
                G = Int(ColorArr(i, j).G / Range) * Range
                B = Int(ColorArr(i, j).B / Range) * Range
                ResultBitmap.SetPixel(i, j, Color.FromArgb(R, G, B))
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定位图的二值化数据
    ''' </summary>
    ''' <param name="gBitmap"></param>
    ''' <returns></returns>
    Private Function GetImageBol(ByVal gBitmap As Bitmap) As Integer(,)
        Dim ResultArr(gBitmap.Width - 1, gBitmap.Height - 1) As Integer
        For i = 0 To gBitmap.Width - 1
            For j = 0 To gBitmap.Height - 1
                If Not gBitmap.GetPixel(i, j).Equals(Color.FromArgb(255, 255, 255)) Then
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
        If Not (x > 0 And y > 0 And x < BolArr.GetUpperBound(0) And y < BolArr.GetUpperBound(1)) Then Return True
        If BolArr(x - 1, y) = 1 And BolArr(x + 1, y) = 1 And BolArr(x, y - 1) = 1 And BolArr(x, y + 1) = 1 Then
            Return True '当前点为实体内部
        Else
            Return False '当前点为实体边缘
        End If
    End Function
End Class

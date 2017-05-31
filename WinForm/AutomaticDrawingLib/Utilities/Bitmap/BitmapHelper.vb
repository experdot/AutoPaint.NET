Imports System.Drawing
''' <summary>
''' 位图帮助类，提供对位图图像和颜色的一系列操作的对象
''' </summary>
Public Class BitmapHelper
    ''' <summary> 
    ''' 基于RGB根据指定阈值判断两个颜色是否相近
    ''' </summary> 
    Public Shared Function CompareRGB(color1 As Color, color2 As Color, distance As Single) As Boolean
        Dim r As Integer = Int(color1.R) - Int(color2.R)
        Dim g As Integer = Int(color1.G) - Int(color2.G)
        Dim b As Integer = Int(color1.B) - Int(color2.B)
        Dim temp As Integer = Math.Sqrt(r * r + g * g + b * b)
        If temp < distance Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary> 
    ''' 基于HSB根据指定阈值判断两个颜色是否相近
    ''' </summary> 
    Public Shared Function CompareHSB(color1 As Color, color2 As Color, distance As Single) As Boolean
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
        Dim h1 As Single = color1.GetHue / 360
        Dim s1 As Single = color1.GetSaturation
        Dim b1 As Single = color1.GetBrightness
        Dim h2 As Single = color2.GetHue / 360
        Dim s2 As Single = color2.GetSaturation
        Dim b2 As Single = color2.GetBrightness
        Dim absDis As Single = (h1 * h2 + s1 * s2 + b1 * b2) / (Math.Sqrt(h1 * h1 + s1 * s1 + b1 * b1) * Math.Sqrt(h2 * h2 + s2 * s2 + b2 * b2))
        If absDis > distance / 5 + 0.8 Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary> 
    ''' 返回指定颜色的中值
    ''' </summary> 
    Public Shared Function GetMedian(color As Color)
        Dim HD, r, g, b As Integer
        r = color.R
        g = color.G
        b = color.B
        HD = (r + g + b) / 3
        Return HD
    End Function
    ''' <summary>
    ''' 返回指定位图的颜色数组
    ''' </summary>
    Public Shared Function GetColorArr(bmp As Bitmap) As Color(,)
        Dim result(bmp.Width - 1, bmp.Height - 1) As Color
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                result(i, j) = bmp.GetPixel(i, j)
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定矩形区域的屏幕图像
    ''' </summary>
    Public Shared Function GetScreenImage(ByVal rect As Rectangle) As Bitmap
        Dim result As New Bitmap(rect.Width, rect.Height)
        Using pg As Graphics = Graphics.FromImage(result)
            pg.CopyFromScreen(rect.X, rect.Y, 0, 0, New Size(rect.Width, rect.Height))
        End Using
        Return result
    End Function
    ''' <summary>
    ''' 返回指定文字生成的位图
    ''' </summary>
    Public Shared Function GetTextImage(ByVal text As String, ByVal font As Font, ByVal width As Integer, ByVal height As Integer) As Bitmap
        Dim result As New Bitmap(width, height)
        Using pg = Graphics.FromImage(result)
            pg.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias '抗锯齿
            pg.DrawString(text, font, Brushes.Black, 0, 0)
        End Using
        Return result
    End Function
    ''' <summary>
    ''' 返回指定图位图的二值化图像
    ''' </summary>
    Public Shared Function GetThresholdImage(ByVal bmp As Bitmap, ByVal threshold As Single, Optional isHSB As Boolean = False) As Bitmap
        Dim result As New Bitmap(bmp.Width, bmp.Height)
        Dim colors(,) = GetColorArr(bmp)
        Dim TempHD As Integer
        Dim IsOverThreshold = Function(ByVal C1 As Color, ByVal gNum As Single)
                                  TempHD = GetMedian(C1)
                                  Return (If(isHSB, (C1.GetHue / 360 + C1.GetSaturation + C1.GetBrightness) / 3 < gNum, TempHD < gNum))
                              End Function
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                result.SetPixel(i, j, If(IsOverThreshold(colors(i, j), threshold), Color.Black, Color.White))
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定位图的轮廓图像
    ''' </summary>
    Public Shared Function GetOutLineImage(ByVal bmp As Bitmap, ByVal distance As Single, Optional isHSB As Boolean = False) As Bitmap
        Dim xArray2() As Short = {0, 1, 0, -1}
        Dim yArray2() As Short = {-1, 0, 1, 0}
        Dim result As New Bitmap(bmp.Width, bmp.Height)
        Dim color1, color2 As Color
        Dim CompareColor = Function(c1 As Color, c2 As Color, d As Single)
                               Return If(isHSB,
                               CompareHSB(c1, c2, d),
                               CompareRGB(c1, c2, d))
                           End Function
        Dim CompareColorExtra = Function(c1 As Color, c2 As Color)
                                    Return If(isHSB,
                                    color1.GetBrightness - color2.GetBrightness > 0,
                                    GetMedian(c1) - GetMedian(c2) > 0)
                                End Function
        Dim colors(,) = GetColorArr(bmp)
        For i = 1 To bmp.Width - 2
            For j = 1 To bmp.Height - 2
                result.SetPixel(i, j, Color.White)
                color1 = colors(i, j)
                For p = 0 To 3
                    color2 = colors(i + xArray2(p), j + yArray2(p))
                    If Not CompareColor(color1, color2, distance) And CompareColorExtra(color1, color2) Then
                        result.SetPixel(i, j, Color.Black)
                    End If
                Next
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定位图的空心图像
    ''' </summary>
    Public Shared Function GetAroundImage(bmp As Bitmap)
        Dim result As New Bitmap(bmp.Width, bmp.Height)
        Dim imageBol(,) As Integer = GetImageBol(bmp)
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                If imageBol(i, j) = 1 AndAlso BitmapHelper.CheckPointAround(imageBol, i, j) = False Then
                    result.SetPixel(i, j, Color.Black)
                Else
                    result.SetPixel(i, j, Color.White)
                End If
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定位图的反相图像
    ''' </summary>
    Public Shared Function GetInvertImage(bmp As Bitmap)
        Dim result As New Bitmap(bmp.Width, bmp.Height)
        Dim imageBol(,) As Integer = GetImageBol(bmp)
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                If imageBol(i, j) = 1 Then
                    result.SetPixel(i, j, Color.White)
                Else
                    result.SetPixel(i, j, Color.Black)
                End If
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定位图的色块图像
    ''' </summary>
    Public Shared Function GetLumpImage(bmp As Bitmap, Optional range As Integer = 10)
        Dim ResultBitmap As New Bitmap(bmp.Width, bmp.Height)
        Dim ColorArr(,) = GetColorArr(bmp)
        Dim R, G, B As Integer
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                R = Int(ColorArr(i, j).R / range) * range
                G = Int(ColorArr(i, j).G / range) * range
                B = Int(ColorArr(i, j).B / range) * range
                ResultBitmap.SetPixel(i, j, Color.FromArgb(R, G, B))
            Next
        Next
        Return ResultBitmap
    End Function
    ''' <summary>
    ''' 返回指定位图的二值化数据
    ''' </summary>
    Public Shared Function GetImageBol(ByVal bmp As Bitmap) As Integer(,)
        Dim result(bmp.Width - 1, bmp.Height - 1) As Integer
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                If Not bmp.GetPixel(i, j).Equals(Color.FromArgb(255, 255, 255)) Then
                    result(i, j) = 1
                Else
                    result(i, j) = 0
                End If
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 检查一个点是否被包围,被包围返回True
    ''' </summary>
    Private Shared Function CheckPointAround(bolArr As Integer(,), x As Integer, y As Integer) As Boolean
        If Not (x > 0 And y > 0 And x < bolArr.GetUpperBound(0) And y < bolArr.GetUpperBound(1)) Then Return True
        If bolArr(x - 1, y) = 1 And bolArr(x + 1, y) = 1 And bolArr(x, y - 1) = 1 And bolArr(x, y + 1) = 1 Then
            Return True
        Else
            Return False
        End If
    End Function
End Class

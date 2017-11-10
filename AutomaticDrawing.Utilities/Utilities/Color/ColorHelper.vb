Imports System.Drawing
Imports System.Numerics
Imports AutomaticDrawing.Core
''' <summary>
''' 颜色辅助类
''' </summary>
Public Class ColorHelper
    ''' <summary>
    ''' 返回颜色集合的平均颜色
    ''' </summary>
    Public Shared Function GetAverageColor(colors As IEnumerable(Of Color)) As Color
        Dim result As Color
        Dim a As Integer = colors.Sum(Function(c As Color) c.A) / colors.Count
        Dim r As Integer = colors.Sum(Function(c As Color) c.R) / colors.Count
        Dim g As Integer = colors.Sum(Function(c As Color) c.G) / colors.Count
        Dim b As Integer = colors.Sum(Function(c As Color) c.B) / colors.Count
        result = Color.FromArgb(a, r, g, b)
        Return result
    End Function
    ''' <summary>
    ''' 返回两个颜色的平均颜色
    ''' </summary>
    Public Shared Function GetAverageColor(color1 As Color, color2 As Color) As Color
        Dim result As Color
        Dim a As Integer = (CInt(color1.A) + CInt(color2.A)) / 2
        Dim r As Integer = (CInt(color1.R) + CInt(color2.R)) / 2
        Dim g As Integer = (CInt(color1.G) + CInt(color2.G)) / 2
        Dim b As Integer = (CInt(color1.B) + CInt(color2.B)) / 2
        result = Color.FromArgb(a, r, g, b)
        Return result
    End Function
    ''' <summary>
    ''' 返回两个颜色的相似度
    ''' </summary>
    Public Shared Function GetColorSimilarity(color1 As Color, color2 As Color) As Single
        Dim result As Single = 0
        Dim vec1 As New Vector3(color1.R, color1.G, color1.B)
        Dim vec2 As New Vector3(color2.R, color2.G, color2.B)
        result = 1 / (1 + (vec1 - vec2).LengthSquared)
        Return result
    End Function
    ''' <summary> 
    ''' 基于RGB判断两个颜色是否相似
    ''' </summary> 
    Public Shared Function CompareBaseRGB(color1 As Color, color2 As Color, distance As Single) As Boolean
        Dim r As Integer = CInt(color1.R) - CInt(color2.R)
        Dim g As Integer = CInt(color1.G) - CInt(color2.G)
        Dim b As Integer = CInt(color1.B) - CInt(color2.B)
        Dim temp As Integer = Math.Sqrt(r * r + g * g + b * b)
        If temp < distance Then
            Return True
        Else
            Return False
        End If
    End Function
    ''' <summary> 
    ''' 基于HSB判断两个颜色是否相似
    ''' </summary> 
    Public Shared Function CompareBaseHSB(color1 As Color, color2 As Color, distance As Single) As Boolean
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
    ''' 返回指定颜色的灰度值
    ''' </summary> 
    Public Shared Function GetGrayOfColor(color As Color) As Integer
        Dim mid, r, g, b As Integer
        r = color.R
        g = color.G
        b = color.B
        mid = (r + g + b) / 3
        Return mid
    End Function

    ''' <summary>
    ''' 返回指定像素数据的块变换
    ''' </summary>
    Public Shared Function GetLumpPixelData(pixels As PixelData, Optional range As Integer = 10) As PixelData
        Dim colors(,) = pixels.GetColorsClone
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                Dim r = (colors(i, j).R \ range) * range
                Dim g = (colors(i, j).G \ range) * range
                Dim b = (colors(i, j).B \ range) * range
                colors(i, j) = Color.FromArgb(r, g, b)
            Next
        Next
        Return PixelData.CreateFromColors(colors)
    End Function
    ''' <summary>
    ''' 返回指定像素数据的二值化变换
    ''' </summary>
    Public Shared Function GetThresholdPixelData(pixels As PixelData, threshold As Single, Optional isHSB As Boolean = False) As PixelData
        Dim colors(,) = pixels.GetColorsClone()
        Dim IsOverThreshold = Function(ByVal color As Color)
                                  Return If(isHSB, (color.GetHue / 360 + color.GetBrightness + color.GetSaturation) / 3 < threshold, GetGrayOfColor(color) < threshold)
                                  'Return If(isHSB, (color.GetSaturation) < threshold, GetMedian(color) < threshold)
                              End Function
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                colors(i, j) = If(IsOverThreshold(colors(i, j)), Color.Black, Color.White)
            Next
        Next
        Return PixelData.CreateFromColors(colors)
    End Function
    ''' <summary>
    ''' 返回指定位图的轮廓图像
    ''' </summary>
    Public Shared Function GetOutLinePixelData(pixels As PixelData, distance As Single, Optional isHSB As Boolean = False) As PixelData
        Static OffsetX() As Short = {0, 1, 0, -1}
        Static OffsetY() As Short = {-1, 0, 1, 0}
        Dim CompareColor = Function(c1 As Color, c2 As Color, d As Single)
                               Return If(isHSB,
                               CompareBaseHSB(c1, c2, d),
                               CompareBaseRGB(c1, c2, d))
                           End Function
        Dim CompareColorExtra = Function(c1 As Color, c2 As Color)
                                    Return If(isHSB,
                                    c1.GetBrightness - c2.GetBrightness > 0,
                                    GetGrayOfColor(c1) - GetGrayOfColor(c2) > 0)
                                End Function
        Dim colors = pixels.GetColorsClone()
        Dim color1, color2 As Color
        For i = 1 To pixels.Width - 2
            For j = 1 To pixels.Height - 2
                color1 = pixels.Colors(i, j)
                For p = 0 To 3
                    color2 = pixels.Colors(i + OffsetX(p), j + OffsetY(p))
                    If Not CompareColor(color1, color2, distance) AndAlso CompareColorExtra(color1, color2) Then
                        colors(i, j) = Color.Black
                    Else
                        colors(i, j) = Color.White
                    End If
                Next
            Next
        Next
        Return PixelData.CreateFromColors(colors)
    End Function
    ''' <summary>
    ''' 返回指定二值化像素数据的空心变换
    ''' </summary>
    Public Shared Function GetHollowPixelData(pixels As PixelData) As PixelData
        Dim colors = pixels.GetColorsClone()
        Dim bools(,) As Integer = GetPixelDataBools(pixels)
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                If bools(i, j) = 1 AndAlso IsBesieged(bools, i, j) = False Then
                    colors(i, j) = Color.Black
                Else
                    colors(i, j) = Color.White
                End If
            Next
        Next
        Return PixelData.CreateFromColors(colors)
    End Function
    ''' <summary>
    ''' 返回指定二值化像素数据的反相变换
    ''' </summary>
    Public Shared Function GetInvertPixelData(pixels As PixelData) As PixelData
        Dim colors = pixels.GetColorsClone()
        Dim bools(,) As Integer = GetPixelDataBools(pixels)
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                If bools(i, j) = 0 Then
                    colors(i, j) = Color.Black
                Else
                    colors(i, j) = Color.White
                End If
            Next
        Next
        Return PixelData.CreateFromColors(colors)
    End Function
    ''' <summary>
    ''' 返回指定二值化像素数据的的布尔数组
    ''' </summary>
    Public Shared Function GetPixelDataBools(pixels As PixelData) As Integer(,)
        Dim result(pixels.Width - 1, pixels.Height - 1) As Integer
        Dim colors(,) = pixels.GetColorsClone
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                If colors(i, j).Equals(Color.FromArgb(255, 255, 255)) Then
                    result(i, j) = 0
                Else
                    result(i, j) = 1
                End If
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定索引位置是否被包围
    ''' </summary>
    Private Shared Function IsBesieged(bools As Integer(,), x As Integer, y As Integer) As Boolean
        Static OffsetX() As Short = {0, 1, 0, -1}
        Static OffsetY() As Short = {-1, 0, 1, 0}
        For i = 0 To 3
            Dim dx As Integer = x + OffsetX(i)
            Dim dy As Integer = y + OffsetY(i)
            If IsIndexOverFlow(bools, dx, dy) OrElse bools(dx, dy) = 0 Then
                Return False
            End If
        Next
        Return True
    End Function
    ''' <summary>
    ''' 返回指定数组索引是否越界
    ''' </summary>
    Private Shared Function IsIndexOverFlow(array As Array, x As Integer, y As Integer)
        Dim w As Integer = array.GetUpperBound(0)
        Dim h As Integer = array.GetUpperBound(1)
        Return Not (x >= 0 AndAlso y >= 0 AndAlso x <= w AndAlso y <= h)
    End Function
End Class

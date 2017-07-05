Imports System.Drawing
''' <summary>
''' 位图帮助类，提供对位图图像和颜色的一系列操作的对象
''' </summary>
Public Class BitmapHelper

    ''' <summary>
    ''' 返回指定矩形区域的屏幕图像
    ''' </summary>
    Public Shared Function GetScreenImage(rect As Rectangle) As Bitmap
        Dim result As New Bitmap(rect.Width, rect.Height)
        Using pg As Graphics = Graphics.FromImage(result)
            pg.CopyFromScreen(rect.X, rect.Y, 0, 0, New Size(rect.Width, rect.Height))
        End Using
        Return result
    End Function
    ''' <summary>
    ''' 返回指定文字生成的位图
    ''' </summary>
    Public Shared Function GetTextImage(text As String, font As Font, width As Integer, height As Integer) As Bitmap
        Dim result As New Bitmap(width, height)
        Using pg = Graphics.FromImage(result)
            pg.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias '抗锯齿
            pg.DrawString(text, font, Brushes.Black, 0, 0)
        End Using
        Return result
    End Function

    ''' <summary>
    ''' 返回指定位图包含的像素数据
    ''' </summary>
    Public Shared Function GetPixelDataFromBitmap(bmp As Bitmap) As PixelData
        Dim colors(bmp.Width - 1, bmp.Height - 1) As Color
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                colors(i, j) = bmp.GetPixel(i, j)
            Next
        Next
        Return PixelData.CreateFromColors(colors)
    End Function
    ''' <summary>
    ''' 返回指定颜色数组生成的位图
    ''' </summary>
    Public Shared Function GetBitmapFromPixelData(pixels As PixelData) As Bitmap
        Return GetBitmapFromColors(pixels.Colors)
    End Function
    ''' <summary>
    ''' 返回指定位图包含的颜色数组
    ''' </summary>
    Public Shared Function GetColorsFromBitmap(bmp As Bitmap) As Color(,)
        Dim result(bmp.Width - 1, bmp.Height - 1) As Color
        For i = 0 To bmp.Width - 1
            For j = 0 To bmp.Height - 1
                result(i, j) = bmp.GetPixel(i, j)
            Next
        Next
        Return result
    End Function
    ''' <summary>
    ''' 返回指定颜色数组生成的位图
    ''' </summary>
    Public Shared Function GetBitmapFromColors(colors As Color(,)) As Bitmap
        Dim w As Integer = colors.GetUpperBound(0) + 1
        Dim h As Integer = colors.GetUpperBound(1) + 1
        Dim result As New Bitmap(w, h)
        For i = 0 To w - 1
            For j = 0 To h - 1
                result.SetPixel(i, j, colors(i, j))
            Next
        Next
        Return result
    End Function
End Class

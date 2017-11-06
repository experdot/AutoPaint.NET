Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports AutomaticDrawing.Core

Public Module BitmapExtensions
    ''' <summary>
    ''' 返回当前位图的像素数据
    ''' </summary>
    <Extension>
    Public Function GetPixelData(this As Drawing.Bitmap) As PixelData
        Return BitmapHelper.GetPixelDataFromBitmap(this)
    End Function
    ''' <summary>
    ''' 从当前像素数据生成位图
    ''' </summary>
    <Extension>
    Public Function CreateBitmap(this As PixelData) As Drawing.Bitmap
        Return BitmapHelper.GetBitmapFromPixelData(this)
    End Function
End Module

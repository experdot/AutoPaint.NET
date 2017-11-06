Imports System.Drawing
''' <summary>
''' 像素数据
''' </summary>
Public Structure PixelData
    ''' <summary>
    ''' 颜色数组
    ''' </summary>
    Public Colors As Color(,)
    ''' <summary>
    ''' 宽度
    ''' </summary>
    Public Width As Integer
    ''' <summary>
    ''' 高度
    ''' </summary>
    Public Height As Integer
    ''' <summary>
    ''' 创建并初始化一个对象
    ''' </summary>
    Public Sub New(w As Integer, h As Integer)
        ReDim Colors(w - 1, h - 1)
        Me.Width = w
        Me.Height = h
    End Sub
    ''' <summary>
    ''' 从指定的颜色数组创建像素数据
    ''' </summary>
    Public Shared Function CreateFromColors(colors As Color(,)) As PixelData
        Dim w As Integer = colors.GetUpperBound(0) + 1
        Dim h As Integer = colors.GetUpperBound(1) + 1
        Dim result As New PixelData(w, h) With {.Colors = colors}
        Return result
    End Function
    ''' <summary>
    ''' 返回颜色数组的浅表副本
    ''' </summary>
    Public Function GetColorsClone() As Color(,)
        Return Colors.Clone
    End Function
End Structure

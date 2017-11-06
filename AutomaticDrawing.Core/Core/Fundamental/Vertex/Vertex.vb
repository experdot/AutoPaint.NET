Imports System.Drawing
Imports System.Numerics
''' <summary>
''' 具有位置、颜色和大小的顶点
''' </summary>
Public Class Vertex
    ''' <summary>
    ''' 位置
    ''' </summary>
    Public Property Position As Vector2
    ''' <summary>
    ''' 颜色
    ''' </summary>
    Public Property Color As Color
    ''' <summary>
    ''' 大小
    ''' </summary>
    Public Property Size As Single
    ''' <summary>
    ''' 位置X分量
    ''' </summary>
    Public ReadOnly Property X As Integer
        Get
            Return Position.X
        End Get
    End Property
    ''' <summary>
    ''' 位置Y分量
    ''' </summary>
    Public ReadOnly Property Y As Integer
        Get
            Return Position.Y
        End Get
    End Property
End Class

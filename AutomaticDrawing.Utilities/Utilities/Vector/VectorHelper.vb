Imports System.Numerics
''' <summary>
''' 向量帮助类
''' </summary>
Public Class VectorHelper
    ''' <summary>
    ''' 返回向量集合的平均位置
    ''' </summary>
    Public Shared Function GetAveratePosition(positions As IEnumerable(Of Vector2)) As Vector2
        Dim result As Vector2
        Dim x As Single = positions.Sum(Function(p As Vector2) p.X) / positions.Count
        Dim y As Single = positions.Sum(Function(p As Vector2) p.Y) / positions.Count
        result = New Vector2(x, y)
        Return result
    End Function
End Class

Imports System.Numerics
Imports AutomaticDrawing.Core
Imports AutomaticDrawing.Recognition.Clustering
''' <summary>
''' 由单元格表示的层
''' </summary>
Public Class GridHierarchy
    Inherits HierarchyBase
    ''' <summary>
    ''' 单元格
    ''' </summary>
    Public Property Grid As List(Of Cluster)(,)
    ''' <summary>
    ''' 宽度
    ''' </summary>
    Public Property Width As Integer
    ''' <summary>
    ''' 高度
    ''' </summary>
    Public Property Height As Integer
    ''' <summary>
    ''' 单元格大小
    ''' </summary>
    Public Property Size As Single

    Private Shared OffsetX() As Integer = {0, -1, 0, 1, 1, 1, 0, -1, -1}
    Private Shared OffsetY() As Integer = {0, -1, -1, -1, 0, 1, 1, 1, 0}

    ''' <summary>
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(w As Integer, h As Integer, size As Single)
        If w < 1 Then w = 1
        If h < 1 Then h = 1
        Me.Width = w
        Me.Height = h
        Me.Size = size
        ReDim Grid(w - 1, h - 1)
        For i = 0 To w - 1
            For j = 0 To h - 1
                Grid(i, j) = New List(Of Cluster)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 由指定的<see cref="PixelData"/>对象创建一个实例
    ''' </summary>
    Public Shared Function CreateFromPixels(pixels As PixelData) As GridHierarchy
        Dim result As New GridHierarchy(pixels.Width, pixels.Height, 1)
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                Dim cluster As New Cluster With
                {
                    .Position = New Vector2(i, j),
                    .Color = pixels.Colors(i, j)
                }
                result.Grid(i, j).Add(cluster)
                result.Clusters.Add(cluster)
                'result.AddCluster(cluster)'该方法存在性能问题
            Next
        Next
        Return result
    End Function

    Public Overrides Function Generate() As IHierarchy
        'Dim result As New GroupHierarchy With {.Rank = Me.Rank + 1}
        'For Each SubCluster In Clusters
        '    Dim similar As Cluster = SubCluster.GetMostSimilar(GetNeighbours(SubCluster))
        '    If similar IsNot Nothing Then
        '        result.AddCluster(Cluster.Combine(SubCluster, similar))
        '    End If
        'Next
        Dim rate As Single = 2.0F
        Dim newSize As Single = Me.Size * rate
        Dim result As New GridHierarchy(CInt(Math.Ceiling(Me.Width / rate) + 1), CInt(Math.Ceiling(Me.Height / rate) + 1), newSize) With {.Rank = Me.Rank + 1}

        '合并为新簇
        For Each SubCluster In Clusters
            Dim similar As Cluster = SubCluster.GetMostSimilar(GetNeighbours(SubCluster)).First
            If similar IsNot Nothing Then
                result.AddCluster(Cluster.Combine(SubCluster, similar), False)
            End If
        Next
        '设置属性
        For Each SubCluster In result.Clusters
            SubCluster.Position = SubCluster.GetAveragePosition()
            SubCluster.Color = SubCluster.GetAverageColor()
        Next
        '分配至单元格
        For Each SubCluster In result.Clusters
            Dim p As Vector2 = SubCluster.Position
            Dim x As Integer = CInt(p.X / result.Size)
            Dim y As Integer = CInt(p.Y / result.Size)
            result.Grid(x, y).Add(SubCluster)
        Next
        Return result
    End Function

    Public Overrides Function ToString() As String
        Return $"Rank:{Rank}Clusters.Count:{Clusters.Count}"
    End Function


    Private Function GetNeighbours(cluster As Cluster) As List(Of Cluster)
        Dim result As New List(Of Cluster)
        Dim xBound As Integer = Grid.GetUpperBound(0)
        Dim yBound As Integer = Grid.GetUpperBound(1)
        Dim dx, dy As Integer
        Dim x As Integer = CInt(cluster.Position.X / Size)
        Dim y As Integer = CInt(cluster.Position.Y / Size)
        For i = 0 To 8
            dx = x + OffsetX(i)
            dy = y + OffsetY(i)
            If (dx >= 0 AndAlso dy >= 0 AndAlso dx <= xBound AndAlso dy <= yBound) Then
                result.AddRange(Grid(dx, dy))
            Else
                Continue For
            End If
        Next
        result.Remove(cluster)
        Return result
    End Function
End Class

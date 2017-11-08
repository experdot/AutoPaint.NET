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
    Public Property Grid As Cluster(,)

    Private Shared OffsetX() As Integer = {-1, 0, 1, 1, 1, 0, -1, -1}
    Private Shared OffsetY() As Integer = {-1, -1, -1, 0, 1, 1, 1, 0}

    ''' <summary>
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(w As Integer, h As Integer)
        ReDim Grid(w - 1, h - 1)
    End Sub

    ''' <summary>
    ''' 由指定的<see cref="PixelData"/>对象创建一个实例
    ''' </summary>
    Public Shared Function CreateFromPixels(pixels As PixelData) As GridHierarchy
        Dim result As New GridHierarchy(pixels.Width, pixels.Height)
        For i = 0 To pixels.Width - 1
            For j = 0 To pixels.Height - 1
                Dim cluster As New Cluster With
                {
                    .Position = New Vector2(i, j),
                    .Color = pixels.Colors(i, j)
                }
                result.Grid(i, j) = cluster
                result.AddCluster(cluster)
            Next
        Next
        Return result
    End Function

    Public Overrides Function Generate() As IHierarchy
        Dim result As New GroupHierarchy With {.Rank = Me.Rank + 1}

        For Each SubCluster In Clusters
            Dim similar As Cluster = SubCluster.GetMostSimilar(GetNeighbours(SubCluster))
            If similar IsNot Nothing Then
                result.AddCluster(Cluster.Combine(SubCluster, similar))
            End If
        Next

        Return result
    End Function

    Private Function GetNeighbours(cluster As Cluster) As List(Of Cluster)
        Dim result As New List(Of Cluster)
        Dim xBound As Integer = Grid.GetUpperBound(0)
        Dim yBound As Integer = Grid.GetUpperBound(1)
        Dim dx, dy As Integer
        Dim x As Integer = cluster.Position.X
        Dim y As Integer = cluster.Position.Y
        For i = 0 To 7
            dx = x + OffsetX(i)
            dy = y + OffsetY(i)
            If (dx >= 0 AndAlso dy >= 0 AndAlso dx <= xBound AndAlso dy <= yBound) Then
                result.Add(Grid(dx, dy))
            Else
                Continue For
            End If
        Next
        Return result
    End Function
End Class

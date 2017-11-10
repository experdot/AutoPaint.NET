Imports System.Numerics
Imports AutomaticDrawing.Core
''' <summary>
''' 聚类AI
''' </summary>
Public Class ClusterAI
    ''' <summary>
    ''' 线条集
    ''' </summary>
    Public Property Lines As New List(Of ILine)
    ''' <summary>
    ''' 层集
    ''' </summary>
    Public Property Hierarchies As New List(Of IHierarchy)

    ''' <summary>
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(pixels As PixelData, Optional maxRank As Integer = 5)
        Hierarchies.Add(GridHierarchy.CreateFromPixels(pixels))
        For i = 0 To maxRank - 1
            Hierarchies.Add(Hierarchies.Last.Generate())
            Debug.WriteLine($"Total:{maxRank},Current:{i + 1}")
        Next
        Lines = GenerateLines(Hierarchies.Last)
    End Sub

    Private Function GenerateLines(hierarchy As IHierarchy) As List(Of ILine)
        Dim result As New List(Of ILine)
        For Each SubCluster In hierarchy.Clusters
            Dim line As New Line
            For Each SubLeaf In SubCluster.Leaves
                line.Vertices.Add(New Vertex With {.Color = SubLeaf.Color, .Position = SubLeaf.Position, .Size = 1.0F})
            Next
            result.Add(line)
        Next
        Return result
    End Function

End Class

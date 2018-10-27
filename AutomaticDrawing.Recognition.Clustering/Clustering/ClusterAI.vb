Imports System.Drawing
Imports System.Numerics
Imports AutoPaint.Core
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
    Public Sub New(pixels As PixelData, Optional maxRank As Integer = 10)
        Dim start As DateTime = DateTime.Now
        Debug.WriteLine("Initialize Start")
        Hierarchies.Add(GridHierarchy.CreateFromPixels(pixels))
        Debug.WriteLine($"Initialize Over,TimeConsuming:{DateTime.Now - start}")
        Debug.WriteLine($"Generation Start")
        For i = 0 To maxRank - 1
            start = DateTime.Now
            Hierarchies.Add(Hierarchies.Last.Generate())
            Debug.WriteLine($"Total:{maxRank},Current:{i + 1},Time Consuming:{DateTime.Now - start},Hierarchy[{Hierarchies.Last.ToString()}]")
        Next
        Debug.WriteLine(pixels.Colors.Length)
        For i = maxRank - 1 To 0 Step -1
            Lines.AddRange(GenerateLines(Hierarchies(i)))
        Next
    End Sub

    Private Function GenerateLines(hierarchy As IHierarchy) As List(Of ILine)
        Dim result As New List(Of ILine)
        Dim count As Integer
        For Each SubCluster In hierarchy.Clusters
            Dim line As New Line
            For Each SubLeaf In SubCluster.Leaves
                Dim c As Color = SubCluster.Color
                Dim p As Color = Color.FromArgb(CInt(c.A / (hierarchy.Rank + 1.0F)), c.R, c.G, c.B)
                line.Vertices.Add(New Vertex With {.Color = SubCluster.Color, .Position = SubLeaf.Position, .Size = hierarchy.Rank + 1.0F})

            Next
            result.Add(line)
            count += line.Vertices.Count
        Next
        Debug.WriteLine(count)
        Return result
    End Function

End Class

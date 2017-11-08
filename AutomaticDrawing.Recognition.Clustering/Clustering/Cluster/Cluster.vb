Imports System.Drawing
Imports System.Numerics
''' <summary>
''' 簇
''' </summary>
Public Class Cluster
    ''' <summary>
    ''' 父簇
    ''' </summary>
    Public Property Parent As Cluster
    ''' <summary>
    ''' 子簇
    ''' </summary>
    Public Property Children As New List(Of Cluster)
    ''' <summary>
    ''' 位置
    ''' </summary>
    Public Property Position As Vector2
    ''' <summary>
    ''' 颜色
    ''' </summary
    Public Property Color As Color
    ''' <summary>
    ''' 叶子子簇
    ''' </summary>
    Public ReadOnly Property Leaves As List(Of Cluster)
        Get
            If Children.Count = 0 Then
                Return New List(Of Cluster) From {Me}
            Else
                Return GetLeavesOfChildren()
            End If
        End Get
    End Property
    ''' <summary>
    ''' 子簇平均位置
    ''' </summary>
    Public ReadOnly Property AveragePostion As Vector2
        Get
            If Children.Count = 0 OrElse Not Position = Vector2.Zero Then
                Return Position
            Else
                Dim postions As IEnumerable(Of Vector2) = GetAvearagePostionsOfChidren()
                Position = GetAveratePosition(postions)
                Return Position
            End If
        End Get
    End Property
    ''' <summary>
    ''' 子簇平均颜色
    ''' </summary>
    Public ReadOnly Property AverageColor As Color
        Get
            If Children.Count = 0 Then
                Return Color
            Else
                Dim colors As IEnumerable(Of Color) = GetAvearageColorOfChildren()
                Return GetAverateColor(colors)
            End If
        End Get
    End Property


    ''' <summary>
    ''' 合并簇
    ''' </summary>
    Public Shared Function Combine(cluster1 As Cluster, cluster2 As Cluster) As Cluster
        Dim result As Cluster
        If cluster1.Parent IsNot Nothing Then
            result = cluster1.Parent
        ElseIf cluster2.Parent IsNot Nothing Then
            result = cluster2.Parent
        Else
            result = New Cluster
        End If

        result.AddChild(cluster1)
        result.AddChild(cluster2)

        result.Position = (cluster1.Position + cluster2.Position) / 2
        result.Color = GetAverateColor(cluster1.Color, cluster2.Color)
        Return result
    End Function
    ''' <summary>
    ''' 添加子簇
    ''' </summary>
    Public Sub AddChild(child As Cluster, Optional repeat As Boolean = False)
        If repeat OrElse Not Children.Contains(child) Then
            Children.Add(child)
            child.Parent = Me
        End If
    End Sub
    ''' <summary>
    ''' 返回最相似的簇
    ''' </summary>
    Public Function GetMostSimilar(clusters As List(Of Cluster)) As Cluster
        Dim result As New Cluster
        Dim maxValue As Single = Single.MinValue
        For Each SubCluster In clusters
            If SubCluster IsNot Me Then
                Dim value As Single = Compare(Me, SubCluster)
                If value > maxValue Then
                    maxValue = value
                    result = SubCluster
                End If
            End If
        Next
        Return result
    End Function

    Private Shared Function GetAverateColor(color1 As Color, color2 As Color) As Color
        Dim result As Color
        Dim a As Integer = (CInt(color1.A) + CInt(color2.A)) / 2
        Dim r As Integer = (CInt(color1.R) + CInt(color2.R)) / 2
        Dim g As Integer = (CInt(color1.G) + CInt(color2.G)) / 2
        Dim b As Integer = (CInt(color1.B) + CInt(color2.B)) / 2
        result = Color.FromArgb(a, r, g, b)
        Return result
    End Function
    Private Shared Function GetAveratePosition(positions As IEnumerable(Of Vector2)) As Vector2
        Dim result As Vector2

        Dim x As Single = positions.Sum(Function(p As Vector2) p.X) / positions.Count
        Dim y As Single = positions.Sum(Function(p As Vector2) p.Y) / positions.Count

        result = New Vector2(x, y)
        Return result
    End Function
    Private Shared Function GetAverateColor(colors As IEnumerable(Of Color)) As Color
        Dim result As Color

        Dim a As Integer = colors.Sum(Function(c As Color) c.A) / colors.Count
        Dim r As Integer = colors.Sum(Function(c As Color) c.R) / colors.Count
        Dim g As Integer = colors.Sum(Function(c As Color) c.G) / colors.Count
        Dim b As Integer = colors.Sum(Function(c As Color) c.B) / colors.Count

        result = Color.FromArgb(a, r, g, b)
        Return result
    End Function

    Private Function Compare(cluster1 As Cluster, cluster2 As Cluster) As Single
        Dim result As Single = 0

        'Dim p1 As Vector2 = cluster1.Position
        'Dim p2 As Vector2 = cluster2.Position
        'Dim positionDistance As Single = 1 / (p1 - p2).Length

        Dim color1 As Color = cluster1.AverageColor
        Dim color2 As Color = cluster2.AverageColor
        Dim colorDistance As Single

        'Dim h1 As Single = color1.GetHue / 360
        'Dim s1 As Single = color1.GetSaturation
        'Dim b1 As Single = color1.GetBrightness
        'Dim h2 As Single = color2.GetHue / 360
        'Dim s2 As Single = color2.GetSaturation
        'Dim b2 As Single = color2.GetBrightness
        'colorDistance = (h1 * h2 + s1 * s2 + b1 * b2) / (Math.Sqrt(h1 * h1 + s1 * s1 + b1 * b1) * Math.Sqrt(h2 * h2 + s2 * s2 + b2 * b2))

        Dim vec1 As New Vector3(color1.R, color1.G, color1.B)
        Dim vec2 As New Vector3(color2.R, color2.G, color2.B)
        colorDistance = 1 / (1 + (vec1 - vec2).LengthSquared)

        result = colorDistance 'positionDistance * colorDistance
        Return result
    End Function

    Private Function GetLeavesOfChildren() As List(Of Cluster)
        Return (Children.SelectMany(Of Cluster)(Function(c As Cluster) c.Leaves)).ToList
    End Function
    Private Function GetAvearagePostionsOfChidren() As IEnumerable(Of Vector2)
        Return From c As Cluster In Children Select c.AveragePostion
    End Function
    Private Function GetAvearageColorOfChildren() As IEnumerable(Of Color)
        Return From c As Cluster In Children Select c.AverageColor
    End Function
End Class

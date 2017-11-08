Imports AutomaticDrawing.Recognition.Clustering

Public Interface IHierarchy
    ''' <summary>
    ''' 簇集
    ''' </summary>
    Property Clusters As List(Of Cluster)
    ''' <summary>
    ''' 层级
    ''' </summary>
    Property Rank As Integer
    ''' <summary>
    ''' 生成
    ''' </summary>
    Function Generate() As IHierarchy
End Interface

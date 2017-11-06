Imports System.Numerics
Imports AutomaticDrawing.Core
''' <summary>
''' 自动循迹并生成绘制序列的AI
''' </summary>
Public Class SequenceAI
    Implements IDisposable
    ''' <summary>
    ''' 线条集
    ''' </summary>
    Public Property Lines As List(Of ILine)
    ''' <summary>
    ''' 扫描方式
    ''' </summary>
    Public Property ScanMode As ScanMode = ScanMode.Rect
    ''' <summary>
    ''' 最大搜索深度
    ''' </summary>
    Public Property DepthMax As Integer = 1000
    ''' <summary>
    ''' 是否检查边界权值
    ''' </summary>
    Public Property IsCheckAround As Boolean = False
    ''' <summary>
    ''' 边界权值下界
    ''' </summary>
    Public Property AroundLower As Integer = 3
    ''' <summary>
    ''' 边界权值上界
    ''' </summary>
    Public Property AroundUpper As Integer = 7

    Private Shared OffsetX() As Integer = {-1, 0, 1, 1, 1, 0, -1, -1}
    Private Shared OffsetY() As Integer = {-1, -1, -1, 0, 1, 1, 1, 0}
    Private NewLine As Boolean
    ''' <summary>
    ''' 创建并初始化一个示例
    ''' </summary>
    Public Sub New(bools(,) As Integer, Optional mode As ScanMode = ScanMode.Rect, Optional isCheckAround As Boolean = False)
        Me.ScanMode = mode
        Me.IsCheckAround = isCheckAround
        Lines = New List(Of ILine)
        CalculateSequence(bools)
    End Sub

    ''' <summary>
    ''' 新增一个序列
    ''' </summary>
    Private Sub CreateNewSequence()
        Lines.Add(New Line)
    End Sub
    ''' <summary>
    ''' 在序列List末尾项新增一个点
    ''' </summary>
    Private Sub AddPoint(position As Vector2)
        Lines.Last.Vertices.Add(New Vertex() With {.Position = position, .Size = 1})
    End Sub
    ''' <summary>
    ''' 计算序列
    ''' </summary>
    Private Sub CalculateSequence(bools(,) As Integer)
        If ScanMode = ScanMode.Rect Then
            ScanRect(bools)
        Else
            ScanCircle(bools)
        End If
    End Sub
    ''' <summary>
    ''' 圆形扫描
    ''' </summary>
    Private Sub ScanCircle(bools(,) As Integer)
        Dim xlength As Integer = bools.GetLength(0)
        Dim ylength As Integer = bools.GetLength(1)
        Dim center As New Vector2(CSng(xlength / 2), CSng(ylength / 2))
        Dim radius As Integer = 0
        For radius = 0 To CInt(Math.Sqrt(xlength * xlength + ylength * ylength))
            For Theat = 0 To Math.PI * 2 Step 1 / radius
                Dim dx As Integer = CInt(center.X + radius * Math.Cos(Theat))
                Dim dy As Integer = CInt(center.Y + radius * Math.Sin(Theat))
                If Not (dx >= 0 AndAlso dy >= 0 AndAlso dx < xlength AndAlso dy < ylength) Then Continue For
                If bools(dx, dy) = 1 Then
                    bools(dx, dy) = 0
                    Me.CreateNewSequence()
                    Me.AddPoint(New Vector2(dx, dy))
                    MoveNext(bools, dx, dy, 0)
                    NewLine = True
                End If
            Next
        Next
    End Sub
    ''' <summary>
    ''' 矩形扫描
    ''' </summary>
    Private Sub ScanRect(BolArr(,) As Integer)
        Dim xCount As Integer = BolArr.GetUpperBound(0)
        Dim yCount As Integer = BolArr.GetUpperBound(1)
        For i = 0 To xCount - 1
            For j = 0 To yCount - 1
                Dim dx As Integer = i
                Dim dy As Integer = j
                If Not (dx > 0 AndAlso dy > 0 AndAlso dx < xCount AndAlso dy < yCount) Then Continue For
                If BolArr(dx, dy) = 1 Then
                    BolArr(dx, dy) = 0
                    Me.CreateNewSequence()
                    Me.AddPoint(New Vector2(dx, dy))
                    MoveNext(BolArr, dx, dy, 0)
                    NewLine = True
                End If
            Next
        Next
    End Sub
    ''' <summary>
    ''' 递归循迹
    ''' </summary>
    Private Sub MoveNext(bools(,) As Integer, x As Integer, y As Integer, depth As Integer)
        If depth > DepthMax Then Return
        Dim xBound As Integer = bools.GetUpperBound(0)
        Dim yBound As Integer = bools.GetUpperBound(1)
        Dim dx, dy As Integer
        If IsCheckAround Then
            Dim around As Integer = GetAroundValue(bools, x, y)
            If around >= AroundLower AndAlso around <= AroundUpper Then
                Return
            End If
        End If
        For i = 0 To 7
            dx = x + OffsetX(i)
            dy = y + OffsetY(i)
            If Not (dx >= 0 AndAlso dy >= 0 AndAlso dx <= xBound AndAlso dy <= yBound) Then
                Return
            ElseIf bools(dx, dy) = 1 Then
                bools(dx, dy) = 0
                If NewLine = True Then
                    Me.CreateNewSequence()
                    Me.AddPoint(New Vector2(dx, dy))
                    NewLine = False
                Else
                    Me.AddPoint(New Vector2(dx, dy))
                End If
                MoveNext(bools, dx, dy, depth + 1)
                NewLine = True
            End If
        Next
    End Sub
    ''' <summary>
    ''' 返回点权值
    ''' </summary>
    Private Function GetAroundValue(bools(,) As Integer, x As Integer, y As Integer) As Integer
        Dim dx, dy, result As Integer
        Dim xBound As Integer = bools.GetUpperBound(0)
        Dim yBound As Integer = bools.GetUpperBound(1)
        For i = 0 To 7
            dx = x + OffsetX(i)
            dy = y + OffsetY(i)
            If dx >= 0 AndAlso dy >= 0 AndAlso dx <= xBound AndAlso dy <= yBound Then
                If bools(dx, dy) = 1 Then
                    result += 1
                End If
            End If
        Next
        Return result
    End Function

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 要检测冗余调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)。
                Lines.Clear()
            End If

            ' TODO: 释放未托管资源(未托管对象)并在以下内容中替代 Finalize()。
            ' TODO: 将大型字段设置为 null。
        End If
        disposedValue = True
    End Sub

    ' TODO: 仅当以上 Dispose(disposing As Boolean)拥有用于释放未托管资源的代码时才替代 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码以正确实现可释放模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
        Dispose(True)
        ' TODO: 如果在以上内容中替代了 Finalize()，则取消注释以下行。
        ' GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
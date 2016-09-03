''' <summary>
''' 提供由图像循迹生成绘图序列的对象
''' </summary>
Public Class SequenceManager
    ''' <summary>
    ''' 绘制序列的集合
    ''' </summary>
    Public SequenceList As List(Of PointSequence)

    Public Sub New(BolArr(,) As Integer)
        SequenceList = New List(Of PointSequence)
        CalculateSequence(BolArr)
    End Sub
    ''' <summary>
    ''' 创建新的序列
    ''' </summary>
    Private Sub CreateNewSequence()
        SequenceList.Add(New PointSequence)
    End Sub
    ''' <summary>
    ''' 添加新的位置
    ''' </summary>
    Private Sub AddPoint(point As PointF)
        SequenceList.Last.PointList.Add(point)
    End Sub
    Dim xArray() As Integer = {-1, 0, 1, 1, 1, 0, -1, -1}
    Dim yArray() As Integer = {-1, -1, -1, 0, 1, 1, 1, 0}
    Dim NewStart As Boolean
    ''' <summary>
    ''' 递归循迹
    ''' </summary>
    Private Sub CheckMove(ByRef BolArr(,) As Integer, ByVal x As Integer, ByVal y As Integer, ByVal StepNum As Integer)
        Application.DoEvents() '处理主线程消息
        If StepNum > 10000 Then Return
        Dim xBound As Integer = BolArr.GetUpperBound(0)
        Dim yBound As Integer = BolArr.GetUpperBound(1)
        Dim dx, dy As Integer
        Dim AroundValue As Integer = GetAroundValue(BolArr, x, y)
        If AroundValue > 2 AndAlso AroundValue < 8 Then
            Return
        End If
        For i = 0 To 7
            dx = x + xArray(i)
            dy = y + yArray(i)
            If Not (dx > 0 And dy > 0 And dx < xBound And dy < yBound) Then
                Return
            ElseIf BolArr(dx, dy) = 1 Then
                BolArr(dx, dy) = 0
                If NewStart = True Then
                    Me.CreateNewSequence()
                    Me.AddPoint(New PointF(dx, dy))
                    NewStart = False
                Else
                    Me.AddPoint(New PointF(dx, dy))
                End If
                CheckMove(BolArr, dx, dy, StepNum + 1)
                NewStart = True
            End If
        Next
    End Sub
    ''' <summary>
    ''' 计算序列
    ''' </summary>
    Private Sub CalculateSequence(BolArr(,) As Integer)
        Dim xCount As Integer = BolArr.GetUpperBound(0)
        Dim yCount As Integer = BolArr.GetUpperBound(1)
        Dim CP As New Point(xCount / 2, yCount / 2)
        Dim R As Integer = 0
        For R = 0 To If(xCount > yCount, xCount, yCount)
            For Theat = 0 To Math.PI * 2 Step 1 / R
                Dim dx As Integer = CP.X + R * Math.Cos(Theat)
                Dim dy As Integer = CP.Y + R * Math.Sin(Theat)
                If Not (dx > 0 And dy > 0 And dx < xCount And dy < yCount) Then Continue For
                If BolArr(dx, dy) = 1 Then
                    BolArr(dx, dy) = 0
                    Me.CreateNewSequence()
                    Me.AddPoint(New PointF(dx, dy))
                    CheckMove(BolArr, dx, dy, 0)
                    NewStart = True
                End If
            Next
        Next
    End Sub
    ''' <summary>
    ''' 返回指定像素位置的权值
    ''' </summary>
    Private Function GetAroundValue(ByRef BolArr(,) As Integer, ByVal x As Integer, ByVal y As Integer) As Integer
        Dim dx, dy, ResultValue As Integer
        Dim xBound As Integer = BolArr.GetUpperBound(0)
        Dim yBound As Integer = BolArr.GetUpperBound(1)
        For i = 0 To 7
            dx = x + xArray(i)
            dy = y + yArray(i)
            If dx > 0 And dy > 0 And dx < xBound And dy < yBound Then
                If BolArr(dx, dy) = 1 Then
                    ResultValue += 1
                End If
            End If
        Next
        Return ResultValue
    End Function
End Class

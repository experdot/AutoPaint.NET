Imports System.Numerics
Imports AutomaticDrawing.Core
Imports AutomaticDrawing.Utilities.Keyboard
''' <summary>
''' 提供鼠标控制的绘图器
''' </summary>
Public Class MousePainter
    Implements IPainter
    Public Event UpdatePaint As IPainter.UpdatePaintEventHandler Implements IPainter.UpdatePaint
    ''' <summary>
    ''' 鼠标事件间隔
    ''' </summary>
    Public Property SleepTime As Integer
    ''' <summary>
    ''' 绘制偏移
    ''' </summary>
    Public Property Offset As Vector2

    ''' <summary>
    ''' 创建并初始化一个实例
    ''' </summary>
    Public Sub New(offset As Vector2, Optional sleepTime As Integer = 3)
        Me.SleepTime = sleepTime
        Me.Offset = offset
    End Sub

    Public Sub Start(lines As List(Of ILine)) Implements IPainter.Start
        Dim totalCount As Integer = lines.SelectMany(Function(e As ILine)
                                                         Return e.Vertices
                                                     End Function).Count
        Dim current As Integer = 0
        For Each SubLine In lines
            If CheckKey() = False Then
                Return
            End If
            VirtualKeyboard.MouseMove(SubLine.Vertices.First.X + Offset.X, SubLine.Vertices.First.Y + Offset.Y, SleepTime)
            VirtualKeyboard.MouseDownOrUp(True, SleepTime)
            For Each SubPoint In SubLine.Vertices
                VirtualKeyboard.MouseMove(SubPoint.X + Offset.X, SubPoint.Y + Offset.Y, SleepTime)
                current += 1
                RaiseEvent UpdatePaint(Me, New UpdatePaintEventArgs(SubPoint, current / totalCount))
            Next
            VirtualKeyboard.MouseDownOrUp(False, SleepTime)
        Next
    End Sub
    Public Sub Pause() Implements IPainter.Pause
        Throw New NotImplementedException()
    End Sub
    Public Sub [Stop]() Implements IPainter.Stop
        Throw New NotImplementedException()
    End Sub

    Private Function CheckKey() As Boolean
        Dim key As Char = ChrW(VirtualKeyboard.GetActiveLetterKey())
        If key = StaticSource.HotKey_Pause Then
            Debug.WriteLine("Pause")
            Dim tempKey As Char
            While Not tempKey = StaticSource.HotKey_Continue
                tempKey = ChrW(VirtualKeyboard.GetActiveLetterKey())
                If tempKey = StaticSource.HotKey_Stop Then
                    Return False
                End If
                System.Threading.Thread.Sleep(10)
            End While
            Debug.WriteLine("Continue")
        ElseIf key = StaticSource.HotKey_Stop Then
            Debug.WriteLine("Stop")
            Return False
        End If
        Return True
    End Function
End Class

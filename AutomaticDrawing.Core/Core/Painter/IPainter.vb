''' <summary>
''' 绘制器接口
''' </summary>
Public Interface IPainter
    ''' <summary>
    ''' 绘制时发生的事件
    ''' </summary>
    Event UpdatePaint(sender As Object, e As UpdatePaintEventArgs)
    ''' <summary>
    ''' 开始
    ''' </summary>
    Sub Start(lines As List(Of ILine))
    ''' <summary>
    ''' 暂停
    ''' </summary>
    Sub Pause()
    ''' <summary>
    ''' 结束
    ''' </summary>
    Sub [Stop]()
End Interface

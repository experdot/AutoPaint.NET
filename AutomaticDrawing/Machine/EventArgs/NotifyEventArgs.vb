''' <summary>
''' 为消息通知事件提供数据
''' </summary>
Public Class NotifyEventArgs
    ''' <summary>
    ''' 级别
    ''' </summary>
    Public Property Level As NotifyLevels
    ''' <summary>
    ''' 文本
    ''' </summary>
    Public Property Text As String
    ''' <summary>
    ''' 时间
    ''' </summary>
    Public Property Time As Date

    ''' <summary>
    ''' 通过指定的文本或可选的消息级别创建并初始化消息通知类的实例
    ''' </summary>
    Public Sub New(text As String, Optional level As NotifyLevels = 0)
        Me.Text = text
        Me.Level = level
        Me.Time = Date.Now
    End Sub

    ''' <summary>
    ''' 消息级别枚举
    ''' </summary>
    Public Enum NotifyLevels
        Info = 0
        Warning = 1
        [Error] = 2
    End Enum
End Class

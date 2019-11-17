Public Class NotifyEventArgs
    Public Property Level As NotifyLevels
    Public Property Text As String
    Public Property Time As Date

    Public Sub New(text As String, Optional level As NotifyLevels = 0)
        Me.Text = text
        Me.Level = level
        Me.Time = Date.Now
    End Sub

    Public Enum NotifyLevels
        Info = 0
        Warning = 1
        [Error] = 2
    End Enum
End Class

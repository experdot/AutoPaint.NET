Imports System.Drawing
Imports System.Windows.Forms
''' <summary>
''' 绘制器
''' </summary>
Public Class Painter
    ''' <summary>
    ''' 预览更新时发生的事件
    ''' </summary>
    Public Event UpdatePreview(sender As Object, e As UpdatePreviewEventArgs)
    ''' <summary>
    ''' 绘图区域
    ''' </summary>
    Public Property Rect As Rectangle
    ''' <summary>
    ''' 鼠标事件间隔
    ''' </summary>
    Dim SleepTime As Integer = 3
    ''' <summary>
    ''' 绘制
    ''' </summary>
    Public Sub StartPaint(bmp As Bitmap, x As Integer, y As Integer)
        Me.Rect = New Rectangle(x, y, bmp.Width, bmp.Height)
        Painting(New SequenceAI(ColorHelper.GetPixelDataBools(bmp.GetPixelData)), Me.Rect)
    End Sub
    ''' <summary>
    ''' 预览
    ''' </summary>
    Public Sub StartPreview(bmp As Bitmap, view As Bitmap)
        Previewing(New SequenceAI(ColorHelper.GetPixelDataBools(BitmapHelper.GetPixelDataFromBitmap(bmp))), view)
    End Sub

    Private Sub Painting(sequence As SequenceAI, rect As Rectangle)
        For Each SubLine In sequence.Lines
            If CheckKey() = False Then
                Return
            End If
            VirtualKeyboard.MouseMove(SubLine.Vertices.First.X + rect.X, SubLine.Vertices.First.Y + rect.Y, SleepTime)
            VirtualKeyboard.MouseDownOrUp(True, SleepTime)
            For Each SubPoint In SubLine.Vertices
                VirtualKeyboard.MouseMove(SubPoint.X + rect.X, SubPoint.Y + rect.Y, SleepTime)
            Next
            VirtualKeyboard.MouseDownOrUp(False, SleepTime)
        Next
    End Sub
    Private Sub Previewing(sequence As SequenceAI, view As Bitmap)
        Static Rnd As New Random
        Dim tempColor As Color = Color.FromArgb(255, 0, 0, 0)
        Using pg As Graphics = Graphics.FromImage(view)
            pg.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            For Each SubSequence In sequence.Lines
                Dim TempR = Rnd.NextDouble * 255
                Dim TempG = Rnd.NextDouble * 255
                Dim TempB = Rnd.NextDouble * 255
                For Each SubPoint In SubSequence.Vertices
                    Dim Index As Single = SubSequence.Vertices.IndexOf(SubPoint)
                    Dim alpha As Integer = 255 - Index * 255 / SubSequence.Vertices.Count
                    If alpha < 1 Then alpha = 1
                    tempColor = Color.FromArgb(alpha, 0, 0, 0)
                    'tempColor = Color.FromArgb(alpha, TempR, TempG, TempB)
                    Dim Count As Single = SubSequence.Vertices.Count
                    'Dim penWidth As Single = 0.01 + (Count / 2 - Math.Abs(Index - Count / 2)) / 20
                    Dim penWidth As Single = 0.5 + Math.Abs(Index - Count) / 30
                    Dim maxWidth As Single = 2
                    If penWidth > maxWidth Then penWidth = maxWidth
                    If penWidth > 2 AndAlso CInt(Index) Mod (maxWidth + 2 - CInt(penWidth)) = 0 Then Continue For
                    Dim mypen As New Pen(tempColor, 1 + penWidth)
                    pg.DrawEllipse(mypen, New RectangleF(SubPoint.X - penWidth / 2, SubPoint.Y - penWidth / 2, penWidth, penWidth))
                    'pg.DrawLine(mypen, SubPoint, SubSequence.PointList.First)
                    RaiseEvent UpdatePreview(Me, New UpdatePreviewEventArgs(SubPoint))
                Next
            Next
        End Using
    End Sub

    Private Function CheckKey() As Boolean
        Dim key As Char = ChrW(VirtualKeyboard.GetActiveLetterKey())
        If key = "P" Then
            Debug.WriteLine("Pause")
            Dim tempKey As Char
            While Not tempKey = "R"
                tempKey = ChrW(VirtualKeyboard.GetActiveLetterKey())
                If tempKey = "S" Then
                    Return False
                End If
                System.Threading.Thread.Sleep(10)
            End While
            Debug.WriteLine("Run")
        ElseIf key = "S" Then
            Debug.WriteLine("Stop")
            Return False
        End If
        Return True
    End Function
End Class

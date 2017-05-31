Imports AutomaticDrawingLib

Public Class Form1

    Dim Machine As New Machine

    ''' <summary>
    ''' 窗体加载时
    ''' </summary>
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Machine = New Machine()
        AddHandler Machine.Painter.UpdatePreviewImage, AddressOf RefreshPicturebox2
    End Sub
    ''' <summary>
    ''' 复制屏幕
    ''' </summary>
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        TabControl1.SelectedIndex = 0
        Machine.Original = BitmapHelper.GetScreenImage(New Rectangle(TabPage1.PointToScreen(New Drawing.Point(0, 0)).X + 3, TabPage1.PointToScreen(New Drawing.Point(0, 0)).Y + 3, TabPage1.Width - 6, TabPage1.Height - 6))
        TabControl1.SelectedIndex = 1
        TrackBar_MouseUp(TrackBar1, e)
    End Sub
    ''' <summary>
    ''' 鼠标绘制
    ''' </summary>
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.Hide()
        Machine.Start(TabPage1.PointToScreen(New Drawing.Point(0, 0)).X + 3, TabPage1.PointToScreen(New Drawing.Point(0, 0)).Y + 3)
        Me.Show()
    End Sub
    ''' <summary>
    ''' 预览绘制
    ''' </summary>
    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        If Not Machine.Final Is Nothing Then
            TabControl1.SelectedIndex = 2
            Label1.Hide()
            Machine.Preview = New Bitmap(Machine.Final.Width, Machine.Final.Height)
            PictureBox2.Image = Machine.Preview
            Machine.Painter.StartPreview(Machine.Final, Machine.Preview）
            If CheckBox3.Checked Then
                Label1.Show()
            End If
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    ''' <summary>
    ''' 滑动按钮松开时
    ''' </summary>
    Private Sub TrackBar_MouseUp(sender As Object, e As MouseEventArgs) Handles TrackBar1.MouseUp, TrackBar2.MouseUp, TrackBar3.MouseUp
        If Not Machine.Original Is Nothing Then
            If sender Is TrackBar1 Then
                If CheckBox4.Checked Then
                    Machine.Current = BitmapHelper.GetThresholdImage(Machine.Original, sender.Value / 255, True)
                Else
                    Machine.Current = BitmapHelper.GetThresholdImage(Machine.Original, sender.Value)
                End If
                RadioButton1.Checked = True
            ElseIf sender Is TrackBar2 Then
                Dim temp = If(CheckBox5.Checked, BitmapHelper.GetLumpImage(Machine.Original, TrackBar3.Value + 1), Machine.Original)
                If CheckBox4.Checked Then
                    Machine.Current = BitmapHelper.GetOutLineImage(temp, sender.Value / 255 * Math.Sqrt(1), True)
                Else
                    Machine.Current = BitmapHelper.GetOutLineImage(temp, 255 - sender.Value)
                End If
                RadioButton2.Checked = True
            Else
                Machine.Current = BitmapHelper.GetLumpImage(Machine.Original, sender.Value + 1)
            End If
            ImageChanged()
        Else
            MsgBox("请先复制屏幕")
        End If
    End Sub
    ''' <summary>
    ''' 选择按钮状态改变时
    ''' </summary>
    Private Sub CheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged， CheckBox2.CheckedChanged
        ImageChanged()
    End Sub
    ''' <summary>
    ''' 单选按钮状态改变时
    ''' </summary>
    Private Sub RadioButton_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged
        ImageChanged()
    End Sub
    ''' <summary>
    ''' 签名
    ''' </summary>
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Label1.Text = TextBox1.Text
    End Sub
    ''' <summary>
    ''' 画布大小改变时
    ''' </summary>
    Private Sub PictureBox2_SizeChanged(sender As Object, e As EventArgs) Handles PictureBox2.SizeChanged
        ToolStripStatusLabel3.Text = "Size:" & PictureBox2.Width & "*" & PictureBox2.Height & "Pixel"
    End Sub
    ''' <summary>
    ''' 选项卡切换时
    ''' </summary>
    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If TabControl1.SelectedIndex = 2 Then
            Panel1.Hide()
        Else
            Panel1.Show()
        End If
    End Sub
    ''' <summary>
    ''' HSB颜色空间选择按钮状态改变时
    ''' </summary>
    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        TrackBar_MouseUp(If(RadioButton1.Checked, TrackBar1, TrackBar2), Nothing)
    End Sub

    ''' <summary>
    ''' 更新预览画布
    ''' </summary>
    Private Sub RefreshPicturebox2(ePoint As Vertex, ePen As Pen)
        Static Count As Integer = 0
        ToolStripStatusLabel2.Text = "Position:[" & ePoint.X & "," & ePoint.Y & "]"
        Count += 1
        If Count Mod 2 = 0 Then
            PictureBox2.Refresh()
            If Count > 1000 Then Count = 0
        End If
        Application.DoEvents()
    End Sub
    ''' <summary>
    ''' 对图像更改时调用该方法
    ''' </summary>
    Private Sub ImageChanged()
        If Machine.Current IsNot Nothing Then
            Machine.Final = New Bitmap(Machine.Current)
            If CheckBox1.Checked Then
                Machine.Final = BitmapHelper.GetAroundImage(Machine.Current)
            End If
            If CheckBox2.Checked Then
                Machine.Final = BitmapHelper.GetInvertImage(Machine.Final)
            End If
            PictureBox1.Image = Machine.Final
        End If
    End Sub

End Class

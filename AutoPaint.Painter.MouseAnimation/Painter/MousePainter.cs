using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Diagnostics;
using AutoPaint.Core;
using AutoPaint.Utilities;

namespace AutoPaint.Painter
{
    public class MousePainter : IPainter
    {
        public event EventHandler<UpdatePaintEventArgs> UpdatePaint;

        /// <summary>
        ///     ''' 鼠标事件间隔
        ///     ''' </summary>
        public int SleepTime { get; set; }
        /// <summary>
        ///     ''' 绘制偏移
        ///     ''' </summary>
        public Vector2 Offset { get; set; }
        /// <summary>
        ///     ''' 是否检测Ctrl+Alt组合键
        ///     ''' </summary>
        public bool IsCheckCtrlAlKey { get; set; } = true;

        /// <summary>
        ///     ''' 创建并初始化一个实例
        ///     ''' </summary>
        public MousePainter(Vector2 offset, int sleepTime = 3)
        {
            this.SleepTime = sleepTime;
            this.Offset = offset;
        }

        public void Start(List<ILine> lines)
        {
            int totalCount = lines.SelectMany(e => e.Vertices).Count();
            int current = 0;
            foreach (var SubLine in lines)
            {
                if (CheckKey() == false)
                    return;
                VirtualKeyboard.MouseMove((int)(SubLine.Vertices.First().X + Offset.X), (int)(SubLine.Vertices.First().Y + Offset.Y), SleepTime);
                VirtualKeyboard.MouseDownOrUp(true, SleepTime);
                foreach (var SubPoint in SubLine.Vertices)
                {
                    VirtualKeyboard.MouseMove((int)(SubPoint.X + Offset.X), (int)(SubPoint.Y + Offset.Y), SleepTime);
                    current += 1;
                    UpdatePaint?.Invoke(this, new UpdatePaintEventArgs(SubPoint, current / totalCount));
                }
                VirtualKeyboard.MouseDownOrUp(false, SleepTime);
            }
        }
        public void Pause()
        {
            throw new NotImplementedException();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }


        private bool CheckKey()
        {
            // if ((!IsCheckCtrlAlKey) || (My.Computer.Keyboard.CtrlKeyDown && My.Computer.Keyboard.AltKeyDown)) TODO
            if ((!IsCheckCtrlAlKey))
            {
                char key = (char)VirtualKeyboard.GetActiveLetterKey();
                if (key == StaticResources.HotKey_Pause)
                {
                    Debug.WriteLine("Pause");
                    char? tempKey = null;
                    while (tempKey != StaticResources.HotKey_Continue)
                    {
                        tempKey = (char)VirtualKeyboard.GetActiveLetterKey();
                        if (tempKey == StaticResources.HotKey_Stop)
                            return false;
                        System.Threading.Thread.Sleep(10);
                    }
                    Debug.WriteLine("Continue");
                }
                else if (key == StaticResources.HotKey_Stop)
                {
                    Debug.WriteLine("Stop");
                    return false;
                }
            }
            return true;
        }
    }

}

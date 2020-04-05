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
        public event EventHandler<OnPaintingUpdatedEventArgs> OnPaintingUpdated;

        public int SleepTime { get; set; }
        public Vector2 Offset { get; set; }
        public bool IsCheckCtrlAlKey { get; set; } = true;

        public MousePainter(Vector2 offset, int sleepTime = 3)
        {
            this.SleepTime = sleepTime;
            this.Offset = offset;
        }

        public void Start(IPainting painting)
        {
            var lines = painting.Lines;
            int totalCount = lines.SelectMany(e => e.Vertices).Count();
            int current = 0;
            foreach (var line in lines)
            {
                if (CheckKey() == false)
                    return;
                VirtualKeyboard.MouseMove((int)(line.Vertices.First().X + Offset.X), (int)(line.Vertices.First().Y + Offset.Y), SleepTime);
                VirtualKeyboard.MouseDown(SleepTime);
                foreach (var SubPoint in line.Vertices)
                {
                    VirtualKeyboard.MouseMove((int)(SubPoint.X + Offset.X), (int)(SubPoint.Y + Offset.Y), SleepTime);
                    current += 1;
                    OnPaintingUpdated?.Invoke(this, new OnPaintingUpdatedEventArgs(SubPoint, current / (float)totalCount));
                }
                VirtualKeyboard.MouseUp(SleepTime);
            }
        }
        public void Pause()
        {
            throw new NotImplementedException();
        }
        public void Resume()
        {
            throw new NotImplementedException();
        }
        public void Stop()
        {
            throw new NotImplementedException();
        }


        private bool CheckKey()
        {
            if ((!IsCheckCtrlAlKey) || (VirtualKeyboard.IsKeyDown(VirtualKeys.VK_CONTROL) && VirtualKeyboard.IsKeyDown(VirtualKeys.VK_MENU)))
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

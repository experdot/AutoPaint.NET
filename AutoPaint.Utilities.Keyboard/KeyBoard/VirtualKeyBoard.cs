using System;
using System.Collections.Generic;
using System.Text;

namespace AutoPaint.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualBasic;

    public class VirtualKeyboard
    {
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SetCursorPos(int x, int y);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int MapVirtualKey(int wCode, int wMapType);
        [System.Runtime.InteropServices.DllImport("user32 ")]
        private static extern int GetAsyncKeyState(int vKey);
        /// <summary>
        /// 鼠标左键按下或弹起
        /// </summary>
        public static void MouseDownOrUp(bool type, int interval)
        {
            if (type)
                mouse_event(0x2, 0, 0, 0, (int)IntPtr.Zero);
            else
                mouse_event(0x4, 0, 0, 0, (int)IntPtr.Zero);
            System.Threading.Thread.Sleep(interval);
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        public static void MouseMove(int x, int y, int interval)
        {
            SetCursorPos(x, y);
            System.Threading.Thread.Sleep(interval);
        }
        /// <summary>
        /// 发送一组按键
        /// </summary>
        public static void SendString(string str, int interval)
        {
            string[] strArr = str.Split(',');
            foreach (var SubStr in strArr)
            {
                if (SubStr.First() == '#')
                    System.Threading.Thread.Sleep(System.Convert.ToInt32(SubStr.Substring(1)));
                else
                    foreach (char SubChar in SubStr)
                    {
                        VirtualKeyDown((VirtualKeys)SubChar);
                        System.Threading.Thread.Sleep(interval);
                        VirtualKeyUp((VirtualKeys)SubChar);
                    }
            }
        }
        /// <summary>
        /// 发送一组扩展的按键
        /// </summary>
        public static void SendStringEx(string str, int interval, params VirtualKeys[] extra)
        {
            foreach (var SubKey in extra)
                VirtualKeyDown(SubKey);
            SendString(str, interval);
            foreach (var SubKey in extra)
                VirtualKeyUp(SubKey);
        }
        /// <summary>
        /// 发送单个按键
        /// </summary>
        public static void SendKey(VirtualKeys vKey, int interval)
        {
            VirtualKeyDown(vKey);
            System.Threading.Thread.Sleep(interval);
            VirtualKeyUp(vKey);
        }
        /// <summary>
        /// 同时发送两个按键
        /// </summary>
        public static void SendCouple(VirtualKeys vKey1, VirtualKeys vKey2, int interval)
        {
            VirtualKeyDown(vKey1);
            VirtualKeyDown(vKey2);
            System.Threading.Thread.Sleep(interval);
            VirtualKeyUp(vKey1);
            VirtualKeyUp(vKey2);
        }
        /// <summary>
        /// 获取A~Z的按键状态
        /// </summary>
        public static int GetActiveLetterKey()
        {
            for (var i = 65; i <= 90; i++) // A~Z的ASICC码
            {
                if (CurrentKeyState(i) == true)
                    return i;
            }
            return 0;
        }

        private static bool[] KeyState = new bool[256];
        /// <summary>
        /// 获取键盘按键状态
        /// </summary>
        private static bool CurrentKeyState(int KeyCode)
        {
            int temp = GetAsyncKeyState(KeyCode);
            if (temp == 0)
                KeyState[KeyCode] = false;
            else
            {
                if (KeyState[KeyCode] == false)
                {
                    KeyState[KeyCode] = true;
                    return true;
                }
                KeyState[KeyCode] = true;
                return false;
            }
            return false;
        }
        /// <summary>
        /// 按下指定按键
        /// </summary>
        private static void VirtualKeyDown(VirtualKeys vKey)
        {
            keybd_event((byte)vKey, (byte)MapVirtualKey((int)vKey, 0), 0x1 | 0, 0); // 按下
        }
        /// <summary>
        /// 松开指定按键
        /// </summary>
        private static void VirtualKeyUp(VirtualKeys vKey)
        {
            keybd_event((byte)vKey, (byte)MapVirtualKey((int)vKey, 0), 0x1 | 0x2, 0); // 弹起
        }
    }
}

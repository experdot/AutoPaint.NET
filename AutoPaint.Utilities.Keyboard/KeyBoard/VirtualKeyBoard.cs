using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AutoPaint.Utilities
{
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

        public static void MouseDown(int interval)
        {
            mouse_event(0x2, 0, 0, 0, (int)IntPtr.Zero);
            System.Threading.Thread.Sleep(interval);
        }

        public static void MouseUp(int interval)
        {
            mouse_event(0x4, 0, 0, 0, (int)IntPtr.Zero);
            System.Threading.Thread.Sleep(interval);
        }

        public static void MouseMove(int x, int y, int interval)
        {
            SetCursorPos(x, y);
            System.Threading.Thread.Sleep(interval);
        }

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

        public static void SendStringEx(string str, int interval, params VirtualKeys[] extra)
        {
            foreach (var SubKey in extra)
                VirtualKeyDown(SubKey);
            SendString(str, interval);
            foreach (var SubKey in extra)
                VirtualKeyUp(SubKey);
        }

        public static void SendKey(VirtualKeys vKey, int interval)
        {
            VirtualKeyDown(vKey);
            System.Threading.Thread.Sleep(interval);
            VirtualKeyUp(vKey);
        }

        public static void SendCouple(VirtualKeys vKey1, VirtualKeys vKey2, int interval)
        {
            VirtualKeyDown(vKey1);
            VirtualKeyDown(vKey2);
            System.Threading.Thread.Sleep(interval);
            VirtualKeyUp(vKey1);
            VirtualKeyUp(vKey2);
        }

        public static bool IsKeyDown(VirtualKeys vKey)
        {
            return CurrentKeyState((byte)vKey);
        }

        public static int GetActiveLetterKey()
        {
            for (var i = 65; i <= 90; i++)
            {
                if (CurrentKeyState(i) == true)
                    return i;
            }
            return 0;
        }

        private static readonly bool[] _keyState = new bool[256];
        private static bool CurrentKeyState(int KeyCode)
        {
            int temp = GetAsyncKeyState(KeyCode);
            if (temp == 0)
                _keyState[KeyCode] = false;
            else
            {
                if (_keyState[KeyCode] == false)
                {
                    _keyState[KeyCode] = true;
                    return true;
                }
                _keyState[KeyCode] = true;
                return false;
            }
            return false;
        }

        private static void VirtualKeyDown(VirtualKeys vKey)
        {
            keybd_event((byte)vKey, (byte)MapVirtualKey((int)vKey, 0), 0x1 | 0, 0);
        }

        private static void VirtualKeyUp(VirtualKeys vKey)
        {
            keybd_event((byte)vKey, (byte)MapVirtualKey((int)vKey, 0), 0x1 | 0x2, 0);
        }
    }
}

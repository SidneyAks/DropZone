﻿using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace GlobalLowLevelHooks
{
    /// <summary>
    /// Class for intercepting low level Windows mouse hooks.
    /// </summary>
    public class MouseHook
    {
        /// <summary>
        /// Internal callback processing function
        /// </summary>
        private delegate IntPtr MouseHookHandler(int nCode, IntPtr wParam, IntPtr lParam);
        private MouseHookHandler hookHandler;

        /// <summary>
        /// Function to be called when defined even occurs
        /// </summary>
        /// <param name="mouseStruct">MSLLHOOKSTRUCT mouse structure</param>
        public delegate void MouseHookCallback(MSLLHOOKSTRUCT mouseStruct);

        #region Events
        public event MouseHookCallback LeftButtonDown { add => EventLookup[MouseMessages.WM_LBUTTONDOWN] += value;  remove => EventLookup[MouseMessages.WM_LBUTTONDOWN] -= value; }
        public event MouseHookCallback LeftButtonUp { add => EventLookup[MouseMessages.WM_LBUTTONUP] += value; remove => EventLookup[MouseMessages.WM_LBUTTONUP] -= value; }
        public event MouseHookCallback RightButtonDown { add => EventLookup[MouseMessages.WM_RBUTTONDOWN] += value; remove => EventLookup[MouseMessages.WM_RBUTTONDOWN] -= value; }
        public event MouseHookCallback RightButtonUp { add => EventLookup[MouseMessages.WM_RBUTTONUP] += value; remove => EventLookup[MouseMessages.WM_RBUTTONUP] -= value; }
        public event MouseHookCallback MouseMove { add => EventLookup[MouseMessages.WM_MOUSEMOVE] += value; remove => EventLookup[MouseMessages.WM_MOUSEMOVE] -= value; }
        public event MouseHookCallback MouseWheel { add => EventLookup[MouseMessages.WM_MOUSEWHEEL] += value; remove => EventLookup[MouseMessages.WM_MOUSEWHEEL] -= value; }
        public event MouseHookCallback DoubleClick { add => EventLookup[MouseMessages.WM_LBUTTONDBLCLK] += value; remove => EventLookup[MouseMessages.WM_LBUTTONDBLCLK] -= value; }
        public event MouseHookCallback MiddleButtonDown { add => EventLookup[MouseMessages.WM_MBUTTONDOWN] += value; remove => EventLookup[MouseMessages.WM_MBUTTONDOWN] -= value; }
        public event MouseHookCallback MiddleButtonUp { add => EventLookup[MouseMessages.WM_MBUTTONUP] += value; remove => EventLookup[MouseMessages.WM_MBUTTONUP] -= value; }

        private Dictionary<MouseMessages, MouseHookCallback> EventLookup = Enum.GetValues(typeof(MouseMessages)).OfType<MouseMessages>().ToDictionary(k => k, v => (MouseHookCallback)null);
        
        public void ResetEvents()
        {
            EventLookup = Enum.GetValues(typeof(MouseMessages)).OfType<MouseMessages>().ToDictionary(k => k, v => (MouseHookCallback)null);
        }
        #endregion

        /// <summary>
        /// Low level mouse hook's ID
        /// </summary>
        private IntPtr hookID = IntPtr.Zero;

        /// <summary>
        /// Install low level mouse hook
        /// </summary>
        /// <param name="mouseHookCallbackFunc">Callback function</param>
        public void Install()
        {
            hookHandler = HookFunc;
            hookID = SetHook(hookHandler);
        }

        /// <summary>
        /// Remove low level mouse hook
        /// </summary>
        public void Uninstall()
        {
            if (hookID == IntPtr.Zero)
                return;

            UnhookWindowsHookEx(hookID);
            hookID = IntPtr.Zero;
        }

        /// <summary>
        /// Destructor. Unhook current hook
        /// </summary>
        ~MouseHook()
        {
            Uninstall();
        }

        /// <summary>
        /// Sets hook and assigns its ID for tracking
        /// </summary>
        /// <param name="proc">Internal callback function</param>
        /// <returns>Hook ID</returns>
        private IntPtr SetHook(MouseHookHandler proc)
        {
            using (ProcessModule module = Process.GetCurrentProcess().MainModule)
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(module.ModuleName), 0);
        }

        /// <summary>
        /// Callback function
        /// </summary>
        private IntPtr HookFunc(int nCode, IntPtr wParam, IntPtr lParam)
        {
#if DEBUG
            if ((MouseMessages)wParam != MouseMessages.WM_MOUSEMOVE && (MouseMessages)wParam != MouseMessages.WM_MOUSEWHEEL)
            {
                Console.WriteLine($"Mouse Recieved -- wparam {wParam} lparam {lParam}");
            }
#endif
            // parse system messages
            if (nCode >= 0 && (MouseMessages)wParam != MouseMessages.WM_MOUSEWHEEL)
            {
                EventLookup[(MouseMessages)wParam]?.Invoke(((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))));
            }
            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

#region WinAPI
        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_LBUTTONDBLCLK = 0x0203,
            WM_MBUTTONDOWN = 0x0207,
            WM_MBUTTONUP = 0x0208
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            MouseHookHandler lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
#endregion
    }
}

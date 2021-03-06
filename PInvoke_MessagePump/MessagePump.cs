﻿using System;
using System.Diagnostics;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace MessagePump
{
    public static class Pump
    {
        private static class User32Wrapper
        {
            // GetMessage
            [DllImport(@"user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
            public static extern bool GetMessage(ref MSG message, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);
            [DllImport(@"user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
            public static extern bool TranslateMessage(ref MSG message);
            [DllImport(@"user32.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
            public static extern long DispatchMessage(ref MSG message);

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                long x;
                long y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MSG
            {
                IntPtr hwnd;
                public uint message;
                public UIntPtr wParam;
                public IntPtr lParam;
                public uint time;
                public POINT pt;
            }
        }

        private static int? ExitCode;

        public static int Start(Action<String, Exception> ErrorHandler)
        {
            User32Wrapper.MSG msg = new User32Wrapper.MSG();
            while (!ExitCode.HasValue && User32Wrapper.GetMessage(ref msg, IntPtr.Zero, 0x100, 0x020E))
            {
                try 
                {
                    User32Wrapper.TranslateMessage(ref msg);
                    User32Wrapper.DispatchMessage(ref msg);
                    System.Threading.Thread.Sleep(10);
                } catch (Exception ex)
                {
                    ErrorHandler($"Caught exception for msg {msg.message}, wparam={msg.wParam}, lparam={msg.lParam}, tine={msg.time} pt={msg.pt}", ex);
                }
            }
            return ExitCode.Value;
        }

        public static void Quit(int exitcode = 0)
        {
            ExitCode = exitcode;
        }
    }
}

using GDIWindow;
using GDIWindow.Win32Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemTray
{
    public class TrayIcon
    {

        [DllImport("shell32.dll", SetLastError = true)]
        static extern bool Shell_NotifyIcon(NIM dwMessage, [In] ref NotifyIconData pnid);

        public enum NIM : int
        {
            ADD = 0x00000000,
            MODIFY = 0x00000001,
            DELETE = 0x00000002,
            SETFOCUS = 0x00000003,
            SETVERSION = 0x00000004,
        }

        [Flags]
        public enum NIF
        {
            MESSAGE = 0x01,
            ICON = 0x02,
            TIP = 0x04,
            STATE = 0x08,
            INFO = 0x10,
            GUID = 0x20,
            REALTIME = 0x40,
            SHOWTIP = 0x80,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NotifyIconData
        {
            public System.Int32 cbSize; // DWORD
            public System.IntPtr hWnd; // HWND
            public System.Int32 uID; // UINT
            public NIF uFlags; // UINT
            public uint uCallbackMessage; // UINT
            public System.IntPtr hIcon; // HICON
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public System.String szTip; // char[128]
            public System.Int32 dwState; // DWORD
            public System.Int32 dwStateMask; // DWORD
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public System.String szInfo; // char[256]
            public System.Int32 uTimeoutOrVersion; // UINT
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public System.String szInfoTitle; // char[64]
            public System.Int32 dwInfoFlags; // DWORD
        }

        public enum MouseMessages
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

        public Dictionary<MouseMessages, EventHandler> Events = Enum.GetValues(typeof(MouseMessages)).OfType<MouseMessages>().ToDictionary(k => k, v => (EventHandler)null);//new Dictionary<MouseMessages, EventHandler>();

        private Window stHelperWindow;

        private GDIWindow.Win32Delegates.WndProc serviceFunc => _serviceFunc ?? (_serviceFunc = (GDIWindow.Win32Delegates.WndProc)((hWnd, message, wParam, lParam) =>
        {
            if (message == (uint)MouseMessageForTrayIcon)
            {
                if (Events.ContainsKey((MouseMessages)lParam))
                    Events[(MouseMessages)lParam]?.Invoke(null, null);
            }
            else if (message == (uint)WM.CLOSE)
            {
                Remove();
            }
            return GDIWindow.WinApi.WinAPI.DefWindowProc(hWnd, (GDIWindow.Win32Enums.WM)message, wParam, lParam);
        }));
        private GDIWindow.Win32Delegates.WndProc _serviceFunc;

        public uint MouseMessageForTrayIcon;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern uint RegisterWindowMessage(string lpString);

        public int ID;

        private NotifyIconData data;

        public TrayIcon(IntPtr Parent, string ToolTip, IntPtr iconPtr)
        {
            stHelperWindow = new Window("ToolTipWindow", serviceFunc, IntPtr.Zero, hidden: true, delayShow: true)
            {
                Top = 5,
                Left = 5,
                Width = 20,
                Height = 20,
                Style = GDIWindow.Win32Enums.WindowStylesEx.WS_EX_VisualStudioEmulation,
                ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost,
                Transparency = 128
            };

            stHelperWindow.DelayedShow();

            data = new NotifyIconData()
            {
                uID = ID = new Random().Next(),
                hWnd = stHelperWindow.Handle,
                hIcon = iconPtr,
                uFlags = NIF.ICON | NIF.MESSAGE | NIF.TIP,// | NIF.INFO,
                szTip = "ToolTip",
                uCallbackMessage = (MouseMessageForTrayIcon = RegisterWindowMessage("MouseMessageForTrayIcon"))
            };
            data.cbSize = Marshal.SizeOf(data);


            Shell_NotifyIcon(NIM.ADD, ref data);
        }

        public void Remove()
        {
            Shell_NotifyIcon(NIM.DELETE, ref data);
        }

        public void PostInfo(string title, string text)
        {
            data.szInfoTitle = title;
            data.szInfo = text;
            data.uFlags = NIF.INFO;
            Shell_NotifyIcon(NIM.MODIFY, ref data);

        }
    }
}

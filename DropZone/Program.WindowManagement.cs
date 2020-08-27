using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;

namespace DropZone
{
    public partial class Program
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(GlobalLowLevelHooks.MouseHook.POINT Point);

        public static WindowRef GetWindowDetailsFromPoint(GlobalLowLevelHooks.MouseHook.POINT Point)
        {
            var hWnd = WindowFromPoint(Point);
            return new WindowRef(hWnd);
        }

        public class WindowRef
        {
            public WindowRef(IntPtr AppHandle)
            {
                Handle = AppHandle;
            }

            public readonly IntPtr Handle;

            public string Title
            {
                get
                {
                    return GetTitle(Handle);
                }
            }

            public List<string> TitleTree 
            { 
                get
                {
                    var titles = new List<string>();
                    var hwnd = Handle;
                    while (hwnd != IntPtr.Zero)
                    {
                        titles.Add(GetTitle(hwnd));
                        hwnd = GetParent(hwnd);
                    }
                    return titles;
                }
            }

            public IntPtr ThreadProcessID
            {
                get
                {
                    GetWindowThreadProcessId(Handle, out var id);
                    return id;
                }
            }

            public string Filename
            {
                get
                {
                    return Process.GetProcessById((int)ThreadProcessID).MainModule.FileName;
                }
            }

            public WindowRef Parent
            {
                get
                {
                    return new WindowRef(GetParent(Handle));
                }
            }

            public RECT rect
            { 
                get
                {
                    GetWindowRect(Handle, out var r);
                    return r;
                }
            }

            public int TitleBarHeight => GetSystemMetrics(/*SM_CYSIZE = */31);

            public bool IsPartOfWindowsUI
            {
                get
                {
                    var desktopwindow = new WindowRef(GetShellWindow());
                    var result = (desktopwindow.ThreadProcessID == ThreadProcessID && !TitleTree.Contains("File Explorer"));
                    return result;
                }
            }

            public void MoveTo(int left, int top, int right, int bottom)
            {
                var wp = new WINDOWPLACEMENT()
                {
                    length = Marshal.SizeOf(typeof(WINDOWPLACEMENT)),
                    flags = 0,
                    showCmd = 1,
                    rcNormalPosition = new System.Drawing.Rectangle(left, top, right, bottom)
                };
                SetWindowPlacement(Handle, ref wp);

                var rect = this.rect;

                if (Parent.Handle != IntPtr.Zero && (left != rect.Left || top != rect.Top || right != rect.Right || bottom != rect.Bottom))
                {
                    Parent.MoveTo(left, top, right, bottom);
                }
            }

            private static string GetTitle(IntPtr hwnd)
            {
                var length = GetWindowTextLength(hwnd) + 1;
                var title = new StringBuilder(length);
                GetWindowText(hwnd, title, length);
                return title.ToString();
            }


            #region Gross Pinvoke stuff

            [StructLayout(LayoutKind.Sequential)]
            private struct WINDOWPLACEMENT
            {
                public int length;
                public int flags;
                public int showCmd;
                public System.Drawing.Point ptMinPosition;
                public System.Drawing.Point ptMaxPosition;
                public System.Drawing.Rectangle rcNormalPosition;
            }

            [DllImport("user32.dll")]
            private static extern int GetSystemMetrics(int smIndex);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
            private static extern IntPtr GetParent(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

            [DllImport("user32.dll", SetLastError = true)]
            private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

            private static int GWL_STYLE = -16;
            [DllImport("USER32.DLL")]
            public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            private static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = false)]
            private static extern IntPtr GetShellWindow();
            #endregion
        }
    }
}

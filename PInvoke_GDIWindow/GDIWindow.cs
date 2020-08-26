using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GDIWindow.Win32Enums;
using GDIWindow.Win32Structs;
using GDIWindow.WinApi;

namespace GDIWindow
{
    public class Window
    {
        const uint TransparencyKey = 0x000000;
        private readonly WNDCLASSEX wnd;
        public readonly IntPtr Handle;

        public int Width
        {
            get => width;
            set 
            {
                width = value;
                updatePos();
            }
        }
        private int width;

        public int Height
        {
            get => height;
            set
            {
                height = value;
                updatePos();
            }
        }
        private int height;

        public int Top
        {
            get => top;
            set
            {
                top = value;
                updatePos();
            }
        }
        private int top;

        public int Left
        {
            get => left;
            set
            {
                left = value;
                updatePos();
            }
        }
        private int left;

        public Win32Enums.HWNDPosStates ZPos
        {
            get => zPos;
            set
            {
                zPos = value;
                updatePos();
            }
        }
        private Win32Enums.HWNDPosStates zPos;

        private void updatePos()
        {
            WinAPI.SetWindowPos(Handle, ZPos,
                    Left, Top,
                    this.Width, this.Height,
                    0);
        }

        public byte Transparency
        {
            get => transparency;
            set
            {
                transparency = value;
                if (!Hidden)
                {
                    WinAPI.SetLayeredWindowAttributes(Handle, TransparencyKey, value, Win32Enums.LayeredWindowFlags.ALPHA);
                }
                else
                {
                    WinAPI.SetLayeredWindowAttributes(Handle, TransparencyKey, 0, Win32Enums.LayeredWindowFlags.ALPHA);

                }
            }
        }
        private byte transparency;

        public Win32Enums.WindowStylesEx Style
        {
            get => (Win32Enums.WindowStylesEx)WinAPI.GetWindowLongPtr(Handle, Win32Enums.GWL.GWL_EXSTYLE);
            set => WinAPI.SetWindowLongPtr(Handle, Win32Enums.GWL.GWL_EXSTYLE, (IntPtr)((int)value));
        }

        public bool Hidden;

        public Window(string WindowName, GDIWindow.Win32Delegates.WndProc WndProcFunction, IntPtr parentHandle, bool hidden = false, bool delayShow = true, WindowStylesEx style = default)
        {
            Hidden = hidden;

            wnd = new Win32Structs.WNDCLASSEX();
            wnd.cbSize = Marshal.SizeOf(typeof(Win32Structs.WNDCLASSEX));
            wnd.style = (int)(Win32Enums.ClassStyles.HorizontalRedraw | Win32Enums.ClassStyles.VerticalRedraw);
            wnd.hInstance = parentHandle;
            wnd.hCursor = WinAPI.LoadCursor(IntPtr.Zero, (int)Win32Enums.Win32_IDC_Constants.IDC_ARROW);
            wnd.hbrBackground = WinAPI.GetStockObject(Win32Enums.StockObjects.NULL_BRUSH);
            wnd.lpszMenuName = null;
            wnd.lpszClassName = WindowName;

            wnd.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WndProcFunction);

            Handle = WinAPI.CreateWindowEx2(
                0,
                WinAPI.RegisterClassEx2(ref wnd),
                WindowName,
                style,
                0, //x
                0, //y
                0, //width
                0, //height
                IntPtr.Zero,
                IntPtr.Zero,
                wnd.hInstance,
                IntPtr.Zero);

            transparency = 0;
            if (!delayShow){
                DelayedShow();
            }
        }

        private bool shown = false;

        public void DelayedShow()
        {
            if (!shown)
            {
                WinAPI.UpdateWindow(Handle);
                WinAPI.ShowWindow(Handle, Win32Enums.ShowWindowCommands.Show);
                WinAPI.UpdateWindow(Handle);
            }
            shown = true;
        }


        public void Show()
        {
            WinAPI.SetLayeredWindowAttributes(Handle, TransparencyKey, transparency, Win32Enums.LayeredWindowFlags.COLORKEY | Win32Enums.LayeredWindowFlags.ALPHA);
            Hidden = false;
            UpdateWindow();
        }

        public void Hide()
        {
            WinAPI.SetLayeredWindowAttributes(Handle, TransparencyKey, 0, Win32Enums.LayeredWindowFlags.COLORKEY | Win32Enums.LayeredWindowFlags.ALPHA);
            Hidden = true;
            UpdateWindow();
        }

        public void UpdateWindow()
        {
            WinAPI.UpdateWindow(Handle);
            WinAPI.RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, Win32Enums.RedrawWindowFlags.Invalidate | Win32Enums.RedrawWindowFlags.EraseNow);
        }
    }
}

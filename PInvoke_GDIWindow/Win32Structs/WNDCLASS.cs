using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GDIWindow.Win32Structs
{
    // http://www.pinvoke.net/default.aspx/Structures.WNDCLASS  
    [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct WNDCLASS
    {
        public Win32Enums.ClassStyles style;
        [MarshalAs(UnmanagedType.FunctionPtr)]
        public Win32Delegates.WndProc lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public IntPtr hInstance;
        public IntPtr hIcon;
        public IntPtr hCursor;
        public IntPtr hbrBackground;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszMenuName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpszClassName;
    }
}

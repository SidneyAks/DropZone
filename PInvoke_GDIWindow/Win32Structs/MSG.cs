using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GDIWindow.Win32Structs
{
    [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 8)]
    public struct MSG
    {
        public IntPtr hwnd;
        public UInt32 message;
        public UIntPtr wParam;
        public UIntPtr lParam;
        public UInt32 time;
        public POINT pt;
    }
}

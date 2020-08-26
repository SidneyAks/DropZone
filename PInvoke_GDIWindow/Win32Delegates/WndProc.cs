using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDIWindow.Win32Delegates
{
    // http://www.pinvoke.net/default.aspx/user32/WndProcDelegate.html  
    public delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
}

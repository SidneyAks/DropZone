using GDIWindow.Win32Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using ZoneRenderer;

namespace DropZone
{
    public partial class Program
    {
        static IZoneRenderer renderer;

        public static LayoutCollection LayoutCollection;
        [STAThread]
        public static int StartCore()
        {
            if (PriorProcess != null)
            {
                PriorProcess.Kill();
            }

            IZoneRenderer.ActiveZoneColor = Color.FromArgb(Int32.Parse(DropZone.Settings.ActiveColor.TrimStart('#'), System.Globalization.NumberStyles.HexNumber));
            IZoneRenderer.BackgroundOpacity = DropZone.Settings.BackgroundOpacity;
            IZoneRenderer.LabelColor = Color.FromArgb(Int32.Parse(DropZone.Settings.LabelColor.TrimStart('#'), System.Globalization.NumberStyles.HexNumber));

            var dic = ScreenInfo.GetDisplays();
            LayoutCollection = DropZone.Settings.Zones;
            renderer = new ZoneRenderer.GDI.GDIZone(LayoutCollection.ActiveLayout);

            var t = new SystemTray.TrayIcon(Process.GetCurrentProcess().Handle, "Exit Drop Zone", GetDropzoneIcon().GetHicon());
            t.Events[SystemTray.TrayIcon.MouseMessages.WM_LBUTTONDOWN] += (s, e) => {
                MessagePump.Pump.Quit();
            };
            t.Events[SystemTray.TrayIcon.MouseMessages.WM_LBUTTONDBLCLK] += (s, e) =>
            {
                MessagePump.Pump.Quit();
            };

            t.PostInfo("DropZone Running", "Dropzone will run in the background, click the system try icon to close the process");
#if DEBUG
            ShowConsole();
#else
            HideConsole();
#endif
            RegisterInputHooks();
            return MessagePump.Pump.Start((str, ex) => {
                t.PostInfo("DropZone Error", str);
            });
        }

        private static Bitmap GetDropzoneIcon()
        {
            var bmp = new Bitmap(32, 32);
            var g = Graphics.FromImage(bmp);

            foreach (var i in Enumerable.Range(0, 5))
            {
                var layerWidth = 6;
                var ptOffset = (layerWidth/2) * i;
                var rdOffset = 32 - ((layerWidth/2) * i) - ptOffset;
                g.FillEllipse(i % 2 == 1 ? Brushes.Red : Brushes.White, ptOffset, ptOffset, rdOffset, rdOffset);
            }



            return bmp;
        }

        public class ContextMenu
        {
            private GDIWindow.Win32Delegates.WndProc serviceFunc => _serviceFunc ?? (_serviceFunc = (GDIWindow.Win32Delegates.WndProc)((hWnd, message, wParam, lParam) =>
            {
                if ((GDIWindow.Win32Enums.WM)message == GDIWindow.Win32Enums.WM.PAINT)
                {
                    IntPtr hdc;
                    GDIWindow.Win32Structs.PAINTSTRUCT ps;
                    GDIWindow.Win32Structs.RECT rect;

                    hdc = GDIWindow.WinApi.WinAPI.BeginPaint(hWnd, out ps);
                    GDIWindow.WinApi.WinAPI.GetClientRect(hWnd, out rect);

                    using (var g = System.Drawing.Graphics.FromHwnd(hWnd))
                    {
                        g.FillRectangle(new SolidBrush(System.Drawing.SystemColors.Window), rect.Left, rect.Top, rect.Width, rect.Height);
                        var i = 0;
                        foreach (var str in a.Keys)
                        {
                            var h = g.MeasureString(str, System.Drawing.SystemFonts.DefaultFont);
                            g.DrawString(str, System.Drawing.SystemFonts.DefaultFont,Brushes.Black, new Point(2, 2 + (int)(h.Height * i)));
                            i++;
                        }
                    }

                    GDIWindow.WinApi.WinAPI.EndPaint(hWnd, ref ps);
                    return IntPtr.Zero;
                }

                return GDIWindow.WinApi.WinAPI.DefWindowProc(hWnd, (GDIWindow.Win32Enums.WM)message, wParam, lParam);
            }));
            private GDIWindow.Win32Delegates.WndProc _serviceFunc;

            private Dictionary<string, Action> a;

            public ContextMenu(int x, int y, Dictionary<string, Action> actions)
            {
                a = actions;
                var window = new GDIWindow.Window("Context", serviceFunc, IntPtr.Zero, style: WindowStylesEx.WS_EX_ContextMenu, delayShow: false)
                {
                    Style = WindowStylesEx.WS_EX_ContextMenu,
                    Transparency = 255,
                    ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost
                };
                using (var image = new Bitmap(1, 1))
                {
                    using (var g = Graphics.FromImage(image))
                    {
                        window.Width = actions.Keys.Max(i => (int)g.MeasureString(i, System.Drawing.SystemFonts.DefaultFont).Width);
                        window.Height = actions.Keys.Sum(i => (int)g.MeasureString(i, System.Drawing.SystemFonts.DefaultFont).Height)
                             + GDIWindow.WinApi.WinAPI.GetSystemMetrics(GDIWindow.Win32Enums.SystemMetric.SM_CYCAPTION) + 15;
                    }
                }

            }
        }

        public static Process PriorProcess
        {
            get
            {
                Process curr = Process.GetCurrentProcess();
                Process[] procs = Process.GetProcessesByName(curr.ProcessName);
                foreach (Process p in procs)
                {
                    if ((p.Id != curr.Id) &&
                        (p.MainModule.FileName == curr.MainModule.FileName))
                        return p;
                }
                return null;
            }
        }
    }
}

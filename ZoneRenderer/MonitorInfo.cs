using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace ZoneRenderer
{
    public static class ScreenInfo
    {
        struct Size
        {
            public Size(int l, int w)
            {
                Length = l;
                Width = w;
            }

            public int Length { get; set; }
            public int Width { get; set; }
        }

        [DllImport("user32.dll")]
        static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        // size of a device name string
        private const int CCHDEVICENAME = 32;

        [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct MonitorInfo
        {
            public uint Size;
            public Rect Monitor;
            public Rect WorkArea;
            public uint Flags;

            public void Init()
            {
                this.Size = 40 + 2 * CCHDEVICENAME;
            }

            public MonitorInfo Scale(float scale, int offsetX, int offsetY)
            {
                return new MonitorInfo()
                {
                    Size = this.Size,
                    Monitor = this.Monitor.Scale(scale, offsetX, offsetY),
                    WorkArea = this.WorkArea.Scale(scale, offsetX, offsetY),
                    Flags = this.Flags,
                };
            }
        }

        [StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rect Scale(float scale, int offsetX, int offsetY)
            {
                return new Rect()
                {
                    Left = offsetX + (int)(Left * scale),
                    Top = offsetY + (int)(Top * scale),
                    Right = offsetX + (int)(Right * scale),
                    Bottom = offsetY + (int)(Bottom * scale),
                };
            }
        }

        /// <summary>
        /// The struct that contains the display information
        /// </summary>
        [DebuggerDisplay("{WorkArea.Left},{WorkArea.Top},{WorkArea.Right},{WorkArea.Bottom}")]
        public class DisplayInfo
        {
            public string Availability { get; set; }
            public string ScreenHeight { get; set; }
            public string ScreenWidth { get; set; }
            public Rect MonitorArea { get; set; }
            public Rect WorkArea { get; set; }
        }

        /// <summary>
        /// Collection of display information
        /// </summary>
        public class DisplayInfoCollection : List<DisplayInfo>
        {
            public int MaxWidth => this.Max(x => x.WorkArea.Right);
            public int MaxHeight => this.Max(x => x.WorkArea.Bottom);
        }


        /// <summary>
        /// Returns the number of Displays using the Win32 functions
        /// </summary>
        /// <returns>collection of Display Info</returns>
        public static DisplayInfoCollection GetDisplays(float? scale = null, int? offsetX = null, int? offsetY = null)
        {
            DisplayInfoCollection col = new DisplayInfoCollection();

            EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
                delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
                {
                    MonitorInfo mi = new MonitorInfo();
                    mi.Size = (uint)Marshal.SizeOf(mi);
                    bool success = GetMonitorInfo(hMonitor, ref mi);
                    mi = mi.Scale(scale ?? 1, offsetX ?? 0, offsetY ?? 0);
                    if (success)
                    {
                        DisplayInfo di = new DisplayInfo();
                        di.ScreenWidth = (mi.Monitor.Right - mi.Monitor.Left).ToString();
                        di.ScreenHeight = (mi.Monitor.Bottom - mi.Monitor.Top).ToString();
                        di.MonitorArea = mi.Monitor;
                        di.WorkArea = mi.WorkArea;
                        di.Availability = mi.Flags.ToString();
                        col.Add(di);
                    }
                    return true;
                }, IntPtr.Zero);
            col.Sort((x,y) => x.WorkArea.Left.CompareTo(y.WorkArea.Left));
            return col;
        }
    }
}

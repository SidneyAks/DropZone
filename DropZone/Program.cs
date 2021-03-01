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
using System.Management;

namespace DropZone
{
    public partial class Program
    {
        static IZoneRenderer renderer;

        public static LayoutCollection LayoutCollection;

        public static int StartCore()
        {
            IZoneRenderer.ActiveZoneColor = Color.FromArgb(Int32.Parse(DropZone.Settings.ActiveColor.TrimStart('#'), System.Globalization.NumberStyles.HexNumber));
            IZoneRenderer.BackgroundOpacity = DropZone.Settings.BackgroundOpacity;
            IZoneRenderer.LabelColor = Color.FromArgb(Int32.Parse(DropZone.Settings.LabelColor.TrimStart('#'), System.Globalization.NumberStyles.HexNumber));

            var dic = ScreenInfo.GetDisplays();
            LayoutCollection = DropZone.Settings.Zones;
            renderer = new ZoneRenderer.GDI.GDIZone(LayoutCollection.ActiveLayout);

            var t = new SystemTray.TrayIcon(Process.GetCurrentProcess().Handle, "Exit Drop Zone", GetDropZoneIcon().GetHicon());
            t.Events[SystemTray.TrayIcon.MouseMessages.WM_LBUTTONDOWN] += (s, e) => {
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location, "management");
            };
            t.Events[SystemTray.TrayIcon.MouseMessages.WM_LBUTTONDBLCLK] += (s, e) =>
            {
                System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location, "management");
            };

            if (!Environment.GetCommandLineArgs().Any(x => x == "Silent"))
            {
                t.PostInfo("DropZone Running", "DropZone will run in the background, click the system try icon to close the process");
            }
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

        public static void RestartCore()
        {
            RefreshInputHooks();
        }


        private static Bitmap GetDropZoneIcon()
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
    }
}

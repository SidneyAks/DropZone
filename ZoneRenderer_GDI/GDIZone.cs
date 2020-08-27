using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;
using ZoneRenderer;
using System.Runtime.CompilerServices;
using GDIWindow.Win32Structs;
using System.Linq;
using System.Security.Permissions;
using System.Reflection.Emit;
using GDIWindow;

namespace ZoneRenderer.GDI
{

    public class GDIZone : IZoneRenderer
    {
        const uint TransparencyKey = 0x000000;
        private Pen ThickRedPen = new Pen(LabelColor);
        private Pen ThinRedPen = new Pen(LabelColor, .5f);
        private Brush RedBrush = new SolidBrush(LabelColor);
        private Brush ActiveBrush = new SolidBrush(ActiveZoneColor);
        private Brush InactiveBrush = new SolidBrush(BackgroundColor);
        private Brush Black = new SolidBrush(Color.FromArgb(255, 0, 0, 0));


        private Layout Layout;

        private RenderedZone ActiveZone;

        private bool visible = false;

        private bool TransparencyDirty = true;
        private GDIWindow.Win32Delegates.WndProc TransparencyPaintFunc => _transparencyPaintFunc ?? (_transparencyPaintFunc = (GDIWindow.Win32Delegates.WndProc)((hWnd, message, wParam, lParam) =>
        {
            if ((GDIWindow.Win32Enums.WM)message == GDIWindow.Win32Enums.WM.PAINT)
            {
                if (!TransparencyDirty || !visible) return IntPtr.Zero;
                IntPtr hdc;
                GDIWindow.Win32Structs.PAINTSTRUCT ps;
                GDIWindow.Win32Structs.RECT rect;

                hdc = GDIWindow.WinApi.WinAPI.BeginPaint(hWnd, out ps);
                GDIWindow.WinApi.WinAPI.GetClientRect(hWnd, out rect);

                using (var g = System.Drawing.Graphics.FromHwnd(hWnd))
                {
                    g.FillRectangle(InactiveBrush,
                        rect.Left, rect.Top, rect.Width, rect.Height);

                    if (ActiveZone != null)
                    {
                        g.FillRectangle(ActiveBrush,
                            ActiveZone.Target.Left, ActiveZone.Target.Top, ActiveZone.TargetWidth, ActiveZone.TargetHeight);
                    }
                }

                GDIWindow.WinApi.WinAPI.EndPaint(hWnd, ref ps);
                TransparencyDirty = false;
                return IntPtr.Zero;
            }

            return GDIWindow.WinApi.WinAPI.DefWindowProc(hWnd, (GDIWindow.Win32Enums.WM)message, wParam, lParam);
        }));
        private GDIWindow.Win32Delegates.WndProc _transparencyPaintFunc;

        private bool LabelsDirty = true;
        private GDIWindow.Win32Delegates.WndProc LabelPaintFunc => _labelPaintFunc ?? (_labelPaintFunc = (GDIWindow.Win32Delegates.WndProc)((hWnd, message, wParam, lParam) =>
        {

            if ((GDIWindow.Win32Enums.WM)message == GDIWindow.Win32Enums.WM.PAINT)
            {
                if (!LabelsDirty || !visible) return IntPtr.Zero;
                IntPtr hdc;
                GDIWindow.Win32Structs.PAINTSTRUCT ps;
                GDIWindow.Win32Structs.RECT rect;

                hdc = GDIWindow.WinApi.WinAPI.BeginPaint(hWnd, out ps);
                GDIWindow.WinApi.WinAPI.GetClientRect(hWnd, out rect);

                using (var g = System.Drawing.Graphics.FromHwnd(hWnd))
                {
                    g.FillRectangle(Black,
                    rect.Left, rect.Top, rect.Width, rect.Height);

                    if (Layout.Name != null)
                    {
                        var size = g.MeasureString(Layout.Name, LabelFont).ToSize();
                        var point = new Point((LabelWind.Width - size.Width) / 2, size.Height / 2);
                        g.DrawString(Layout.Name, LabelFont, RedBrush, point);
                        g.DrawRectangle(ThickRedPen, new Rectangle(point, size));
                    }

                    int i = 0;
                    foreach (var Zone in Layout.RenderedZones)
                    {
                        i += 1;
                        var text = Zone.Name ?? i.ToString();
                        var size = g.MeasureString(text, LabelFont).ToSize();
                        var point = new Point(Zone.Trigger.Left + ((Zone.TriggerWidth - size.Width) / 2), Zone.Trigger.Top + ((Zone.TriggerHeight - size.Height) / 2));

                        g.DrawString(text, LabelFont, RedBrush, point);
                        g.DrawRectangle(ThinRedPen, new Rectangle(Zone.Trigger.Left, Zone.Trigger.Top, Zone.TriggerWidth, Zone.TriggerHeight));
                    }
                }

                GDIWindow.WinApi.WinAPI.EndPaint(hWnd, ref ps);
                LabelsDirty = false;
                return IntPtr.Zero;
            }

            return GDIWindow.WinApi.WinAPI.DefWindowProc(hWnd, (GDIWindow.Win32Enums.WM)message, wParam, lParam);
        }));
        private GDIWindow.Win32Delegates.WndProc _labelPaintFunc;

        private Window TransWind;
        private Window LabelWind;
        public GDIZone(int width, int height, Layout Layout)
        {
            var instanceHandle = System.Diagnostics.Process.GetCurrentProcess().Handle;

            TransWind = new Window("ZoneMove", TransparencyPaintFunc, instanceHandle, hidden: true, delayShow: true)
            {
                Left = -5,
                Top = 0 - GDIWindow.WinApi.WinAPI.GetSystemMetrics(GDIWindow.Win32Enums.SystemMetric.SM_CYCAPTION) - 7,
                Width = ScreenInfo.GetDisplays().MaxWidth + 10,
                Height = ScreenInfo.GetDisplays().MaxHeight + GDIWindow.WinApi.WinAPI.GetSystemMetrics(GDIWindow.Win32Enums.SystemMetric.SM_CYCAPTION) + 15,
                ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost,
                Style = GDIWindow.Win32Enums.WindowStylesEx.WS_EX_VisualStudioEmulation | GDIWindow.Win32Enums.WindowStylesEx.WS_EX_TOPMOST,
                Transparency = 128,
            };

            LabelWind = new Window("ZoneMove1", LabelPaintFunc, TransWind.Handle, hidden: true, delayShow: true)
            {
                Left = -5,
                Top = 0 - GDIWindow.WinApi.WinAPI.GetSystemMetrics(GDIWindow.Win32Enums.SystemMetric.SM_CYCAPTION) - 7,
                Width = ScreenInfo.GetDisplays().MaxWidth + 10,
                Height = ScreenInfo.GetDisplays().MaxHeight + GDIWindow.WinApi.WinAPI.GetSystemMetrics(GDIWindow.Win32Enums.SystemMetric.SM_CYCAPTION) + 15,
                ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost,
                Style = GDIWindow.Win32Enums.WindowStylesEx.WS_EX_VisualStudioEmulation | GDIWindow.Win32Enums.WindowStylesEx.WS_EX_TOPMOST,
                Transparency = 255,
            };

            TransWind.DelayedShow();
            LabelWind.DelayedShow();

            UpdateZones(Layout);
        }

        public override void UpdateZones(Layout Layout)
        {
            LabelsDirty = true;
            this.Layout = Layout;
            TransWind.UpdateWindow();
            LabelWind.UpdateWindow();

        }

        public override void RenderZone()
        {
            visible = true;
            TransWind.ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost;
            LabelWind.ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost;
            TransWind.Show();
            LabelWind.Show();
        }

        public override void HideZone()
        {
            visible = false;
            TransWind.Hide();
            LabelWind.Hide();
        }

        public override void ActivateSector(RenderedZone Zone)
        {
            if (!Object.ReferenceEquals(ActiveZone,Zone))
            {
                TransparencyDirty = true;
                ActiveZone = Zone;
                TransWind.UpdateWindow();
                LabelWind.UpdateWindow();
            }
        }
    }
}

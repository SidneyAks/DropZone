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
        private static Pen ThickRedPen = new Pen(LabelColor);
        private static Pen ThinRedPen = new Pen(LabelColor, .5f);
        private static Brush RedBrush = new SolidBrush(LabelColor);
        private static Brush ActiveBrush = new SolidBrush(ActiveZoneColor);
        private static Brush InactiveBrush = new SolidBrush(BackgroundColor);
        private static Brush noTransparentInactiveBrush = new SolidBrush(Color.FromArgb(255, BackgroundColor.R, BackgroundColor.G, BackgroundColor.B));
        private static Brush Black = new SolidBrush(Color.FromArgb(255, 0, 0, 0));

        public static void PaintTransparency(Graphics g, RECT rect, RenderedZone ActiveZone = null, bool clear = true, bool rendererSupportsTransparency = false)
        {
            if (clear)
            {
                if (rendererSupportsTransparency)
                {
                    g.FillRectangle(InactiveBrush, rect.Left, rect.Top, rect.Width, rect.Height);
                }
                else
                {
                    g.FillRectangle(noTransparentInactiveBrush, rect.Left, rect.Top, rect.Width, rect.Height);
                }
                //                g.FillRectangle(Black, rect.Left, rect.Top, rect.Width, rect.Height);

            }

            if (ActiveZone != null)
            {
                g.FillRectangle(ActiveBrush,
                    ActiveZone.Target.Left, ActiveZone.Target.Top, ActiveZone.TargetWidth, ActiveZone.TargetHeight);
            }
        }
        public static void PaintLabel(Graphics g, RECT rect, RenderedLayout Layout, bool clear = true, bool title = true)
        {
            if (clear)
            {
                g.FillRectangle(Black, rect.Left, rect.Top, rect.Width, rect.Height);
            }

            if (Layout.Base.Name != null && title)
            {
                var size = g.MeasureString(Layout.Base.Name, LabelFont).ToSize();
                var point = new Point((rect.Width - size.Width) / 2, size.Height / 2);
                g.DrawString(Layout.Base.Name, LabelFont, RedBrush, point);
                g.DrawRectangle(ThickRedPen, new Rectangle(point, size));
            }

            int i = 0;
            foreach (var Zone in Layout.Zones)
            {
                i += 1;
                var text = Zone.Name ?? i.ToString();
                var size = g.MeasureString(text, LabelFont).ToSize();
                var point = new Point(Zone.Trigger.Left + ((Zone.TriggerWidth - size.Width) / 2), Zone.Trigger.Top + ((Zone.TriggerHeight - size.Height) / 2));

                g.DrawString(text, LabelFont, RedBrush, point);
                g.DrawRectangle(ThinRedPen, new Rectangle(Zone.Trigger.Left, Zone.Trigger.Top, Zone.TriggerWidth, Zone.TriggerHeight));
            }
        }

        public GDIZone(RenderedLayout Layout)
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
                Transparency = BackgroundColor.A,
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

        public override void UpdateZones(RenderedLayout Layout)
        {
            LabelsDirty = true;
            this.Layout = Layout;
            TransWind.UpdateWindow();
            LabelWind.UpdateWindow();

        }

        public override void RenderZone(int ScreenWidth, int ScreenHeight)
        {
            TransWind.Width = LabelWind.Width = ScreenWidth;
            TransWind.Height = LabelWind.Height = ScreenHeight + GDIWindow.WinApi.WinAPI.GetSystemMetrics(GDIWindow.Win32Enums.SystemMetric.SM_CYCAPTION) + 15;

            visible = true;
            LabelWind.ZPos = TransWind.ZPos = GDIWindow.Win32Enums.HWNDPosStates.TopMost;

            LabelsDirty = true;

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
            if (!Object.ReferenceEquals(ActiveZone, Zone))
            {
                TransparencyDirty = true;
                ActiveZone = Zone;
                TransWind.UpdateWindow();
                LabelWind.UpdateWindow();
            }
        }

        private RenderedLayout Layout;
        private RenderedZone ActiveZone;

        private bool visible = false;
        private bool TransparencyDirty = true;
        private bool LabelsDirty = true;

        private GDIWindow.Win32Delegates.WndProc TransparencyPaintFunc => _transparencyPaintFunc ?? (_transparencyPaintFunc = (GDIWindow.Win32Delegates.WndProc)((hWnd, message, wParam, lParam) =>
        {
            if ((GDIWindow.Win32Enums.WM)message == GDIWindow.Win32Enums.WM.PAINT)
            {
                if (!TransparencyDirty || !visible)
                {
                    return IntPtr.Zero;
                }
                else
                {
                    GDIWindow.WinApi.WinAPI.BeginPaint(hWnd, out PAINTSTRUCT ps);
                    GDIWindow.WinApi.WinAPI.GetClientRect(hWnd, out RECT rect);
                    using (var g = System.Drawing.Graphics.FromHwnd(hWnd))
                    {
                        PaintTransparency(g, rect, ActiveZone);
                    }
                    GDIWindow.WinApi.WinAPI.EndPaint(hWnd, ref ps);

                    TransparencyDirty = false;
                    return IntPtr.Zero;
                }
            }

            return GDIWindow.WinApi.WinAPI.DefWindowProc(hWnd, (GDIWindow.Win32Enums.WM)message, wParam, lParam);
        }));
        private GDIWindow.Win32Delegates.WndProc _transparencyPaintFunc;

        private GDIWindow.Win32Delegates.WndProc LabelPaintFunc => _labelPaintFunc ?? (_labelPaintFunc = (GDIWindow.Win32Delegates.WndProc)((hWnd, message, wParam, lParam) =>
        {

            if ((GDIWindow.Win32Enums.WM)message == GDIWindow.Win32Enums.WM.PAINT)
            {
                if (!LabelsDirty || !visible) return IntPtr.Zero;
                GDIWindow.WinApi.WinAPI.BeginPaint(hWnd, out PAINTSTRUCT ps);
                GDIWindow.WinApi.WinAPI.GetClientRect(hWnd, out RECT rect);
                using (var g = System.Drawing.Graphics.FromHwnd(hWnd))
                {
                    PaintLabel(g, rect, Layout);
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
    }
}

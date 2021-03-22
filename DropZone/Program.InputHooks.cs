using GlobalLowLevelHooks;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using ZoneRenderer;
using static GlobalLowLevelHooks.MouseHook;

namespace DropZone
{
    public enum MouseButtonTriggers
    {
        LeftButton,
        MiddleButton,
        RightButton,
    }

    public partial class Program
    {
        private static KeyboardHook khook = new KeyboardHook();
        private static MouseHook mhook = new MouseHook();

        [Flags]
        public enum state
        {
            ctrl = 1,
            alt = 2,
            win = 4,
            shift = 8,
            active = 16

        }
        public static state HotkeyState = default;

        public static state TriggerHotkeys
        {
            get
            {
                if (trigger == null)
                {
                    trigger = 0;
                    trigger |= DropZone.Settings.RequireCtrl ? state.ctrl : 0;
                    trigger |= DropZone.Settings.RequireAlt ? state.alt : 0;
                    trigger |= DropZone.Settings.RequireShift ? state.shift : 0;
                    trigger |= DropZone.Settings.RequireWinKey ? state.win : 0;
                }
                return trigger.Value;
            }
        }
        private static state? trigger;

        private static WindowRef candidateWindow;

        private static void printState(MSLLHOOKSTRUCT m)
        {
            new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    var data = GetWindowDetailsFromPoint(m.pt);

                    Console.WriteLine($"({m.pt.x},{m.pt.y})" +
                        $"\t{((HotkeyState == 0) ? "" : HotkeyState.ToString()),12}" +
                        $"\t{data.Title, 40}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }).Start();
        }

        private static void RegisterInputHooks()
        {
            if (DropZone.Settings.TriggerButton == DropZone.Settings.SwapButton)
            {
                throw new Exception("Trigger and Swap buttons cannot be same mouse button");
            }

            SetMKEvents();

            mhook.Install();
            khook.Install();
        }

        private static void RefreshInputHooks ()
        {
            trigger = null;
            SetMKEvents();
        }

        private static void SetMKEvents()
        {
            mhook.ResetEvents();
            khook.ResetEvents();

            switch (DropZone.Settings.TriggerButton)
            {
                case MouseButtonTriggers.LeftButton:
                    mhook.LeftButtonDown += MouseButtonDown;
                    mhook.LeftButtonUp += MouseButtonUp;
                    break;
                case MouseButtonTriggers.MiddleButton:
                    mhook.MiddleButtonDown += MouseButtonDown;
                    mhook.MiddleButtonUp += MouseButtonUp;
                    break;
                case MouseButtonTriggers.RightButton:
                    mhook.RightButtonDown += MouseButtonDown;
                    mhook.RightButtonUp += MouseButtonUp;
                    break;
            }

            switch (DropZone.Settings.SwapButton)
            {
                case MouseButtonTriggers.LeftButton:
                    mhook.LeftButtonDown += SwapButtonDown;
                    break;
                case MouseButtonTriggers.MiddleButton:
                    mhook.MiddleButtonDown += SwapButtonDown;
                    break;
                case MouseButtonTriggers.RightButton:
                    mhook.RightButtonDown += SwapButtonDown;
                    break;
            }

            khook.KeyDown += (KeyboardHook.VKeys key) =>
            {
                new System.Threading.Tasks.Task(() =>
                {
                    switch (key)
                    {
                        case KeyboardHook.VKeys.CONTROL:
                        case KeyboardHook.VKeys.LCONTROL:
                        case KeyboardHook.VKeys.RCONTROL:
                            HotkeyState.Add(state.ctrl);
                            break;
                        case KeyboardHook.VKeys.MENU:
                        case KeyboardHook.VKeys.LMENU:
                        case KeyboardHook.VKeys.RMENU:
                            HotkeyState.Add(state.alt);
                            break;
                        case KeyboardHook.VKeys.LWIN:
                        case KeyboardHook.VKeys.RWIN:
                            HotkeyState.Add(state.win);
                            break;
                        case KeyboardHook.VKeys.SHIFT:
                        case KeyboardHook.VKeys.LSHIFT:
                        case KeyboardHook.VKeys.RSHIFT:
                            HotkeyState.Add(state.shift);
                            break;

                        default:
                            break;
                    }
                }).Start();
            };

            khook.KeyUp += (KeyboardHook.VKeys key) =>
            {
                new System.Threading.Tasks.Task(() =>
                {
                    switch (key)
                    {
                        case KeyboardHook.VKeys.CONTROL:
                        case KeyboardHook.VKeys.LCONTROL:
                        case KeyboardHook.VKeys.RCONTROL:
                            HotkeyState.Remove(state.ctrl);
                            break;
                        case KeyboardHook.VKeys.MENU:
                        case KeyboardHook.VKeys.LMENU:
                        case KeyboardHook.VKeys.RMENU:
                            HotkeyState.Remove(state.alt);
                            break;
                        case KeyboardHook.VKeys.LWIN:
                        case KeyboardHook.VKeys.RWIN:
                            HotkeyState.Remove(state.win);
                            break;
                        case KeyboardHook.VKeys.SHIFT:
                        case KeyboardHook.VKeys.LSHIFT:
                        case KeyboardHook.VKeys.RSHIFT:
                            HotkeyState.Remove(state.shift);
                            break;
                        default:
                            break;
                    }
                }).Start();
            };
        }

        private static void MouseButtonDown(MSLLHOOKSTRUCT m)
        {
            if (HotkeyState.HasFlag(TriggerHotkeys) || TriggerHotkeys == default(state))
            {
                candidateWindow = GetWindowDetailsFromPoint(m.pt);
                if (candidateWindow.IsPartOfWindowsUI)
                {
                    Console.WriteLine("Candidate Window is shell window -- NO!");
                    candidateWindow = null;
                    return;
                } if (DropZone.Settings.OnlyTriggerOnTitleBarClick && m.pt.y > candidateWindow.rect.Top + candidateWindow.TitleBarHeight)
                {
                    Console.WriteLine("Click detected, but not in title bar constraints");
                    candidateWindow = null;
                    return;
                }

                HotkeyState.Add(state.active);
                new System.Threading.Tasks.Task(() =>
                {
                    mhook.MouseMove += Mhook_MouseMove;
                    renderer.ActivateSector(LayoutCollection.ActiveLayout.GetActiveZoneFromPoint(m.pt.x, m.pt.y));

                    renderer.RenderZone(ScreenInfo.GetDisplays().MaxWidth, ScreenInfo.GetDisplays().MaxHeight);
                }).Start();
#if DEBUG
                printState(m);
#endif
            }

        }

        private static void MouseButtonUp(MSLLHOOKSTRUCT m)
        {
            HotkeyState.Remove(state.active);
            new System.Threading.Tasks.Task(() =>
            {
                mhook.MouseMove -= Mhook_MouseMove;
                renderer.HideZone();
                var Zone = LayoutCollection.ActiveLayout.GetActiveZoneFromPoint(m.pt.x, m.pt.y);
                if (Zone != null && candidateWindow != null)
                {
                    candidateWindow.MoveTo(Zone.Target.Left, Zone.Target.Top, Zone.Target.Right, Zone.Target.Bottom);
                }
                candidateWindow = null;
            }).Start();
        }

        private static void SwapButtonDown(MSLLHOOKSTRUCT m)
        {
            if (HotkeyState.HasFlag(state.active))
            {
                new System.Threading.Tasks.Task(() =>
                {
                    renderer.UpdateZones(LayoutCollection.ActivateNextZone());
                    renderer.ActivateSector(null);
                    renderer.ActivateSector(LayoutCollection.ActiveLayout.GetActiveZoneFromPoint(m.pt.x, m.pt.y));
                }).Start();
            }
        }

        private static void Mhook_MouseMove(MSLLHOOKSTRUCT m)
        {
            new System.Threading.Tasks.Task(() =>
            {
                try
                {
                    var ag = LayoutCollection.ActiveLayout.GetActiveZoneFromPoint(m.pt.x, m.pt.y);
                    renderer.ActivateSector(ag);
                }
                catch (Exception ex)
                {
                    //We don't have a console window if we're not debugging, so swallow the exception.
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                }
            }).Start();
        }

    }

    public static class flags
    {
        public static Program.state Add(this ref Program.state myFlags, Program.state flag)
        {
            return (myFlags |= flag);
        }

        public static Program.state Remove(this ref Program.state myFlags, Program.state flag)
        {
            return (myFlags &= ~flag);
        }
    }

    public static class StringExtensions
    {
        public static string Right(this string str, int length)
        {
            return str.Substring(str.Length - length, length);
        }
    }
}

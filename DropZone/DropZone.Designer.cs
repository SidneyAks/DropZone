﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using ZoneRenderer;

namespace DropZone
{
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class DropZone : global::System.Configuration.ApplicationSettingsBase
    {
        private static DropZone defaultInstance;

        static DropZone()
        {
            defaultInstance = ((DropZone)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DropZone())));
        }

        public static DropZone Settings => defaultInstance;

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Left")]
        public MouseButtonTriggers TriggerButton
        {
            get
            {
                return ((MouseButtonTriggers)(this["TriggerButton"]));
            }
            set
            {
                this["TriggerButton"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Right")]
        public MouseButtonTriggers SwapButton
        {
            get
            {
                return ((MouseButtonTriggers)(this["SwapButton"]));
            }
            set
            {
                this["SwapButton"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#BBFFFFFF")]
        public String ActiveColor
        {
            get
            {
                return ((String)(this["ActiveColor"]));
            }
            set
            {
                this["ActiveColor"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("200")]
        public Byte BackgroundOpacity
        {
            get
            {
                return ((Byte)(this["BackgroundOpacity"]));
            }
            set
            {
                this["BackgroundOpacity"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#FFFF7D7D")]
        public String LabelColor
        {
            get
            {
                return ((String)(this["LabelColor"]));
            }
            set
            {
                this["LabelColor"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RequireShift
        {
            get
            {
                return ((bool)(this["RequireShift"]));
            }
            set
            {
                this["RequireShift"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool RequireCtrl
        {
            get
            {
                return ((bool)(this["RequireCtrl"]));
            }
            set
            {
                this["RequireCtrl"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool RequireAlt
        {
            get
            {
                return ((bool)(this["RequireAlt"]));
            }
            set
            {
                this["RequireAlt"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool RequireWinKey
        {
            get
            {
                return ((bool)(this["RequireWinKey"]));
            }
            set
            {
                this["RequireWinKey"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool OnlyTriggerOnTitleBarClick
        {
            get
            {
                return ((bool)(this["OnlyTriggerOnTitleBarClick"]));
            }
            set
            {
                this["OnlyTriggerOnTitleBarClick"] = value;
            }
        }

        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(null)]
        public LayoutCollection Zones
        {
            get
            {
#if DEBUG
                return DefaultLayoutCollection;
#else
                if (this["Zones"] == null)
                {
                    this.Zones = DefaultLayoutCollection;
                    this.Save();
                }
                return ((LayoutCollection)(this["Zones"]));
#endif
            }
            set
            {
                this["Zones"] = value;
            }
        }

        private static Layout DefaultUniveralLayout => new Layout()
        {
            Name = "Global",
            List =
            {
                new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>()
                {
                    Name = "This Monitor",
                    Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                    {
                        Left = "0",
                        Top = "0",
                        Right = "1",
                        Bottom = "1"
                    },
                    Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                    {
                        Left = "0",
                        Top = "0",
                        Right = "1",
                        Bottom = "1/10"
                    }
                }
            }
        };

        private static LayoutCollection DefaultLayoutCollection => new LayoutCollection()
        {
            ParentLayout = DefaultUniveralLayout,
            List = new System.Collections.Generic.List<Layout>() {
                new Layout()
                {
                    Name = "Vertical Thirds",
                    List = {
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name = "Top 1/3",
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "1",
                                Bottom = "1/3"
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "1",
                                Bottom = "1/3",
                            },
                        },
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name = "Top 2/3",
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "1",
                                Bottom = "2/3",
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "1/3",
                                Right = "1",
                                Bottom = "1/2",
                            },
                        },
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name = "Bottom 2/3",
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "1/3",
                                Right = "1",
                                Bottom = "1"
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "1/2",
                                Right = "1",
                                Bottom = "2/3",
                            },
                        },
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name = "Bottom 1/3",
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "2/3",
                                Right = "1",
                                Bottom = "1"
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "2/3",
                                Right = "1",
                                Bottom = "1",
                            },
                        }

                    }
                },
                new Layout()
                {
                    Name = "Per Monitor",
                    List = {
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name = "Left 2 Monitors",
                            Layout = ZoneRenderer.LayoutKind.Spanning,
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "2/3",
                                Bottom = "1",
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "1/10",
                                Right = "1/3",
                                Bottom = "1",
                            }
                        },
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name = "All 3 Monitors",
                            Layout = ZoneRenderer.LayoutKind.Spanning,
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "1",
                                Bottom = "1",
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "1/3",
                                Top = "1/10",
                                Right = "2/3",
                                Bottom = "1",
                            }
                        },
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                            Name="Right 2 Monitors",
                            Layout = ZoneRenderer.LayoutKind.Spanning,
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "1/3",
                                Top = "0",
                                Right = "1",
                                Bottom = "1",
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "2/3",
                                Top = "1/10",
                                Right = "1",
                                Bottom = "1",
                            }
                        },
                        new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Name = "This Monitor",
                            Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "1",
                                Bottom = "1"
                            },
                            Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                            {
                                Left = "0",
                                Top = "0",
                                Right = "1",
                                Bottom = "1/10"
                            }
                        }
                    }
                },
                new Layout()
            {
                Name = "SplitValue",
                List =
                {
                    new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                        Name = "Top 1/3",
                        Layout = LayoutKind.SelectedScreens,
                        ScreenIndexes = new []{ 0 },
                        Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1/3"
                        },
                        Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1/3",
                        },
                    },
                    new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                        Name = "Top 2/3",
                        Layout = LayoutKind.SelectedScreens,
                        ScreenIndexes = new []{ 0 },
                        Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "2/3",
                        },
                        Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "1/3",
                            Right = "1",
                            Bottom = "1/2",
                        },
                    },
                    new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                        Name = "Bottom 2/3",
                        Layout = LayoutKind.SelectedScreens,
                        ScreenIndexes = new []{ 0 },
                        Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "1/3",
                            Right = "1",
                            Bottom = "1"
                        },
                        Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "1/2",
                            Right = "1",
                            Bottom = "2/3",
                        },
                    },
                    new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                        Name = "Bottom 1/3",
                        Layout = LayoutKind.SelectedScreens,
                        ScreenIndexes = new []{ 0 },
                        Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "2/3",
                            Right = "1",
                            Bottom = "1"
                        },
                        Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "2/3",
                            Right = "1",
                            Bottom = "1",
                        },
                    },
                    new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                        Name = "All 3 Monitors",
                        Layout = ZoneRenderer.LayoutKind.Spanning,
                        Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1",
                        },
                        Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "1/3",
                            Top = "1/10",
                            Right = "2/3",
                            Bottom = "1",
                        }
                    },
                    new RenderableZoneBase<Ratio,Ratio,Ratio,Ratio>() {
                        Name="Right 2 Monitors",
                        Layout = ZoneRenderer.LayoutKind.Spanning,
                        Target = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "1/3",
                            Top = "0",
                            Right = "1",
                            Bottom = "1",
                        },
                        Trigger = new Bounds<Ratio,Ratio,Ratio,Ratio>()
                        {
                            Left = "2/3",
                            Top = "1/10",
                            Right = "1",
                            Bottom = "1",
                        }
                    },
                }
            }
            }
        };

    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ZoneRenderer;

namespace DropZone
{


    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.6.0.0")]
    internal sealed partial class DropZone : global::System.Configuration.ApplicationSettingsBase
    {

        private static DropZone defaultInstance = ((DropZone)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DropZone())));

        public static DropZone Settings
        {
            get
            {
                return defaultInstance;
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

        private static LayoutCollection DefaultLayoutCollection => new LayoutCollection()
        {
/*            new Layout()
            {
                Name = "Quarters",
                LayoutType = ZoneRenderer.LayoutKind.PerScreen,
                List = {
                    new Zone() {
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1/2",
                            Bottom = "1/2",
                        },
                    },
                    new Zone() {
                        Target = new Bounds<Ratio>()
                        {
                            Left = "1/2",
                            Top = "0",
                            Right = "1",
                            Bottom = "1/2"
                        },
                    },
                    new Zone() {
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "1/2",
                            Right = "1/2",
                            Bottom = "1",
                        },
                    },
                    new Zone() {
                        Target = new Bounds<Ratio>()
                        {
                            Left = "1/2",
                            Top = "1/2",
                            Right = "1",
                            Bottom = "1"
                        },
                    }
                }
            },*/
            new Layout()
            {
                Name = "Vertical Thirds",
                List = {
                    new Zone() {
                        Name = "Top 1/3",
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1/3"
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1/3",
                        },
                    },
                    new Zone() {
                        Name = "Top 2/3",
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "2/3",
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "1/3",
                            Right = "1",
                            Bottom = "1/2",
                        },
                    },
                    new Zone() {
                        Name = "Bottom 2/3",
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "1/3",
                            Right = "1",
                            Bottom = "1"
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "1/2",
                            Right = "1",
                            Bottom = "2/3",
                        },
                    },
                    new Zone() {
                        Name = "Bottom 1/3",
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "2/3",
                            Right = "1",
                            Bottom = "1"
                        },
                        Trigger = new Bounds<Ratio>()
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
                    new Zone() {
                        Name = "Left 2 Monitors",
                        Layout = ZoneRenderer.LayoutKind.Spanning,
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "2/3",
                            Bottom = "1",
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "1/10",
                            Right = "1/3",
                            Bottom = "1",
                        }
                    },
                    new Zone() {
                        Name = "All 3 Monitors",
                        Layout = ZoneRenderer.LayoutKind.Spanning,
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1",
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "1/3",
                            Top = "1/10",
                            Right = "2/3",
                            Bottom = "1",
                        }
                    },
                    new Zone() {
                        Name="Right 2 Monitors",
                        Layout = ZoneRenderer.LayoutKind.Spanning,
                        Target = new Bounds<Ratio>()
                        {
                            Left = "1/3",
                            Top = "0",
                            Right = "1",
                            Bottom = "1",
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "2/3",
                            Top = "1/10",
                            Right = "1",
                            Bottom = "1",
                        }
                    },
                    new Zone()
                    {
                        Name = "This Monitor",
                        Target = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1"
                        },
                        Trigger = new Bounds<Ratio>()
                        {
                            Left = "0",
                            Top = "0",
                            Right = "1",
                            Bottom = "1/10"
                        }
                    }
                }
            }
        };
       
    }
}

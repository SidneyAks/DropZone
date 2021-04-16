using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Drawing;
using ZoneRenderer;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Markup;
using System.Globalization;

namespace DropZone
{
    public class EnumToCollectionExtension : MarkupExtension
    {
        public Type EnumType { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null) throw new ArgumentNullException(nameof(EnumType));

            return Enum.GetValues(EnumType);
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            PictureBox.Image = new Bitmap(SystemInformation.VirtualScreen.Width,
                               SystemInformation.VirtualScreen.Height,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics screenGraph = Graphics.FromImage(PictureBox.Image);
            screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                       SystemInformation.VirtualScreen.Y,
                                       0,
                                       0,
                                       SystemInformation.VirtualScreen.Size,
                                       CopyPixelOperation.SourceCopy);

            Backer = DropZone.Settings.Zones.ParentLayout;
            this.PropertyChanged += (s, e) => PictureBox.Refresh();
            this.ContentRendered += (s, e) =>
            {
                ListBox_SelectionChanged(this.TriggerRadio, null);
                ListBox_SelectionChanged(this.SwapRadio, null);
            };
        }

        public List<IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound>> DataGridZones
        {
            get
            {
                return (Backer == DropZone.Settings.Zones.ParentLayout) ? Backer.List : 
                        DropZone.Settings.Zones.ParentLayout?.List.Union(Backer.List).ToList() ?? Backer.List;
            }
        }

        public Layout Backer
        {
            get => _backer; 
            set
            {
                _backer = value;
                OnPropertyChanged();
                OnPropertyChanged("DataGridZones");
            }
        }

        public List<RenderedZone> ActiveZones
        {
            get => _activeZones ?? new List<RenderedZone>();
            set
            {
                _activeZones = value;
            }
        }

        public IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound> ActiveZone
        {
            get => _activeZone;
            set
            {
                _activeZone = value;
            }
        }

        private Layout _backer;

        private List<RenderedZone> _activeZones;

        private IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound> _activeZone;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged(this, new PropertyChangedEventArgs(propertyName));

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void KillDropzone_Click(object sender, RoutedEventArgs e)
        {
            SingleInstance.Dispatcher.SendArgToPipeLine(App.DaemonPipeName, "Close");
        }

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {

            if (PictureBox.Image != null)
            {
                var BackgroundWidth = (float)PictureBox.Image.Width;
                var BackgroundHeight = (float)PictureBox.Image.Height;

                var FrameWidth = (float)PictureBox.Width;
                var FrameHeight = (float)PictureBox.Height;

                var BackgroundAspect = BackgroundWidth / BackgroundHeight;
                var FrameAspect = FrameWidth / FrameHeight;


                var Width = FrameAspect > BackgroundAspect ? BackgroundWidth * (FrameHeight / BackgroundHeight) : FrameWidth;
                var Height = FrameAspect > BackgroundAspect ? FrameHeight : BackgroundHeight * (FrameWidth / BackgroundWidth);

                var Top = (FrameHeight - Height) / 2;
                var Left = (FrameWidth - Width) / 2;

                var rect1 = new System.Drawing.RectangleF(Left, Top, Width, Height);
                var scale = Width / BackgroundWidth;
                var offset = new PointF(Left, Top);

                var rect = new GDIWindow.Win32Structs.RECT()
                {
                    Top = (int)rect1.Top,
                    Right = (int)rect1.Right,
                    Bottom = (int)rect1.Bottom,
                    Left = (int)rect1.Left,
                };

                PictureBox.Tag = Backer.Render(ScreenInfo.GetDisplays(scale, (int)offset.X, (int)offset.Y), DropZone.Settings.Zones.ParentLayout);
                var layout = PictureBox.Tag as RenderedLayout;

                if (layout != null)
                {
                    ZoneRenderer.GDI.GDIZone.PaintTransparency(e.Graphics, rect, ActiveZones, true, true);
                    ZoneRenderer.GDI.GDIZone.PaintLabel(e.Graphics, rect, layout, false, false, ActiveZones);
                }
            }
        }

        private void PictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var coordinates = PictureBox.PointToClient(System.Windows.Forms.Cursor.Position);
            var l = (PictureBox.Tag as RenderedLayout)?.GetActiveZoneFromPoint(coordinates.X, coordinates.Y);
            if (l != null)
            {
                if (!(ActiveZones.Contains(l) && ActiveZones.Count == 1))
                {
                    ActiveZones = new List<RenderedZone>() { l };
                    ActiveZone = l?.Zone;
                    LayoutExplorer.SelectedItem = ActiveZone;
                    PictureBox.Refresh();
                }
            }
            else if (ActiveZones.Count > 0)
            {
                ActiveZones.Clear();
                LayoutExplorer.SelectedItem = null;
                PictureBox.Refresh();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Backer = Tabs.SelectedItem as Layout ?? DropZone.Settings.Zones.ParentLayout;
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var ColumnEditActions =
                new Dictionary<string, Action<object, DataGridBeginningEditEventArgs>>
                {
                    { "Screens", (_sender, _e) => { e.Cancel = true; } },
                    { "Target", (_sender, _e) => { e.Cancel = true; } },
                    { "Trigger", (_sender, _e) => { e.Cancel = true; } }
                };

            if (ColumnEditActions.ContainsKey(e.Column.Header.ToString()))
            {
                ColumnEditActions[e.Column.Header.ToString()](sender, e);
            }
        }

        private void LayoutExplorer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!System.Environment.StackTrace.Contains("DropZone.MainWindow.PictureBox_MouseMove"))
            {
                if (LayoutExplorer.SelectedItem is IRenderableZoneBase<IRenderableBound, IRenderableBound, IRenderableBound, IRenderableBound> az)
                {
                    ActiveZone = az;
                    var rl = PictureBox.Tag as RenderedLayout;
                    var zones = rl?.Zones.Where(x => x.Zone.Equals(ActiveZone)).ToList(); ;
                    ActiveZones = zones;
                    PictureBox.Refresh();
                }
                else
                {
                    ActiveZone = null;
                    ActiveZones = new List<RenderedZone>();
                    PictureBox.Refresh();
                }
            }
        }

        private void LayoutExplorer_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            bool? browsable = (e.PropertyDescriptor as PropertyDescriptor).Attributes.Cast<Attribute>().OfType<BrowsableAttribute>().FirstOrDefault()?.Browsable;
            e.Cancel = !(browsable.HasValue ? browsable.Value : true);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            DropZone.Settings.PropertyChanged -= App.PropertyChangedHandler;
            DropZone.Settings.Reset();
            DropZone.Settings.Save();
            DropZone.Settings.PropertyChanged += App.PropertyChangedHandler;
//            OnPropertyChanged("LeftTrigger");
/*            OnPropertyChanged("MiddleTrigger");
            OnPropertyChanged("RightTrigger");
            OnPropertyChanged("LeftSwap");
            OnPropertyChanged("MiddleSwap");
            OnPropertyChanged("RightSwap");

            OnPropertyChanged("RequireCtrl");
            OnPropertyChanged("RequireAlt");
            OnPropertyChanged("RequireShift");
            OnPropertyChanged("RequireWinKey");
            OnPropertyChanged("OnlyTriggerOnTitleBarClick");*/
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is System.Windows.Controls.ListBox lb && lb.Items != null)
            {
                var otherControl = lb.Name == "TriggerRadio" ? this.SwapRadio : TriggerRadio;

                foreach (var value in Enum.GetValues(typeof(MouseButtonTriggers)))
                {
                    ListBoxItem el = otherControl?.ItemContainerGenerator.ContainerFromItem(value) as ListBoxItem;
                    if (el != null)
                    {
                        el.IsEnabled = (int)value != (int)(lb?.SelectedItem ?? value);
                    }
                }
            }
        }
    }
}

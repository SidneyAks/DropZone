using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Drawing;
using ZoneRenderer;

namespace DropZone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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

            ARC = new AspectRatioConverter();
            ARC.backer = DropZone.Settings.Zones.First();
            PictureBox_Resize(null, null);

        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void KillDropzone_Click(object sender, RoutedEventArgs e)
        {
            SingleInstance.Dispatcher.SendArgToPipeLine(App.DaemonPipeName, "Close");
        }

        public AspectRatioConverter ARC{get;set;}

        private void PictureBox_Paint(object sender, PaintEventArgs e)
        {
            var rect = new GDIWindow.Win32Structs.RECT()
            {
                Top = (int)ARC.rect.Top,
                Right = (int)ARC.rect.Right,
                Bottom = (int)ARC.rect.Bottom,
                Left = (int)ARC.rect.Left,
            };


            if (ARC.layout != null)
            {

                ZoneRenderer.GDI.GDIZone.PaintTransparency(e.Graphics, rect, ARC.active, true, true);
                ZoneRenderer.GDI.GDIZone.PaintLabel(e.Graphics, rect, ARC.layout, false, false);
            }
        }

        private void PictureBox_Resize(object sender, EventArgs e)
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

                ARC.rect = new System.Drawing.RectangleF(Left, Top, Width, Height);
                ARC.scale = Width / BackgroundWidth;
                ARC.offset = new PointF(Left, Top);
                PictureBox.Refresh();
            }
        }

        public class AspectRatioConverter
        {
            public ZoneRenderer.RenderedLayout layout;
            public Layout backer
            {
                get => _backer;
                set
                {
                    _backer = value;
                    layout = _backer.Render(ScreenInfo.GetDisplays(scale, (int)offset.X, (int)offset.Y));
                }
            }
            private Layout _backer;

            public RectangleF rect
            {
                get => rect1; set
                {
                    rect1 = value;
                    layout = _backer.Render(ScreenInfo.GetDisplays(scale, (int)offset.X, (int)offset.Y));
                }
            }
            public float scale
            {
                get => scale1; set
                {
                    scale1 = value;
                    layout = _backer.Render(ScreenInfo.GetDisplays(scale, (int)offset.X, (int)offset.Y));
                }
            }
            public PointF offset
            {
                get => offset1; set
                {
                    offset1 = value;
                    layout = _backer.Render(ScreenInfo.GetDisplays(scale, (int)offset.X, (int)offset.Y));
                }
            }
            public RenderedZone active { get; set; }

            private RectangleF rect1;
            private float scale1;
            private PointF offset1;
        }

        private void PictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var coordinates = PictureBox.PointToClient(System.Windows.Forms.Cursor.Position);
            var l = ARC.layout?.GetActiveZoneFromPoint(coordinates.X, coordinates.Y);

            if (ARC.active != l)
            {
                ARC.active = l;
                PictureBox.Refresh();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ARC.backer = Tabs.SelectedItem as Layout ;
            PictureBox.Refresh();
        }
    }
}

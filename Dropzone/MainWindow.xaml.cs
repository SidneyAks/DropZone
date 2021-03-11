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

            BG = new Bitmap(SystemInformation.VirtualScreen.Width,
                               SystemInformation.VirtualScreen.Height,
                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics screenGraph = Graphics.FromImage(BG);
            screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                       SystemInformation.VirtualScreen.Y,
                                       0,
                                       0,
                                       SystemInformation.VirtualScreen.Size,
                                       CopyPixelOperation.SourceCopy);

            PictureBox.Image = BG;
        }

        private System.Drawing.Image BG;

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
            var ARC = (AspectRatioConverter)PictureBox.Tag;

            var renderedDI = ZoneRenderer.ScreenInfo.GetDisplays(ARC.scale, (int)ARC.offset.X, (int)ARC.offset.Y);

            var rect = new GDIWindow.Win32Structs.RECT()
            {
                Top = (int)ARC.rect.Top,
                Right = (int)ARC.rect.Right,
                Bottom = (int)ARC.rect.Bottom,
                Left = (int)ARC.rect.Left,
            };

            ZoneRenderer.GDI.GDIZone.PaintTransparency(e.Graphics, rect, null, true, true);
            ZoneRenderer.GDI.GDIZone.PaintLabel(e.Graphics, rect, DropZone.Settings.Zones.First().Render(renderedDI), false, false);
        }

        private void PictureBox_Resize(object sender, EventArgs e)
        {
            if (BG != null)
            {
                var BackgroundWidth = (float)BG.Width;
                var BackgroundHeight = (float)BG.Height;

                var FrameWidth = (float)PictureBox.Width;
                var FrameHeight = (float)PictureBox.Height;

                var BackgroundAspect = BackgroundWidth / BackgroundHeight;
                var FrameAspect = FrameWidth / FrameHeight;


                var Width = FrameAspect > BackgroundAspect ? BackgroundWidth * (FrameHeight / BackgroundHeight) : FrameWidth;
                var Height = FrameAspect > BackgroundAspect ? FrameHeight : BackgroundHeight * (FrameWidth / BackgroundWidth);

                var Top = (FrameHeight - Height) / 2;
                var Left = (FrameWidth - Width) / 2;

                PictureBox.Tag = new AspectRatioConverter()
                {
                    rect = new System.Drawing.RectangleF(Left, Top, Width, Height),
                    scale = Width / BackgroundWidth,
                    offset = new PointF(Left, Top)
                };
            }
        }

        private class AspectRatioConverter
        {
            public RectangleF rect { get; set; }
            public float scale { get; set; }
            public PointF offset { get; set; }
        }
    }
}

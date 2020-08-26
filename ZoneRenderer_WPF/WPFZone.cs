using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Interop;
using ZoneRenderer;

namespace ZoneRenderer.WPF
{
    public class MainWindow : Window
    {

    }

    [Obsolete("The WPFZone Renderer was deemed to be non performant (taking 300 mb of memory) and does not support virtual desktops",true)]
    public class WPFZone : IZoneRenderer
    {
        private Application WinApp;
        private Window MainWindow;
        private Canvas Canvas;
        private Brush ActiveBrush;
        private Brush InactiveBrush;

        public Dictionary<RenderedZone, Rectangle> Rectangles = new Dictionary<RenderedZone, Rectangle>();

        public WPFZone(int width, int height, IEnumerable<RenderedZone> Zones)
        {
            var t = new Thread(() => {
                //Allow application to bootup
                //TODO: Replace with semaphore or other time agnostic thread guarding mechanism
                Thread.Sleep(1000);


                //TODO: Allow for custom configuration via config file? 
                ActiveBrush = new SolidColorBrush(Color.FromArgb(195, 255, 255, 255));
                InactiveBrush = new SolidColorBrush(Color.FromArgb(125, 0, 0, 0));

                WinApp = new Application();
                MainWindow = new MainWindow();
                
                //Generate Window
                MainWindow.WindowStyle = WindowStyle.None;
                MainWindow.AllowsTransparency = true;
                MainWindow.Background = Brushes.Transparent;
                MainWindow.Topmost = true;
                MainWindow.Left = 0;
                MainWindow.Top = 0;
                //Can be pretty big, the size of n monitors (For my testing this is 3600x1200)
                MainWindow.Width = ScreenInfo.GetDisplays().MaxWidth;
                MainWindow.Height = ScreenInfo.GetDisplays().MaxHeight;
                MainWindow.Content = (Canvas = new Canvas());

                updateZones(Zones);
                WinApp.Run(MainWindow); // note: blocking call
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

        }

        public override void HideZone()
        {
            WinApp.Dispatcher.Invoke(() =>
            {
                MainWindow.Background = Brushes.Transparent;
                MainWindow.Hide();
            });
        }

        public override void RenderZone()
        {
            WinApp?.Dispatcher.Invoke(() =>
            {
                MainWindow.Background = InactiveBrush;
                MainWindow.Show();
            });
        }

        public override void ActivateSector(RenderedZone Zone)
        {
            WinApp?.Dispatcher.Invoke(() =>
            {
                foreach (var rect in Rectangles)
                {
                    rect.Value.Fill = rect.Key == Zone ?
                        ActiveBrush :
                        Brushes.Transparent;
                }
            });
        }

        public override void UpdateZones(Layout Layout)
        {
            WinApp.Dispatcher.Invoke(() =>
            {
                updateZones(Layout.RenderedZones);
            });
        }

        private void updateZones(IEnumerable<RenderedZone> Zones)
        {
            Canvas.Children.Clear();
            Rectangles.Clear();
            foreach (var Zone in Zones)
            {
                var rect = new System.Windows.Shapes.Rectangle();
                rect.StrokeThickness = 2;
                rect.Stroke = Brushes.Black;
                rect.Width = Zone.TargetWidth;
                rect.Height = Zone.TargetHeight;
                Canvas.SetLeft(rect, Zone.Target.Left);
                Canvas.SetTop(rect, Zone.Target.Top);
                Canvas.Children.Add(rect);
                Rectangles[Zone] = rect;
            }
        }
    }
}
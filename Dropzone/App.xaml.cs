using SingleInstance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DropZone

{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public readonly static string DaemonPipeName = "DropzoneDaemonPipe";
        public readonly static string ConfigrationPipeName = "DropzoneConfiguratorPipe";

        [STAThread]
        public static void Main()
        {
            int i = 0;
            var args = Environment.GetCommandLineArgs().Skip(1);
            if (!args.Any(x => x == "management"))
            {
                SingleInstance.Dispatcher.Intialize(DaemonPipeName, "Dropzone", args.ToArray());
                Program.StartCore();
            }
            else
            {
                SingleInstance.Dispatcher.Intialize(ConfigrationPipeName, "Dropzone", args.ToArray());
                var application = new App();
                DropZone.Settings.PropertyChanged += PropertyChangedHandler;
                application.InitializeComponent();
                application.Run();
            }
        }

        public static void PropertyChangedHandler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DropZone.Settings.Save();
            SingleInstance.Dispatcher.SendArgToPipeLine(App.DaemonPipeName, "Restart");
        }

        [DipatchRegistration("Restart")]
        public static void Reload()
        {
            DropZone.Settings.Reload();
            Program.RestartCore();
//            SingleInstance.Dispatcher.ClosePipe();
//            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location, "Silent");
//            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        [DipatchRegistration("Close")]
        public static void Close()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}

using SingleInstance;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using ZoneRenderer;

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
        }

        [DipatchRegistration("Close")]
        public static void Close()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}

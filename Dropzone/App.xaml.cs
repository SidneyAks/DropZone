using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dropzone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            int i = 0;
            var args = Environment.GetCommandLineArgs().Skip(1);
            if (!args.Any())
            {
                DropZone.Program.StartCore();
            }
            else
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();
            }
        }
    }
}

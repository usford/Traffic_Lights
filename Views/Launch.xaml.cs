using System;
using System.Windows;
using System.Diagnostics;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;

namespace Traffic_Lights.Views {
    public partial class Launch : Window {
        public Launch() {
            InitializeComponent();
            try {
                //Console.OutputEncoding = System.Text.Encoding.UTF8;
                var config = new ConfigHandler();

                config.ConfigJson.isSetup = false;
                config.Update();
                Process proc = new Process();
                proc.StartInfo.FileName = @$"{config.PathToDirectory}\uninstallServer.bat";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();

                var menuTasksView = new MenuTasksView(config);
                menuTasksView.Show();
                Hide();
            }catch (Exception e) {
                //Console.WriteLine(e);
                var errorWindow = new ErrorWindow(e.ToString());
                errorWindow.ShowDialog();
            }
        }
    }
}

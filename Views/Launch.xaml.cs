using System;
using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Text;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;

namespace Traffic_Lights.Views {
    public partial class Launch : Window {
        public Launch() {
            InitializeComponent();
            try {
                //Console.OutputEncoding = System.Text.Encoding.UTF8;              
                var config = new ConfigHandler();
                var sb = new StringBuilder();
                using (var streamReader = new StreamReader(@$"{config.PathToDirectory}\MySQL Server 8.0\data\my.ini"))
                {
                    var line = streamReader.ReadLine();
                    int counter = 0;
                    while (line != null)
                    {
                        Debug.WriteLine($"{++counter}: {line}");
                        if (line.StartsWith("datadir"))
                        {
                            line = $"datadir=\"{config.PathToDirectory}\\MySQL Server 8.0\\data\\Data\"";
                        }else if (line.StartsWith("secure-file-priv"))
                        {
                            line = $"secure-file-priv=\"{config.PathToDirectory}\\MySQL Server 8.0\\data\\Uploads\"";
                        }

                        sb.AppendLine(line);
                        line = streamReader.ReadLine();
                    }
                }

                using (var streamWriter = new StreamWriter(@$"{config.PathToDirectory}\MySQL Server 8.0\data\my.ini"))
                {
                    streamWriter.Write(sb.ToString());
                }
                config.ConfigJson.isSetup = false;
                config.Update();
                Process proc = new Process();
                proc.StartInfo.FileName = @$"{config.PathToDirectory}\MySQL Server 8.0\start.bat";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                //Process proc = new Process();
                //proc.StartInfo.FileName = @$"{config.PathToDirectory}\uninstallServer.bat";
                //proc.StartInfo.UseShellExecute = true;
                //proc.StartInfo.Verb = "runas";
                //proc.Start();

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

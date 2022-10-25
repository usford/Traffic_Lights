using System;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using Traffic_Lights.ViewsModels;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Interfaces;

namespace Traffic_Lights.Views {
    public partial class MenuTasksView : Window {
        private IConfigHandler _configHandler;
        public MenuTasksView(IConfigHandler configHandler)  {
            InitializeComponent();
            //Console.OutputEncoding = Encoding.UTF8;
            try
            {
                _configHandler = configHandler;

                DataContext = new MenuTasksViewModel(_configHandler);
            }
            catch (Exception e)
            {
                var errorWindow = new ErrorWindow(e.ToString());
                var debugLogger = new DebugLogger();
                errorWindow.ShowDialog();
                debugLogger.Start();
                debugLogger.Write("критическая ошибка:\n" + e.Message);
                debugLogger.Stop();
            }
            
        }
        public void ButtonClose (object sender, RoutedEventArgs e) {
            Close();
        }
    }
}

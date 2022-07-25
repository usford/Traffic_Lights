using System;
using System.Text;
using System.Windows;
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
            _configHandler = configHandler;

            DataContext = new MenuTasksViewModel(_configHandler);
        }
        public void ButtonClose (object sender, RoutedEventArgs e) {
            Close();
        }
    }
}

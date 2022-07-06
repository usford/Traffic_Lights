using System;
using System.Text;
using System.Windows;
using Traffic_Lights.ViewsModels;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;

namespace Traffic_Lights.Views {
    public partial class MenuTasksView : Window {
        private MySQLConnection _mySqlConnection;
        private ConfigHandler _configHandler;
        public MenuTasksView(MySQLConnection mySqlConnection, ConfigHandler configHandler)  {
            InitializeComponent();
            Console.OutputEncoding = Encoding.UTF8;
            _mySqlConnection = mySqlConnection;
            _configHandler = configHandler;

            DataContext = new MenuTasksViewModel(_mySqlConnection, _configHandler);
        }
        public void ButtonClose (object sender, RoutedEventArgs e) {
            Close();
        }
    }
}

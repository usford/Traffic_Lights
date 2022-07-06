using System;
using System.Windows;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;

namespace Traffic_Lights.Views {
    public partial class Launch : Window {
        public Launch() {
            InitializeComponent();
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var config = new ConfigHandler();
            var mySqlConnection = new MySQLConnection();
            mySqlConnection.Start();
            mySqlConnection.Open();
            var menuTasksView = new MenuTasksView(mySqlConnection, config);
            menuTasksView.Show();
            Hide();
        }
    }
}

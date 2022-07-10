using System;
using System.Windows;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;

namespace Traffic_Lights.Views {
    public partial class Launch : Window {
        public Launch() {
            InitializeComponent();
            try {
                Console.OutputEncoding = System.Text.Encoding.UTF8;
                var config = new ConfigHandler();
                var menuTasksView = new MenuTasksView(config);
                menuTasksView.Show();
                Hide();
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                //Console.ReadLine();
            }
            
        }
    }
}

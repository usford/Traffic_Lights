using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using Traffic_Lights.ViewsModels;

namespace Traffic_Lights.Views {
    public partial class SetupBar : Window {
        public SetupBar() {
            InitializeComponent();
            //Console.OutputEncoding = Encoding.UTF8;
            DataContext = new SetupBarViewModel();
        }
    }
}

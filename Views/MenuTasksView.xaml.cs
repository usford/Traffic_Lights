using System;
using System.Text;
using System.Windows;
using Traffic_Lights.ViewsModels;
using System.IO;
using System.Runtime;
using System.Xml.Linq;
using System.Linq;

namespace Traffic_Lights.Views {
    public partial class MenuTasksView : Window {
        public MenuTasksView()  {
            InitializeComponent();
            Console.OutputEncoding = Encoding.UTF8;
            DataContext = new MenuTasksViewModel();
        }
    }
}

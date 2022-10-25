using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using Traffic_Lights.ViewsModels;

namespace Traffic_Lights.Views {
    public partial class ErrorWindow : Window {
        private string _errorText;
        private string _errorTitle;
        public ErrorWindow(string errorText, string errorTitle = "Ошибка: ") {
            InitializeComponent();
            //Console.OutputEncoding = Encoding.UTF8;
            DataContext = new SetupBarViewModel();
            error_text_block.Text = errorText;
            error_title.Text = errorTitle;
            Title = errorTitle;
        }
    }
}

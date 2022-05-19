using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Traffic_Lights {
    public partial class MainWindow : Window {
        public static string pathDirectory = @"E:\VS projects\Traffic_Lights\"; //Директория программы
        public MainWindow() {
            Console.OutputEncoding = Encoding.UTF8; //Кодировка для правильного отображения различных символов в консоли
            InitializeComponent();     
            sp_buttons.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClick)); //Обработка нажатий всех кнопок
            MySQLUtility.RunConnection();

            string schemePath = pathDirectory + @"Элементы схемы\Светофор.svg";
            string buttonRedPath = pathDirectory + @"Элементы схемы\buttonOFF_red.svg";
            string buttonYellowPath = pathDirectory + @"Элементы схемы\buttonOFF_yellow.svg";
            string buttonGreenPath = pathDirectory + @"Элементы схемы\buttonOFF_green.svg";

            try {
                svg_scheme.StreamSource = new StreamReader(schemePath).BaseStream;
                svg_buttonRed.StreamSource = new StreamReader(buttonRedPath).BaseStream;
                svg_buttonYellow.StreamSource = new StreamReader(buttonYellowPath).BaseStream;
                svg_buttonGreen.StreamSource = new StreamReader(buttonGreenPath).BaseStream;
            }
            catch {
                Console.WriteLine("Ошибка в чтении файлов");
            }

            
        }
        private void ButtonClick(object sender, RoutedEventArgs e) {
            string name = (e.OriginalSource as Button)!.Name;
            Console.WriteLine(name);
        }
    }
}

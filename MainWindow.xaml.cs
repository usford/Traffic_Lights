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
                КН1.StreamSource = new StreamReader(buttonRedPath).BaseStream;
                КН2.StreamSource = new StreamReader(buttonYellowPath).BaseStream;
                КН3.StreamSource = new StreamReader(buttonGreenPath).BaseStream;
            }
            catch {
                Console.WriteLine("Ошибка в чтении файлов");
            }
        }
        //Нажатие на любую кнопку
        private void ButtonClick(object sender, RoutedEventArgs e) {
            string name = (e.OriginalSource as Button)!.Name;
            Console.WriteLine(name);
        }
        //Изменение элементов, где name = наименование элемента, а elementCode его код в excel файле
        public void ChangeElement(string? name, string? elementCode) {
            var svgElement = sp_buttons.FindName(name) as SharpVectors.Converters.SvgViewbox;
            string path = pathDirectory + @"Элементы схемы\" + elementCode + ".svg";
            svgElement!.StreamSource = new StreamReader(path).BaseStream;
        }
    }
}

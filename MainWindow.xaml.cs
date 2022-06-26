using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SvgViewBox = SharpVectors.Converters.SvgViewbox;
using System.IO;
using BehaviorsLayout = Microsoft.Xaml.Behaviors.Layout;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using System.Xml;
using System.Threading.Tasks;
using System.Threading;
using Traffic_Lights.Views;


namespace Traffic_Lights {
    public partial class MainWindow : Window {
        public XmlDocument xDoc = new XmlDocument();
        public static string pathDirectory = new DirectoryInfo(@"..\..\..").FullName; //Директория программы
        //public static string pathDirectory = new DirectoryInfo(@"..").FullName + @"Traffic_Lights"; //Для установщика
        public MainWindow() {
            Console.OutputEncoding = Encoding.UTF8; //Кодировка для правильного отображения различных символов в консоли
            InitializeComponent();
            xDoc.Load((@$"{pathDirectory}\Элементы схемы\схема.svg"));
            try {
                //CreateXAML(this);
                var dataConnection = ExcelTaskJobRepository.GetConnection();
                var mySQL = new MySQLUtility(this, dataConnection);
                mySQL.RunConnection();
            }
            catch (Exception e) {
                Console.WriteLine("Ошибка в чтении схемы");
                Console.WriteLine(e);
            }
        }

        public void ButtonExit(object sender, RoutedEventArgs e) {
            Hide();
            var menuTasksView = new MenuTasksView();
            menuTasksView.Show();
        }
        public void ButtonClick(object sender, MouseButtonEventArgs e) {
            string name = (e.OriginalSource as SvgViewBox)!.Name.Split("_")[1];
            var dataConnection = ExcelTaskJobRepository.GetConnection();
            var mySQL = new MySQLUtility(this, dataConnection);
            mySQL.InsertStateTable2(name);
        }
        //Изменение элементов, где name = наименование элемента, а state = его состояние
        public void ChangeElement(string? elementCode, int state) {     

            using (StreamWriter streamReader = new StreamWriter(@"E:\VS projects\Traffic_Lights\Элементы схемы\схема.svg")) {
                var layers = xDoc.DocumentElement.ChildNodes;
                foreach (XmlNode layer in layers)  {
                    if (layer.Attributes["inkscape:label"] != null) {
                        if (layer.Attributes["inkscape:label"].Value == elementCode) {
                            layer.Attributes["style"].Value = $"display:{(VisibleElement)state}";
                        }
                    }
                }
                xDoc.Save(streamReader.BaseStream);
            }

            using (StreamReader streamReader = new StreamReader(@"E:\VS projects\Traffic_Lights\Элементы схемы\схема.svg")) {
                svgMain.StreamSource = streamReader.BaseStream;
            }
        }
    }
    public enum VisibleElement {
        none,
        inline
    }
}

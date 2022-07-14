using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Threading;
using Traffic_Lights.SvgHandler;
using Traffic_Lights.Enums;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Views;

namespace Traffic_Lights {
    public partial class MainWindow : Window {
        public List<Button> buttons;
        public XmlDocument xDoc = new XmlDocument();
        public XmlDocument xamlDocument = new XmlDocument();
        public Dictionary<string, string> mapElementsSvg;
        private string _titleTask;
        private IConfigHandler _configHandler;
        private IMySQLConnection _mySqlConnection;
        private ExcelTaskJobRepository _excelTaskJobRepository;
        public MainWindow(IMySQLConnection mySQLConnection, IConfigHandler configHandler, string titleTask) {
            Console.OutputEncoding = Encoding.UTF8; //Кодировка для правильного отображения различных символов в консоли
            InitializeComponent();
            _titleTask = titleTask;
            _configHandler = configHandler;
            _mySqlConnection = mySQLConnection;
            _excelTaskJobRepository = new ExcelTaskJobRepository(configHandler);
            xDoc.Load((@$"{_configHandler.PathToSvgElements}\схема.svg"));

            var svgElementsParse = new SvgElementsParse();
            mapElementsSvg = svgElementsParse.GetElementsMap();

            ButtonRendering(svgElementsParse.GetCoordinatesButtons());

            buttons = new List<Button>();
            foreach (Button btn in canvasButtons.Children) {
                buttons.Add(btn);
            }

            try {
                var dataConnection = _excelTaskJobRepository.GetConnection();
                var mySQL = new MySQLUtility(this, _mySqlConnection, _configHandler, _titleTask);
                mySQL.RunConnection();
                ChangeSvg();
            }
            catch (Exception e) {
                Console.WriteLine("Ошибка в чтении схемы");
                Console.WriteLine(e);
            }

        }
        //Наложение xaml кнопок поверх кнопок свг
        public void ButtonRendering(List<SvgElementsParse.XamlButtons> xamlButtons) {
            foreach (var xamlButton in xamlButtons) {
                //Console.WriteLine(lol.ID);
                //Console.WriteLine("X: " + lol.Coordinates.x + ":" + "Y: " + lol.Coordinates.y);
                var button = new Button();
                button.Name = xamlButton.ID;
                button.Click += ButtonClick;
                button.Opacity = 0;
                Canvas.SetLeft(button, xamlButton.Coordinates.x);
                Canvas.SetTop(button, xamlButton.Coordinates.y);
                canvasButtons.Children.Add(button);
            }
        }
        public void ButtonExit(object sender, RoutedEventArgs e) {
            var menuTasksView = new MenuTasksView(_configHandler);
            menuTasksView.Show();
            Close();
        }
        public void ButtonClick(object sender, RoutedEventArgs e) {
            //string name = (e.OriginalSource as SvgViewBox)!.Name.Split("_")[1];
            string name = (e.OriginalSource as Button).Name;
            var dataConnection = _excelTaskJobRepository.GetConnection();
            var mySQL = new MySQLUtility(this, _mySqlConnection, _configHandler, _titleTask);
            //Console.WriteLine((e.OriginalSource as SvgDrawingCanvas).Children.Count);
            Console.WriteLine($"Нажатие на кнопку: {name}");
            mySQL.InsertStateTable2(name);
        }
        async void ChangeSvg() {
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(200));
            while (await timer.WaitForNextTickAsync()) {
                try {
                    using (StreamReader streamReader = new StreamReader($@"{_configHandler.PathToSvgElements}\схема.svg")) {
                        svgMain.StreamSource = streamReader.BaseStream;
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
        //Изменение элементов, где name = наименование элемента, а state = его состояние
        //TODO вынести layers
        public async void ChangeElement(string? elementCode, int state) {
            Console.WriteLine($"Проверяется элемент: {elementCode}, состояние {state}");
            if (elementCode.StartsWith("kn")) {
                if (state == 1) {
                    int index = (int)Char.GetNumericValue(elementCode[7]) - 1;
                    //Console.WriteLine("------------------------");
                    //Console.WriteLine("Текущее id кнопки: " + buttons[index].Name);
                    //Console.WriteLine("Новое id кнопки: " + elementCode);
                    buttons[index].Name = elementCode;
                    canvasButtons.InvalidateVisual();
                }
            }
            string layerName = mapElementsSvg[elementCode];
            var layers = xDoc.DocumentElement.ChildNodes;
            foreach (XmlNode layer in layers) {
                if (layer.Attributes["inkscape:label"] != null) {
                    if (layer.Attributes["inkscape:label"].Value == layerName) {
                        layer.Attributes["style"].Value = $"display:{(VisibleElement)state}";
                    }
                }
            }

            try {
                using (StreamWriter streamReader = new StreamWriter($@"{_configHandler.PathToSvgElements}\схема.svg")) {
                    xDoc.Save(streamReader.BaseStream);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
    
}

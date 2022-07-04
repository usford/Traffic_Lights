﻿using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SvgViewBox = SharpVectors.Converters.SvgViewbox;
using SvgDrawingCanvas = SharpVectors.Runtime.SvgDrawingCanvas;
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
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;


namespace Traffic_Lights {
    public partial class MainWindow : Window {
        public List<Button> buttons;
        public XmlDocument xDoc = new XmlDocument();
        public XmlDocument xamlDocument = new XmlDocument();
        //public static string pathDirectory = new DirectoryInfo(@"..\..\..").FullName; //Директория программы
        public static string pathDirectory = new DirectoryInfo(@"..").FullName + @"Traffic_Lights"; //Для установщика
        public Dictionary<string, string> mapElementsSvg;
        public MainWindow() {
            Console.OutputEncoding = Encoding.UTF8; //Кодировка для правильного отображения различных символов в консоли
            InitializeComponent();
            xDoc.Load((@$"{pathDirectory}\Элементы схемы\схема.svg"));

            var svgElementsParse = new SvgElementsParse();
            mapElementsSvg = svgElementsParse.GetElementsMap();

            ButtonRendering(svgElementsParse.GetCoordinatesButtons());

            buttons = new List<Button>();
            foreach (Button btn     in canvasButtons.Children) {
                buttons.Add(btn);
            }

            //xamlDocument.Load(@$"{pathDirectory}\MainWindow.xaml");


            //WpfDrawingSettings settings = new WpfDrawingSettings();
            //settings.IncludeRuntime = false;
            //settings.TextAsGeometry = true;
            //FileSvgConverter converter = new FileSvgConverter(settings);
            //string svgTestFile = @$"{pathDirectory}\Элементы схемы\схема.svg";
            //converter.Convert(svgTestFile);

            //var canvas = xamlDocument.DocumentElement.LastChild.LastChild.LastChild.LastChild.LastChild;
            //canvas.RemoveAll();
            //var child = xDoc.DocumentElement.FirstChild;
            //var layers = child.ChildNodes;
            //foreach (XmlNode layer in layers) {
            //    if (layer.Attributes["x:Name"] != null) {
            //        if (layer.Attributes["x:Name"].Value == "layer2") {
            //            //XmlAttribute nameAttr = xDoc.CreateAttribute("ButtonBase.Click");
            //            //nameAttr.Value = "ButtonExit";
            //            //layer.Attributes.Append(nameAttr);
            //        }
            //    }
            //    //ButtonBase.Click="ButtonExit"
            //}
            //XmlNode importNode = canvas.OwnerDocument.ImportNode(child, true);

            //canvas.AppendChild(importNode);

            //using (StreamWriter streamReader = new StreamWriter(@$"{pathDirectory}\MainWindow.xaml")) {
            //    xamlDocument.Save(streamReader.BaseStream);
            //}
            try {
                //CreateXAML(this);
                var dataConnection = ExcelTaskJobRepository.GetConnection();
                var mySQL = new MySQLUtility(this, dataConnection);
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
            var menuTasksView = new MenuTasksView();
            menuTasksView.Show();
            Close();
        } 
        public void ButtonClick(object sender, RoutedEventArgs e) {
            //string name = (e.OriginalSource as SvgViewBox)!.Name.Split("_")[1];
            string name = (e.OriginalSource as Button).Name;
            var dataConnection = ExcelTaskJobRepository.GetConnection();
            var mySQL = new MySQLUtility(this, dataConnection);
            //Console.WriteLine((e.OriginalSource as SvgDrawingCanvas).Children.Count);
            Console.WriteLine(name);
            mySQL.InsertStateTable2(name);
        }
        async void ChangeSvg() {
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(200));
            while (await timer.WaitForNextTickAsync()) {
                try {
                    using (StreamReader streamReader = new StreamReader($@"{pathDirectory}\Элементы схемы\схема.svg")) {
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
            //Console.WriteLine(elementCode + ":" + state);
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
                using (StreamWriter streamReader = new StreamWriter($@"{pathDirectory}\Элементы схемы\схема.svg")) {
                    xDoc.Save(streamReader.BaseStream);
                }
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
    public enum VisibleElement {
        none,
        inline
    }
}

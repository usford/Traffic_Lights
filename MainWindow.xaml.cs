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



namespace Traffic_Lights {
    public partial class MainWindow : Window {
        public static string pathDirectory = new DirectoryInfo(@"..\..\..").FullName; //Директория программы
        //public static string pathDirectory = new DirectoryInfo(@"..\..\..").FullName; //Для установщика
        public MainWindow() {
            Console.OutputEncoding = Encoding.UTF8; //Кодировка для правильного отображения различных символов в консоли
            InitializeComponent();
            try {
                CreateXAML(this);
                var dataConnection = ExcelTaskJobRepository.GetConnection();
                var mySQL = new MySQLUtility(this, dataConnection);
                mySQL.RunConnection();
            }
            catch (Exception e) {
                Console.WriteLine("Ошибка в чтении схемы");
                Console.WriteLine(e);
            }

        }
        public void ButtonClick(object sender, MouseButtonEventArgs e) {
            if (UtilitySettings.buttonDragging) return;
            string name = (e.OriginalSource as SvgViewBox)!.Name.Split("_")[1];
            var dataConnection = ExcelTaskJobRepository.GetConnection();
            var mySQL = new MySQLUtility(this, dataConnection);
            mySQL.InsertStateTable2(name);
        }
        public void Editing(object sender, MouseButtonEventArgs e) {
            if (UtilitySettings.buttonDragging) return;
            UtilitySettings.editing = !UtilitySettings.editing;
            Console.WriteLine(UtilitySettings.editing);
        }
        //Динамическое создание схемы
        public static void CreateXAML(MainWindow mw) {
            List<ExcelTaskJobRepository.ElementXAML> elements = ExcelTaskJobRepository.GetElementsXAML();
            var mainCanvas = mw.FindName("mainCanvas") as Canvas;

            foreach (var element in elements) {
                var child = new SvgViewBox();
                //var schemePath = $@"E:\VS projects\TestWPF\Элементы схемы\{element.id}.svg";

                child.Name = element.id;
                Canvas.SetLeft(child, element.x);
                Canvas.SetTop(child, element.y);
                //child.StreamSource = new StreamReader(schemePath).BaseStream;
                Interaction.GetBehaviors(child).Add(new CustomMouseDragElementBehavior());
                            
                if (element.type == "кнопка") {
                    child.Cursor = Cursors.Hand;
                    child.MouseLeftButtonUp += mw.ButtonClick;
                }

                mainCanvas.Children.Add(child);
            }
        }
        //Изменение элементов, где name = наименование элемента, а elementCode его код в excel файле
        public void ChangeElement(string? name, string? elementCode) {
            foreach (var element in mainCanvas.Children) {
                var check = (element as SvgViewBox).Name.Split("_")[0];
                if (check == name) {
                    var svgElement = element as SvgViewBox;
                    if (svgElement.Cursor == Cursors.Hand) svgElement.Name = $"{name}_{elementCode}";
                    //Console.WriteLine("Изменение элемента: " + svgElement.Name);
                    string path = pathDirectory + @"Элементы схемы\" + elementCode + ".svg";
                    svgElement!.StreamSource = new StreamReader(path).BaseStream;
                }
            }
            if (mainCanvas.FindName(name) is not null) {
                
            }          
        }
    }
    public class CustomMouseDragElementBehavior : BehaviorsLayout.MouseDragElementBehavior {
        protected override void OnAttached() {
            base.OnAttached();
            DragFinished += Finished;
            Dragging += DragMove;
        }
        private void DragMove(object sender, MouseEventArgs e) {
            UtilitySettings.buttonDragging = true;
        }
        private void Finished(object sender, MouseEventArgs e) {
            UtilitySettings.buttonDragging = false;
            //Console.WriteLine("Перестал перетаскивать");

            if (X.ToString().Length != 8 && Y.ToString().Length != 8) {
                string id = AssociatedObject.Name.Split("_")[0];
                int x = Convert.ToInt32(X);
                int y = Convert.ToInt32(Y);
                var element = new ExcelTaskJobRepository.ElementXAML(id, "empty", x, y);
                ExcelTaskJobRepository.SaveXAML(element);
            }      
        }
    }
}

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SvgViewBox = SharpVectors.Converters.SvgViewbox;
using System.IO;
using BehaviorsLayout = Microsoft.Xaml.Behaviors.Layout;
using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;


namespace Traffic_Lights {
    public partial class MainWindow : Window {
        public static string pathDirectory = @"E:\VS projects\Traffic_Lights\"; //Директория программы
        //public static string pathDirectory = @"C:\Traffic_Lights\"; //Для установщика
        public MainWindow() {
            Console.OutputEncoding = Encoding.UTF8; //Кодировка для правильного отображения различных символов в консоли
            InitializeComponent();     
            sp_buttons.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClick)); //Обработка нажатий всех кнопок

            string schemePath = pathDirectory + @"Элементы схемы\1010001.svg";
            try {  
                svg_scheme.StreamSource = new StreamReader(schemePath).BaseStream;
                var dataConnection = ExcelUtility.GetConnection();
                var mySQL = new MySQLUtility(this, dataConnection);
                mySQL.RunConnection();
            }
            catch (Exception e) {
                Console.WriteLine("Ошибка в чтении схемы");
                Console.WriteLine(e);
            }

        }
        //Нажатие на любую кнопку
        private void ButtonClick(object sender, RoutedEventArgs e) {
            string name = (e.OriginalSource as Button)!.Name;
            var dataConnection = ExcelUtility.GetConnection();
            var mySQL = new MySQLUtility(this, dataConnection);
            mySQL.InsertStateTable2(name);
        }
        //Изменение элементов, где name = наименование элемента, а elementCode его код в excel файле
        public void ChangeElement(string? name, string? elementCode) {
            if (sp_buttons.FindName(name) is not null) {
                var svgElement = sp_buttons.FindName(name) as SvgViewBox;
                if (svgElement.Parent as Button != null) (svgElement.Parent as Button).Name = elementCode;
                //Console.WriteLine("Изменение элемента: " + (svgElement.Parent as Button).Name);
                string path = pathDirectory + @"Элементы схемы\" + elementCode + ".svg";
                svgElement!.StreamSource = new StreamReader(path).BaseStream;
            }          
        }
    }
    public class CustomMouseDragElementBehavior : BehaviorsLayout.MouseDragElementBehavior {
        protected override void OnAttached() {
            base.OnAttached();
            DragFinished += Finished;
            
        }
        private void Begun(object sender, MouseEventArgs e) {
            Console.WriteLine($"X: {X}, Y: {Y}");
        }
        private void Finished(object sender, MouseEventArgs e) {
            Console.WriteLine("Перестал перетаскивать");
            Console.WriteLine($"X: {X}, Y: {Y}");
        }
    }
}

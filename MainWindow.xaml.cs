﻿using System;
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
            

            string schemePath = pathDirectory + @"Элементы схемы\Светофор.svg";
            //string lnPath1 = pathDirectory + @"Элементы схемы\el1010042.svg";
            //string lnPath2 = pathDirectory + @"Элементы схемы\el1010052.svg";
            //string lnPath3 = pathDirectory + @"Элементы схемы\el1010062.svg";
            try {  
                svg_scheme.StreamSource = new StreamReader(schemePath).BaseStream;
                //лН1.StreamSource = new StreamReader(lnPath1).BaseStream;
                //лН2.StreamSource = new StreamReader(lnPath2).BaseStream;
                //лН3.StreamSource = new StreamReader(lnPath3).BaseStream;
            }
            catch {
                Console.WriteLine("Ошибка в чтении схемы");
            }
            var dataConnection = ExcelUtility.GetConnection();
            var mySQL = new MySQLUtility(this, dataConnection);
            mySQL.RunConnection();
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
                var svgElement = sp_buttons.FindName(name) as SharpVectors.Converters.SvgViewbox;
                if (svgElement.Parent as Button != null) (svgElement.Parent as Button).Name = elementCode;
                //Console.WriteLine("Изменение элемента: " + (svgElement.Parent as Button).Name);
                string path = pathDirectory + @"Элементы схемы\" + elementCode + ".svg";
                svgElement!.StreamSource = new StreamReader(path).BaseStream;
            }          
        }
    }
}

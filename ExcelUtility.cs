using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System.IO;

namespace Traffic_Lights {
    class ExcelUtility {
        //Получение объекта, в котором хранится информация о подключении к бд MySQL
        public static DataConnectionMySQL GetConnection() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}Excel файлы\Подключение к бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                string? host = Convert.ToString(worksheet.Cell(2, 2).Value);
                short port = Convert.ToInt16(worksheet.Cell(3, 2).Value);
                string? username = Convert.ToString(worksheet.Cell(4, 2).Value);
                string? password = Convert.ToString(worksheet.Cell(5, 2).Value);
                string? database = Convert.ToString(worksheet.Cell(6, 2).Value);
                short updateInterval = Convert.ToInt16(worksheet.Cell(7, 2).Value);

                var connection = new DataConnectionMySQL(host, port, username, password, database, updateInterval);

                return connection;
            }
        }
        //Взятие элементов из excel файла с шаблоном бд
        public static List<ElementInfoDB> GetElementsFromExcel(int index) {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}Excel файлы\Шаблон бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var elements = new List<ElementInfoDB>();
                var columns = worksheet.RangeUsed().ColumnsUsed();

                int counter = 0;
                foreach(var column in columns) {
                    if (counter++ == 0) continue;

                    var cellID = column.Cell(index).Value;
                    var cellName = column.Cell(index + 1).Value;
                    var cellState = column.Cell(index + 2).Value;
                    var cellComment = column.Cell(index + 3).Value;

                    string? id = (cellID != "") ? Convert.ToString(cellID) : null;
                    string? name = (cellName != "") ? Convert.ToString(cellName) : null;
                    int? state = (cellState != "") ? Convert.ToInt32(cellState) : null;
                    string? comment = (cellComment != "") ? Convert.ToString(cellComment) : null;

                    var element = new ElementInfoDB(id, name, state, comment);
                    elements.Add(element);
                }

                return elements;
            }            
        }
        //Логика состояний элементов из excel файла Логика
        public static List<ElementInfoExcel> GetLogicElement() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}Excel файлы\Логика.xlsx")) {
                var worksheet = workbook.Worksheet(3);
                var rows = worksheet.RangeUsed().RowsUsed();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                var elements = new List<ElementInfoExcel>();

                int counterRows = 0;
                int indexStates = 6;
                foreach(var row in rows) {
                    if (counterRows++ < 5) continue;
                    var states = new Dictionary<string[], int?>();
                    string? name = Convert.ToString(row.Cell(2).Value);
                    string? code = Convert.ToString(row.Cell(3).Value);
                    string[] cell = Convert.ToString(row.Cell(5).Value)!.Split(" ");

                    int counterColumns = 0;
                    foreach (var column in columns) {
                        if (counterColumns++ < 8) continue;
                        if ((column.Cell(indexStates).Value != "")) {
                            string[] cellState = Convert.ToString(column.Cell(4).Value)!.Split(" ");
                            int? state = Convert.ToInt32(column.Cell(indexStates).Value);
                            states.Add(cellState, state);
                        }     
                    }
                    indexStates++;

                    var element = new ElementInfoExcel(name, code, cell, states);
                    elements.Add(element);
                }

                return elements;
            }
        }
        public class ElementInfoExcel {
            public string? Name { get => _name; }
            private string? _name;
            public string? Code { get => _code; }
            private string? _code;
            public string[] Cell { get => _cell; }
            private string[] _cell;
            public Dictionary<string[], int?> States { get => _states; }
            private Dictionary<string[], int?> _states;
            public ElementInfoExcel(string? name, string? code, string[] cell, Dictionary<string[], int?> states) {
                _name = name;
                _code = code;
                _cell = cell;
                _states = states;
            }
        }
        //Элементы схемы в бд
        public class ElementInfoDB {
            public string? ID { get => _id; }
            private string? _id;
            public string? Name { get => _name; }
            private string? _name;
            public int? State { get => _state; }
            private int? _state;
            public string? Comment { get => _comment; }
            private string? _comment;

            public ElementInfoDB(string? id, string? name, int? state, string? comment) {
                _id = id;
                _name = name;
                _state = state;
                _comment = comment;
            }
        }
        //Конфигурация подключения к бд MySQL
        public class DataConnectionMySQL {
            public string Host { get => _host!; }
            private string? _host;
            public short Port { get => _port; }
            private short _port;
            public string Username { get => _username!; }
            private string? _username;
            public string Password { get => _password!; }
            private string? _password;
            public string Database { get => _database!; }
            private string? _database;
            public short UpdateInterval { get => _updateInterval; }
            private short _updateInterval;
            public DataConnectionMySQL(string? host, short port, string? username, string? password, string? database, short updateInterval) {
                _host = host;
                _port = port;
                _username = username;
                _password = password;
                _database = database;
                _updateInterval = updateInterval;
            }
        }
    } 
}

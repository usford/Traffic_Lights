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
        public static List<ElementInfo> GetElementsFromExcel(int index) {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}Excel файлы\Шаблон бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var elements = new List<ElementInfo>();
                var rows = worksheet.RangeUsed().ColumnsUsed();

                int counter = 0;
                foreach(var row in rows) {
                    if (counter++ == 0) continue;

                    var cellID = row.Cell(index).Value;
                    var cellName = row.Cell(index + 1).Value;
                    var cellState = row.Cell(index + 2).Value;
                    var cellComment = row.Cell(index + 3).Value;

                    string? id = (cellID != "") ? Convert.ToString(cellID) : null;
                    string? name = (cellName != "") ? Convert.ToString(cellName) : null;
                    int? state = (cellState != "") ? Convert.ToInt32(cellState) : null;
                    string? comment = (cellComment != "") ? Convert.ToString(cellComment) : null;

                    var element = new ElementInfo(id, name, state, comment);
                    elements.Add(element);
                }

                return elements;
            }            
        }
        //Элементы схемы
        public class ElementInfo {
            public string? ID { get => _id; }
            private string? _id;
            public string? Name { get => _name; }
            private string? _name;
            public int? State { get => _state; }
            private int? _state;
            public string? Comment { get => _comment; }
            private string? _comment;

            public ElementInfo(string? id, string? name, int? state, string? comment) {
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

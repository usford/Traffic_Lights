using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using Traffic_Lights.Models;

namespace Traffic_Lights {
    public class ExcelTaskJobRepository : TaskJobRepository {
        public ExcelTaskJobRepository() {

        }
        //Получение объекта, в котором хранится информация о подключении к бд MySQL
        public static DataConnectionMySQL GetConnection() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Подключение к бд.xlsx")) {
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
        //TODO убрать этот метод, перенести в GetTaskJobs()
        public static List<ElementInfoDB> GetElementsFromExcel(int index) {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Шаблон бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var elements = new List<ElementInfoDB>();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                int counter = 0;
                index--;
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
        //Получения списка всех таблиц из excel файла с шаблоном бд
        //TODO база данных должна быть одна, наименование таблиц формируется как {Name_task.name_table}
        public List<TaskJob> GetTaskJobs() {
            var taskJobList = new List<TaskJob>();

            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Шаблон бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();
                string nameDB = "empty";
                string comment = "empty";
                var tables = new List<List<ElementInfoDB>>();

                foreach (var row in rows) {
                    string firstCell = Convert.ToString(row.FirstCell().Value);

                    if (firstCell.Contains("Наименование бд")) {
                        nameDB = Convert.ToString(row.Cell(2).Value);
                    }

                    if (firstCell.Contains("Комментарий")) {
                        comment = Convert.ToString(row.Cell(2).Value);
                    }

                    if (firstCell.Contains("id")) {
                        var table = GetElementsFromExcel(row.RowNumber());
                        tables.Add(table);
                    }

                    if (tables.Count == 2) {
                        taskJobList.Add(new TaskJob(
                            title: nameDB,
                            comment: comment,
                            tables: new List<List<ElementInfoDB>>(tables),
                            enabled: (comment == "Неизвестно") ? false : true
                        ));
                        tables.Clear();
                    }
                }
            }

            return taskJobList;
        }
        //Логика состояний элементов в файле логика.xlsx
        public static List<ElementInfoExcel> GetLogicElement() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Логика.xlsx")) {
                var worksheet = workbook.Worksheet(4);
                var rows = worksheet.RangeUsed().RowsUsed();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                var elements = new List<ElementInfoExcel>();

                int counterRows = 0;
                int indexStates = 6;
                string? nameTable = Convert.ToString(worksheet.Cell(2, 8).Value);
                foreach(var row in rows) {
                    if (counterRows++ < 5) continue;
                    var states = new Dictionary<string[], int?>();
                    string? name = Convert.ToString(row.Cell(2).Value);
                    string? code = Convert.ToString(row.Cell(3).Value);
                    string[] cell = Convert.ToString(row.Cell(5).Value)!.Split(" ");

                    int counterColumns = 0;
                    foreach (var column in columns) {
                        if (counterColumns++ < 7) continue;
                        
                        if ((column.Cell(indexStates).Value != "")) {
                            int state;
                            bool isInt = int.TryParse(Convert.ToString(column.Cell(indexStates).Value), out state);

                            if (isInt) {
                                string[] cellState = { Convert.ToString(column.Cell(4).Value)!, nameTable! };
                                states.Add(cellState, state);
                            }
                        }     
                    }
                    indexStates++;

                    var element = new ElementInfoExcel(name, cell, states, code: code);
                    elements.Add(element);
                }

                return elements;
            }
        }
        //Логика связий ячеек table1 и table 2 в файле логика.xlsx
        public static List<ElementInfoExcel> GetLogicRelations() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Логика.xlsx")) {
                var worksheet = workbook.Worksheet(3);
                var rows = worksheet.RangeUsed().RowsUsed();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                var elements = new List<ElementInfoExcel>();

                int counterRows = 0;
                int indexStates = 6;
                string? nameTable = Convert.ToString(worksheet.Cell(2, 6).Value);
                foreach (var row in rows) {
                    if (counterRows++ < 5) continue;
                    var states = new Dictionary<string[], int?>();
                    string? name = Convert.ToString(row.Cell(2).Value);
                    string[] cell = Convert.ToString(row.Cell(3).Value)!.Split(" ");
                    int logicState = Convert.ToInt32(row.Cell(4).Value);

                    int counterColumns = 0;
                    foreach (var column in columns) {
                        if (counterColumns++ < 5) continue;
                        if ((column.Cell(indexStates).Value != "")) {
                            int state;
                            bool isInt = int.TryParse(Convert.ToString(column.Cell(indexStates).Value), out state);

                            if (isInt) {
                                string[] cellState = { Convert.ToString(column.Cell(4).Value)!, nameTable! };
                                states.Add(cellState, state);
                            }
                        }
                    }
                    indexStates++;

                    var element = new ElementInfoExcel(name, cell, states, logicState: logicState);
                    elements.Add(element);
                }

                return elements;
            }
        }
        //Логика включения кнопок управления в файле логика.xlsx
        public static List<ElementInfoExcel> GetStateButtons() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Логика.xlsx")) {
                var worksheet = workbook.Worksheet(2);
                var rows = worksheet.RangeUsed().RowsUsed();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                var elements = new List<ElementInfoExcel>();

                int counterRows = 0;
                int indexStates = 6;
                string? nameTable = Convert.ToString(worksheet.Cell(2, 8).Value);
                foreach (var row in rows) {
                    if (counterRows++ < 5) continue;
                    var states = new Dictionary<string[], int?>();
                    string? name = Convert.ToString(row.Cell(2).Value);
                    string? code = Convert.ToString(row.Cell(3).Value);
                    string[] cell = Convert.ToString(row.Cell(5).Value)!.Split(" ");

                    int counterColumns = 0;
                    foreach (var column in columns) {
                        if (counterColumns++ < 7) continue;
                        if ((column.Cell(indexStates).Value != "")) {
                            int state;
                            bool isInt = int.TryParse(Convert.ToString(column.Cell(indexStates).Value), out state);

                            if (isInt) {
                                string[] cellState = { Convert.ToString(column.Cell(4).Value)!, nameTable! };
                                states.Add(cellState, state);
                            }
                        }
                    }
                    indexStates++;

                    var element = new ElementInfoExcel(name, cell, states, code: code);
                    elements.Add(element);
                }

                return elements;
            }
        }
        //Логика разрешения включения кнопок управления в файле логика.xlsx
        public static List<ElementInfoExcel> GetPermitStateButtons() {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Excel файлы\Логика.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                var elements = new List<ElementInfoExcel>();

                int counterRows = 0;
                int indexStates = 6;
                string? nameTable = Convert.ToString(worksheet.Cell(2, 8).Value);
                foreach (var row in rows) {
                    if (counterRows++ < 5) continue;
                    var states = new Dictionary<string[], int?>();
                    string? name = Convert.ToString(row.Cell(2).Value);
                    string? code = Convert.ToString(row.Cell(3).Value);
                    string[] cell = Convert.ToString(row.Cell(5).Value)!.Split(" ");

                    int counterColumns = 0;
                    foreach (var column in columns) {
                        if (counterColumns++ < 7) continue;
                        if ((column.Cell(indexStates).Value != "")) {
                            int state;
                            bool isInt = int.TryParse(Convert.ToString(column.Cell(indexStates).Value), out state);

                            if (isInt) {
                                string[] cellState = { Convert.ToString(column.Cell(4).Value)!, nameTable! };
                                states.Add(cellState, state);
                            }  
                        }
                    }
                    indexStates++;

                    var element = new ElementInfoExcel(name, cell, states, code: code);
                    elements.Add(element);
                }

                return elements;
            }
        }
        //Взятие элементов из файла Все элементы.xlsx
        public static List<ElementXAML> GetElementsXAML() {
            var elements = new List<ElementXAML>();

            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Элементы схемы\Все элементы.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                int counter = 0;
                foreach (var row in rows) {
                    if (counter++ == 0) continue;
                    string id = Convert.ToString(row.Cell(1).Value)!;
                    string type = Convert.ToString(row.Cell(2).Value)!;
                    int x = Convert.ToInt32(row.Cell(3).Value);
                    int y = Convert.ToInt32(row.Cell(4).Value);

                    var element = new ElementXAML(id, type, x, y);
                    elements.Add(element);
                }
            }

            return elements;
        }
        public static void SaveXAML(ElementXAML element) {
            using (var workbook = new XLWorkbook($@"{MainWindow.pathDirectory}\Элементы схемы\Все элементы.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();

                int counter = 0;
                foreach (var row in rows) {
                    if (counter++ == 0) continue;
                    if (row.Cell(1).Value.ToString() == element.id) {
                        row.Cell(3).Value = element.x;
                        row.Cell(4).Value = element.y;
                    }
                }

                workbook.Save();
            }
        }
        //Для динамического создания схемы
        public struct ElementXAML {
            public string id;
            public string type;
            public int x;
            public int y;

            public ElementXAML(string id, string type, int x, int y) {
                this.id = id;
                this.type = type;
                this.x = x;
                this.y = y;
            }
        }
        //Элементы схемы в excel
        public class ElementInfoExcel {
            public string? Name { get => _name; }
            private string? _name;
            public string? Code { get => _code; }
            private string? _code;
            public string[] Cell { get => _cell; }
            private string[] _cell;
            public int? LogicState { get => _logicState; }
            private int? _logicState;
            public Dictionary<string[], int?> States { get => _states; }
            private Dictionary<string[], int?> _states;
            public ElementInfoExcel(string? name, string[] cell, Dictionary<string[], int?> states, string? code = null, int? logicState = null) {
                _name = name;
                _code = code;
                _cell = cell;
                _states = states;
                _logicState = logicState;
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

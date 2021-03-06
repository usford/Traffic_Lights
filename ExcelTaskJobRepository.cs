using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Models;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.Info;

namespace Traffic_Lights {
    public class ExcelTaskJobRepository {
        private IConfigHandler _configHandler;
        public ExcelTaskJobRepository(IConfigHandler configHandler) {
            _configHandler = configHandler;
        }
        //Получение объекта, в котором хранится информация о подключении к бд MySQL
        public DataConnectionMySQL GetConnection() {
            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Подключение к бд.xlsx")) {
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
        //Логика состояний элементов в файле логика.xlsx
        public List<ElementInfoExcel> GetLogicElement() {
            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Логика.xlsx")) {
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
        public List<ElementInfoExcel> GetLogicRelations() {
            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Логика.xlsx")) {
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
        public List<ElementInfoExcel> GetStateButtons() {
            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Логика.xlsx")) {
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
        public List<ElementInfoExcel> GetPermitStateButtons() {
            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Логика.xlsx")) {
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
        public List<ElementXAML> GetElementsXAML() {
            var elements = new List<ElementXAML>();

            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Все элементы.xlsx")) {
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
        public void SaveXAML(ElementXAML element) {
            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Все элементы.xlsx")) {
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

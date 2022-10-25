using System;
using System.Diagnostics;
using System.Collections.Generic;
using ClosedXML.Excel;
using Traffic_Lights.Models;
using Traffic_Lights.Interfaces;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Info;

namespace Traffic_Lights {
    public class TaskJobRepository : ITaskJobRepository {
        private IConfigHandler _configHandler;
        private IMySQLConnection _mySqlConnection;
        private ExcelTaskJobRepository _excelTaskJobRepository;
        public TaskJobRepository(IMySQLConnection mySqlConnection, IConfigHandler configHandler) {
            _configHandler = configHandler;
            _mySqlConnection = mySqlConnection;
            _excelTaskJobRepository = new ExcelTaskJobRepository(configHandler);
        }
        //Получения списка всех таблиц из excel файла с шаблоном бд
        //TODO база данных должна быть одна, наименование таблиц формируется как {Name_task.name_table}
        public List<TaskJobButton> GetTaskJobButtons() {
            var taskJobList = new List<TaskJobButton>();

            using (var workbook = new XLWorkbook($@"{_configHandler.PathToExcelFiles}\Шаблон бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RangeUsed().RowsUsed();
                var columns = worksheet.RangeUsed().ColumnsUsed();
                string nameDB = "empty";
                string comment = "empty";
                var tables = new List<List<ElementInfoDB>>();

                //var timer = new Stopwatch();
                //timer.Start();
                foreach (var row in rows) {
                    string firstCell = Convert.ToString(row.FirstCell().Value);

                    if (firstCell.Contains("Наименование бд")) {
                        nameDB = Convert.ToString(row.Cell(2).Value);
                    }

                    if (firstCell.Contains("Комментарий")) {
                        comment = Convert.ToString(row.Cell(2).Value);
                    }

                    if (firstCell.Contains("id")) {
                        var table = GetElementsFromExcel(columns, row.RowNumber());
                        tables.Add(table);
                    }

                    if (tables.Count == 2) {
                        if (comment != "") {
                            taskJobList.Add(new TaskJobButton(
                            new TaskJob(
                                title: nameDB,
                                comment: comment,
                                tables: new List<List<ElementInfoDB>>(tables),
                                enabled: true
                            ),
                            _mySqlConnection,
                            _configHandler
                        ));
                        }       
                        tables.Clear();
                    }
                    //Console.WriteLine("Идёт загрузка.. " + timer.Elapsed);
                }
                //timer.Stop();
            }

            return taskJobList;
        }

        private List<ElementInfoDB> GetElementsFromExcel(IXLRangeColumns columns, int index) {
            var elements = new List<ElementInfoDB>();
            int counter = 0;
            index--;
            foreach (var column in columns) {
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
}

using System;
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
                        var table = _excelTaskJobRepository.GetElementsFromExcel(row.RowNumber());
                        tables.Add(table);
                    }

                    if (tables.Count == 2) {
                        taskJobList.Add(new TaskJobButton(
                            new TaskJob(
                                title: nameDB,
                                comment: comment,
                                tables: new List<List<ElementInfoDB>>(tables),
                                enabled: (comment == "Неизвестно") ? false : true
                            ),
                            _mySqlConnection,
                            _configHandler
                        ));
                        tables.Clear();
                    }
                }
            }

            return taskJobList;
        }
    }
}

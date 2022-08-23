using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Views;

namespace Traffic_Lights {
    class MySQLUtility {
        private MainWindow _mainWindow { get; set; }
        private IMySQLConnection _mySqlConnection;
        private IConfigHandler _configHandler;
        private ExcelTaskJobRepository _excelTaskJobRepository;
        private string _titleTask;
        
        //Запуск работы с бд MySQL
        public MySQLUtility(MainWindow mainWindow, IMySQLConnection mySqlConnection, IConfigHandler configHandler, string titleTask) {
            _titleTask = titleTask;
            _mainWindow = mainWindow;
            _mySqlConnection = mySqlConnection;
            _configHandler = configHandler;
            _excelTaskJobRepository = new ExcelTaskJobRepository(configHandler); 
        }
        public void RunConnection() {
            if (_configHandler.ConfigJson.dropDatabase) {
                var cmd = new MySqlCommand();
                cmd.Connection = _mySqlConnection.Connection;
                cmd.CommandText = $"drop database if exists {_mySqlConnection.Database}";
                cmd.ExecuteNonQuery();
                _configHandler.ConfigJson.dropDatabase = true;
                _configHandler.Update();
            }
            try {
                CheckElement();
                CheckTables();
            }
            catch (Exception ex) {
                //Console.WriteLine($"Error: {ex}");
                
            }
            finally {
                //_mySqlConnection.Close();
                //_mySqlConnection.Dispose();
            }
        }
        //Проверка изменений через заданный интервал
        async void CheckTables() {
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_mySqlConnection.UpdateInterval));
            var cmd = new MySqlCommand();
            cmd.Connection = _mySqlConnection.Connection;
            while (await timer.WaitForNextTickAsync()) {
                //Почему-то здесь соединение закрыто, хотя во всем приложении открыто, видимо это особенность асинхронного потока
                if (Convert.ToString(_mySqlConnection.Connection.State) == "Closed") _mySqlConnection.Open();
                //Изменения в таблице 1
                cmd.CommandText = $"select count(*) from {_mySqlConnection.Database}.{_titleTask}_table1_changes";
                int checkTable1 = Convert.ToInt32(cmd.ExecuteScalar());
                if (checkTable1 > 0) {
                    //Console.WriteLine("Данные поменялись в table1");
                    CheckElement();
                    cmd.CommandText = $"delete from {_mySqlConnection.Database}.{_titleTask}_table1_changes";
                    cmd.ExecuteNonQuery();
                }
                //Изменения в таблице 2
                cmd.CommandText = $"select count(*) from {_mySqlConnection.Database}.{_titleTask}_table2_changes";
                int checkTable2 = Convert.ToInt32(cmd.ExecuteScalar());
                if (checkTable2 > 0) {
                    //Console.WriteLine("Данные поменялись в table2");
                    CheckRelationsElement();
                    cmd.CommandText = $"delete from {_mySqlConnection.Database}.{_titleTask}_table2_changes";
                    cmd.ExecuteNonQuery();
                }
            }         
        }
        //Проверка элементов согласно логике в логика.xlsx
        private void CheckElement() {
            //Console.WriteLine("Проверка элементов...");
            var cmd = new MySqlCommand();
            cmd.Connection = _mySqlConnection.Connection;

            var changedElements = new Dictionary<string, int>();
            List<ExcelTaskJobRepository.ElementInfoExcel> elements = _excelTaskJobRepository.GetLogicElement();
            foreach (var element in elements) { 
                bool check = true;
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {_mySqlConnection.Database}.{_titleTask}_{state.Key[1]} Where id = '{state.Key[0]}'";
                    //Console.WriteLine("---------------");
                    //Console.WriteLine(element.Name);
                    //Console.WriteLine(state.Key[0] + " " + state.Key[1]);
                    int stateCheck = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (stateCheck != state.Value) {
                        check = false;
                        break;
                    }                      
                }
                if (!string.IsNullOrEmpty(element.Code))
                {
                    changedElements.Add(element.Code, Convert.ToInt32(check));
                }
                
                //_mainWindow.ChangeElement(element.Code, Convert.ToInt32(check));
            }
            _mainWindow.ChangeElement(changedElements);
        }
        //Проверка элементов согласно логики связей состояний ячеек в логика.xlsx
        private void CheckRelationsElement() {
            var cmd = new MySqlCommand();
            cmd.Connection = _mySqlConnection.Connection;

            List<ExcelTaskJobRepository.ElementInfoExcel> elements = _excelTaskJobRepository.GetLogicRelations();
            foreach (var element in elements) {
                bool check = true;
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {_mySqlConnection.Database}.{_titleTask}_{state.Key[1]} Where id = '{state.Key[0]}'";
                    int stateCheck = Convert.ToInt32(cmd.ExecuteScalar());

                    if (stateCheck != state.Value) {
                        check = false;
                        break;
                    }
                }
                //Если логика верна, изменяем элемент согласно ей  
                cmd.CommandText = $"update {_mySqlConnection.Database}.{_titleTask}_{element.Cell[1]} set state = {Convert.ToInt32(check)} " +
                        $"Where id = '{element.Cell[0]}' ";
                cmd.ExecuteNonQuery();
            }
        }
        //Вставка значения в таблицу 2 по нажатию кнопки
        public void InsertStateTable2(string code) {
            List<ExcelTaskJobRepository.ElementInfoExcel> elements = _excelTaskJobRepository.GetStateButtons();
            List<ExcelTaskJobRepository.ElementInfoExcel> permitElements = _excelTaskJobRepository.GetPermitStateButtons();
            var cmd = new MySqlCommand();
            var sbUpdate = new StringBuilder();
            cmd.Connection = _mySqlConnection.Connection;
            bool check = true;

            foreach (var element in permitElements.Where(e => e.Code == code)) {
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {_mySqlConnection.Database}.{_titleTask}_{state.Key[1]} Where id = '{state.Key[0]}'";
                    int stateCheck = Convert.ToInt32(cmd.ExecuteScalar());
                    if (stateCheck != state.Value) {
                        check = false;
                        break;
                    }
                }
            }
            if (check) {
                foreach (var element in elements.Where(e => e.Code == code)) {
                    foreach (var state in element.States) {
                        sbUpdate.Append($"update {_mySqlConnection.Database}.{_titleTask}_{state.Key[1]} set state = {state.Value} " +
                            $"Where id = '{state.Key[0]}'; \n");                       
                    }
                }
                cmd.CommandText = sbUpdate.ToString();
                cmd.ExecuteNonQuery();
            }      
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Threading;
using Newtonsoft.Json;
using System.IO;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Interfaces;

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
            }
            try {
                //CreateDB();
                CheckElement();
                CheckTables();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex}");
                
            }
            finally {
                _mySqlConnection.Close();
                _mySqlConnection.Dispose();
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
                    Console.WriteLine("Данные поменялись в table1");
                    CheckElement();
                    cmd.CommandText = $"delete from {_mySqlConnection.Database}.{_titleTask}_table1_changes";
                    cmd.ExecuteNonQuery();
                }
                //Изменения в таблице 2
                cmd.CommandText = $"select count(*) from {_mySqlConnection.Database}.{_titleTask}_table2_changes";
                int checkTable2 = Convert.ToInt32(cmd.ExecuteScalar());
                if (checkTable2 > 0) {
                    Console.WriteLine("Данные поменялись в table2");
                    CheckRelationsElement();
                    cmd.CommandText = $"delete from {_mySqlConnection.Database}.{_titleTask}_table2_changes";
                    cmd.ExecuteNonQuery();
                }
            }         
        }
        //Создание базы данных если её нет (с таблицами)
        private void CreateDB() {
            var cmd = new MySqlCommand();
            cmd.Connection = _mySqlConnection.Connection;

            cmd.CommandText = $"create database if not exists {_mySqlConnection.Database}";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.table1 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.table2 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.table1_changes (" +
                $"count int AUTO_INCREMENT," +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(count))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.table2_changes (" +
                $"count int AUTO_INCREMENT," +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(count))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"select count(*) from {_mySqlConnection.Database}.table2";
            int countRecords = Convert.ToInt32(cmd.ExecuteScalar());
            //Заполнение таблиц данными, если они отсутствуют
            if (countRecords == 0) {
                Console.WriteLine("Создаётся база данных...");
                var elements = _excelTaskJobRepository.GetElementsFromExcel(10);
                cmd.CommandText = $"insert into {_mySqlConnection.Database}.table1 (id, name, state, comment) values " +
                        $"(@id, @name, @state, @comment)";
                var id = cmd.Parameters.Add("@id", MySqlDbType.String);
                var name = cmd.Parameters.Add("@name", MySqlDbType.String);
                var state = cmd.Parameters.Add("@state", MySqlDbType.Int32);
                var comment = cmd.Parameters.Add("@comment", MySqlDbType.String);

                foreach (var element in elements) {
                    id.Value = element.ID;
                    name.Value = element.Name;
                    state.Value = element.State;
                    comment.Value = element.Comment;
                    cmd.ExecuteNonQuery();
                }

                elements = _excelTaskJobRepository.GetElementsFromExcel(20);
                cmd.CommandText = $"insert into {_mySqlConnection.Database}.table2 (id, name, state, comment) values " +
                        $"(@id, @name, @state, @comment)";

                foreach (var element in elements) {
                    id.Value = element.ID;
                    name.Value = element.Name;
                    state.Value = element.State;
                    comment.Value = element.Comment;
                    cmd.ExecuteNonQuery();
                }

                //Создание триггеров дли отслеживания изменений в таблицах
                cmd.CommandText = $"use {_mySqlConnection.Database}; " +
                    $"create trigger table1_update " +
                    $"after update on table1 " +
                    $"for each row begin " +
                    $"insert into table1_changes Set " +
                    $"id = NEW.id," +
                    $"name = NEW.name," +
                    $"state = NEW.state," +
                    $"comment = NEW.comment;" +
                    @$"end;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"use {_mySqlConnection.Database}; " +
                    $"create trigger table2_update " +
                    $"after update on table2 " +
                    $"for each row begin " +
                    $"insert into table2_changes Set " +
                    $"id = NEW.id," +
                    $"name = NEW.name," +
                    $"state = NEW.state," +
                    $"comment = NEW.comment;" +
                    @$"end;";
                cmd.ExecuteNonQuery();
                Console.WriteLine("База данных установлена");
            }
        }
        //Проверка элементов согласно логике в логика.xlsx
        private void CheckElement() {
            Console.WriteLine("Проверка элементов...");
            var cmd = new MySqlCommand();
            cmd.Connection = _mySqlConnection.Connection;
            
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
                _mainWindow.ChangeElement(element.Code, Convert.ToInt32(check));
            }
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
            cmd.Connection = _mySqlConnection.Connection;

            bool check = true;

            foreach (var element in permitElements.Where(e => e.Code == code)) {
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {_mySqlConnection.Database}.{_titleTask}_{state.Key[1]} Where id = '{state.Key[0]}'";
                    int stateCheck = Convert.ToInt32(cmd.ExecuteScalar());
                    //Console.WriteLine(state.Key[1]);
                    //Console.WriteLine(state.Key[0]);
                    //Console.WriteLine(cmd.CommandText);
                    //Console.WriteLine(stateCheck);
                    if (stateCheck != state.Value) {
                        check = false;
                        break;
                    }
                }
            }
            if (check) {
                foreach (var element in elements.Where(e => e.Code == code)) {
                    foreach (var state in element.States) {
                        cmd.CommandText = $"update {_mySqlConnection.Database}.{_titleTask}_{state.Key[1]} set state = {state.Value} " +
                            $"Where id = '{state.Key[0]}' ";
                        //Console.WriteLine(cmd.CommandText);
                        cmd.ExecuteNonQuery();
                    }
                }
            }      
        }
    }
}

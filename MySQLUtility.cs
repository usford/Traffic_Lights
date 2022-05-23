using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Windows.Controls;
using System.Timers;
using System.Threading;

namespace Traffic_Lights {
    class MySQLUtility {
        MainWindow mainWindow { get; set; }
        ExcelUtility.DataConnectionMySQL dataConnection { get; set; }
        MySqlConnection connection { get; set; }
        //Запуск работы с бд MySQL
        public MySQLUtility(MainWindow mainWindow, ExcelUtility.DataConnectionMySQL dataConnection) {
            this.mainWindow = mainWindow;
            this.dataConnection = dataConnection;
            connection = GetDBConnection(dataConnection);
            connection.Open();
        }
        public void RunConnection() {
            try {
                CreateDB();
                CheckElement();
                CheckTables();
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex}");
            }
            finally {
                connection.Close();
                connection.Dispose();
            }
        }
        //Проверка изменений через заданный интервал
        async void CheckTables() {
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(dataConnection.UpdateInterval));
            var cmd = new MySqlCommand();
            cmd.Connection = connection;

            while (await timer.WaitForNextTickAsync()) {
                if (Convert.ToString(connection.State) == "Closed") connection.Open();

                //Изменения в таблице 1
                cmd.CommandText = $"select count(*) from {dataConnection.Database}.table1_changes";
                int checkTable1 = Convert.ToInt32(cmd.ExecuteScalar());
                if (checkTable1 > 0) {
                    Console.WriteLine("Данные поменялись в table1");
                    CheckElement();
                    cmd.CommandText = $"delete from {dataConnection.Database}.table1_changes";
                    cmd.ExecuteNonQuery();
                }
                //Изменения в таблице 2
                cmd.CommandText = $"select count(*) from {dataConnection.Database}.table2_changes";
                int checkTable2 = Convert.ToInt32(cmd.ExecuteScalar());
                if (checkTable2 > 0) {
                    Console.WriteLine("Данные поменялись в table2");
                    CheckRelationsElement();
                    cmd.CommandText = $"delete from {dataConnection.Database}.table2_changes";
                    cmd.ExecuteNonQuery();
                }
            }         
        }
        //Создание базы данных если её нет (с таблицами)
        void CreateDB() {
            var cmd = new MySqlCommand();
            cmd.Connection = connection;

            cmd.CommandText = $"create database if not exists {dataConnection.Database}";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {dataConnection.Database}.table1 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {dataConnection.Database}.table2 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {dataConnection.Database}.table1_changes (" +
                $"count int AUTO_INCREMENT," +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(count))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {dataConnection.Database}.table2_changes (" +
                $"count int AUTO_INCREMENT," +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(count))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"select count(*) from {dataConnection.Database}.table2";
            int countRecords = Convert.ToInt32(cmd.ExecuteScalar());
            //Заполнение таблиц данными, если они отсутствуют
            if (countRecords == 0) {
                Console.WriteLine("Создаётся база данных...");
                var elements = ExcelUtility.GetElementsFromExcel(7);
                cmd.CommandText = $"insert into {dataConnection.Database}.table1 (id, name, state, comment) values " +
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

                elements = ExcelUtility.GetElementsFromExcel(17);
                cmd.CommandText = $"insert into {dataConnection.Database}.table2 (id, name, state, comment) values " +
                        $"(@id, @name, @state, @comment)";

                foreach (var element in elements) {
                    id.Value = element.ID;
                    name.Value = element.Name;
                    state.Value = element.State;
                    comment.Value = element.Comment;
                    cmd.ExecuteNonQuery();
                }

                //Создание триггеров дли отслеживания изменений в таблицах
                cmd.CommandText = $"use {dataConnection.Database}; " +
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

                cmd.CommandText = $"use {dataConnection.Database}; " +
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
            }
        }
        //Проверка элементов согласно логике в логика.xlsx
        void CheckElement() {
            Console.WriteLine("Проверка элементов...");
            var cmd = new MySqlCommand();
            cmd.Connection = connection;
            
            List<ExcelUtility.ElementInfoExcel> elements = ExcelUtility.GetLogicElement();
            foreach (var element in elements) {
                bool check = true;
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {dataConnection.Database}.{state.Key[1]} Where id = '{state.Key[0]}'";
                    int stateCheck = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (stateCheck != state.Value) {
                        check = false;
                        break;
                    }                      
                }
                //Если логика верна, изменяем элемент согласно ей  
                if (check) mainWindow.ChangeElement(element.Name, element.Code);
            }
        }
        //Проверка элементов согласно логики связей состояний ячеек в логика.xlsx
        void CheckRelationsElement() {
            var cmd = new MySqlCommand();
            cmd.Connection = connection;

            List<ExcelUtility.ElementInfoExcel> elements = ExcelUtility.GetLogicRelations();
            foreach (var element in elements) {
                bool check = true;
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {dataConnection.Database}.{state.Key[1]} Where id = '{state.Key[0]}'";
                    int stateCheck = Convert.ToInt32(cmd.ExecuteScalar());

                    if (stateCheck != state.Value) {
                        check = false;
                        break;
                    }
                }
                //Если логика верна, изменяем элемент согласно ей  
                cmd.CommandText = $"update {dataConnection.Database}.{element.Cell[1]} set state = {Convert.ToInt32(check)} " +
                        $"Where id = '{element.Cell[0]}' ";
                cmd.ExecuteNonQuery();
            }
        }
        //Вставка значения в таблицу 2 по нажатию кнопки
        public void InsertStateTable2(string code) {
            List<ExcelUtility.ElementInfoExcel> elements = ExcelUtility.GetStateButtons();
            var cmd = new MySqlCommand();
            cmd.Connection = connection;

            foreach (var element in elements.Where(e => e.Code == code)) {
                foreach(var state in element.States) {
                    cmd.CommandText = $"update {dataConnection.Database}.{state.Key[1]} set state = {state.Value} " +
                        $"Where id = '{state.Key[0]}' ";
                    //Console.WriteLine(cmd.CommandText);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        //Подключение к бд MySQL
        MySqlConnection GetDBConnection(ExcelUtility.DataConnectionMySQL dataConnection) {
            var connection = new MySqlConnection($"Server={dataConnection.Host};" +
                $"Port={dataConnection.Port};" +
                $"User id={dataConnection.Username};" +
                $"Password={dataConnection.Password}");

            return connection;
        }
    }
}

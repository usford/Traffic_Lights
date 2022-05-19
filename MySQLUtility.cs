using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;
using System.Windows.Controls;

namespace Traffic_Lights {
    class MySQLUtility {
        MainWindow mainWindow { get; set; }
        //Запуск работы с бд MySQL
        public MySQLUtility(MainWindow mainWindow) {
            this.mainWindow = mainWindow;
        }
        public void RunConnection() {
            ExcelUtility.DataConnectionMySQL dataConnection = ExcelUtility.GetConnection();
            MySqlConnection connection = GetDBConnection(dataConnection);
            
            connection.Open();

            try {
                CreateDB(connection, dataConnection);
                CheckElement(connection, dataConnection);
            }
            catch (Exception ex) {
                Console.WriteLine($"Error: {ex}");
            }
            finally {
                connection.Close();
                connection.Dispose();
            }
        }
        //Создание базы данных если её нет (с таблицами)
        void CreateDB(MySqlConnection connection, ExcelUtility.DataConnectionMySQL dataConnection) {
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
            }
            
        }
        //Проверка элементов согласно логике в excel файле
        void CheckElement(MySqlConnection connection, ExcelUtility.DataConnectionMySQL dataConnection) {
            Console.WriteLine("Проверка элементов...");
            var cmd = new MySqlCommand();
            cmd.Connection = connection;
            
            List<ExcelUtility.ElementInfoExcel> elements = ExcelUtility.GetLogicElement();

            foreach (var element in elements) {
                bool check = true;
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {dataConnection.Database}.table1 Where id = '{state.Key[0]}'";
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

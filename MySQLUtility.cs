using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Traffic_Lights {
    class MySQLUtility {
        //Запуск работы с бд MySQL
        public static void RunConnection() {
            ExcelUtility.DataConnectionMySQL dataConnection = ExcelUtility.GetConnection();
            MySqlConnection connection = GetDBConnection(dataConnection);
            
            connection.Open();

            try {
                CreateDB(connection, dataConnection);
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
        static void CreateDB(MySqlConnection connection, ExcelUtility.DataConnectionMySQL dataconnection) {
            var cmd = new MySqlCommand();
            cmd.Connection = connection;

            cmd.CommandText = $"create database if not exists {dataconnection.Database}";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {dataconnection.Database}.table1 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {dataconnection.Database}.table2 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"select count(*) from {dataconnection.Database}.table2";
            int countRecords = Convert.ToInt32(cmd.ExecuteScalar());
            //Заполнение таблиц данными, если они отсутствуют
            if (countRecords == 0) {
                Console.WriteLine("Создаётся база данных...");
                var elements = ExcelUtility.GetElementsFromExcel(7);
                cmd.CommandText = $"insert into {dataconnection.Database}.table1 (id, name, state, comment) values " +
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
                cmd.CommandText = $"insert into {dataconnection.Database}.table2 (id, name, state, comment) values " +
                        $"(@id, @name, @state, @comment)";

                foreach (var element in elements) {
                    id.Value = element.ID;
                    name.Value = element.Name;
                    state.Value = element.State;
                    comment.Value = element.Comment;
                    cmd.ExecuteNonQuery();
                }
            }
            CheckElement(connection, dataconnection);
        }
        //Проверка элемента согласно логике в excel файле
        private static void CheckElement(MySqlConnection connection, ExcelUtility.DataConnectionMySQL dataconnection) {
            Console.WriteLine("Проверка элементов...");
            var cmd = new MySqlCommand();
            cmd.Connection = connection;
            
            List<ExcelUtility.ElementInfoExcel> elements = ExcelUtility.GetLogicElement();

            foreach (var element in elements) {
                foreach (var state in element.States) {
                    cmd.CommandText = $"select state from {dataconnection.Database}.table1 Where id = '{state.Key[0]}'";
                    int check = Convert.ToInt32(cmd.ExecuteScalar());
                    //Если логика верна, изменяем элемент согласно ей
                    if (check == state.Value) {
                        Console.WriteLine(element.Name + " " + element.Code);
                        //new MainWindow().ChangeElement(element.Name, element.Code);
                    }
                }
            }
        }
        //Подключение к бд MySQL
        static MySqlConnection GetDBConnection(ExcelUtility.DataConnectionMySQL dataConnection) {
            var connection = new MySqlConnection($"Server={dataConnection.Host};" +
                $"Port={dataConnection.Port};" +
                $"User id={dataConnection.Username};" +
                $"Password={dataConnection.Password}");

            return connection;
        }
    }
}

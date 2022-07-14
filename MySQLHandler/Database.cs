using System;
using MySql.Data.MySqlClient;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Models;

namespace Traffic_Lights.MySQLHandler {
    public class Database {
        private IMySQLConnection _mySqlConnection;
        private TaskJob _taskJob;
        public Database(IMySQLConnection mySqlConnection, TaskJob taskJob) {
            _taskJob = taskJob;
            _mySqlConnection = mySqlConnection;
        }
        public void Create() {
            var cmd = new MySqlCommand();
            cmd.Connection = _mySqlConnection.Connection;

            cmd.CommandText = $"create database if not exists {_mySqlConnection.Database}";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.{_taskJob.Title}_table1 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.{_taskJob.Title}_table2 (" +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(id))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.{_taskJob.Title}_table1_changes (" +
                $"count int AUTO_INCREMENT," +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(count))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"create table if not exists {_mySqlConnection.Database}.{_taskJob.Title}_table2_changes (" +
                $"count int AUTO_INCREMENT," +
                $"id varchar(45) not null," +
                $"name varchar(45)," +
                $"state int," +
                $"comment varchar(45)," +
                $"primary key(count))";
            cmd.ExecuteNonQuery();

            cmd.CommandText = $"select count(*) from {_mySqlConnection.Database}.{_taskJob.Title}_table2";
            int countRecords = Convert.ToInt32(cmd.ExecuteScalar());
            //Заполнение таблиц данными, если они отсутствуют
            if (countRecords == 0) {
                Console.WriteLine("Создаётся база данных...");
                //var elements = _excelTaskJobRepository.GetElementsFromExcel(10);
                cmd.CommandText = $"insert into {_mySqlConnection.Database}.{_taskJob.Title}_table1 (id, name, state, comment) values " +
                        $"(@id, @name, @state, @comment)";
                var id = cmd.Parameters.Add("@id", MySqlDbType.String);
                var name = cmd.Parameters.Add("@name", MySqlDbType.String);
                var state = cmd.Parameters.Add("@state", MySqlDbType.Int32);
                var comment = cmd.Parameters.Add("@comment", MySqlDbType.String);

                foreach (var element in _taskJob.Tables[0]) {
                    id.Value = element.ID;
                    name.Value = element.Name;
                    state.Value = element.State;
                    comment.Value = element.Comment;
                    cmd.ExecuteNonQuery();
                }

                //elements = _excelTaskJobRepository.GetElementsFromExcel(20);
                cmd.CommandText = $"insert into {_mySqlConnection.Database}.{_taskJob.Title}_table2 (id, name, state, comment) values " +
                        $"(@id, @name, @state, @comment)";

                foreach (var element in _taskJob.Tables[1]) {
                    id.Value = element.ID;
                    name.Value = element.Name;
                    state.Value = element.State;
                    comment.Value = element.Comment;
                    cmd.ExecuteNonQuery();
                }

                //Создание триггеров дли отслеживания изменений в таблицах
                cmd.CommandText = $"use {_mySqlConnection.Database}; " +
                    $"create trigger {_taskJob.Title}_table1_update " +
                    $"after update on {_taskJob.Title}_table1 " +
                    $"for each row begin " +
                    $"insert into {_taskJob.Title}_table1_changes Set " +
                    $"id = NEW.id," +
                    $"name = NEW.name," +
                    $"state = NEW.state," +
                    $"comment = NEW.comment;" +
                    @$"end;";
                cmd.ExecuteNonQuery();

                cmd.CommandText = $"use {_mySqlConnection.Database}; " +
                    $"create trigger {_taskJob.Title}_table2_update " +
                    $"after update on {_taskJob.Title}_table2 " +
                    $"for each row begin " +
                    $"insert into {_taskJob.Title}_table2_changes Set " +
                    $"id = NEW.id," +
                    $"name = NEW.name," +
                    $"state = NEW.state," +
                    $"comment = NEW.comment;" +
                    @$"end;";
                cmd.ExecuteNonQuery();
                Console.WriteLine("База данных установлена");
            }
        }

    }
}

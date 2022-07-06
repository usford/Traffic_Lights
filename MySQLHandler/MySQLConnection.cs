using System;
using MySql.Data.MySqlClient;

namespace Traffic_Lights.MySQLHandler {
    public class MySQLConnection {
        public readonly string Host;
        public readonly short Port;
        public readonly string Username;
        public readonly string Password;
        public readonly string Database;
        public readonly short UpdateInterval;
        public MySqlConnection? Connection { get; set; }
        public MySQLConnection(string host, short port, string username, string password, string database, short updateInterval) {
            Host = host;
            Port = port;
            Username = username;
            Password = password;
            Database = database;
            UpdateInterval = updateInterval;
        }
        public void Start() {
            var connection = new MySqlConnection($"Server={Host};" +
                $"Port={Port};" +
                $"User id={Username};" +
                $"Password={Password}");

            Connection = connection;
        }
    }
}

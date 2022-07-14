using System;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.Interfaces;

namespace Traffic_Lights.MySQLHandler {
    public class MySQLConnection : IMySQLConnection {
        public string Host { get { return _host; } }
        private string _host = "";
        public short Port { get { return _port; } }
        private short _port;
        public string Username { get { return _username; } }
        private string _username = "";
        public string Password { get { return _password; } }
        private string _password = "";
        public string Database { get { return _database; } }
        private string _database = "";
        public short UpdateInterval { get { return _updateInterval; } }
        private short _updateInterval;
        public IConfigHandler ConfigHandler { get; }
        public MySqlConnection? Connection { get; set; }
        public MySQLConnection(IConfigHandler configHandler) {
            ConfigHandler = configHandler;
            GetConnectionFromExcel();
        }
        public void GetConnectionFromExcel() {
            using (var workbook = new XLWorkbook($@"{ConfigHandler.PathToExcelFiles}\Подключение к бд.xlsx")) {
                var worksheet = workbook.Worksheet(1);
                string host = Convert.ToString(worksheet.Cell(2, 2).Value) 
                    ?? throw new ArgumentNullException(nameof(host));

                short port = Convert.ToInt16(worksheet.Cell(3, 2).Value);

                string username = Convert.ToString(worksheet.Cell(4, 2).Value)
                    ?? throw new ArgumentNullException(nameof(username));

                string password = Convert.ToString(worksheet.Cell(5, 2).Value)
                    ?? throw new ArgumentNullException(nameof(password));

                string database = Convert.ToString(worksheet.Cell(6, 2).Value)
                    ?? throw new ArgumentNullException(nameof(database));
                short updateInterval = Convert.ToInt16(worksheet.Cell(7, 2).Value);

                _host = host;
                _port = port;
                _username = username;
                _password = password;
                _database = database;
                _updateInterval = updateInterval;
            }
        }
        public void Start() {
            var connection = new MySqlConnection($"Server={Host};" +
                $"Port={Port};" +
                $"User id={Username};" +
                $"Password={Password}");
            Connection = connection;
        }
        public void Open() {
            Console.WriteLine($"Запустился сервер. Host: {Host}, Port={Port}");
            Connection!.Open();
        }
        public void Close() {
            Connection!.Close();
        }
        public void Dispose() {
            Connection!.Dispose();
        }
    }
}

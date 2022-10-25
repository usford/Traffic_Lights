using System;
using MySql.Data.MySqlClient;

namespace Traffic_Lights.Interfaces {
    public interface IMySQLConnection {
        public string Host { get; }
        public short Port { get; }
        public string Username { get; }
        public string Password { get; }
        public string Database { get; }
        public short UpdateInterval { get; }
        public IConfigHandler ConfigHandler { get; }
        public MySqlConnection? Connection { get; set; }
        public void GetConnectionFromExcel();
        public void Start();
        public void Open();
        public void Close();
        public void Dispose();
    }
}

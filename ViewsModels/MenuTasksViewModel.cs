using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Traffic_Lights.Models;
using Traffic_Lights.Views;
using System.Threading;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Traffic_Lights.ViewsModels {
    public class MenuTasksViewModel : INotifyPropertyChanged {
        private List<TaskJobButton>? _taskJobButtons;
        private string _textMenuTasks = "";
        private string _setupText = "";
        private bool _installInProgress = false;
        private IConfigHandler _configHandler;
        private IMySQLConnection _mySqlConnection;
        public bool InstallInProgress {
            get { return _installInProgress; }
            set {
                _installInProgress = value;
                OnPropertyChanged("InstallInProgress");
            }
        }
        public string SetupText {
            get { return _setupText; }
            set {
                _setupText = value;
                OnPropertyChanged("SetupText");
            }
        }
        public string TextMenuTasks {
            get { return _textMenuTasks; }
            set {
                _textMenuTasks = value;
                OnPropertyChanged("TextMenuTasks");
            }
        }
        public List<TaskJobButton> TaskJobButtons {
            get { return _taskJobButtons!; }
            set {
                _taskJobButtons = value;
                OnPropertyChanged("TaskJobButtons");
            }
        }
        public MenuTasksViewModel(IConfigHandler configHandler) {
            _configHandler = configHandler;

            if (_configHandler.ConfigJson.isSetup) {
                var mySqlConnection = new MySQLConnection(_configHandler);
                mySqlConnection.Start();
                mySqlConnection.Open();
                _mySqlConnection = mySqlConnection;
                if (TaskJobButtons == null) TaskJobButtons = new TaskJobButtonList(new TaskJobRepository(_mySqlConnection, _configHandler)).taskList!;
                TextMenuTasks = "Переход в меню отдельных программ Задачи";
                SetupText = "Удалить";
            }
            else {
                SetupText = "Установить";
            }
        }
        public ICommand SetupClick {
            get {
                return new CommandHandler(() => SetupClickAction(), () => SetupClickCanExecute);
            }
        }
        private async void SetupClickAction() {
            //Удаление сервера
            if (_configHandler.ConfigJson.isSetup) {
                _configHandler.ConfigJson.isSetup = false;
                _configHandler.Update();
                SetupText = "Установить";
                TextMenuTasks = "";
                TaskJobButtons = null;
                Process proc = new Process();
                proc.StartInfo.FileName = @$"{_configHandler.PathToDirectory}\uninstallServer.bat";
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Verb = "runas";
                proc.Start();
                return;
            }
            InstallInProgress = true;
            //var sqlStart = System.Diagnostics.Process.Start(@$"{new DirectoryInfo(@"..\..\..\..").FullName}\mysqlserver.exe");
            Process? sqlStart = null;
            bool sqlSetup = false;
            if (!Directory.Exists(@"C:\MySQL Server 8.0"))
            {
                sqlStart = Process.Start(@$"{_configHandler.PathToDirectory}\MySQLHandler\mysqlserver.exe");
            }
            else
            {
                sqlSetup = true;
            }
            
            //Console.WriteLine("Установка программы");
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(3000));
            var timerProgress = new PeriodicTimer(TimeSpan.FromMilliseconds(900));
            bool setup = false;
            SetupBar setupBar = new SetupBar();
            setupBar.Show();

            string[] progressDescription = { 
                "Установка MySQL сервера...",
                "Чтение файла Шаблон БД...",
                "Подключение к MySQL серверу..."
            };

            int countIndex = 0;
            while (await timerProgress.WaitForNextTickAsync()) {
                if (countIndex == progressDescription.Length) {
                    break;
                }
                (setupBar.DataContext as SetupBarViewModel).SetupState = "Установка компонентов: \n" + progressDescription[countIndex++];
            }
            

            while (await timer.WaitForNextTickAsync()) {
                if (sqlStart != null) sqlSetup = sqlStart.HasExited;
                if (!setup && sqlSetup) {   
                    var mySqlConnection = new MySQLConnection(_configHandler);
                    mySqlConnection.Start();
                    mySqlConnection.Open();
                    _mySqlConnection = mySqlConnection;                    
                    setupBar.Close();
                    if (TaskJobButtons == null) _taskJobButtons = new TaskJobButtonList(new TaskJobRepository(_mySqlConnection, _configHandler)).taskList!;
                    var createDb = new CreateDb(_mySqlConnection, _taskJobButtons);
                    createDb.ShowDialog();
                    createDb.Close();
                    TaskJobButtons = new TaskJobButtonList(new TaskJobRepository(_mySqlConnection, _configHandler)).taskList!;
                    _configHandler.ConfigJson.isSetup = true;
                    _configHandler.Update();
                    setup = true;
                    InstallInProgress = false;
                    SetupText = "Удалить";
                    TextMenuTasks = "Переход в меню отдельных программ Задачи";
                }         
            }        
        }

        private bool SetupClickCanExecute {
            get { return true; }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

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

namespace Traffic_Lights.ViewsModels {
    public class MenuTasksViewModel : INotifyPropertyChanged {
        private List<TaskJobButton>? _taskJobButtons;
        private string _setupState = "";
        private string _textMenuTasks = "";
        private IConfigHandler _configHandler;
        private IMySQLConnection _mySqlConnection;
        public string SetupState {
            get { return _setupState; }
            set {
                _setupState = value;
                OnPropertyChanged("SetupState");
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
                SetupState = "Программа успешно установлена";
                TextMenuTasks = "Переход в меню отдельных программ Задачи";
            }
        }
        public ICommand SetupClick {
            get {
                return new CommandHandler(() => SetupClickAction(), () => SetupClickCanExecute);
            }
        }
        private async void SetupClickAction() {
            if (_configHandler.ConfigJson.isSetup) return;
            //var sqlStart = System.Diagnostics.Process.Start(@$"{new DirectoryInfo(@"..\..\..\..").FullName}\mysqlserver.exe");
            //var sqlStart = System.Diagnostics.Process.Start(@$"E:\mysqlserver.exe");
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
                if (!setup /*&& sqlStart.HasExited*/) {
                    setup = true;
                    var mySqlConnection = new MySQLConnection(_configHandler);
                    mySqlConnection.Start();
                    mySqlConnection.Open();
                    _mySqlConnection = mySqlConnection;
                    if (TaskJobButtons == null) TaskJobButtons = new TaskJobButtonList(new TaskJobRepository(_mySqlConnection, _configHandler)).taskList!;
                    SetupState = "Программа успешно установлена";
                    TextMenuTasks = "Переход в меню отдельных программ Задачи";
                    _configHandler.ConfigJson.isSetup = true;
                    _configHandler.Update();
                    setupBar.Close();
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

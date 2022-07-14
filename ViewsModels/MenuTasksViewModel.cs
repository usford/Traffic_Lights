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
        private IConfigHandler _configHandler;
        private IMySQLConnection _mySqlConnection;
        public string SetupState {
            get { return _setupState; }
            set {
                _setupState = value;
                OnPropertyChanged("SetupState");
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
            }
        }
        public ICommand SetupClick {
            get {
                return new CommandHandler(() => SetupClickAction(), () => SetupClickCanExecute);
            }
        }
        private async void SetupClickAction() {
            if (_configHandler.ConfigJson.isSetup) return;
            var sqlStart = System.Diagnostics.Process.Start(@$"{new DirectoryInfo(@"..\..\..\..").FullName}\mysqlserver.exe");
            //var sqlStart = System.Diagnostics.Process.Start(@$"E:\mysqlserver.exe");
            //Console.WriteLine("Установка программы");
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(3000));
            bool setup = false;
            SetupBar setupBar = new SetupBar();
            setupBar.Show();
            while (await timer.WaitForNextTickAsync()) {
                if (!setup && sqlStart.HasExited) {
                    setup = true;
                    var mySqlConnection = new MySQLConnection(_configHandler);
                    mySqlConnection.Start();
                    mySqlConnection.Open();
                    _mySqlConnection = mySqlConnection;
                    if (TaskJobButtons == null) TaskJobButtons = new TaskJobButtonList(new TaskJobRepository(_mySqlConnection, _configHandler)).taskList!;
                    SetupState = "Программа успешно установлена";
                    _configHandler.ConfigJson.isSetup = true;
                    _configHandler.Update();
                    setupBar.Hide();
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

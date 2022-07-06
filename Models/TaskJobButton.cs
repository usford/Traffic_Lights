﻿using System;
using System.Windows.Input;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;

namespace Traffic_Lights.Models {
    public class TaskJobButton {
        private ICommand _clickCommand;
        public TaskJob TaskJob { get; set; }
        private MySQLConnection _mySQLConnection;
        private ConfigHandler _configHandler;
        public TaskJobButton(TaskJob taskJob, MySQLConnection mySqlConnection, ConfigHandler configHandler) {
            TaskJob = taskJob;
            _mySQLConnection = mySqlConnection;
            _configHandler = configHandler;
            _clickCommand = new CommandHandler(() => MyAction(), () => CanExecute);
        }
        public ICommand ClickCommand {
            get { return _clickCommand; }
        }
        public bool CanExecute {
            get { return true; }
        }
        public void MyAction() {
            MainWindow mw = new MainWindow(_mySQLConnection, _configHandler);
            mw.Show();
        }
    }
}

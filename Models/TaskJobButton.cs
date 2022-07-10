using System;
using System.Windows.Input;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Interfaces;

namespace Traffic_Lights.Models {
    public class TaskJobButton {
        private ICommand _clickCommand;
        public TaskJob TaskJob { get; set; }
        private IConfigHandler _configHandler;
        public TaskJobButton(TaskJob taskJob, IConfigHandler configHandler) {
            TaskJob = taskJob;
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
            var mySqlConnection = new MySQLConnection(_configHandler);
            mySqlConnection.Start();
            mySqlConnection.Open();
            MainWindow mw = new MainWindow(mySqlConnection, _configHandler);
            mw.Show();
        }
    }
}

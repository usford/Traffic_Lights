using System;
using System.Windows.Input;
using Traffic_Lights.ConfigProgram;
using Traffic_Lights.MySQLHandler;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Views;

namespace Traffic_Lights.Models {
    public class TaskJobButton {
        private ICommand _clickCommand;
        public TaskJob TaskJob { get; set; }
        private IConfigHandler _configHandler;
        private IMySQLConnection _mySqlConnection;
        public TaskJobButton(TaskJob taskJob, IMySQLConnection mySqlConnection, IConfigHandler configHandler) {
            TaskJob = taskJob;
            _configHandler = configHandler;
            _mySqlConnection = mySqlConnection;
            _clickCommand = new CommandHandler(() => MyAction(), () => CanExecute);
        }
        public ICommand ClickCommand {
            get { return _clickCommand; }
        }
        public bool CanExecute {
            get { return true; }
        }
        public void MyAction() {
            //Database db = new Database(_mySqlConnection, TaskJob);
            //try {
            //    db.Create();
            //}
            //catch (Exception e) {
            //    var errorWindow = new ErrorWindow("Ошибка в создании базы данных.\n Проверьте правильность файла Шаблон.бд");
            //    errorWindow.ShowDialog();
            //}
            
            MainWindow mw = new MainWindow(_mySqlConnection, _configHandler, TaskJob.Title);
            mw.Show();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Models;
using Traffic_Lights.MySQLHandler;
using System.Threading;
using System.ComponentModel;

namespace Traffic_Lights.Views {
    public partial class CreateDb : Window {
        private IMySQLConnection _mySqlConnection;
        private List<TaskJobButton>? _taskJobButtons;
        private int count = 0;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public CreateDb(IMySQLConnection mySqlConnection, List<TaskJobButton> taskJobButtons) {
            InitializeComponent();
            _mySqlConnection = mySqlConnection;
            _taskJobButtons = taskJobButtons;
            Create();
            if (_taskJobButtons.Count > 0) {
                text_block.Text += $"Устанавливается бд {_taskJobButtons[0].TaskJob.Title}\n";
            }else {
                text_block.Text = "Нет доступных баз данных\n";
            }
        }

        public void Create() {
            if (_taskJobButtons.Count > 0) {
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;       
                worker.RunWorkerAsync();
            } 
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            var taskJobButton = _taskJobButtons[count];
            Database db = new Database(_mySqlConnection, taskJobButton.TaskJob);
            try {
                db.Create();
            }
            catch (Exception ex) {
                var errorWindow = new ErrorWindow($"Ошибка в создании базы данных для {taskJobButton.TaskJob.Title}.\n Проверьте правильность файла Шаблон.бд");
                errorWindow.ShowDialog();
            }
            count++;
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            var taskJobButton = _taskJobButtons[count - 1];
            //text_block.Text += $"Установка {taskJobButton.TaskJob.Title} прошла успешно\n";
            if (count != _taskJobButtons.Count) {
                text_block.Text = $"Устанавливается бд {_taskJobButtons[count].TaskJob.Title}\n";
                worker.RunWorkerAsync();
            }
            if (count == _taskJobButtons.Count) {
                Close();
            }
        }
    }
}

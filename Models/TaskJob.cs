using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Traffic_Lights.Models {
    public class TaskJob {
        private ICommand _clickCommand;
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public List<List<ExcelTaskJobRepository.ElementInfoDB>>? Tables { get; set; }
        public bool Enabled { get; set; }
        public ICommand ClickCommand {
            get {
                if (_clickCommand == null) {
                    _clickCommand = new CommandHandler(() => MyAction(), () => CanExecute);
                };
                return _clickCommand;
            }
        }
        public bool CanExecute {
            get { return true; }
        }
        public void MyAction() {
            Console.WriteLine(Tables.Count);
        }
        public TaskJob(string title, string comment, List<List<ExcelTaskJobRepository.ElementInfoDB>> tables, bool enabled) {
            Title = title;
            Comment = comment;
            Tables = tables;
            Enabled = enabled;
        }
    }
    public class TaskJobList {
        public TaskJobList(TaskJobRepository taskJobRepository) {
            taskList = taskJobRepository.GetTaskJobs();
        }
        public List<TaskJob>? taskList;

        public List<TaskJob>? FakeList() {
            TaskJob[] collectionTasks = new TaskJob[] {
                    //new TaskJob(
                    //    title: "Задача 0",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(7, 17),
                    //    enabled: true
                    //),
                    //new TaskJob(
                    //    title: "Задача 1",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(27, 37),
                    //    enabled: false
                    //),
                    //new TaskJob(
                    //    title: "Задача 2",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(47, 57),
                    //    enabled: false
                    //),
                    //new TaskJob(
                    //    title: "Задача 3",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(67, 77),
                    //    enabled: false
                    //),
                    //new TaskJob(
                    //    title: "Задача 4",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(87, 97),
                    //    enabled: false
                    //),
                    //new TaskJob(
                    //    title: "Задача 5",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(107, 117),
                    //    enabled: false
                    //),
                    //new TaskJob(
                    //    title: "Задача 6",
                    //    comment: "Коммент",
                    //    tables: MenuTasksModel.GetTaskTables(127, 137),
                    //    enabled: false
                    //),
                };
            taskList = new List<TaskJob>();
            taskList!.AddRange(collectionTasks);

            return taskList;
        }
    }
}

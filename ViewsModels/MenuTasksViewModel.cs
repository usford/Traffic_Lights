using System;
using System.Collections.Generic;
using Traffic_Lights.Models;
using System.Windows.Input;
using Traffic_Lights;

namespace Traffic_Lights.ViewsModels {
    public class MenuTasksViewModel {
        public MenuTasksViewModel() {
            TaskJobList = new TaskJobList(new ExcelTaskJobRepository());
        }
        public TaskJobList TaskJobList;
        public List<TaskJob> GetTaskJobList { get {
                return TaskJobList.taskList!;
        }}
    }
}

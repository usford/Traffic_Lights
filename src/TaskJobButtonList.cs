using System;
using System.Collections.Generic;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Models;

namespace Traffic_Lights {
    public class TaskJobButtonList {
        public List<TaskJobButton>? taskList;
        public TaskJobButtonList(ITaskJobRepository taskJobRepository) {
            taskList = taskJobRepository.GetTaskJobButtons();
        }
    }
}

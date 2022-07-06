using System;
using System.Collections.Generic;
using System.Windows.Input;
using Traffic_Lights.Interfaces;
using Traffic_Lights.Info;

namespace Traffic_Lights.Models {
    public class TaskJob {
        public string? Title { get; set; }
        public string? Comment { get; set; }
        public List<List<ElementInfoDB>>? Tables { get; set; }
        public bool Enabled { get; set; }
        public TaskJob(string title, string comment, List<List<ElementInfoDB>> tables, bool enabled) {
            Title = title;
            Comment = comment;
            Tables = tables;
            Enabled = enabled;
        }
    }
}

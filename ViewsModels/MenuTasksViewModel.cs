using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Traffic_Lights.Models;
using Traffic_Lights.Views;
using System.Threading;

namespace Traffic_Lights.ViewsModels {
    public class MenuTasksViewModel : INotifyPropertyChanged {
        private List<TaskJob> _taskJobs;
        private string _setupState;
        public string SetupState {
            get { return _setupState; }
            set {
                _setupState = value;
                OnPropertyChanged("SetupState");
            }
        }
        public List<TaskJob> TaskJobs {
            get { return _taskJobs; }
            set {
                _taskJobs = value;
                OnPropertyChanged("TaskJobs");
            }
        }
        public MenuTasksViewModel() {
        }
        public ICommand SetupClick {
            get {
                return new CommandHandler(() => SetupClickAction(), () => SetupClickCanExecute);
            }
        }
        private async void SetupClickAction() {
            Console.WriteLine("Установка программы");
            var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(3000));
            bool setup = false;
            SetupBar setupBar = new SetupBar();
            setupBar.Show();
            while (await timer.WaitForNextTickAsync()) {
                if (!setup) {
                    setup = true;
                    if (TaskJobs == null) TaskJobs = new TaskJobList(new ExcelTaskJobRepository()).taskList!;
                    SetupState = "Программа успешно установлена";
                    setupBar.Hide();
                }         
            }       
        }
        private bool SetupClickCanExecute {
            get { return true; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

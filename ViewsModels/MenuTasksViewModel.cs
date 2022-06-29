using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Traffic_Lights.Models;
using Traffic_Lights.Views;
using System.Threading;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace Traffic_Lights.ViewsModels {
    public class MenuTasksViewModel : INotifyPropertyChanged {
        private List<TaskJob> _taskJobs;
        private string _setupState;
        private ConfigJson _configJson;
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
            _configJson = JsonConvert.DeserializeObject<ConfigJson>(File.ReadAllText(@$"{MainWindow.pathDirectory}\config.json"));
            if (_configJson.isSetup) {
                if (TaskJobs == null) TaskJobs = new TaskJobList(new ExcelTaskJobRepository()).taskList!;
                SetupState = "Программа успешно установлена";
            }
            
        }
        public ICommand SetupClick {
            get {
                return new CommandHandler(() => SetupClickAction(), () => SetupClickCanExecute);
            }
        }
        private async void SetupClickAction() {
            if (_configJson.isSetup) return;
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
                    _configJson.isSetup = true;
                    string output = JsonConvert.SerializeObject(_configJson, Formatting.Indented);
                    File.WriteAllText(@$"{MainWindow.pathDirectory}\config.json", output);
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

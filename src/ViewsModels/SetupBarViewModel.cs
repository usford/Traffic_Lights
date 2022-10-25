using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Traffic_Lights.Interfaces;

namespace Traffic_Lights.ViewsModels {
    public class SetupBarViewModel : INotifyPropertyChanged, ISetupLogger {
        private string _setupState = "Установка компонентов:";

        public string SetupState {
            get { return _setupState; }
            set {
                _setupState = value;
                OnPropertyChanged("SetupState");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        
        public void OnPropertyChanged([CallerMemberName]string prop = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}

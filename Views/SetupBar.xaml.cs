using System;
using System.Threading;
using System.Windows;
using System.ComponentModel;

namespace Traffic_Lights.Views {
    /// <summary>
    /// Логика взаимодействия для SetupBar.xaml
    /// </summary>
    public partial class SetupBar : Window {
        public SetupBar() {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            //BackgroundWorker worker = new BackgroundWorker();
            //worker.WorkerReportsProgress = true;
            //worker.DoWork += worker_DoWork!;
            //worker.ProgressChanged += worker_ProgressChanged!;

            //worker.RunWorkerAsync();
        }
        //private void worker_DoWork(object sender, DoWorkEventArgs e) {
        //    for (int i = 0; i < 100; i++) {
        //        (sender as BackgroundWorker)!.ReportProgress(i);
        //        Thread.Sleep(100);
        //    }
        //}
        //private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
        //    pbStatus.Value = e.ProgressPercentage;
        //}
    }
}

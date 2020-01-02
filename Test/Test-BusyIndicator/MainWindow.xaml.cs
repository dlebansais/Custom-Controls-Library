using System;
using System.Threading;
using System.Windows;

namespace TestBusyIndicator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            StopTimer = new Timer(new TimerCallback(StopTimerCallback), this, TimeSpan.FromSeconds(5), Timeout.InfiniteTimeSpan);
        }

        private void StopTimerCallback(object parameter)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnStopTimer));
        }

        private void OnStopTimer()
        {
            Close();
        }

        private Timer StopTimer;
    }
}

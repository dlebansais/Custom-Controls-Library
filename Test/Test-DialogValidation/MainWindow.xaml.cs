namespace TestDialogValidation
{
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Loaded += OnLoaded;
            int CommandCount = 0;
            foreach (object Item in ctrl.ActualActiveCommands)
                CommandCount++;

            Debug.Assert(CommandCount == 2);
            Debug.WriteLine("Init Completed");
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ctrl.ActiveCommands.Clear();
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Ok);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Cancel);
        }

        public bool IsYesAdded
        {
            get { return ctrl.ActiveCommands.Count == 3; }
        }

        public bool IsHorizontal
        {
            get { return ctrl.Orientation == Orientation.Horizontal; }
        }

        private void OnOk(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnCanExecuteOk(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnCancel(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void OnIsLocalizedSet(object sender, RoutedEventArgs e)
        {
            if (!ctrl.IsLocalized)
                ctrl.IsLocalized = true;
        }

        private void OnIsLocalizedCleared(object sender, RoutedEventArgs e)
        {
            if (ctrl.IsLocalized)
                ctrl.IsLocalized = false;
        }

        private void OnAddYesSet(object sender, RoutedEventArgs e)
        {
            Debug.Assert(ctrl.ActiveCommands.Count == 2);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Yes);
            NotifyPropertyChanged(nameof(IsYesAdded));
        }

        private void OnAddYesCleared(object sender, RoutedEventArgs e)
        {
            Debug.Assert(ctrl.ActiveCommands.Count == 3);
            ctrl.ActiveCommands.RemoveAt(2);
            NotifyPropertyChanged(nameof(IsYesAdded));
        }

        private void OnHorizontalSet(object sender, RoutedEventArgs e)
        {
            if (ctrl.Orientation == Orientation.Vertical)
            {
                ctrl.Orientation = Orientation.Horizontal;
                NotifyPropertyChanged(nameof(IsHorizontal));
            }
        }

        private void OnHorizontalCleared(object sender, RoutedEventArgs e)
        {
            Debug.Assert(ctrl.Orientation == Orientation.Horizontal);
            ctrl.Orientation = Orientation.Vertical;
            NotifyPropertyChanged(nameof(IsHorizontal));
        }

        private void OnSetCustomCommands(object sender, RoutedEventArgs e)
        {
            Debug.Assert(((RoutedUICommand)ctrl.CommandOk).Text == "Ok");
            ctrl.CommandOk = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentOk == "OK");
            ctrl.ContentOk = "OK!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandCancel).Text == "Cancel");
            ctrl.CommandCancel = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentCancel == "Cancel");
            ctrl.ContentCancel = "Cancel!";
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

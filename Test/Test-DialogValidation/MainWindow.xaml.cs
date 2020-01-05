namespace TestDialogValidation
{
    using CustomControls;
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

            int CommandCount = 0;
            foreach (object Item in ctrl.ActualActiveCommands)
                CommandCount++;
            Debug.Assert(CommandCount == 2);

            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ActiveCommandCollection ActiveCommands = ctrl.ActiveCommands;

            ActiveCommands.Clear();
            ActiveCommands.Add(CustomControls.ActiveCommand.Ok);
            ActiveCommands.Add(CustomControls.ActiveCommand.Cancel);

            var CollectionConverter = TypeDescriptor.GetConverter(ActiveCommands);
            Debug.Assert(CollectionConverter.CanConvertFrom(typeof(string)));
            Debug.Assert(!CollectionConverter.CanConvertFrom(typeof(int)));
            Debug.Assert(CollectionConverter.ConvertTo(ActiveCommands, typeof(string)) is string);

            bool IsConvertedFromBad = true;
            try
            {
                CollectionConverter.ConvertFrom(0);
            }
            catch
            {
                IsConvertedFromBad = false;
            }

            Debug.Assert(!IsConvertedFromBad);

            bool IsConvertedToBad = true;
            try
            {
                CollectionConverter.ConvertTo(ActiveCommands, typeof(int));
            }
            catch
            {
                IsConvertedToBad = false;
            }

            Debug.Assert(!IsConvertedToBad);

            var Converter = TypeDescriptor.GetConverter(ActiveCommands[0]);
            Debug.Assert(Converter.CanConvertFrom(typeof(string)));
            Debug.Assert(!Converter.CanConvertFrom(typeof(int)));
            Debug.Assert(Converter.ConvertTo(ActiveCommands[0], typeof(string)) is string);

            IsConvertedFromBad = true;
            try
            {
                Converter.ConvertFrom(0);
            }
            catch
            {
                IsConvertedFromBad = false;
            }

            Debug.Assert(!IsConvertedFromBad);

            IsConvertedToBad = true;
            try
            {
                Converter.ConvertTo(ActiveCommands, typeof(int));
            }
            catch
            {
                IsConvertedToBad = false;
            }

            Debug.Assert(!IsConvertedToBad);
        }

        public bool IsYesAdded
        {
            get { return ctrl.ActiveCommands.Count != 2; }
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
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Abort);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Retry);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Ignore);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Yes);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.No);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Close);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Help);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.TryAgain);
            ctrl.ActiveCommands.Add(CustomControls.ActiveCommand.Continue);
            NotifyPropertyChanged(nameof(IsYesAdded));
        }

        private void OnAddYesCleared(object sender, RoutedEventArgs e)
        {
            Debug.Assert(ctrl.ActiveCommands.Count == 11);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
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

            Debug.Assert(((RoutedUICommand)ctrl.CommandAbort).Text == "Abort");
            ctrl.CommandAbort = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentAbort == "_Abort");
            ctrl.ContentAbort = "_Abort!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandRetry).Text == "Retry");
            ctrl.CommandRetry = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentRetry == "_Retry");
            ctrl.ContentRetry = "_Retry!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandIgnore).Text == "Ignore");
            ctrl.CommandIgnore = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentIgnore == "_Ignore");
            ctrl.ContentIgnore = "_Ignore!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandYes).Text == "Yes");
            ctrl.CommandYes = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentYes == "_Yes");
            ctrl.ContentYes = "_Yes!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandNo).Text == "No");
            ctrl.CommandNo = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentNo == "_No");
            ctrl.ContentNo = "_No!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandClose).Text == "Close");
            ctrl.CommandClose = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentClose == "_Close");
            ctrl.ContentClose = "_Close!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandHelp).Text == "Help");
            ctrl.CommandHelp = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentHelp == "Help");
            ctrl.ContentHelp = "Help!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandTryAgain).Text == "TryAgain");
            ctrl.CommandTryAgain = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentTryAgain == "_Try Again");
            ctrl.ContentTryAgain = "_Try Again!";

            Debug.Assert(((RoutedUICommand)ctrl.CommandContinue).Text == "Continue");
            ctrl.CommandContinue = new RoutedUICommand();
            Debug.Assert((string)ctrl.ContentContinue == "_Continue");
            ctrl.ContentContinue = "_Continue!";
        }

        #region Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
        #endregion
    }
}

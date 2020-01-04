namespace TestDialogValidation
{
    using System.Windows;
    using System.Windows.Input;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
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
            ctrl.IsLocalized = true;
        }

        private void OnIsLocalizedCleared(object sender, RoutedEventArgs e)
        {
            ctrl.IsLocalized = false;
        }
    }
}

namespace TestExtendedCommandControls
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

        private void TestCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
        }

        private void TestExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }
    }
}

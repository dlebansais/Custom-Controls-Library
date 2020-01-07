using System.Windows;

namespace TestEnumComboBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public TestEnum TestProperty { get; set; }

        private void OnEditableSet(object sender, RoutedEventArgs e)
        {
        }

        private void OnEditableCleared(object sender, RoutedEventArgs e)
        {
        }
    }
}

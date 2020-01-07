namespace TestEnumComboBox
{
    using System.Globalization;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            ctrl.NameConverterParameter = string.Empty;
            ctrl.NameConverterCulture = CultureInfo.InvariantCulture;
        }

        public TestEnum TestProperty { get; set; }

        private void OnNullSet(object sender, RoutedEventArgs e)
        {
            ctrl.SelectedIndex = -1;
            ctrl.EnumBinding = null;
        }
    }
}

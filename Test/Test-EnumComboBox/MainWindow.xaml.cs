namespace TestEnumComboBox
{
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

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
            OldEnumBinding = ctrl.EnumBinding;

            ctrl.EnumBinding = null;
            OldNameConverter = ctrl.NameConverter;
        }

        private void OnNullCleared(object sender, RoutedEventArgs e)
        {
            ctrl.SelectedIndex = 0;
            ctrl.EnumBinding = OldEnumBinding;
            if (OldNameConverter != null)
                ctrl.NameConverter = OldNameConverter;
        }

        private void OnBadSet(object sender, RoutedEventArgs e)
        {
            OldEnumBinding = ctrl.EnumBinding;

            ctrl.SelectedIndex = -1;
            ctrl.EnumBinding = BadBinding;
        }

        private void OnBadCleared(object sender, RoutedEventArgs e)
        {
            ctrl.SelectedIndex = 0;
            ctrl.EnumBinding = OldEnumBinding;
        }

        private object? OldEnumBinding = null;
        private IValueConverter? OldNameConverter = null;
        private int BadBinding = 0;
    }
}

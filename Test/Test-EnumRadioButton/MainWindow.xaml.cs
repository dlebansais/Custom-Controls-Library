namespace TestEnumRadioButton
{
    using System.Windows;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public TestEnum1 TestProperty { get; set; } = TestEnum1.Y;
        public TestEnum1 OtherTestProperty { get; set; } = TestEnum1.Z;
        public int BadProperty { get; set; } = 1;

        private void OnNullSet(object sender, RoutedEventArgs e)
        {
            OldEnumBinding = ctrlX.EnumBinding;

            ctrlX.EnumBinding = OtherTestProperty;
            ctrlY.EnumBinding = OtherTestProperty;
            ctrlZ.EnumBinding = OtherTestProperty;

            ctrlX.EnumBinding = null;
            ctrlY.EnumBinding = null;
            ctrlZ.EnumBinding = null;
        }

        private void OnNullCleared(object sender, RoutedEventArgs e)
        {
            ctrlX.EnumBinding = OldEnumBinding;
            ctrlY.EnumBinding = OldEnumBinding;
            ctrlZ.EnumBinding = OldEnumBinding;

            ctrlX.EnumValue = 0;
            ctrlY.EnumValue = 1;
            ctrlZ.EnumValue = 2;
        }

        private void OnBadSet(object sender, RoutedEventArgs e)
        {
            OldEnumBinding = ctrlX.EnumBinding;

            ctrlX.EnumValue = "X";
            ctrlY.EnumValue = "Y";
            ctrlZ.EnumValue = "Z";

            ctrlX.EnumBinding = BadBinding;
            ctrlY.EnumBinding = BadBinding;
            ctrlZ.EnumBinding = BadBinding;
        }

        private void OnBadCleared(object sender, RoutedEventArgs e)
        {
            ctrlX.EnumBinding = OldEnumBinding;
            ctrlY.EnumBinding = OldEnumBinding;
            ctrlZ.EnumBinding = OldEnumBinding;
        }

        private object? OldEnumBinding;
        private int BadBinding = 0;
    }
}

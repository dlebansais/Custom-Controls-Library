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
    }
}

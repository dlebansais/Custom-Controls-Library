namespace EnumRadioButtonDemo;

using System.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        BadBinding = 0;
    }

    /// <summary>
    /// Gets or sets the test property.
    /// </summary>
    public TestEnum1 TestProperty { get; set; } = TestEnum1.Y;

    /// <summary>
    /// Gets or sets another test property.
    /// </summary>
    public TestEnum1 OtherTestProperty { get; set; } = TestEnum1.Z;

    /// <summary>
    /// Gets or sets a bad property.
    /// </summary>
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
    private readonly int BadBinding;
}

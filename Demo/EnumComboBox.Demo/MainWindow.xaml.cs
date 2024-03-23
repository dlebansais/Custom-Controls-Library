namespace EnumComboBoxDemo;

using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
        ctrl.NameConverterParameter = string.Empty;
        ctrl.NameConverterCulture = CultureInfo.InvariantCulture;
    }

    /// <summary>
    /// Gets or sets the test property.
    /// </summary>
    public TestEnum1 TestProperty { get; set; }

    /// <summary>
    /// Gets or sets another test property.
    /// </summary>
    public TestEnum1 OtherTestProperty1 { get; set; }

    /// <summary>
    /// Gets or sets another test property.
    /// </summary>
    public TestEnum2 OtherTestProperty2 { get; set; }

    private void OnNullSet(object sender, RoutedEventArgs args)
    {
        OldEnumBinding = ctrl.EnumBinding;

        ctrl.EnumBinding = OtherTestProperty1;
        ctrl.EnumBinding = OtherTestProperty2;
        ctrl.SelectedIndex = -1;
        ctrl.SelectedIndex = 0;

        ctrl.EnumBinding = null;
        OldNameConverter = ctrl.NameConverter;
        ctrl.SelectedIndex = -1;
    }

    private void OnNullCleared(object sender, RoutedEventArgs args)
    {
        ctrl.SelectedIndex = 0;
        ctrl.EnumBinding = OldEnumBinding;
        if (OldNameConverter is not null)
            ctrl.NameConverter = OldNameConverter;
    }

    private void OnBadSet(object sender, RoutedEventArgs args)
    {
        OldEnumBinding = ctrl.EnumBinding;

        ctrl.EnumBinding = BadBinding;
        ctrl.SelectedIndex = -1;
    }

    private void OnBadCleared(object sender, RoutedEventArgs args)
    {
        ctrl.SelectedIndex = 0;
        ctrl.EnumBinding = OldEnumBinding;
    }

    private object? OldEnumBinding;
    private IValueConverter? OldNameConverter;
    private readonly int BadBinding;
}

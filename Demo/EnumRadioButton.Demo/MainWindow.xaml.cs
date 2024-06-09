namespace EnumRadioButton.Demo;

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CustomControls;

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

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        bool IsConverted;

        IsConverted = TestConverter(EnumRadioButton.EnumItems, Array.Empty<TestEnum1>(), typeof(TestEnum1));
        Debug.Assert(!IsConverted);

        IsConverted = TestConverter(EnumRadioButton.EnumName, TestEnum1.X.ToString(), typeof(TestEnum1));
        Debug.Assert(!IsConverted);

        IsConverted = TestConverter(new BooleanToVisibilityConverter(), Visibility.Visible, typeof(bool));
        Debug.Assert(IsConverted);
    }

    private static bool TestConverter(IValueConverter converter, object value, Type targetType)
    {
        try
        {
            _ = converter.ConvertBack(value, typeof(string), null!, CultureInfo.InvariantCulture);
            return true;
        }
        catch
        {
            return false;
        }
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

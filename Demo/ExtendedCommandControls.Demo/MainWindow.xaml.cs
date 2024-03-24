namespace ExtendedCommandControlsDemo;

using System.Windows;
using System.Windows.Input;

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
    }

    private void TestCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
    }

    private void TestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
    }
}

namespace ExtendedCommandControls.Demo;

using System.Windows;
using System.Windows.Input;
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

        Loaded += OnLoaded;
        ctrlTest6.IsActiveChanged += OnActiveChanged;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        ctrlTest6.IsDefaultActive = false;
        ctrlTest6.Reference = ctrlTest5.Reference;
        ctrlTest6.IsActive = !ctrlTest6.IsActive;
    }

    private void OnActiveChanged(object sender, RoutedEventArgs e)
    {
        ctrlTest6.IsActiveChanged -= OnActiveChanged;
        ctrlTest6.ResetIsActive();
        ctrlTest5.IsCheckable = !ctrlTest5.IsCheckable;

        ctrlTest5.IsDefaultActive = true;
        ctrlTest5.IsActive = true;
        ((ActiveDocumentRoutedCommand)ctrlTest5.Command).CommandGroup.IsEnabled = true;
    }

    private void TestCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = true;
    }

    private void TestExecuted(object sender, ExecutedRoutedEventArgs e)
    {
    }

    private void OnToolBarMenuLoaded(object sender, RoutedEventArgs e)
    {
        ExtendedToolBarMenuItem Ctrl = (ExtendedToolBarMenuItem)sender;
        Ctrl.CanShow = !Ctrl.CanShow;
    }
}

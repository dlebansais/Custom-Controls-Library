namespace ExtendedPasswordBox.Demo;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using CustomControls;

/// <summary>
/// Interaction logic for MainWindow.xaml.
/// </summary>
internal partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        if (TestEscape == 1)
            _ = Dispatcher.BeginInvoke(Cycle1);
        else if (TestEscape == 2)
            _ = Dispatcher.BeginInvoke(Cycle2);
        else if (TestEscape == 3)
            _ = Dispatcher.BeginInvoke(Cycle3);
    }

    private async void Cycle1()
    {
        Debug.Assert(!ctrl.ShowPassword);
        Debug.Assert(ctrl.IsPasswordEmpty);

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(true);

        isVisible.IsChecked = true;
        Debug.Assert(ctrl.ShowPassword);

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(true);

        ctrl.Text = "test";
        Debug.Assert(ctrl.Text == "test");
        Debug.Assert(!ctrl.IsPasswordEmpty);

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(true);

        ExtendedPassword Password = ctrl.Password;
        Debug.Assert(Password.Password == "test");
        Debug.Assert(!Password.IsSecure);
        Debug.Assert(Password.SecurePassword.Length == 0);

        isVisible.IsChecked = false;
        Debug.Assert(!ctrl.ShowPassword);

        ctrl.Text = string.Empty;
        Debug.Assert(ctrl.Text == string.Empty);
        Debug.Assert(ctrl.IsPasswordEmpty);

        ctrl.Focus();

        ctrl.ShowPassword = true;
        Debug.Assert(ctrl.ShowPassword);

        ctrl.Focus();

        ctrl.ShowPassword = false;
        Debug.Assert(!ctrl.ShowPassword);

        ctrl.Text = "test";
        Debug.Assert(ctrl.Text == "test");
        Debug.Assert(!ctrl.IsPasswordEmpty);
    }

    private async void Cycle2()
    {
        Debug.Assert(!ctrl.ShowPassword);
        Debug.Assert(ctrl.IsPasswordEmpty);

        await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(true);

        ExtendedPassword Password = ctrl.Password;
        Debug.Assert(Password.IsSecure);
        Debug.Assert(Password.SecurePassword.Length == 0);

        ctrl.MarkAsInsecure();

        Password = ctrl.Password;
        Debug.Assert(!Password.IsSecure);

        ctrl.MarkAsInsecure();
    }

    private void Cycle3()
        => _ = ctrl.SetBinding(ExtendedPasswordBox.TextProperty, nameof(TestProperty));

    /// <summary>
    /// Gets or sets a simple test property.
    /// </summary>
    public string TestProperty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the test escape.
    /// </summary>
    public static int TestEscape { get; set; }
}

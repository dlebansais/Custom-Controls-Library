namespace EditableTextBlockDemo;

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CustomControls;

/// <summary>
/// Main window of the EditableTextBlockDemo program.
/// </summary>
public partial class MainWindow : Window, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        Debug.Assert(ctrl.ClickDelay == EditableTextBlock.DefaultClickDelay);
        ctrl.ClickDelay = EditableTextBlock.DefaultClickDelay;
        Debug.Assert(ctrl.Editable);
        ctrl.Editable = true;
        Debug.Assert(!ctrl.IsEditing);
        ctrl.IsEditing = false;

        ctrl.Editable = false;
        ctrl.IsEditing = true;
        ctrl.IsEditing = false;
        ctrl.Editable = true;

        ctrl.EditEnter += OnEditEnter;
        ctrl.TextChanged += OnTextChanged;
        ctrl.EditLeave += OnEditLeave;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (TestEscape == 1)
        {
            using (TestDisposeWindow Dlg = new())
            {
                _ = Dlg.ShowDialog();
            }

            int DoubleClickTime = System.Windows.Forms.SystemInformation.DoubleClickTime;
            ctrl.ClickDelay = TimeSpan.FromMilliseconds(DoubleClickTime);
            ctrl.Focusable = false;
        }

        _ = Dispatcher.BeginInvoke(OnLoadedDone);
    }

    private void OnLoadedDone()
    {
        string s = ctrl.Text;

        Debug.Assert(s == "Init");
        ctrl.Text = "Init";
    }

    /// <summary>
    /// Gets the editable text.
    /// </summary>
    public string EditableText { get; } = "Init";

    private void OnEditEnter(object sender, RoutedEventArgs e)
    {
        EditableTextBlockEventArgs Args = (EditableTextBlockEventArgs)e;

        if (TestEscape == 4)
            Args.Cancel();
        else if (TestEscape > 0)
            EscapeTimer = new Timer(new TimerCallback(EscapeTimerCallback), this, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
    }

    private void OnTextChanged(object sender, RoutedEventArgs e)
    {
        EditableTextBlockEventArgs Args = (EditableTextBlockEventArgs)e;

        if (TestEscape == 5)
            Args.Cancel();
    }

    private void OnEditLeave(object sender, RoutedEventArgs e)
    {
        EditLeaveEventArgs Args = (EditLeaveEventArgs)e;
        if (TestEscape == 3)
            Args.Cancel();
    }

    private void OnEditableSet(object sender, RoutedEventArgs e)
    {
        if (!ctrl.Editable)
            ctrl.Editable = true;
    }

    private void OnEditableCleared(object sender, RoutedEventArgs e)
    {
        if (ctrl.Editable)
            ctrl.Editable = false;
    }

    /// <summary>
    /// Gets or sets the test escape.
    /// </summary>
    public static int TestEscape { get; set; }

    private void EscapeTimerCallback(object? parameter)
    {
        _ = Dispatcher.BeginInvoke(OnEscapeTimer);
    }

    private void OnEscapeTimer()
    {
        switch (EscapeStep)
        {
            case 0:
                SendKey(Key.X);
                break;

            case 1:
                if (TestEscape == 1)
                    SendEscapeKey();
                else if (TestEscape is 2 or 3 or 5)
                    SendReturnKey();
                break;

            case 2:
                ctrl.EditEnter -= OnEditEnter;
                ctrl.TextChanged -= OnTextChanged;
                ctrl.EditLeave -= OnEditLeave;
                break;

            default:
                _ = EscapeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                Close();
                break;
        }

        EscapeStep++;
    }

    private static void SendKey(Key key)
    {
        KeyEventArgs e;

        e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
        };
        _ = InputManager.Current.ProcessInput(e);

        e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
        {
            RoutedEvent = Keyboard.PreviewKeyUpEvent,
        };
        _ = InputManager.Current.ProcessInput(e);
    }

    private static void SendEscapeKey()
    {
        var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Escape)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
        };
        _ = InputManager.Current.ProcessInput(e);
    }

    private static void SendReturnKey()
    {
        var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
        };
        _ = InputManager.Current.ProcessInput(e);
    }

    private Timer EscapeTimer = new(new TimerCallback((object? parameter) => { }));
    private int EscapeStep;
    private bool disposedValue;

    /// <summary>
    /// Disposes of resource.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> if resource should be disposed; Ottherwise, <see langword="false"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                EscapeTimer.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

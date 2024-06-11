namespace DispatcherLagMeter.Demo;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

/// <summary>
/// Window used to test disposing.
/// </summary>
public partial class TestDisposeWindow : Window, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestDisposeWindow"/> class.
    /// </summary>
    public TestDisposeWindow()
    {
        InitializeComponent();
        DataContext = this;

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _ = Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, Close);
    }

    #region Implementation of IDisposable
    /// <summary>
    /// Called when an object should release its resources.
    /// </summary>
    /// <param name="isDisposing">Indicates if resources must be disposed now.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (!IsDisposed)
        {
            IsDisposed = true;

            if (isDisposing)
                DisposeNow();
        }
    }

    /// <summary>
    /// Called when an object should release its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="TestDisposeWindow"/> class.
    /// </summary>
    ~TestDisposeWindow()
    {
        Dispose(false);
    }

    /// <summary>
    /// True after <see cref="Dispose(bool)"/> has been invoked.
    /// </summary>
    private bool IsDisposed;

    /// <summary>
    /// Disposes of every reference that must be cleaned up.
    /// </summary>
    private void DisposeNow()
    {
        ctrl.Dispose();
        ctrl.Dispose(); // Try to dispose twice, to test this corner case.
    }
    #endregion
}

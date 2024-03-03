namespace DialogValidationDemo;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CustomControls;

/// <summary>
/// Main window of the DialogValidationDemo program.
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged, IDisposable
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        Debug.Assert(ctrl.ActualActiveCommands.Count == 2);

        Loaded += OnLoaded;

        if (TestUnset)
            UnsetTimer = new Timer(new TimerCallback(UnsetTimerCallback), this, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2));
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        ActiveCommandCollection ActiveCommands = ctrl.ActiveCommands;

        ActiveCommands.Clear();
        ActiveCommands.Add(ActiveCommand.Ok);
        ActiveCommands.Add(ActiveCommand.Cancel);

        TypeConverter CollectionConverter = TypeDescriptor.GetConverter(ActiveCommands);

        Debug.Assert(CollectionConverter.CanConvertFrom(typeof(string)));
        Debug.Assert(!CollectionConverter.CanConvertFrom(typeof(int)));

        bool IsConvertedFrom;
        bool IsConvertedTo;

        ConvertActiveCommandCollection(CollectionConverter, "Ok", out IsConvertedFrom, ActiveCommands, out IsConvertedTo);
        Debug.Assert(IsConvertedFrom);
        Debug.Assert(IsConvertedTo);

        ConvertActiveCommandCollection(CollectionConverter, 0, out IsConvertedFrom, ActiveCommands, out IsConvertedTo);
        Debug.Assert(!IsConvertedFrom);
        Debug.Assert(!IsConvertedTo);

        TypeConverter Converter = TypeDescriptor.GetConverter(ActiveCommands[0]);
        Debug.Assert(Converter.CanConvertFrom(typeof(string)));
        Debug.Assert(!Converter.CanConvertFrom(typeof(int)));

        ConvertActiveCommand(Converter, "Ok", out IsConvertedFrom, ActiveCommands[0], out IsConvertedTo);
        Debug.Assert(IsConvertedFrom);
        Debug.Assert(IsConvertedTo);

        ConvertActiveCommand(Converter, 0, out IsConvertedFrom, ActiveCommands[0], out IsConvertedTo);
        Debug.Assert(!IsConvertedFrom);
        Debug.Assert(!IsConvertedTo);

        string SystemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string User32Path = Path.Combine(SystemPath, "user32.dll");

        IList<string>? LoadedResources;

        LoadedResources = DialogValidation.LoadStringFromResourceFile(string.Empty, 1);
        Debug.Assert(LoadedResources is not null);
        Debug.Assert(!LoadedResources.Any());

        LoadedResources = DialogValidation.LoadStringFromResourceFile(User32Path, 9999);
        Debug.Assert(LoadedResources is not null);
        Debug.Assert(!LoadedResources.Any());
    }

    private static void ConvertActiveCommandCollection(TypeConverter collectionConverter, object from, out bool isConvertedFrom, ActiveCommandCollection to, out bool isConvertedTo)
    {
        try
        {
            _ = collectionConverter.ConvertFrom(from);
            isConvertedFrom = true;
        }
        catch
        {
            isConvertedFrom = false;
        }

        try
        {
            _ = collectionConverter.ConvertTo(to, from.GetType());
            isConvertedTo = true;
        }
        catch
        {
            isConvertedTo = false;
        }
    }

    private static void ConvertActiveCommand(TypeConverter converter, object from, out bool isConvertedFrom, ActiveCommand to, out bool isConvertedTo)
    {
        try
        {
            _ = converter.ConvertFrom(from);
            isConvertedFrom = true;
        }
        catch
        {
            isConvertedFrom = false;
        }

        try
        {
            _ = converter.ConvertTo(to, from.GetType());
            isConvertedTo = true;
        }
        catch
        {
            isConvertedTo = false;
        }
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets a value indicating whether Unset is tested.
    /// </summary>
    public static bool TestUnset { get; set; }

    /// <summary>
    /// Gets a value indicating whether the Yes command is added.
    /// </summary>
    public bool IsYesAdded => ctrl.ActiveCommands.Count != 2;

    /// <summary>
    /// Gets a value indicating whether the orientation is horizontal.
    /// </summary>
    public bool IsHorizontal => ctrl.Orientation == Orientation.Horizontal;

    /// <summary>
    /// Gets a value indicating whether the set command was executed.
    /// </summary>
    public bool IsSetCommandAvailable { get; private set; } = true;
    #endregion

    #region Event handlers
    private void OnOk(object sender, ExecutedRoutedEventArgs args)
    {
        Close();
    }

    private void OnCanExecuteOk(object sender, CanExecuteRoutedEventArgs args)
    {
        args.CanExecute = true;
    }

    private void OnCancel(object sender, ExecutedRoutedEventArgs args)
    {
        Close();
    }

    private void OnIsLocalizedSet(object sender, RoutedEventArgs args)
    {
        if (!ctrl.IsLocalized)
            ctrl.IsLocalized = true;
    }

    private void OnIsLocalizedCleared(object sender, RoutedEventArgs args)
    {
        if (ctrl.IsLocalized)
            ctrl.IsLocalized = false;
    }

    private void OnAddYesSet(object sender, RoutedEventArgs args)
    {
        if (ctrl.ActiveCommands.Count == 2)
        {
            ctrl.ActiveCommands.Add(ActiveCommand.Abort);
            ctrl.ActiveCommands.Add(ActiveCommand.Retry);
            ctrl.ActiveCommands.Add(ActiveCommand.Ignore);
            ctrl.ActiveCommands.Add(ActiveCommand.Yes);
            ctrl.ActiveCommands.Add(ActiveCommand.No);
            ctrl.ActiveCommands.Add(ActiveCommand.Close);
            ctrl.ActiveCommands.Add(ActiveCommand.Help);
            ctrl.ActiveCommands.Add(ActiveCommand.TryAgain);
            ctrl.ActiveCommands.Add(ActiveCommand.Continue);
            NotifyPropertyChanged(nameof(IsYesAdded));
        }
    }

    private void OnAddYesCleared(object sender, RoutedEventArgs args)
    {
        if (ctrl.ActiveCommands.Count == 11)
        {
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            ctrl.ActiveCommands.RemoveAt(2);
            NotifyPropertyChanged(nameof(IsYesAdded));
        }
    }

    private void OnHorizontalSet(object sender, RoutedEventArgs args)
    {
        if (ctrl.Orientation == Orientation.Vertical)
        {
            ctrl.Orientation = Orientation.Horizontal;
            NotifyPropertyChanged(nameof(IsHorizontal));
        }
    }

    private void OnHorizontalCleared(object sender, RoutedEventArgs args)
    {
        Debug.Assert(ctrl.Orientation == Orientation.Horizontal);
        ctrl.Orientation = Orientation.Vertical;
        NotifyPropertyChanged(nameof(IsHorizontal));
    }

    private void OnSetCustomCommands(object sender, RoutedEventArgs args)
    {
        Debug.Assert(((RoutedUICommand)ctrl.CommandOk).Text == "Ok");
        ctrl.CommandOk = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentOk == "OK");
        ctrl.ContentOk = "OK!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandCancel).Text == "Cancel");
        ctrl.CommandCancel = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentCancel == "Cancel");
        ctrl.ContentCancel = "Cancel!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandAbort).Text == "Abort");
        ctrl.CommandAbort = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentAbort == "_Abort");
        ctrl.ContentAbort = "_Abort!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandRetry).Text == "Retry");
        ctrl.CommandRetry = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentRetry == "_Retry");
        ctrl.ContentRetry = "_Retry!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandIgnore).Text == "Ignore");
        ctrl.CommandIgnore = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentIgnore == "_Ignore");
        ctrl.ContentIgnore = "_Ignore!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandYes).Text == "Yes");
        ctrl.CommandYes = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentYes == "_Yes");
        ctrl.ContentYes = "_Yes!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandNo).Text == "No");
        ctrl.CommandNo = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentNo == "_No");
        ctrl.ContentNo = "_No!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandClose).Text == "Close");
        ctrl.CommandClose = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentClose == "_Close");
        ctrl.ContentClose = "_Close!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandHelp).Text == "Help");
        ctrl.CommandHelp = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentHelp == "Help");
        ctrl.ContentHelp = "Help!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandTryAgain).Text == "TryAgain");
        ctrl.CommandTryAgain = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentTryAgain == "_Try Again");
        ctrl.ContentTryAgain = "_Try Again!";

        Debug.Assert(((RoutedUICommand)ctrl.CommandContinue).Text == "Continue");
        ctrl.CommandContinue = new RoutedUICommand();
        Debug.Assert((string)ctrl.ContentContinue == "_Continue");
        ctrl.ContentContinue = "_Continue!";

        IsSetCommandAvailable = false;
        NotifyPropertyChanged(nameof(IsSetCommandAvailable));
    }
    #endregion

    #region Implementation
    private void UnsetTimerCallback(object? parameter)
    {
        _ = Dispatcher.BeginInvoke(OnUnsetTimer);
    }

    private void OnUnsetTimer()
    {
        switch (UnsetStep++)
        {
            case 0:
                OnAddYesSet(this, new RoutedEventArgs());
                break;

            case 1:
                OnAddYesCleared(this, new RoutedEventArgs());
                break;

            case 2:
                OnIsLocalizedSet(this, new RoutedEventArgs());
                break;

            default:
                _ = UnsetTimer.Change(Timeout.Infinite, Timeout.Infinite);
                Close();
                break;
        }
    }

    private readonly Timer UnsetTimer = new(new TimerCallback((object? parameter) => { }));
    private int UnsetStep;
    private bool disposedValue;
    #endregion

    #region Implementation of INotifyPropertyChanged
    /// <summary>
    /// The PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Notifies that a property is changed.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void NotifyPropertyChanged(string propertyName)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
    #endregion

    #region Implementation of IDisposable
    /// <summary>
    /// Disposes of managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing"><see langword="True"/> if the method should dispose of resources; Otherwise, <see langword="false"/>.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                UnsetTimer.Dispose();
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
    #endregion
}

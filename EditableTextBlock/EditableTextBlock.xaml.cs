namespace CustomControls;

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

/// <summary>
/// Represents a text block that can be edited, for instance to rename a file.
/// Implemented as a normal, styleable TextBlock replaced by a TextBox when the user clicks on it.
/// Features:
/// . The delay between click and editing can be changed.
/// . The focus must be on a parent of the TextBlock for editing to occur.
/// . Reports events such as entering and leaving edit mode. User's actions can be canceled.
/// . The TextBlock and TextBox can be styled independently.
/// . Editing begins with the entire text selected.
/// . Editing mode is left if one of the following occurs:
///   . The TextBox looses the focus.
///   . The selector (listbox) hosting the control becomes inactive.
///   . The user press one of the following keys:
///   . Return, to validate the change.
///   . Escape, to cancel the change.
/// </summary>
public partial class EditableTextBlock : UserControl, IDisposable
{
    #region Constants
    /// <summary>
    /// Gets the delay before editing happens, to ignore double-click.
    /// </summary>
    public static TimeSpan DefaultClickDelay { get; } = TimeSpan.FromSeconds(0.4);
    #endregion

    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="EditableTextBlock"/> class.
    /// </summary>
    public EditableTextBlock()
    {
        InitializeComponent();
        InitializeEditing();
        InitializeImplementation();
    }
    #endregion

    #region Editing
    /// <summary>
    /// Proceeds to the initialization of properties related to editing.
    /// </summary>
    private void InitializeEditing() => _ = Dispatcher.BeginInvoke(InitPositioning);

    /// <summary>
    /// Schedule a timer event to switch to editing mode after a given time.
    /// </summary>
    private void ScheduleStartEditing()
    {
        if (!Editable)
            return;

        // Get the double-click delay from the user's preferences, set in the Windows Control Panel
        int DoubleClickTime = System.Windows.Forms.SystemInformation.DoubleClickTime;
        TimeSpan MinimumDelay = TimeSpan.FromMilliseconds(DoubleClickTime);

        TimeSpan ActualDelay;

        if (ClickDelay >= MinimumDelay)
            ActualDelay = ClickDelay;
        else
            ActualDelay = MinimumDelay;

        _ = StartEditingTimer.Change(ActualDelay, Timeout.InfiniteTimeSpan);
    }

    /// <summary>
    /// Cancel the timer event scheduled by ScheduleStartEditing, for instance in case of a double-click.
    /// </summary>
    private void CancelStartEditing() => _ = StartEditingTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

    /// <summary>
    /// Called when the timer event scheduled by ScheduleStartEditing occurs.
    /// Executed in the context of a timer thread. Reschedule the event to run in the graphic thread context.
    /// </summary>
    /// <param name="parameter">This parameter is not used.</param>
    private void StartEditingTimerCallback(object? parameter) => _ = Dispatcher.BeginInvoke(DispatcherPriority.Normal, new StartEditingHandler(OnStartEditing));

    /// <summary>
    /// Handler of a rescheduled timer event.
    /// </summary>
    private delegate void StartEditingHandler();

    /// <summary>
    /// Called when the timer event scheduled by ScheduleStartEditing occurs.
    /// Executed in the context of the graphic thread.
    /// Attempt to switch the control to editing state, if IsEditable is true and no event handler cancels it.
    /// </summary>
    protected virtual void OnStartEditing()
    {
        using CancellationTokenSource Cancellation = new();
        NotifyEditEnter(Cancellation);

        if (!Cancellation.Token.IsCancellationRequested)
            SetValue(IsEditingProperty, true);
    }

    /// <summary>
    /// Switch the control to non-editing state. This is not cancellable.
    /// </summary>
    protected virtual void OnStopEditing()
    {
        using CancellationTokenSource Cancellation = new();
        NotifyEditLeave(Cancellation, true);

        SetValue(IsEditingProperty, false);
    }

    /// <summary>
    /// Changes the internal TextBlock and TextBox controls to visually show the edit box.
    /// </summary>
    private void SwitchToTextBox()
    {
        ctrlTextBlock.Visibility = Visibility.Collapsed;

        ctrlTextBox.Text = ctrlTextBlock.Text;
        ctrlTextBox.CaretIndex = 0;
        ctrlTextBox.Visibility = Visibility.Visible;
        _ = ctrlTextBox.Focus();
        ctrlTextBox.Select(0, ctrlTextBox.Text.Length);

        // Install a handler to be notifed if we're part of a selection and the selection control looses focus.
        IsSelectionActiveDescriptor.AddValueChanged(this, OnIsSelectionActiveChanged);
    }

    /// <summary>
    /// Changes the internal TextBlock and TextBox controls to visually hide the edit box.
    /// </summary>
    private void SwitchToTextBlock()
    {
        IsSelectionActiveDescriptor.RemoveValueChanged(this, OnIsSelectionActiveChanged);

        ctrlTextBox.Visibility = Visibility.Collapsed;
        ctrlTextBlock.Visibility = Visibility.Visible;
    }

    /// <summary>
    /// Handler of a first initialization event.
    /// </summary>
    private delegate void InitPositioningHandler();

    /// <summary>
    /// Initializes the control once to align the TextBlock and TextBox controls.
    /// </summary>
    private void InitPositioning()
    {
        StartEditingTimer = new Timer(new TimerCallback(StartEditingTimerCallback));
        ctrlTextBox.Padding = new Thickness(Math.Max(0.0, ctrlTextBox.Padding.Left - 1), Math.Max(0.0, ctrlTextBox.Padding.Top - 1), Math.Max(0.0, ctrlTextBox.Padding.Right - 1), Math.Max(0.0, ctrlTextBox.Padding.Bottom - 1));
    }

    /// <summary>
    /// Cancel editing the control if focus moved to another focus zone.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains no event data.</param>
    private void OnIsSelectionActiveChanged(object? sender, EventArgs args) => OnStopEditing();

    /// <summary>
    /// Timer used to schedule even event when the user clicks the control.
    /// </summary>
    private Timer StartEditingTimer = new(new TimerCallback((object? parameter) => { }));

    /// <summary>
    /// Represent the IsSelectionActive attached property.
    /// </summary>
    private readonly DependencyPropertyDescriptor IsSelectionActiveDescriptor = DependencyPropertyDescriptor.FromProperty(Selector.IsSelectionActiveProperty, typeof(EditableTextBlock));
    #endregion

    #region Implementation
    /// <summary>
    /// Proceeds to the initialization of properties related to the control implementation.
    /// </summary>
    private void InitializeImplementation() => ResetClickCount();

    /// <summary>
    /// Called when the user clicks the left mouse button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains no event data.</param>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
    {
        if (args.ClickCount < 2)
        {
            ResetClickCount();

            if (Focusable)
                LastFocusedParent = FocusedParent();
        }
        else
        {
            CancelStartEditing();
            LastFocusedParent = null;
        }
    }

    /// <summary>
    /// Called when the user releases the left mouse button.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains no event data.</param>
    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
    {
        IncrementClickCount();

        if (IsClickCountSimple())
        {
            bool IsStart = !Focusable || LastFocusedParent == FocusedParent();
            if (IsStart)
            {
                ScheduleStartEditing();
                LastFocusedParent = null;
            }
        }
    }

    /// <summary>
    /// Search in the parent chain a UIElement that has the focus.
    /// </summary>
    /// <returns>TRUE if a parent has the focus, FALSE if none was found.</returns>
    private UIElement? FocusedParent()
    {
        DependencyObject Current = this;

        while (Current is UIElement AsUIElement)
        {
            if (AsUIElement.IsFocused)
                return AsUIElement;

            Current = VisualTreeHelper.GetParent(Current);
        }

        return null;
    }

    /// <summary>
    /// Called when the edit box looses focus.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An object that contains no event data.</param>
    private void OnEditLostFocus(object sender, RoutedEventArgs args) => OnStopEditing();

    /// <summary>
    /// Called when the user presses a key on the keyboard.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">State of the key pressed.</param>
    private void OnEditPreviewKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key is Key.Return)
        {
            using CancellationTokenSource Cancellation = new();
            NotifyEditLeave(Cancellation, false);

            if (!Cancellation.Token.IsCancellationRequested)
            {
                SetValue(IsEditingProperty, false);

                NotifyTextChanged(ctrlTextBox.Text, Cancellation);

                if (!Cancellation.Token.IsCancellationRequested)
                    SetValue(TextProperty, ctrlTextBox.Text);
            }

            args.Handled = true;
        }
        else if (args.Key is Key.Escape)
        {
            OnStopEditing();

            args.Handled = true;
        }
    }

    private UIElement? LastFocusedParent;
    #endregion

    #region Click Count
    /// <summary>
    /// Reset the click count to its base value.
    /// </summary>
    private void ResetClickCount() => UpClickCount = 0;

    /// <summary>
    /// Increment the click count to take the user click into account.
    /// </summary>
    private void IncrementClickCount() => UpClickCount++;

    /// <summary>
    /// Check that this is a simple click.
    /// </summary>
    /// <returns>True if the user click is not a double click.</returns>
    private bool IsClickCountSimple() => UpClickCount < 2;

    /// <summary>
    /// Contains a count of mouse clicks, used to decide when it is time to start editing.
    /// </summary>
    private int UpClickCount;
    #endregion

    #region Implementation of IDisposable
    /// <summary>
    /// Called when an object should release its resources.
    /// </summary>
    /// <param name="disposing">Indicates if resources must be disposed now.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                StartEditingTimer.Dispose();
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// Called when an object should release its resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private bool disposedValue;
    #endregion
}

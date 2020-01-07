namespace CustomControls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
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
        /// Delay before editing happens, to ignore double-click.
        /// </summary>
        public static readonly TimeSpan DefaultClickDelay = TimeSpan.FromSeconds(0.4);
        #endregion

        #region Custom properties and events
        #region ClickDelay
        /// <summary>
        /// Identifies the <see cref="ClickDelay"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="ClickDelay"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ClickDelayProperty = DependencyProperty.Register("ClickDelay", typeof(TimeSpan), typeof(EditableTextBlock), new FrameworkPropertyMetadata(DefaultClickDelay), new ValidateValueCallback(IsValidClickDelay));

        /// <summary>
        /// Gets or sets The delay between a click and the actual switch to editing mode.
        /// There is a minimum delay corresponding to the system double-click time.
        /// Only a time span greater than or equal to zero is valid.
        /// </summary>
        public TimeSpan ClickDelay
        {
            get { return (TimeSpan)GetValue(ClickDelayProperty); }
            set { SetValue(ClickDelayProperty, value); }
        }

        /// <summary>
        /// Checks if a click delay is valid.
        /// </summary>
        /// <param name="value">The instance to check.</param>
        /// <returns>True if the delay is valid; Otherwise, false.</returns>
        internal static bool IsValidClickDelay(object value)
        {
            TimeSpan Delay = (TimeSpan)value;
            return Delay >= TimeSpan.Zero;
        }
        #endregion
        #region Editable
        /// <summary>
        /// Identifies the <see cref="Editable"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="Editable"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EditableProperty = DependencyProperty.Register("Editable", typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether the user can click on the control to start editing.
        /// True, the user can click on the control to start editing (or the application can initiate it any other way).
        /// False, the control cannot be edited and the value of IsEditing is ignored.
        /// </summary>
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }
        #endregion
        #region Is Editing
        /// <summary>
        /// Identifies the <see cref="IsEditing"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="IsEditing"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register("IsEditing", typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(false, OnIsEditingChanged));

        /// <summary>
        /// Gets or sets a value indicating whether the text is being edited.
        /// True, the text is being edited. The application can start editing by writing this value.
        /// False, the text is displayed as a normal TextBlock.
        /// </summary>
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        /// <summary>
        /// Called when the <see cref="IsEditing"/> dependency property is changed on <paramref name="modifiedObject"/>.
        /// </summary>
        /// <param name="modifiedObject">The object that had its property modified.</param>
        /// <param name="e">Information about the change.</param>
        private static void OnIsEditingChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
        {
            EditableTextBlock ctrl = (EditableTextBlock)modifiedObject;
            ctrl.OnIsEditingChanged();
        }

        /// <summary>
        /// Called when the <see cref="IsEditing"/> dependency property is changed.
        /// </summary>
        private void OnIsEditingChanged()
        {
            if (Editable)
                if (IsEditing)
                    SwitchToTextBox();
                else
                    SwitchToTextBlock();
        }
        #endregion
        #region Edit Enter event
        /// <summary>
        /// Identifies the <see cref="EditEnter"/> routed event.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="EditEnter"/> routed event.
        /// </returns>
        public static readonly RoutedEvent EditEnterEvent = EventManager.RegisterRoutedEvent("EditEnter", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBlock));

        /// <summary>
        /// Sent when the control is about to enter editing mode because of a user action (clicking the control).
        /// If canceled, the control does not enter editing mode and IsEditing remains false.
        /// </summary>
        public event RoutedEventHandler EditEnter
        {
            add { AddHandler(EditEnterEvent, value); }
            remove { RemoveHandler(EditEnterEvent, value); }
        }

        /// <summary>
        /// Sends a <see cref="EditEnter"/> event.
        /// </summary>
        /// <param name="cancellation">A token to hold cancellation information.</param>
        protected virtual void NotifyEditEnter(CancellationToken cancellation)
        {
            EditableTextBlockEventArgs Args = CreateEditEnterEvent(ctrlTextBlock.Text, cancellation);
            RaiseEvent(Args);
        }

        /// <summary>
        /// Creates arguments for the EditEnter routed event.
        /// </summary>
        /// <param name="textToEdit">The current content of the control.</param>
        /// <param name="cancellation">A token to hold cancellation information.</param>
        /// <returns>The EditableTextBlockEventArgs object created.</returns>
        protected virtual EditableTextBlockEventArgs CreateEditEnterEvent(string textToEdit, CancellationToken cancellation)
        {
            return new EditableTextBlockEventArgs(EditEnterEvent, this, textToEdit, cancellation);
        }
        #endregion
        #region Edit Leave event
        /// <summary>
        /// Identifies the <see cref="EditLeave"/> routed event.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="EditLeave"/> routed event.
        /// </returns>
        public static readonly RoutedEvent EditLeaveEvent = EventManager.RegisterRoutedEvent("EditLeave", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBlock));

        /// <summary>
        /// Sent when the control is about to leave editing mode because of a user action (hitting the Return or Escape key, or changing the focus)
        /// If the user has validated the new text (with the Return key), IsEditCanceled is false, otherwise it is true.
        /// Leaving edit mode can only be canceled if IsEditCanceled is false.
        /// If canceled, the control does not leave editing mode and IsEditing remains true.
        /// </summary>
        public event RoutedEventHandler EditLeave
        {
            add { AddHandler(EditLeaveEvent, value); }
            remove { RemoveHandler(EditLeaveEvent, value); }
        }

        /// <summary>
        /// Sends a <see cref="EditLeave"/> event.
        /// </summary>
        /// <param name="cancellation">A token to hold cancellation information.</param>
        /// <param name="isEditCanceled">A value that indicates if editing has been canceled.</param>
        protected virtual void NotifyEditLeave(CancellationToken cancellation, bool isEditCanceled)
        {
            EditLeaveEventArgs Args = CreateEditLeaveEvent(ctrlTextBox.Text, cancellation, isEditCanceled);
            RaiseEvent(Args);
        }

        /// <summary>
        /// Creates arguments for the EditLeave routed event.
        /// </summary>
        /// <param name="newText">The current content of the control.</param>
        /// <param name="cancellation">A token to hold cancellation information.</param>
        /// <param name="isEditCanceled">A value that indicates if editing has been canceled.</param>
        /// <returns>The EditableTextBlockEventArgs object created.</returns>
        protected virtual EditLeaveEventArgs CreateEditLeaveEvent(string newText, CancellationToken cancellation, bool isEditCanceled)
        {
            return new EditLeaveEventArgs(EditLeaveEvent, this, newText, cancellation, isEditCanceled);
        }
        #endregion
        #region Text
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="Text"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the text displayed by the control. Does not change while the user is editing it.
        /// The new value is reported after the user has pressed the Return key.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        #endregion
        #region Text Changed event
        /// <summary>
        /// Identifies the <see cref="TextChanged"/> routed event.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="TextChanged"/> routed event.
        /// </returns>
        public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent("TextChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBlock));

        /// <summary>
        /// Reports that the user pressed the Return key to validate a change. The Text content may have not been modified.
        /// The control has left editing mode before this event is sent.
        /// If canceled, the previous text is not replaced.
        /// </summary>
        public event RoutedEventHandler TextChanged
        {
            add { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }

        /// <summary>
        /// Sends a <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="newText">The current content of the control.</param>
        /// <param name="cancellation">A token to hold cancellation information.</param>
        protected virtual void NotifyTextChanged(string newText, CancellationToken cancellation)
        {
            EditableTextBlockEventArgs Args = CreateTextChangedEvent(newText, cancellation);
            RaiseEvent(Args);
        }

        /// <summary>
        /// Creates arguments for the TextChanged routed event.
        /// </summary>
        /// <param name="newText">The current content of the control.</param>
        /// <param name="cancellation">A token to hold cancellation information.</param>
        /// <returns>The EditableTextBlockEventArgs object created.</returns>
        protected virtual EditableTextBlockEventArgs CreateTextChangedEvent(string newText, CancellationToken cancellation)
        {
            return new EditableTextBlockEventArgs(TextChangedEvent, this, newText, cancellation);
        }
        #endregion
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
        private void InitializeEditing()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new InitPositioningHandler(InitPositioning));
        }

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

            StartEditingTimer.Change(ActualDelay, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Cancel the timer event scheduled by ScheduleStartEditing, for instance in case of a double-click.
        /// </summary>
        private void CancelStartEditing()
        {
            StartEditingTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// Called when the timer event scheduled by ScheduleStartEditing occurs.
        /// Executed in the context of a timer thread. Reschedule the event to run in the graphic thread context.
        /// </summary>
        /// <param name="parameter">This parameter is not used.</param>
        private void StartEditingTimerCallback(object parameter)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new StartEditingHandler(OnStartEditing));
        }

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
            CancellationToken Cancellation = new CancellationToken();
            NotifyEditEnter(Cancellation);

            if (!Cancellation.IsCanceled)
                SetValue(IsEditingProperty, true);
        }

        /// <summary>
        /// Switch the control to non-editing state. This is not cancellable.
        /// </summary>
        protected virtual void OnStopEditing()
        {
            CancellationToken Cancellation = new CancellationToken();
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
            ctrlTextBox.Focus();
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
        /// <param name="e">An object that contains no event data.</param>
        private void OnIsSelectionActiveChanged(object sender, EventArgs e)
        {
            OnStopEditing();
        }

        /// <summary>
        /// Timer used to schedule even event when the user clicks the control.
        /// </summary>
        private Timer StartEditingTimer = new Timer(new TimerCallback((object parameter) => { }));

        /// <summary>
        /// Represent the IsSelectionActive attached property.
        /// </summary>
        private DependencyPropertyDescriptor IsSelectionActiveDescriptor = DependencyPropertyDescriptor.FromProperty(Selector.IsSelectionActiveProperty, typeof(EditableTextBlock));
        #endregion

        #region Implementation
        /// <summary>
        /// Proceeds to the initialization of properties related to the control implementation.
        /// </summary>
        private void InitializeImplementation()
        {
            ResetClickCount();
        }

        /// <summary>
        /// Called when the user clicks the left mouse button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
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
        /// <param name="e">An object that contains no event data.</param>
        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IncrementClickCount();

            if (IsClickCountSimple())
            {
                if (!Focusable || LastFocusedParent == FocusedParent())
                {
                    ScheduleStartEditing();
                    LastFocusedParent = null;
                }
                else
                {
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

            while (Current != null)
            {
                if (Current is UIElement AsUIElement)
                {
                    if (AsUIElement.IsFocused)
                        return AsUIElement;
                }

                Current = VisualTreeHelper.GetParent(Current);
            }

            return null;
        }

        /// <summary>
        /// Called when the edit box looses focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An object that contains no event data.</param>
        private void OnEditLostFocus(object sender, RoutedEventArgs e)
        {
            OnStopEditing();
        }

        /// <summary>
        /// Called when the user presses a key on the keyboard.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">State of the key pressed.</param>
        private void OnEditPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                CancellationToken Cancellation = new CancellationToken();
                NotifyEditLeave(Cancellation, false);

                if (!Cancellation.IsCanceled)
                {
                    SetValue(IsEditingProperty, false);

                    NotifyTextChanged(ctrlTextBox.Text, Cancellation);

                    if (!Cancellation.IsCanceled)
                        SetValue(TextProperty, ctrlTextBox.Text);
                }

                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                OnStopEditing();

                e.Handled = true;
            }
        }

        private UIElement? LastFocusedParent = null;
        #endregion

        #region Click Count
        /// <summary>
        /// Reset the click count to its base value.
        /// </summary>
        private void ResetClickCount()
        {
            UpClickCount = 0;
        }

        /// <summary>
        /// Increment the click count to take the user click into account.
        /// </summary>
        private void IncrementClickCount()
        {
            UpClickCount++;
        }

        /// <summary>
        /// Check that this is a simple click.
        /// </summary>
        /// <returns>True if the user click is not a double click.</returns>
        private bool IsClickCountSimple()
        {
            return UpClickCount < 2;
        }

        /// <summary>
        /// Contains a count of mouse clicks, used to decide when it is time to start editing.
        /// </summary>
        private int UpClickCount;
        #endregion

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
        /// Finalizes an instance of the <see cref="EditableTextBlock"/> class.
        /// </summary>
        ~EditableTextBlock()
        {
            Dispose(false);
        }

        /// <summary>
        /// True after <see cref="Dispose(bool)"/> has been invoked.
        /// </summary>
        private bool IsDisposed = false;

        /// <summary>
        /// Disposes of every reference that must be cleaned up.
        /// </summary>
        private void DisposeNow()
        {
            StartEditingTimer.Dispose();
        }
        #endregion
    }
}

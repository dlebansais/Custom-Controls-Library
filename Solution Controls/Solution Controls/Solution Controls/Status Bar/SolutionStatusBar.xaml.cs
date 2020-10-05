namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Represents a control displaying the status of a solution.
    /// </summary>
    public partial class SolutionStatusBar : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Custom properties and events
        #region Theme
        /// <summary>
        /// Identifies the <see cref="Theme"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(IStatusTheme), typeof(SolutionStatusBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the status theme.
        /// </summary>
        public IStatusTheme Theme
        {
            get { return (IStatusTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        #endregion
        #region Max Active Status Count
        /// <summary>
        /// Identifies the <see cref="MaxActiveStatusCount"/> attached property.
        /// </summary>
        public static readonly DependencyProperty MaxActiveStatusCountProperty = DependencyProperty.Register("MaxActiveStatusCount", typeof(int), typeof(SolutionStatusBar), new PropertyMetadata(5));

        /// <summary>
        /// Gets or sets the max active status count.
        /// </summary>
        public int MaxActiveStatusCount
        {
            get { return (int)GetValue(MaxActiveStatusCountProperty); }
            set { SetValue(MaxActiveStatusCountProperty, value); }
        }
        #endregion
        #region Active Status List
        /// <summary>
        /// Identifies the <see cref="ActiveStatusList"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ActiveStatusListProperty = ActiveStatusListPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey ActiveStatusListPropertyKey = DependencyProperty.RegisterReadOnly("ActiveStatusList", typeof(IReadOnlyCollection<IApplicationStatus>), typeof(SolutionStatusBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets the list of active statuses.
        /// </summary>
        public IReadOnlyCollection<IApplicationStatus> ActiveStatusList
        {
            get { return (IReadOnlyCollection<IApplicationStatus>)GetValue(ActiveStatusListProperty); }
        }
        #endregion
        #region Current Status
        /// <summary>
        /// Identifies the <see cref="CurrentStatus"/> attached property.
        /// </summary>
        public static readonly DependencyProperty CurrentStatusProperty = CurrentStatusPropertyKey.DependencyProperty;
        private static readonly DependencyPropertyKey CurrentStatusPropertyKey = DependencyProperty.RegisterReadOnly("CurrentStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets the current status.
        /// </summary>
        public IApplicationStatus CurrentStatus
        {
            get { return (IApplicationStatus)GetValue(CurrentStatusProperty); }
        }
        #endregion
        #region Default Initializing Status
        /// <summary>
        /// Identifies the <see cref="DefaultInitializingStatus"/> attached property.
        /// </summary>
        public static readonly DependencyProperty DefaultInitializingStatusProperty = DependencyProperty.Register("DefaultInitializingStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the default status when initializing.
        /// </summary>
        public IApplicationStatus DefaultInitializingStatus
        {
            get { return (IApplicationStatus)GetValue(DefaultInitializingStatusProperty); }
            set { SetValue(DefaultInitializingStatusProperty, value); }
        }
        #endregion
        #region Default Ready Status
        /// <summary>
        /// Identifies the <see cref="DefaultReadyStatus"/> attached property.
        /// </summary>
        public static readonly DependencyProperty DefaultReadyStatusProperty = DependencyProperty.Register("DefaultReadyStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the default ready status.
        /// </summary>
        public IApplicationStatus DefaultReadyStatus
        {
            get { return (IApplicationStatus)GetValue(DefaultReadyStatusProperty); }
            set { SetValue(DefaultReadyStatusProperty, value); }
        }
        #endregion
        #region Default Failure Status
        /// <summary>
        /// Identifies the <see cref="DefaultFailureStatus"/> attached property.
        /// </summary>
        public static readonly DependencyProperty DefaultFailureStatusProperty = DependencyProperty.Register("DefaultFailureStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the default failure status.
        /// </summary>
        public IApplicationStatus DefaultFailureStatus
        {
            get { return (IApplicationStatus)GetValue(DefaultFailureStatusProperty); }
            set { SetValue(DefaultFailureStatusProperty, value); }
        }
        #endregion
        #region Progress Value
        /// <summary>
        /// Identifies the <see cref="ProgressValue"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double), typeof(SolutionStatusBar), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the progress value.
        /// </summary>
        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        #endregion
        #region Progress Max
        /// <summary>
        /// Identifies the <see cref="ProgressMax"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ProgressMaxProperty = DependencyProperty.Register("ProgressMax", typeof(double), typeof(SolutionStatusBar), new PropertyMetadata(0.0));

        /// <summary>
        /// Gets or sets the maximum value for progress.
        /// </summary>
        public double ProgressMax
        {
            get { return (double)GetValue(ProgressMaxProperty); }
            set { SetValue(ProgressMaxProperty, value); }
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionStatusBar"/> class.
        /// </summary>
        public SolutionStatusBar()
        {
            Initialized += OnInitialized; // Dirty trick to avoid warning CA2214.

            InitializeComponent();
            InitializeActiveStatusList();
            InitializeDefaultStatus();
            InitializeFocusTracker();
        }

        /// <summary>
        /// Called when the control has been initialized and before properties are set.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            InitializeTheme();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether there is a caret.
        /// </summary>
        public bool HasCaret
        {
            get { return HasCaretInternal; }
            protected set
            {
                if (HasCaretInternal != value)
                {
                    HasCaretInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private bool HasCaretInternal;

        /// <summary>
        /// Gets or sets the caret line.
        /// </summary>
        public int CaretLine
        {
            get { return CaretLineInternal; }
            protected set
            {
                if (CaretLineInternal != value)
                {
                    CaretLineInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private int CaretLineInternal;

        /// <summary>
        /// Gets or sets the caret column.
        /// </summary>
        public int CaretColumn
        {
            get { return CaretColumnInternal; }
            protected set
            {
                if (CaretColumnInternal != value)
                {
                    CaretColumnInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private int CaretColumnInternal;

        /// <summary>
        /// Gets or sets a value indicating whether the caret is in override mode.
        /// </summary>
        public bool IsCaretOverride
        {
            get { return IsCaretOverrideInternal; }
            protected set
            {
                if (IsCaretOverrideInternal != value)
                {
                    IsCaretOverrideInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private bool IsCaretOverrideInternal;
        #endregion

        #region Implementation
        /// <summary>
        /// Sets the last status message.
        /// </summary>
        /// <param name="lastStatus">The last status message.</param>
        protected virtual void SetLastStatusMessage(IApplicationStatus lastStatus)
        {
            SetValue(CurrentStatusPropertyKey, lastStatus);
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Sets the current status.
        /// </summary>
        /// <param name="status">The status.</param>
        public virtual void SetStatus(IApplicationStatus status)
        {
            if (status != null && !StatusList.Contains(status))
                StatusList.Add(status);
            while (StatusList.Count > 1 && StatusList.Count > MaxActiveStatusCount)
                StatusList.RemoveAt(0);

            Dispatcher.BeginInvoke(new Action<IApplicationStatus>(OnSetStatus), status);
        }

        /// <summary>
        /// Updates the status after it's been changed with <see cref="SetStatus(IApplicationStatus)"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        protected virtual void OnSetStatus(IApplicationStatus status)
        {
            SetLastStatusMessage(status);
            ClearProgress();
        }

        /// <summary>
        /// Resets the current status.
        /// </summary>
        /// <param name="status">The status.</param>
        public virtual void ResetStatus(IApplicationStatus status)
        {
            if (status != null && StatusList.Contains(status))
                StatusList.Remove(status);

            Dispatcher.BeginInvoke(new Action(OnResetStatus));
        }

        /// <summary>
        /// Updates the status after it's been changed with <see cref="ResetStatus(IApplicationStatus)"/>.
        /// </summary>
        protected virtual void OnResetStatus()
        {
            IApplicationStatus LastStatus = (StatusList.Count > 0) ? StatusList[StatusList.Count - 1] : DefaultReadyStatus;
            SetLastStatusMessage(LastStatus);
            ClearProgress();
        }

        /// <summary>
        /// Sets the current status to failure.
        /// </summary>
        public virtual void SetFailureStatus()
        {
            Dispatcher.BeginInvoke(new Action(OnSetFailureStatus));
        }

        /// <summary>
        /// Updates the status after it's been changed with <see cref="ResetStatus"/>.
        /// </summary>
        protected virtual void OnSetFailureStatus()
        {
            SetLastStatusMessage(DefaultFailureStatus);
        }

        /// <summary>
        /// Clears the progress.
        /// </summary>
        protected virtual void ClearProgress()
        {
            ProgressValue = 0;
            ProgressMax = 0;
        }
        #endregion

        #region Theme
        /// <summary>
        /// Initializes the theme.
        /// </summary>
        protected virtual void InitializeTheme()
        {
            Theme = new StatusTheme();
        }
        #endregion

        #region Active Status List
        private void InitializeActiveStatusList()
        {
            SetValue(ActiveStatusListPropertyKey, StatusList);
        }

        private Collection<IApplicationStatus> StatusList = new Collection<IApplicationStatus>();
        #endregion

        #region Default Status
        private void InitializeDefaultStatus()
        {
            DefaultInitializingStatus = new ApplicationStatus(SolutionControlsInternal.Properties.Resources.StatusInitializing, StatusType.Busy);
            DefaultReadyStatus = new ApplicationStatus(SolutionControlsInternal.Properties.Resources.StatusReady, StatusType.Normal);
            DefaultFailureStatus = new ApplicationStatus(SolutionControlsInternal.Properties.Resources.StatusFailure, StatusType.Failure);
        }
        #endregion

        #region Focus Tracker
        private void InitializeFocusTracker()
        {
            HasCaretInternal = false;
            CaretLineInternal = 0;
            CaretColumnInternal = 0;
            IsCaretOverrideInternal = false;
            FindFocusedOperation = null;
            TrackingStarted = false;
            FindFocusedTimer = new Timer(new TimerCallback(FindFocusedTimerCallback), this, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Called when <see cref="FindFocusedTimer"/> elapses.
        /// </summary>
        /// <param name="parameter">The timer parameter.</param>
        protected virtual void FindFocusedTimerCallback(object parameter)
        {
            if (FindFocusedOperation == null || FindFocusedOperation.Status == DispatcherOperationStatus.Completed)
                FindFocusedOperation = Dispatcher.BeginInvoke(new Action(OnFindFocusedTimer));
        }

        /// <summary>
        /// Called when <see cref="FindFocusedTimer"/> elapses.
        /// </summary>
        protected virtual void OnFindFocusedTimer()
        {
            if (!TrackingStarted && Keyboard.FocusedElement is IInputElement FocusedElement)
            {
                FindFocusedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                StartTrackingFocus(FocusedElement);
            }
        }

        /// <summary>
        /// Starts tracking the focus.
        /// </summary>
        /// <param name="focusedElement">The focused element.</param>
        protected virtual void StartTrackingFocus(IInputElement focusedElement)
        {
            TrackingStarted = true;
            AddHandlers(focusedElement);
        }

        /// <summary>
        /// Adds handlers to the focused element.
        /// </summary>
        /// <param name="focusedElement">The focused element.</param>
        protected virtual void AddHandlers(IInputElement focusedElement)
        {
            if (focusedElement == null)
                throw new ArgumentNullException(nameof(focusedElement));

            focusedElement.LostKeyboardFocus += OnLostKeyboardFocus;

            if (focusedElement is IDocumentEditor AsOldEditor)
            {
                UpdateCaretInfo(AsOldEditor);
                AsOldEditor.CaretPositionChanged += OnCaretPositionChanged;
            }
            else
                ResetCaretInfo();
        }

        /// <summary>
        /// Removes handlers from the focused element.
        /// </summary>
        /// <param name="focusedElement">The focused element.</param>
        protected virtual void RemoveHandlers(IInputElement focusedElement)
        {
            if (focusedElement == null)
                throw new ArgumentNullException(nameof(focusedElement));

            focusedElement.LostKeyboardFocus -= OnLostKeyboardFocus;

            if (focusedElement is IDocumentEditor AsOldEditor)
                AsOldEditor.CaretPositionChanged -= OnCaretPositionChanged;
        }

        /// <summary>
        /// Called when the focused element has lost focus.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e != null && e.NewFocus != null)
            {
                RemoveHandlers(e.OldFocus);
                AddHandlers(e.NewFocus);
            }
        }

        /// <summary>
        /// Called when the caret position has changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnCaretPositionChanged(object sender, EventArgs e)
        {
            UpdateCaretInfo((IDocumentEditor)sender);
        }

        /// <summary>
        /// Updates caret info in the status bar.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        protected virtual void UpdateCaretInfo(IDocumentEditor sender)
        {
            if (sender != null)
            {
                HasCaret = true;
                CaretLine = sender.CaretLine;
                CaretColumn = sender.CaretColumn;
                IsCaretOverride = sender.IsCaretOverride;
            }
        }

        /// <summary>
        /// Resets the caret info.
        /// </summary>
        protected virtual void ResetCaretInfo()
        {
            HasCaret = false;
            CaretLine = 0;
            CaretColumn = 0;
            IsCaretOverride = false;
        }

        /// <summary>
        /// Gets the dispatcher operation associated to <see cref="FindFocusedTimer"/>.
        /// </summary>
        protected DispatcherOperation? FindFocusedOperation { get; private set; }

        /// <summary>
        /// Gets the timer updating focus info.
        /// </summary>
        protected Timer FindFocusedTimer { get; private set; } = new Timer(new TimerCallback((object parameter) => { }));

        /// <summary>
        /// Gets a value indicating whether tracking the focus has started.
        /// </summary>
        protected bool TrackingStarted { get; private set; }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        /// Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invoke handlers of the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoke handlers of the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
        /// Finalizes an instance of the <see cref="SolutionStatusBar"/> class.
        /// </summary>
        ~SolutionStatusBar()
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
            FindFocusedTimer.Dispose();
        }
        #endregion
    }
}

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

namespace CustomControls
{
    public partial class SolutionStatusBar : UserControl, INotifyPropertyChanged, IDisposable
    {
        #region Custom properties and events
        #region Theme
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register("Theme", typeof(IStatusTheme), typeof(SolutionStatusBar), new PropertyMetadata(null));

        public IStatusTheme Theme
        {
            get { return (IStatusTheme)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        #endregion
        #region Max Active Status Count
        public static readonly DependencyProperty MaxActiveStatusCountProperty = DependencyProperty.Register("MaxActiveStatusCount", typeof(int), typeof(SolutionStatusBar), new PropertyMetadata(5));

        public int MaxActiveStatusCount
        {
            get { return (int)GetValue(MaxActiveStatusCountProperty); }
            set { SetValue(MaxActiveStatusCountProperty, value); }
        }
        #endregion
        #region Active Status List
        private static readonly DependencyPropertyKey ActiveStatusListPropertyKey = DependencyProperty.RegisterReadOnly("ActiveStatusList", typeof(IReadOnlyCollection<IApplicationStatus>), typeof(SolutionStatusBar), new PropertyMetadata(null));
        public static readonly DependencyProperty ActiveStatusListProperty = ActiveStatusListPropertyKey.DependencyProperty;

        public IReadOnlyCollection<IApplicationStatus> ActiveStatusList
        {
            get { return (IReadOnlyCollection<IApplicationStatus>)GetValue(ActiveStatusListProperty); }
        }
        #endregion
        #region Current Status
        private static readonly DependencyPropertyKey CurrentStatusPropertyKey = DependencyProperty.RegisterReadOnly("CurrentStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));
        public static readonly DependencyProperty CurrentStatusProperty = CurrentStatusPropertyKey.DependencyProperty;

        public IApplicationStatus CurrentStatus
        {
            get { return (IApplicationStatus)GetValue(CurrentStatusProperty); }
        }
        #endregion
        #region Default Initializing Status
        public static readonly DependencyProperty DefaultInitializingStatusProperty = DependencyProperty.Register("DefaultInitializingStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        public IApplicationStatus DefaultInitializingStatus
        {
            get { return (IApplicationStatus)GetValue(DefaultInitializingStatusProperty); }
            set { SetValue(DefaultInitializingStatusProperty, value); }
        }
        #endregion
        #region Default Ready Status
        public static readonly DependencyProperty DefaultReadyStatusProperty = DependencyProperty.Register("DefaultReadyStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        public IApplicationStatus DefaultReadyStatus
        {
            get { return (IApplicationStatus)GetValue(DefaultReadyStatusProperty); }
            set { SetValue(DefaultReadyStatusProperty, value); }
        }
        #endregion
        #region Default Failure Status
        public static readonly DependencyProperty DefaultFailureStatusProperty = DependencyProperty.Register("DefaultFailureStatus", typeof(IApplicationStatus), typeof(SolutionStatusBar), new PropertyMetadata(null));

        public IApplicationStatus DefaultFailureStatus
        {
            get { return (IApplicationStatus)GetValue(DefaultFailureStatusProperty); }
            set { SetValue(DefaultFailureStatusProperty, value); }
        }
        #endregion
        #region Progress Value
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register("ProgressValue", typeof(double), typeof(SolutionStatusBar), new PropertyMetadata(0.0));

        public double ProgressValue
        {
            get { return (double)GetValue(ProgressValueProperty); }
            set { SetValue(ProgressValueProperty, value); }
        }
        #endregion
        #region Progress Max
        public static readonly DependencyProperty ProgressMaxProperty = DependencyProperty.Register("ProgressMax", typeof(double), typeof(SolutionStatusBar), new PropertyMetadata(0.0));

        public double ProgressMax
        {
            get { return (double)GetValue(ProgressMaxProperty); }
            set { SetValue(ProgressMaxProperty, value); }
        }
        #endregion
        #endregion

        #region Init
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
        /// <parameters>
        /// <param name="sender">This parameter is not used.</param>
        /// <param name="e">This parameter is not used.</param>
        /// </parameters>
        protected virtual void OnInitialized(object sender, EventArgs e)
        {
            InitializeTheme();
        }
        #endregion

        #region Properties
        public bool HasCaret
        {
            get { return _HasCaret; }
            protected set
            {
                if (_HasCaret != value)
                {
                    _HasCaret = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private bool _HasCaret;

        public int CaretLine
        {
            get { return _CaretLine; }
            protected set
            {
                if (_CaretLine != value)
                {
                    _CaretLine = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private int _CaretLine;

        public int CaretColumn
        {
            get { return _CaretColumn; }
            protected set
            {
                if (_CaretColumn != value)
                {
                    _CaretColumn = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private int _CaretColumn;

        public bool IsCaretOverride
        {
            get { return _IsCaretOverride; }
            protected set
            {
                if (_IsCaretOverride != value)
                {
                    _IsCaretOverride = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private bool _IsCaretOverride;
        #endregion

        #region Implementation
        protected virtual void SetLastStatusMessage(IApplicationStatus lastStatus)
        {
            SetValue(CurrentStatusPropertyKey, lastStatus);
        }
        #endregion

        #region Client Interface
        public virtual void SetStatus(IApplicationStatus status)
        {
            if (status != null && !StatusList.Contains(status))
                StatusList.Add(status);
            while (StatusList.Count > 1 && StatusList.Count > MaxActiveStatusCount)
                StatusList.RemoveAt(0);

            Dispatcher.BeginInvoke(new SetStatusHandler(OnSetStatus), status);
        }

        protected delegate void SetStatusHandler(IApplicationStatus status);
        protected virtual void OnSetStatus(IApplicationStatus status)
        {
            SetLastStatusMessage(status);
            ClearProgress();
        }

        public virtual void ResetStatus(IApplicationStatus status)
        {
            if (status != null && StatusList.Contains(status))
                StatusList.Remove(status);

            Dispatcher.BeginInvoke(new ResetStatusHandler(OnResetStatus));
        }

        protected delegate void ResetStatusHandler();
        protected virtual void OnResetStatus()
        {
            IApplicationStatus LastStatus = (StatusList.Count > 0) ? StatusList[StatusList.Count - 1] : DefaultReadyStatus;
            SetLastStatusMessage(LastStatus);
            ClearProgress();
        }

        public virtual void SetFailureStatus()
        {
            Dispatcher.BeginInvoke(new SetFailureStatusHandler(OnSetFailureStatus));
        }

        protected delegate void SetFailureStatusHandler();
        protected virtual void OnSetFailureStatus()
        {
            SetLastStatusMessage(DefaultFailureStatus);
        }

        protected virtual void ClearProgress()
        {
            ProgressValue = 0;
            ProgressMax = 0;
        }
        #endregion

        #region Theme
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
            _HasCaret = false;
            _CaretLine = 0;
            _CaretColumn = 0;
            _IsCaretOverride = false;
            FindFocusedOperation = null;
            TrackingStarted = false;
            FindFocusedTimer = new Timer(new TimerCallback(FindFocusedTimerCallback), this, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        protected virtual void FindFocusedTimerCallback(object parameter)
        {
            if (FindFocusedOperation == null || FindFocusedOperation.Status == DispatcherOperationStatus.Completed)
                FindFocusedOperation = Dispatcher.BeginInvoke(new FindFocusedTimerHandler(OnFindFocusedTimer));
        }

        protected delegate void FindFocusedTimerHandler();
        protected virtual void OnFindFocusedTimer()
        {
            if (!TrackingStarted && Keyboard.FocusedElement is IInputElement FocusedElement)
            {
                FindFocusedTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
                StartTrackingFocus(FocusedElement);
            }
        }

        protected virtual void StartTrackingFocus(IInputElement focusedElement)
        {
            TrackingStarted = true;
            AddHandlers(focusedElement);
        }

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

        protected virtual void RemoveHandlers(IInputElement focusedElement)
        {
            if (focusedElement == null)
                throw new ArgumentNullException(nameof(focusedElement));

            focusedElement.LostKeyboardFocus -= OnLostKeyboardFocus;

            if (focusedElement is IDocumentEditor AsOldEditor)
                AsOldEditor.CaretPositionChanged -= OnCaretPositionChanged;
        }

        protected virtual void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e != null && e.NewFocus != null)
            {
                RemoveHandlers(e.OldFocus);
                AddHandlers(e.NewFocus);
            }
        }

        protected virtual void OnCaretPositionChanged(object sender, EventArgs e)
        {
            UpdateCaretInfo((IDocumentEditor)sender);
        }

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

        protected virtual void ResetCaretInfo()
        {
            HasCaret = false;
            CaretLine = 0;
            CaretColumn = 0;
            IsCaretOverride = false;
        }

        protected DispatcherOperation? FindFocusedOperation { get; private set; }
        protected Timer FindFocusedTimer { get; private set; } = new Timer(new TimerCallback((object parameter) => { }));
        protected bool TrackingStarted { get; private set; }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        /// Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
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
        /// Object destructor.
        /// </summary>
        ~SolutionStatusBar()
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
            FindFocusedTimer.Dispose();
        }
        #endregion
    }
}

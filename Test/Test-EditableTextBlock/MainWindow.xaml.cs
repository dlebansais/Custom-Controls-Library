using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace TestEditableTextBlock
{
    public partial class MainWindow : Window, IDisposable
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            Debug.Assert(ctrl.ClickDelay == CustomControls.EditableTextBlock.DefaultClickDelay);
            ctrl.ClickDelay = CustomControls.EditableTextBlock.DefaultClickDelay;
            Debug.Assert(ctrl.Editable);
            ctrl.Editable = true;
            Debug.Assert(!ctrl.IsEditing);

            ctrl.EditEnter += OnEditEnter;
            ctrl.TextChanged += OnTextChanged;
            ctrl.EditLeave += OnEditLeave;
        }

        public string EditableText
        {
            get { return EditableTextInternal; }
            set
            {
                if (EditableTextInternal != value)
                {
                    EditableTextInternal = value;
                }
            }
        }
        private string EditableTextInternal = "Init";

        private void OnEditEnter(object sender, RoutedEventArgs e)
        {
            if (TestEscape)
                EscapeTimer = new Timer(new TimerCallback(EscapeTimerCallback), this, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        }

        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
        }

        private void OnEditLeave(object sender, RoutedEventArgs e)
        {
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

        public static bool TestEscape { get; set; } = false;

        private void EscapeTimerCallback(object parameter)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnEscapeTimer));
        }

        private void OnEscapeTimer()
        {
            switch (EscapeStep++)
            {
                case 0:
                    SendKey(Key.X);
                    break;

                case 1:
                    SendEscapeKey();
                    break;

                default:
                    EscapeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    Close();
                    break;
            }
        }

        private void SendKey(Key key)
        {
            KeyEventArgs e;

            e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            };
            InputManager.Current.ProcessInput(e);
            /*
            e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };
            InputManager.Current.ProcessInput(e);*/

            e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.PreviewKeyUpEvent
             };
            InputManager.Current.ProcessInput(e);
            /*
            e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.KeyUpEvent
            };
            InputManager.Current.ProcessInput(e);*/
        }

        private void SendEscapeKey()
        {
            var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Escape)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            };
            InputManager.Current.ProcessInput(e);
        }

        private Timer EscapeTimer = new Timer(new TimerCallback((object parameter) => { }));
        private int EscapeStep = 0;

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
        ~MainWindow()
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
            ctrl.Dispose();
        }
        #endregion
    }
}

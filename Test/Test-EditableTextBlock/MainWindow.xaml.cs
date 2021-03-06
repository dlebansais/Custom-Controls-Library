﻿namespace TestEditableTextBlock
{
    using CustomControls;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;

    public partial class MainWindow : Window
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
                using (TestDisposeWindow Dlg = new TestDisposeWindow())
                {
                    Dlg.ShowDialog();
                }

                int DoubleClickTime = System.Windows.Forms.SystemInformation.DoubleClickTime;
                ctrl.ClickDelay = TimeSpan.FromMilliseconds(DoubleClickTime);
                ctrl.Focusable = false;
            }

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnLoadedDone));
        }

        private void OnLoadedDone()
        {
            string s = ctrl.Text;

            Debug.Assert(s == "Init");
            ctrl.Text = "Init";
        }

        public string EditableText
        {
            get { return "Init"; }
        }

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

        public static int TestEscape { get; set; } = 0;

        private void EscapeTimerCallback(object parameter)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(OnEscapeTimer));
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
                    else if (TestEscape == 2 || TestEscape == 3 || TestEscape == 5)
                        SendReturnKey();
                    break;

                case 2:
                    ctrl.EditEnter -= OnEditEnter;
                    ctrl.TextChanged -= OnTextChanged;
                    ctrl.EditLeave -= OnEditLeave;
                    break;

                default:
                    EscapeTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    Close();
                    break;
            }

            EscapeStep++;
        }

        private void SendKey(Key key)
        {
            KeyEventArgs e;

            e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            };
            InputManager.Current.ProcessInput(e);

            e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key)
            {
                RoutedEvent = Keyboard.PreviewKeyUpEvent
             };
            InputManager.Current.ProcessInput(e);
        }

        private void SendEscapeKey()
        {
            var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Escape)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            };
            InputManager.Current.ProcessInput(e);
        }

        private void SendReturnKey()
        {
            var e = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return)
            {
                RoutedEvent = Keyboard.PreviewKeyDownEvent
            };
            InputManager.Current.ProcessInput(e);
        }

        private Timer EscapeTimer = new Timer(new TimerCallback((object parameter) => { }));
        private int EscapeStep = 0;
    }
}

using System.ComponentModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CustomControls
{
    /// <summary>
    /// Represents a password box that can optionally display the password characters.
    /// <para>Implemented as user control with a <see cref="PasswordBox"/> and a <see cref="TextBox"/> for displayed characters.</para>
    /// </summary>
    /// <remarks>
    /// <para>Contrary to the .NET PasswordBox, the <see cref="Text"/> property for this control is bindable.</para>
    /// <para>This control is typically used as follow:</para>
    /// <para>. The <see cref="ShowPassword"/> property is bound to some checkbox, initially unchecked.</para>
    /// <para>. If the client application does not care about security the <see cref="Text"/> property is bound to a string.</para>
    /// <para>. If the client does care, the <see cref="Text"/> property is not bound and when the user clicks on some button or press Enter, the application reads the <see cref="Password"/> property. This property returns a password that is as secure as it can be.</para>
    /// </remarks>
    public partial class ExtendedPasswordBox : UserControl
    {
        #region Custom properties and events
        #region Text
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="Text"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ExtendedPasswordBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Bindable(true)]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedPasswordBox ecb = (ExtendedPasswordBox)d;
            ecb.OnTextChanged(e);
        }

        private void OnTextChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!IsUserChange)
            {
                string NewText = (string)e.NewValue;
                if (NewText.Length > 0)
                    HasTextBeenShown = true;

                UpdateIsPasswordEmptyProperty(NewText);

                if (ShowPassword)
                    textPassword.Text = NewText;
                else
                    passPassword.Password = NewText;
            }
        }
        #endregion
        #region Show Password
        /// <summary>
        /// Identifies the <see cref="ShowPassword"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="ShowPassword"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ShowPasswordProperty = DependencyProperty.Register("ShowPassword", typeof(bool), typeof(ExtendedPasswordBox), new FrameworkPropertyMetadata(false, OnShowPasswordChanged));

        /// <summary>
        /// Gets or sets the flag indicating if the password should be displayed as plain text.
        /// </summary>
        public bool ShowPassword
        {
            get { return (bool)GetValue(ShowPasswordProperty); }
            set { SetValue(ShowPasswordProperty, value); }
        }

        private static void OnShowPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ExtendedPasswordBox ecb = (ExtendedPasswordBox)d;
            ecb.OnShowPasswordChanged(e);
        }

        private void OnShowPasswordChanged(DependencyPropertyChangedEventArgs e)
        {
            UninstallHandlers();

            bool NewShowPassword = (bool)e.NewValue;
            if (NewShowPassword)
            {
                Text = passPassword.Password;
                textPassword.Text = passPassword.Password;
                textPassword.SelectAll();
                passPassword.Password = null;

                HasTextBeenShown = true;
            }
            else
            {
                passPassword.Password = textPassword.Text;
                passPassword.SelectAll(); 
                textPassword.Text = null;
            }

            InstallHandlers();
        }
        #endregion
        #region Is Password Empty
        private static readonly DependencyPropertyKey IsPasswordEmptyPropertyKey = DependencyProperty.RegisterReadOnly("IsPasswordEmpty", typeof(bool), typeof(ExtendedPasswordBox), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="IsPasswordEmpty"/> dependency property.
        /// </summary>
        /// <returns>
        /// The identifier for the <see cref="IsPasswordEmpty"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsPasswordEmptyProperty = IsPasswordEmptyPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value indicating if the password is currently empty.
        /// </summary>
        public bool IsPasswordEmpty
        {
            get { return (bool)GetValue(IsPasswordEmptyProperty); }
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedPasswordBox"/> class.
        /// </summary>
        public ExtendedPasswordBox()
        {
            InitializeComponent();

            InstallHandlers();
            IsUserChange = false;
            HasTextBeenShown = false;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Checks if the password has been displayed in plain text, if there was a binding on the Text property or if the client has initialized it, and returns a secure or plain text password accordingly.
        /// </summary>
        /// <returns>
        /// An <see cref="ExtendedPassword"/> object with the following properties:
        /// <para>If the password has been displayed in plain text, if there was a binding on the Text property or if the client has initialized it, the returned object is a plain text password.</para>
        /// <para>Otherwise, the returned object is a secure password.</para>
        /// </returns>
        public ExtendedPassword Password
        {
            get
            {
                if (HasTextBeenShown)
                    return new ExtendedPassword(Text);
                else
                    return new ExtendedPassword(passPassword.SecurePassword);
            }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Replaces the original Focus() method to move the focus to the control shown to the user (either the PasswordBox, or the TextBox if the password is shown).
        /// </summary>
        public new void Focus()
        {
            if (ShowPassword)
                textPassword.Focus();
            else
                passPassword.Focus();
        }

        /// <summary>
        /// Force the password to be plain text. This doesn't show the password to the use, but marks it as plain text if it was still secure.
        /// </summary>
        public void MarkAsInsecure()
        {
            if (!HasTextBeenShown)
            {
                HasTextBeenShown = true;

                string CurrentText = ShowPassword ? textPassword.Text : passPassword.Password;
                UpdateTextProperty(CurrentText);
                UpdateIsPasswordEmptyProperty(CurrentText);
            }
        }
        #endregion

        #region Implementation
        private void InstallHandlers()
        {
            textPassword.TextChanged += OnVisibleTextChanged;
            passPassword.PasswordChanged += OnHiddenTextChanged;
        }

        private void UninstallHandlers()
        {
            textPassword.TextChanged -= OnVisibleTextChanged;
            passPassword.PasswordChanged -= OnHiddenTextChanged;
        }
        #endregion

        #region Events
        private void OnVisibleTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox Ctrl = (TextBox)sender;

            HasTextBeenShown = true;

            string NewText = Ctrl.Text;
            UpdateTextProperty(NewText);
            UpdateIsPasswordEmptyProperty(NewText);
        }

        private void OnHiddenTextChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox Ctrl = (PasswordBox)sender;

            if (!HasTextBeenShown && IsTextPropertyBound())
                HasTextBeenShown = true;

            if (HasTextBeenShown)
            {
                string NewText = Ctrl.Password;
                UpdateTextProperty(NewText);
                UpdateIsPasswordEmptyProperty(NewText);
            }
            else
                UpdateIsPasswordEmptyProperty(Ctrl.SecurePassword);
        }

        private bool IsTextPropertyBound()
        {
            BindingExpression Expression = GetBindingExpression(TextProperty);
            return Expression != null;
        }

        private void UpdateTextProperty(string NewText)
        {
            IsUserChange = true;
            Text = NewText;
            IsUserChange = false;
        }

        private void UpdateIsPasswordEmptyProperty(string NewText)
        {
            bool IsEmpty = (NewText == null || NewText.Length == 0);
            SetValue(IsPasswordEmptyPropertyKey, IsEmpty);
        }

        private void UpdateIsPasswordEmptyProperty(SecureString NewPassword)
        {
            bool IsEmpty = (NewPassword.Length == 0);
            SetValue(IsPasswordEmptyPropertyKey, IsEmpty);
        }

        private bool IsUserChange;
        private bool HasTextBeenShown;
        #endregion
    }
}

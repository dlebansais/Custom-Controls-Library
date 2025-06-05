namespace CustomControls;

using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
    /// Gets a secure or plain text password.
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
            _ = textPassword.Focus();
        else
            _ = passPassword.Focus();
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
    private void OnVisibleTextChanged(object sender, TextChangedEventArgs args)
    {
        TextBox Ctrl = (TextBox)sender;

        HasTextBeenShown = true;

        string NewText = Ctrl.Text;
        UpdateTextProperty(NewText);
        UpdateIsPasswordEmptyProperty(NewText);
    }

    private void OnHiddenTextChanged(object sender, RoutedEventArgs args)
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
        {
            UpdateIsPasswordEmptyProperty(Ctrl.SecurePassword);
        }
    }

    private bool IsTextPropertyBound()
    {
        BindingExpression Expression = GetBindingExpression(TextProperty);
        return Expression is not null;
    }

    private void UpdateTextProperty(string newText)
    {
        IsUserChange = true;
        Text = newText;
        IsUserChange = false;
    }

    private void UpdateIsPasswordEmptyProperty(string newText)
    {
        bool IsEmpty = string.IsNullOrEmpty(newText);
        SetValue(IsPasswordEmptyPropertyKey, IsEmpty);
    }

    private void UpdateIsPasswordEmptyProperty(SecureString newPassword)
    {
        bool IsEmpty = newPassword.Length == 0;
        SetValue(IsPasswordEmptyPropertyKey, IsEmpty);
    }

    private bool IsUserChange;
    private bool HasTextBeenShown;
    #endregion
}

﻿namespace CustomControls;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

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
    #region Text
    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="Text"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ExtendedPasswordBox), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    [Bindable(true)]
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        ExtendedPasswordBox ecb = (ExtendedPasswordBox)d;
        ecb.OnTextChanged(args);
    }

    private void OnTextChanged(DependencyPropertyChangedEventArgs args)
    {
        if (!IsUserChange)
        {
            string NewText = (string)args.NewValue;
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
    public static readonly DependencyProperty ShowPasswordProperty = DependencyProperty.Register(nameof(ShowPassword), typeof(bool), typeof(ExtendedPasswordBox), new FrameworkPropertyMetadata(false, OnShowPasswordChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the password should be displayed as plain text.
    /// </summary>
    public bool ShowPassword
    {
        get => (bool)GetValue(ShowPasswordProperty);
        set => SetValue(ShowPasswordProperty, value);
    }

    private static void OnShowPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        ExtendedPasswordBox ecb = (ExtendedPasswordBox)d;
        ecb.OnShowPasswordChanged(args);
    }

    private void OnShowPasswordChanged(DependencyPropertyChangedEventArgs args)
    {
        UninstallHandlers();

        bool NewShowPassword = (bool)args.NewValue;
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
    private static readonly DependencyPropertyKey IsPasswordEmptyPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsPasswordEmpty), typeof(bool), typeof(ExtendedPasswordBox), new PropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsPasswordEmpty"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsPasswordEmpty"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty IsPasswordEmptyProperty = IsPasswordEmptyPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a value indicating whether the password is currently empty.
    /// </summary>
    public bool IsPasswordEmpty => (bool)GetValue(IsPasswordEmptyProperty);
    #endregion
}

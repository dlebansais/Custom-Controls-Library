﻿namespace CustomControls;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Represents a set of buttons for dialog boxes with commonly used commands.
/// <para>Implemented as a user control with a <see cref="ItemsControl"/> container for buttons.</para>
/// </summary>
/// <remarks>
/// Documentation available in Dialogvalidation.pdf.
/// </remarks>
public partial class DialogValidation : UserControl, INotifyPropertyChanged
{
    #region Globals
    /// <summary>
    /// <see cref="RoutedCommand"/> object for a OK command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandOk = CreateDefaultCommand(ActiveCommand.Ok.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Cancel command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandCancel = CreateDefaultCommand(ActiveCommand.Cancel.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Abort command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandAbort = CreateDefaultCommand(ActiveCommand.Abort.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Retry command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandRetry = CreateDefaultCommand(ActiveCommand.Retry.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Ignore command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandIgnore = CreateDefaultCommand(ActiveCommand.Ignore.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Yes command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandYes = CreateDefaultCommand(ActiveCommand.Yes.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a No command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandNo = CreateDefaultCommand(ActiveCommand.No.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Close command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandClose = CreateDefaultCommand(ActiveCommand.Close.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Help command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandHelp = CreateDefaultCommand(ActiveCommand.Help.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Try Again command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandTryAgain = CreateDefaultCommand(ActiveCommand.TryAgain.Name);

    /// <summary>
    /// <see cref="RoutedCommand"/> object for a Continue command.
    /// </summary>
    public static readonly RoutedUICommand DefaultCommandContinue = CreateDefaultCommand(ActiveCommand.Continue.Name);
    #endregion

    #region Is Localized
    /// <summary>
    /// Identifies the <see cref="IsLocalized"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsLocalized"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty IsLocalizedProperty = DependencyProperty.Register(nameof(IsLocalized), typeof(bool), typeof(DialogValidation), new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether buttons should display the english or localized text.
    /// </summary>
    public bool IsLocalized
    {
        get => (bool)GetValue(IsLocalizedProperty);
        set => SetValue(IsLocalizedProperty, value);
    }
    #endregion

    #region Active Commands
    /// <summary>
    /// Identifies the <see cref="ActiveCommands"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ActiveCommands"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ActiveCommandsProperty = DependencyProperty.Register(nameof(ActiveCommands), typeof(ActiveCommandCollection), typeof(DialogValidation), new PropertyMetadata(new ActiveCommandCollection()));

    /// <summary>
    /// Gets or sets the list of commands to activate. This will display as many buttons as there are active commands.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification="Collection must be R/W to allow Xaml syntax")]
    public ActiveCommandCollection ActiveCommands
    {
        get => (ActiveCommandCollection)GetValue(ActiveCommandsProperty);
        set => SetValue(ActiveCommandsProperty, value);
    }

    /// <summary>
    /// Gets the current set of active commands.
    /// </summary>
    /// <remarks>
    /// Until the <see cref="ActiveCommands"/> property is set, this property returns the default set, which is OK and Cancel.
    /// After the <see cref="ActiveCommands"/> property is set, this property returns the content of the <see cref="ActiveCommands"/> property.
    /// </remarks>
    public IList<ActiveCommand> ActualActiveCommands
    {
        get
        {
            if (ActiveCommands.IsCollectionModified)
                return ActiveCommands;
            else
                return DefaultCommandCollection;
        }
    }

    /// <summary>
    /// The default collection of commands as loaded by the static constructor.
    /// </summary>
    private static readonly IList<ActiveCommand> DefaultCommandCollection = InitDefaultCommandCollection();
    #endregion

    #region Orientation
    /// <summary>
    /// Identifies the <see cref="Orientation"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="Orientation"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(DialogValidation), new PropertyMetadata(Orientation.Horizontal));

    /// <summary>
    /// Gets or sets the orientation (horizontal or vertical) of buttons.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    #endregion

    #region Command OK
    /// <summary>
    /// Identifies the <see cref="CommandOk"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandOk"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandOkProperty = DependencyProperty.Register(nameof(CommandOk), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandOk));

    /// <summary>
    /// Gets or sets the command to use for OK buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandOk
    {
        get => (ICommand)GetValue(CommandOkProperty);
        set => SetValue(CommandOkProperty, value);
    }
    #endregion

    #region Content OK
    /// <summary>
    /// Identifies the <see cref="ContentOk"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentOk"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentOkProperty = DependencyProperty.Register(nameof(ContentOk), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for OK buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentOk
    {
        get => GetValue(ContentOkProperty);
        set => UpdateButtonContent(ContentOkProperty, value);
    }
    #endregion

    #region Command Cancel
    /// <summary>
    /// Identifies the <see cref="CommandCancel"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandCancel"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandCancelProperty = DependencyProperty.Register(nameof(CommandCancel), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandCancel));

    /// <summary>
    /// Gets or sets the command to use for Cancel buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandCancel
    {
        get => (ICommand)GetValue(CommandCancelProperty);
        set => SetValue(CommandCancelProperty, value);
    }
    #endregion

    #region Content Cancel
    /// <summary>
    /// Identifies the <see cref="ContentCancel"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentCancel"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentCancelProperty = DependencyProperty.Register(nameof(ContentCancel), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Cancel buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentCancel
    {
        get => GetValue(ContentCancelProperty);
        set => UpdateButtonContent(ContentCancelProperty, value);
    }
    #endregion

    #region Command Abort
    /// <summary>
    /// Identifies the <see cref="CommandAbort"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandAbort"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandAbortProperty = DependencyProperty.Register(nameof(CommandAbort), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandAbort));

    /// <summary>
    /// Gets or sets the command to use for Abort buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandAbort
    {
        get => (ICommand)GetValue(CommandAbortProperty);
        set => SetValue(CommandAbortProperty, value);
    }
    #endregion

    #region Content Abort
    /// <summary>
    /// Identifies the <see cref="ContentAbort"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentAbort"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentAbortProperty = DependencyProperty.Register(nameof(ContentAbort), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Abort buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentAbort
    {
        get => GetValue(ContentAbortProperty);
        set => UpdateButtonContent(ContentAbortProperty, value);
    }
    #endregion

    #region Command Retry
    /// <summary>
    /// Identifies the <see cref="CommandRetry"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandRetry"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandRetryProperty = DependencyProperty.Register(nameof(CommandRetry), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandRetry));

    /// <summary>
    /// Gets or sets the command to use for Retry buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandRetry
    {
        get => (ICommand)GetValue(CommandRetryProperty);
        set => SetValue(CommandRetryProperty, value);
    }
    #endregion

    #region Content Retry
    /// <summary>
    /// Identifies the <see cref="ContentRetry"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentRetry"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentRetryProperty = DependencyProperty.Register(nameof(ContentRetry), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Retry buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentRetry
    {
        get => GetValue(ContentRetryProperty);
        set => UpdateButtonContent(ContentRetryProperty, value);
    }
    #endregion

    #region Command Ignore
    /// <summary>
    /// Identifies the <see cref="CommandIgnore"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandIgnore"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandIgnoreProperty = DependencyProperty.Register(nameof(CommandIgnore), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandIgnore));

    /// <summary>
    /// Gets or sets the command to use for Ignore buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandIgnore
    {
        get => (ICommand)GetValue(CommandIgnoreProperty);
        set => SetValue(CommandIgnoreProperty, value);
    }
    #endregion

    #region Content Ignore
    /// <summary>
    /// Identifies the <see cref="ContentIgnore"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentIgnore"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentIgnoreProperty = DependencyProperty.Register(nameof(ContentIgnore), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Ignore buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentIgnore
    {
        get => GetValue(ContentIgnoreProperty);
        set => UpdateButtonContent(ContentIgnoreProperty, value);
    }
    #endregion

    #region Command Yes
    /// <summary>
    /// Identifies the <see cref="CommandYes"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandYes"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandYesProperty = DependencyProperty.Register(nameof(CommandYes), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandYes));

    /// <summary>
    /// Gets or sets the command to use for Yes buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandYes
    {
        get => (ICommand)GetValue(CommandYesProperty);
        set => SetValue(CommandYesProperty, value);
    }
    #endregion

    #region Content Yes
    /// <summary>
    /// Identifies the <see cref="ContentYes"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentYes"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentYesProperty = DependencyProperty.Register(nameof(ContentYes), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Yes buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentYes
    {
        get => GetValue(ContentYesProperty);
        set => UpdateButtonContent(ContentYesProperty, value);
    }
    #endregion

    #region Command No
    /// <summary>
    /// Identifies the <see cref="CommandNo"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandNo"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandNoProperty = DependencyProperty.Register(nameof(CommandNo), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandNo));

    /// <summary>
    /// Gets or sets the command to use for No buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandNo
    {
        get => (ICommand)GetValue(CommandNoProperty);
        set => SetValue(CommandNoProperty, value);
    }
    #endregion

    #region Content No
    /// <summary>
    /// Identifies the <see cref="ContentNo"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentNo"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentNoProperty = DependencyProperty.Register(nameof(ContentNo), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for No buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentNo
    {
        get => GetValue(ContentNoProperty);
        set => UpdateButtonContent(ContentNoProperty, value);
    }
    #endregion

    #region Command Close
    /// <summary>
    /// Identifies the <see cref="CommandClose"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandClose"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandCloseProperty = DependencyProperty.Register(nameof(CommandClose), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandClose));

    /// <summary>
    /// Gets or sets the command to use for Close buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandClose
    {
        get => (ICommand)GetValue(CommandCloseProperty);
        set => SetValue(CommandCloseProperty, value);
    }
    #endregion

    #region Content Close
    /// <summary>
    /// Identifies the <see cref="ContentClose"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentClose"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentCloseProperty = DependencyProperty.Register(nameof(ContentClose), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Close buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentClose
    {
        get => GetValue(ContentCloseProperty);
        set => UpdateButtonContent(ContentCloseProperty, value);
    }
    #endregion

    #region Command Help
    /// <summary>
    /// Identifies the <see cref="CommandHelp"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandHelp"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandHelpProperty = DependencyProperty.Register(nameof(CommandHelp), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandHelp));

    /// <summary>
    /// Gets or sets the command to use for Help buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandHelp
    {
        get => (ICommand)GetValue(CommandHelpProperty);
        set => SetValue(CommandHelpProperty, value);
    }
    #endregion

    #region Content Help
    /// <summary>
    /// Identifies the <see cref="ContentHelp"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentHelp"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentHelpProperty = DependencyProperty.Register(nameof(ContentHelp), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Help buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentHelp
    {
        get => GetValue(ContentHelpProperty);
        set => UpdateButtonContent(ContentHelpProperty, value);
    }
    #endregion

    #region Command TryAgain
    /// <summary>
    /// Identifies the <see cref="CommandTryAgain"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandTryAgain"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandTryAgainProperty = DependencyProperty.Register(nameof(CommandTryAgain), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandTryAgain));

    /// <summary>
    /// Gets or sets the command to use for Try Again buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandTryAgain
    {
        get => (ICommand)GetValue(CommandTryAgainProperty);
        set => SetValue(CommandTryAgainProperty, value);
    }
    #endregion

    #region Content TryAgain
    /// <summary>
    /// Identifies the <see cref="ContentTryAgain"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentTryAgain"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentTryAgainProperty = DependencyProperty.Register(nameof(ContentTryAgain), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Try Again buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentTryAgain
    {
        get => GetValue(ContentTryAgainProperty);
        set => UpdateButtonContent(ContentTryAgainProperty, value);
    }
    #endregion

    #region Command Continue
    /// <summary>
    /// Identifies the <see cref="CommandContinue"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="CommandContinue"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty CommandContinueProperty = DependencyProperty.Register(nameof(CommandContinue), typeof(ICommand), typeof(DialogValidation), new PropertyMetadata(DefaultCommandContinue));

    /// <summary>
    /// Gets or sets the command to use for Continue buttons. The initial value is the corresponding static default command.
    /// </summary>
    public ICommand CommandContinue
    {
        get => (ICommand)GetValue(CommandContinueProperty);
        set => SetValue(CommandContinueProperty, value);
    }
    #endregion

    #region Content Continue
    /// <summary>
    /// Identifies the <see cref="ContentContinue"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ContentContinue"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ContentContinueProperty = DependencyProperty.Register(nameof(ContentContinue), typeof(object), typeof(DialogValidation));

    /// <summary>
    /// Gets or sets the content to use for Continue buttons. The default value is the English or localized name string for this command.
    /// </summary>
    public object ContentContinue
    {
        get => GetValue(ContentContinueProperty);
        set => UpdateButtonContent(ContentContinueProperty, value);
    }
    #endregion
}

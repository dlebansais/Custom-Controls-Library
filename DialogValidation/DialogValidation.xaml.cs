namespace CustomControls;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

/// <summary>
/// Represents a set of buttons for dialog boxes with commonly used commands.
/// <para>Implemented as a user control with a <see cref="ItemsControl"/> container for buttons.</para>
/// </summary>
/// <remarks>
/// Documentation available in Dialogvalidation.pdf.
/// </remarks>
[ContentProperty("ActiveCommands")]
[DefaultProperty("ActiveCommands")]
public partial class DialogValidation : UserControl, INotifyPropertyChanged
{
    #region Init
    /// <summary>
    /// Gets default commands to use when the client does not specifically define them.
    /// </summary>
    private static List<ActiveCommand> InitDefaultCommandCollection()
    {
        List<ActiveCommand> DefaultList = new();
        DefaultList.Add(ActiveCommand.Ok);
        DefaultList.Add(ActiveCommand.Cancel);

        return DefaultList;
    }

    /// <summary>
    /// Creates a default command.
    /// </summary>
    private static RoutedUICommand CreateDefaultCommand(string text)
    {
        RoutedUICommand Command = new();
        Command.Text = text;

        return Command;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DialogValidation"/> class.
    /// </summary>
    public DialogValidation()
    {
        Initialized += OnInitialized; // Dirty trick to avoid warning CA2214.
        InitializeComponent();
    }

    /// <summary>
    /// Called when the control has been initialized and before properties are set.
    /// </summary>
    /// <parameters>
    /// <param name="sender">This parameter is not used.</param>
    /// <param name="e">This parameter is not used.</param>
    /// </parameters>
    private void OnInitialized(object? sender, EventArgs e)
    {
        InitializeCommands();
    }

    /// <summary>
    /// Initializes the default commands and localized names.
    /// </summary>
    private void InitializeCommands()
    {
        ActiveCommands = CreateActiveCommandCollection();
        InitializeDefaultString(ContentOkProperty, 0);
        InitializeDefaultString(ContentCancelProperty, 1);
        InitializeDefaultString(ContentAbortProperty, 2);
        InitializeDefaultString(ContentRetryProperty, 3);
        InitializeDefaultString(ContentIgnoreProperty, 4);
        InitializeDefaultString(ContentYesProperty, 5);
        InitializeDefaultString(ContentNoProperty, 6);
        InitializeDefaultString(ContentCloseProperty, 7);
        InitializeDefaultString(ContentHelpProperty, 8);
        InitializeDefaultString(ContentTryAgainProperty, 9);
        InitializeDefaultString(ContentContinueProperty, 10);
    }

    /// <summary>
    /// Creates and initializes a <see cref="ActiveCommandCollection"/> object.
    /// </summary>
    /// <returns>The created object instance.</returns>
    protected virtual ActiveCommandCollection CreateActiveCommandCollection()
    {
        return new ActiveCommandCollection();
    }
    #endregion

    #region Properties
    private void UpdateButtonContent(DependencyProperty contentProperty, object value)
    {
        SetValue(contentProperty, value);

        // Toggle IsLocalized twice to trigger a reload.
        if (IsLocalized)
        {
            IsLocalized = false;
            NotifyPropertyChanged(nameof(IsLocalized));
            IsLocalized = true;
            NotifyPropertyChanged(nameof(IsLocalized));
        }
    }
    #endregion

    #region Strings
    /// <summary>
    /// Locates and loads localized strings to be used as localized command names.
    /// </summary>
    private static IList<string> InitializeStrings()
    {
        string SystemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
        string User32Path = Path.Combine(SystemPath, "user32.dll");

        return LoadStringFromResourceFile(User32Path, 51);
    }

    /// <summary>
    /// Loads a string from a resource file by ID.
    /// </summary>
    /// <param name="filePath">Path to the resource file.</param>
    /// <param name="resourceID">Resource ID.</param>
    /// <returns>The list of localized strings for this ID.</returns>
    internal static IList<string> LoadStringFromResourceFile(string filePath, uint resourceID)
    {
        StringResource StringFromResource = new(filePath, resourceID);
        StringFromResource.Load();

        return StringFromResource.AsStrings;
    }

    /// <summary>
    /// Initializes a ContentXXX dependency property with a localized string.
    /// </summary>
    private void InitializeDefaultString(DependencyProperty contentProperty, int index)
    {
        Debug.Assert(index >= 0);
        Debug.Assert(index < DefaultLocalizedStrings.Count);
        SetValue(contentProperty, DefaultLocalizedStrings[index]);
    }

    /// <summary>
    /// Gets the list of localized string for command friendly names, as loaded by the static constructor.
    /// </summary>
    private static readonly IList<string> DefaultLocalizedStrings = InitializeStrings();
    #endregion

    #region Implementation of INotifyPropertyChanged
    /// <summary>
    /// Occurs when a property has changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Invokes handlers of the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    public void NotifyPropertyChanged(string propertyName)
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }
    #endregion
}

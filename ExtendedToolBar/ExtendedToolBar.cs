﻿namespace CustomControls;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml;

/// <summary>
/// Represents a tool bar with a localized name.
/// </summary>
public partial class ExtendedToolBar : ToolBar
{
    #region Init
    /// <summary>
    /// Initializes static members of the <see cref="ExtendedToolBar"/> class.
    /// </summary>
    static ExtendedToolBar()
    {
        OverrideAncestorMetadata();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedToolBar"/> class.
    /// </summary>
    public ExtendedToolBar()
    {
        InitializeHandlers();
    }
    #endregion

    #region Ancestor Interface
    /// <summary>
    /// Overrides metadata associated to the ancestor control with new ones associated to this control specifically.
    /// </summary>
    protected static void OverrideAncestorMetadata() => OverrideMetadataDefaultStyleKey();

    /// <summary>
    /// Overrides the DefaultStyleKey metadata associated to the ancestor control with a new one associated to this control specifically.
    /// </summary>
    protected static void OverrideMetadataDefaultStyleKey() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedToolBar), new FrameworkPropertyMetadata(typeof(ExtendedToolBar)));
    #endregion

    #region Client Interface
    /// <summary>
    /// Returns all buttons in the toolbar to their default active value.
    /// </summary>
    public virtual void Reset()
    {
        foreach (ExtendedToolBarItem Item in AllButtons)
            Item.Button.ResetIsActive();
    }

    /// <summary>
    /// Serializes the active state of buttons in the toolbar.
    /// </summary>
    /// <returns>
    /// A string containing the toolbar state.
    /// </returns>
    public virtual string SerializeActiveButtons()
    {
        try
        {
            bool[] ActiveTable = new bool[AllButtons.Count];
            for (int i = 0; i < AllButtons.Count && i < ActiveTable.Length; i++)
                ActiveTable[i] = AllButtons[i].Button.IsActive;

            return XamlServices.Save(ActiveTable);
        }
        catch (Exception e)
        {
            if (e.Message is null) // To make the code analyzer happy. Since the doc of XamlServices.Save() doesn't specify any exception, this should safe, right?...
                throw;

            return string.Empty;
        }
    }

    /// <summary>
    /// Deserializes the active state of buttons in the toolbar.
    /// </summary>
    /// <param name="xamlData">A string containing the new state of the toolbar.</param>
    public virtual void DeserializeActiveButtons(string xamlData)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(xamlData);
#else
        if (xamlData is null)
            throw new ArgumentNullException(nameof(xamlData));
#endif

        try
        {
            bool[] ActiveTable = (bool[])XamlServices.Parse(xamlData);
            for (int i = 0; i < AllButtons.Count && i < ActiveTable.Length; i++)
                AllButtons[i].Button.IsActive = ActiveTable[i];
        }
        catch (Exception e)
        {
            if (e.Message is null) // To make the code analyzer happy. Since the doc of XamlServices.Parse() doesn't specify any exception other than NullException, this should safe, right?...
                throw;
        }
    }
    #endregion

    #region Checked Buttons
    /// <summary>
    /// Updates the list of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
    /// </summary>
    /// <param name="e">This parameter is not used.</param>
    protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnItemsChanged(e);

        AllButtons.Clear();

        foreach (object? Item in Items)
            if (Item is ExtendedToolBarButton AsExtendedToolBarButton)
            {
                bool IsCommandGroupEnabled = ExtendedToolBar.IsCommandGroupEnabled(AsExtendedToolBarButton.Command);
                if (IsCommandGroupEnabled)
                {
                    ExtendedToolBarItem NewMenuItem = new(AsExtendedToolBarButton);
                    AllButtons.Add(NewMenuItem);
                }
            }
    }

    /// <summary>
    /// Checks if a command belongs to a group associated to an enabled feature.
    /// </summary>
    /// <param name="command">The <see cref="ICommand"/> command object.</param>
    /// <returns>
    /// True if the command is not associated to any feature, or if that feature is enabled.
    /// False if the command belongs to a group associated to a feature, and that feature is disabled.
    /// </returns>
    public static bool IsCommandGroupEnabled(ICommand command)
    {
        if (command is ExtendedRoutedCommand AsExtendedCommand)
            return AsExtendedCommand.CommandGroup.IsEnabled;
        else
            return true;
    }

    /// <summary>
    /// Gets the collection of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
    /// </summary>
    /// <returns>
    /// The collection of all <see cref="ExtendedToolBarButton"/> objects in the toolbar.
    /// </returns>
    public ObservableCollection<ExtendedToolBarItem> AllButtons { get; } = new ObservableCollection<ExtendedToolBarItem>();
    #endregion

    #region Implementation
    private void InitializeHandlers() => Loaded += OnLoaded;

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        if (VisualTreeHelper.GetChildrenCount(this) > 0)
            if (VisualTreeHelper.GetChild(this, 0) is FrameworkElement FirstChild)
            {
                if (FirstChild.FindName("AddRemoveButton") is ToggleButton AddRemoveButton)
                    AddRemoveButton.Checked += OnAddRemoveButtonChecked;

                if (FirstChild.FindName("OverflowButton") is ToggleButton OverflowButton)
                    OverflowButton.Unchecked += OnOverflowButtonUnchecked;

                if (FirstChild.FindName("ResetToolBarMenuItem") is MenuItem ResetToolBarMenuItem)
                    ResetToolBarMenuItem.Click += OnResetToolBarClicked;
            }
    }

    /// <summary>
    /// Called when the "Add or Remove Button" button is checked.
    /// </summary>
    /// <param name="sender">The button object.</param>
    /// <param name="args">This parameter is not used.</param>
    protected virtual void OnAddRemoveButtonChecked(object sender, RoutedEventArgs args)
    {
        if (sender is ToggleButton AddRemoveButton)
            AddRemoveButton.IsEnabled = false;
    }

    /// <summary>
    /// Called when the toolbar overflow button is unchecked.
    /// </summary>
    /// <param name="sender">The button object.</param>
    /// <param name="args">This parameter is not used.</param>
    protected virtual void OnOverflowButtonUnchecked(object sender, RoutedEventArgs args)
    {
        if (sender is ToggleButton OverflowButton)
            if (OverflowButton.Parent is FrameworkElement ParentControl)
                if (ParentControl.FindName("AddRemoveButton") is ToggleButton AddRemoveButton)
                {
                    AddRemoveButton.IsChecked = false;
                    AddRemoveButton.IsEnabled = true;
                }
    }

    /// <summary>
    /// Called when the "Reset ToolBar" button is clicked.
    /// </summary>
    /// <param name="sender">The button object.</param>
    /// <param name="args">This parameter is not used.</param>
    protected virtual void OnResetToolBarClicked(object sender, RoutedEventArgs args)
    {
        if (IsResetConfirmedByUser())
            Reset();
    }

    /// <summary>
    /// Called when the user has confirmed or canceled the reset.
    /// </summary>
    /// <returns>
    /// True if confirmed.
    /// False if canceled.
    /// </returns>
    private bool IsResetConfirmedByUser()
    {
        string Title = Application.Current.MainWindow.Title;

        string Question;
        if (string.IsNullOrEmpty(ToolBarName))
        {
            Question = Properties.Resources.ConfirmResetThisToolBarQuestion;
        }
        else
        {
            string QuestionFormat = Properties.Resources.ConfirmResetToolBarQuestion;
            Question = string.Format(CultureInfo.CurrentCulture, QuestionFormat, ToolBarName);
        }

        MessageBoxResult Result = MessageBox.Show(Question, Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return Result == MessageBoxResult.Yes;
    }
    #endregion
}

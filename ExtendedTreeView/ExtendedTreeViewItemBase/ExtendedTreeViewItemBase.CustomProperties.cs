﻿namespace CustomControls;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Contracts;

/// <summary>
/// Represents an item in a tree view control.
/// </summary>
public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
{
    #region Is Selected
    /// <summary>
    /// Identifies the <see cref="IsSelected"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(ExtendedTreeViewItemBase));

    /// <summary>
    /// Gets or sets a value indicating whether the item is selected.
    /// </summary>
    public bool IsSelected
    {
        get => Selector.GetIsSelected(this);
        set => Selector.SetIsSelected(this, value);
    }
    #endregion

    #region Is Expanded
    /// <summary>
    /// Identifies the <see cref="IsExpanded"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, new PropertyChangedCallback(OnIsExpandedChanged)));

    /// <summary>
    /// Gets or sets a value indicating whether the item is expanded.
    /// </summary>
    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    /// <summary>
    /// Handles changes of the <see cref="IsExpanded"/> property.
    /// </summary>
    /// <param name="control">The modified object.</param>
    /// <param name="args">An object that contains event data.</param>
    [Access("protected", "static")]
    [RequireNotNull(nameof(control), Type = "DependencyObject", Name = "modifiedObject")]
    private static void OnIsExpandedChangedVerified(ExtendedTreeViewItemBase control, DependencyPropertyChangedEventArgs args) => control.OnIsExpandedChanged(args);

    /// <summary>
    /// Handles changes of the <see cref="IsExpanded"/> property.
    /// </summary>
    /// <param name="args">An object that contains event data.</param>
    protected virtual void OnIsExpandedChanged(DependencyPropertyChangedEventArgs args)
    {
        if (!IsContentInitializing)
        {
            bool NewIsExpanded = (bool)args.NewValue;
            if (NewIsExpanded)
                Host.SetItemExpanded(Content);
            else
                Host.SetItemCollapsed(Content);
        }
    }

    /// <summary>
    /// Begins initialization of the item content.
    /// </summary>
    protected virtual void BeginInitializeContent() => IsContentInitializing = true;

    /// <summary>
    /// Ends initialization of the item content.
    /// </summary>
    protected virtual void EndInitializeContent() => IsContentInitializing = false;

    /// <summary>
    /// Gets a value indicating whether the content is being initialized.
    /// </summary>
    protected bool IsContentInitializing { get; private set; }
    #endregion

    #region Is Drop Over
    /// <summary>
    /// Identifies the <see cref="IsDropOver"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IsDropOverProperty = Contract.AssertNotNull(IsDropOverPropertyKey).DependencyProperty;
    private static readonly DependencyPropertyKey IsDropOverPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDropOver), typeof(bool), typeof(ExtendedTreeViewItemBase), new PropertyMetadata(false));

    /// <summary>
    /// Gets a value indicating whether the item is the destination of a drop.
    /// </summary>
    public bool IsDropOver => (bool)GetValue(IsDropOverProperty);
    #endregion
}

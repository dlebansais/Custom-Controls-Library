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
        get { return Selector.GetIsSelected(this); }
        set { Selector.SetIsSelected(this, value); }
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
        get { return (bool)GetValue(IsExpandedProperty); }
        set { SetValue(IsExpandedProperty, value); }
    }

    /// <summary>
    /// Handles changes of the <see cref="IsExpanded"/> property.
    /// </summary>
    /// <param name="modifiedObject">The modified object.</param>
    /// <param name="args">An object that contains event data.</param>
    protected static void OnIsExpandedChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs args)
    {
        ExtendedTreeViewItemBase ctrl = Contract.AssertNotNull((ExtendedTreeViewItemBase)modifiedObject);
        ctrl.OnIsExpandedChanged(args);
    }

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
    protected virtual void BeginInitializeContent()
    {
        IsContentInitializing = true;
    }

    /// <summary>
    /// Ends initialization of the item content.
    /// </summary>
    protected virtual void EndInitializeContent()
    {
        IsContentInitializing = false;
    }

    /// <summary>
    /// Gets a value indicating whether the content is being initialized.
    /// </summary>
    protected bool IsContentInitializing { get; private set; }
    #endregion

    #region Is Drop Over
    /// <summary>
    /// Identifies the <see cref="IsDropOver"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IsDropOverProperty = IsDropOverPropertyKey?.DependencyProperty!;
    private static readonly DependencyPropertyKey IsDropOverPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsDropOver), typeof(bool), typeof(ExtendedTreeViewItemBase), new PropertyMetadata(false));

    /// <summary>
    /// Gets a value indicating whether the item is the destination of a drop.
    /// </summary>
    public bool IsDropOver
    {
        get { return (bool)GetValue(IsDropOverProperty); }
    }
    #endregion
}

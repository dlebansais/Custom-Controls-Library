﻿namespace CustomControls;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Contracts;

/// <summary>
/// Represents an item in a tree view control.
/// </summary>
public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
{
    /// <summary>
    /// Overrides inherited metadata.
    /// </summary>
    protected static void OverrideAncestorMetadata()
    {
        OverrideMetadataContent();
        OverrideMetadataDefaultStyleKey();
    }

    /// <summary>
    /// Override metadata for the <see cref="ContentControl.Content"/> property.
    /// </summary>
    protected static void OverrideMetadataContent()
    {
        ContentProperty.OverrideMetadata(typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnContentChanged)));
    }

    /// <summary>
    /// Override metadata for the <see cref="FrameworkElement.DefaultStyleKey"/> property.
    /// </summary>
    protected static void OverrideMetadataDefaultStyleKey()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ExtendedTreeViewItemBase), new FrameworkPropertyMetadata(typeof(ExtendedTreeViewItemBase)));
    }

    /// <summary>
    /// Called when the <see cref="ContentControl.Content"/> property has changed.
    /// </summary>
    /// <param name="modifiedObject">The object for which the property changed.</param>
    /// <param name="e">The event data.</param>
    protected static void OnContentChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
    {
        ExtendedTreeViewItemBase ctrl = Contract.AssertNotNull((ExtendedTreeViewItemBase)modifiedObject);
        ctrl.OnContentChanged(e);
    }

    /// <summary>
    /// Called when the <see cref="ContentControl.Content"/> property has changed.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e)
    {
        BeginInitializeContent();

        object NewContent = e.NewValue;
        IsExpanded = Host.IsExpanded(NewContent);

        EndInitializeContent();
    }
}

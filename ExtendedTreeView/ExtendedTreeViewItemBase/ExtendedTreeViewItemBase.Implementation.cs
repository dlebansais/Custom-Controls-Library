namespace CustomControls;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

/// <summary>
/// Represents an item in a tree view control.
/// </summary>
public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
{
    /// <summary>
    /// Called when the <see cref="ContentControl.Content"/> property changes.
    /// </summary>
    /// <param name="oldContent">The old value of the <see cref="ContentControl.Content"/> property.</param>
    /// <param name="newContent">The new value of the <see cref="ContentControl.Content"/> property.</param>
    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);

        if (newContent != BindingOperations.DisconnectedSource)
            NotifyPropertyChanged(nameof(Level));
    }

    /// <summary>
    /// Raises the <see cref="UIElement.GotFocus"/> routed event by using the event data that is provided.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);

        Host.ContainerGotFocus(this);
    }

    /// <summary>
    /// Raises the <see cref="UIElement.LostFocus"/> routed event by using the event data that is provided.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        Host.ContainerLostFocus();

        base.OnLostFocus(e);
    }
}

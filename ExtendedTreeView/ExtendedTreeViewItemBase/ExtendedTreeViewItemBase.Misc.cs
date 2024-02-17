namespace CustomControls;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

/// <summary>
/// Represents an item in a tree view control.
/// </summary>
public partial class ExtendedTreeViewItemBase : ContentControl, INotifyPropertyChanged
{
    #region Debugging
    /// <summary>
    /// Logs a call entry.
    /// </summary>
    /// <param name="callerName">Name of the caller.</param>
    protected virtual void DebugCall([CallerMemberName] string callerName = "")
    {
        bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
        if (EnableTraces)
            System.Diagnostics.Debug.Print(GetType().Name + ": " + callerName);
    }

    /// <summary>
    /// Logs a message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    [Localizable(false)]
    protected virtual void DebugMessage(string message)
    {
        bool EnableTraces = ExtendedTreeViewBase.EnableTraces;
        if (EnableTraces)
            System.Diagnostics.Debug.Print(GetType().Name + ": " + message);
    }
    #endregion

    #region Implementation of INotifyPropertyChanged
    /// <summary>
    /// Implements the PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Invoke handlers of the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Invoke handlers of the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

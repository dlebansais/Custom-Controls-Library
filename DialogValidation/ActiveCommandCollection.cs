namespace CustomControls;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Converters;

/// <summary>
/// Represents a collection of <see cref="ActiveCommand"/> objects.
/// </summary>
[TypeConverter(typeof(ActiveCommandCollectionTypeConverter))]
public class ActiveCommandCollection : ObservableCollection<ActiveCommand>
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="ActiveCommandCollection"/> class.
    /// </summary>
    public ActiveCommandCollection()
    {
        IsCollectionModified = false;
        CollectionChanged += OnCollectionChanged;
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets a value indicating whether the collection was changed after initialisation.
    /// </summary>
    /// <remarks>
    /// Allows the caller to distinguish between an empty and an uninitialized collection.
    /// </remarks>
    public bool IsCollectionModified { get; private set; }
    #endregion

    #region Implementation
    /// <summary>
    /// Called when the content of the collection has changed.
    /// </summary>
    /// <param name="sender">This parameter is not used.</param>
    /// <param name="e">Arguments for the associated event.</param>
    protected virtual void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SetCollectionModified();
    }

    /// <summary>
    /// Marks the collection as modified.
    /// </summary>
    protected virtual void SetCollectionModified()
    {
        IsCollectionModified = true;
    }
    #endregion
}

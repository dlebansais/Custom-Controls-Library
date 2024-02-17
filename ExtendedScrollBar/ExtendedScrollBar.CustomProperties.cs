namespace CustomControls;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

/// <summary>
/// Represents a scroll with specific support for binding to a scrollviewer.
/// <para>Implemented as a derived class of the <see cref="ScrollBar"/> parent.</para>
/// </summary>
/// <remarks>
/// <para>. Retain all features of a standard scrollbar.</para>
/// <para>. Include an additional property to use for binding specifically on a scrollviewer.</para>
/// </remarks>
public partial class ExtendedScrollBar : ScrollBar
{
    #region Bound Scroll Viewer
    /// <summary>
    /// Identifies the <see cref="BoundScrollViewer"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="BoundScrollViewer"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty BoundScrollViewerProperty = DependencyProperty.Register("BoundScrollViewer", typeof(ScrollViewer), typeof(ExtendedScrollBar), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnBoundScrollViewerPropertyChanged)));

    /// <summary>
    /// Gets or sets the scroll viewer property to bind on.
    /// </summary>
    [Bindable(true)]
    public ScrollViewer BoundScrollViewer
    {
        get { return (ScrollViewer)GetValue(BoundScrollViewerProperty); }
        set { SetValue(BoundScrollViewerProperty, value); }
    }

    private static void OnBoundScrollViewerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ExtendedScrollBar ctrl = (ExtendedScrollBar)d;
        ctrl.OnBoundScrollViewerPropertyChanged(e);
    }

    private void OnBoundScrollViewerPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is not null)
            UpdateBindings();
    }
    #endregion
}

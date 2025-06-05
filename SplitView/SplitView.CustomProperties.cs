namespace CustomControls;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

/// <summary>
/// <para>Represents a view of an arbitrary content that can be split horizontally in two views.</para>
/// <para>Implemented as a user control with a <see cref="ViewTemplate"/> template for views.</para>
/// </summary>
/// <remarks>
/// <para>Features:</para>
/// <para>. Begins as a single view with a grip on the top right corner to start splitting.</para>
/// <para>. Uses a <see cref="GridSplitter"/> to separate the views.</para>
/// <para>. Hides one of the views if it becomes too small to be visible.</para>
/// </remarks>
public partial class SplitView : UserControl, INotifyPropertyChanged
{
    #region Cell Template
    /// <summary>
    /// Identifies the <see cref="ViewTemplate"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ViewTemplate"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ViewTemplateProperty = DependencyProperty.Register(nameof(ViewTemplate), typeof(DataTemplate), typeof(SplitView), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the <see cref="DataTemplate"/> used to display each view.
    /// </summary>
    public DataTemplate ViewTemplate
    {
        get => (DataTemplate)GetValue(ViewTemplateProperty);
        set => SetValue(ViewTemplateProperty, value);
    }
    #endregion

    #region Zoom Options
    private static readonly DependencyPropertyKey ZoomOptionsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(ZoomOptions), typeof(Collection<double>), typeof(SplitView), new PropertyMetadata(null));
    /// <summary>
    /// Identifies the <see cref="ZoomOptions"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ZoomOptions"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ZoomOptionsProperty = ZoomOptionsPropertyKey.DependencyProperty;

    /// <summary>
    /// Gets a collection of zooms that can be applied to each view separately.
    /// </summary>
    public Collection<double> ZoomOptions => (Collection<double>)GetValue(ZoomOptionsProperty);
    #endregion

    #region Content Loaded
    /// <summary>
    /// Identifies the ViewLoaded routed event.
    /// </summary>
    public static readonly RoutedEvent ViewLoadedEvent = EventManager.RegisterRoutedEvent(nameof(ViewLoaded), RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

    /// <summary>
    /// Sent when a view has been loaded.
    /// </summary>
    public event RoutedEventHandler ViewLoaded
    {
        add { AddHandler(ViewLoadedEvent, value); }
        remove { RemoveHandler(ViewLoadedEvent, value); }
    }

    /// <summary>
    /// Sends the <see cref="ViewLoaded"/> event.
    /// </summary>
    /// <param name="viewContent">The control representing the view that has been loaded.</param>
    protected virtual void NotifyViewLoaded(FrameworkElement viewContent)
    {
        ViewLoadedEventArgs Args = new(ViewLoadedEvent, viewContent);
        RaiseEvent(Args);
    }
    #endregion

    #region Content Unloaded
    /// <summary>
    /// Identifies the ViewUnloaded routed event.
    /// </summary>
    public static readonly RoutedEvent ViewUnloadedEvent = EventManager.RegisterRoutedEvent(nameof(ViewUnloaded), RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

    /// <summary>
    /// Sent when a view has been unloaded.
    /// </summary>
    public event RoutedEventHandler ViewUnloaded
    {
        add { AddHandler(ViewUnloadedEvent, value); }
        remove { RemoveHandler(ViewUnloadedEvent, value); }
    }

    /// <summary>
    /// Sends the <see cref="ViewUnloaded"/> event.
    /// </summary>
    /// <param name="viewContent">The control representing the view that has been unloaded.</param>
    protected virtual void NotifyViewUnloaded(FrameworkElement viewContent)
    {
        ViewUnloadedEventArgs Args = new(ViewUnloadedEvent, viewContent);
        RaiseEvent(Args);
    }
    #endregion

    #region Top Row Visibility Changed
    /// <summary>
    /// Identifies the TopRowVisibilityChanged routed event.
    /// </summary>
    public static readonly RoutedEvent TopRowVisibilityChangedEvent = EventManager.RegisterRoutedEvent(nameof(TopRowVisibilityChanged), RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

    /// <summary>
    /// Sent when the top row view has becomed visible or collapsed.
    /// </summary>
    public event RoutedEventHandler TopRowVisibilityChanged
    {
        add { AddHandler(TopRowVisibilityChangedEvent, value); }
        remove { RemoveHandler(TopRowVisibilityChangedEvent, value); }
    }

    /// <summary>
    /// Sends the <see cref="TopRowVisibilityChanged"/> event.
    /// </summary>
    protected virtual void NotifyTopRowVisibilityChanged()
    {
        RoutedEventArgs Args = new(TopRowVisibilityChangedEvent, this);
        RaiseEvent(Args);
    }
    #endregion

    #region Zoom Changed
    /// <summary>
    /// Identifies the ZoomChanged routed event.
    /// </summary>
    public static readonly RoutedEvent ZoomChangedEvent = EventManager.RegisterRoutedEvent(nameof(ZoomChanged), RoutingStrategy.Bubble, typeof(RoutedEvent), typeof(SplitView));

    /// <summary>
    /// Sent when the zoom of one of the views has changed.
    /// </summary>
    public event RoutedEventHandler ZoomChanged
    {
        add { AddHandler(ZoomChangedEvent, value); }
        remove { RemoveHandler(ZoomChangedEvent, value); }
    }

    /// <summary>
    /// Sends the <see cref="ViewUnloaded"/> event.
    /// </summary>
    /// <param name="viewContent">The control representing the view to which the new zoom applies.</param>
    /// <param name="zoom">The new zoom value.</param>
    /// <remarks>
    /// A value of <paramref name="zoom"/>=1.0 means no zoom (or 100%).
    /// </remarks>
    protected virtual void NotifyZoomChanged(FrameworkElement viewContent, double zoom)
    {
        ZoomChangedEventArgs Args = new(ZoomChangedEvent, viewContent, zoom);
        RaiseEvent(Args);
    }
    #endregion
}

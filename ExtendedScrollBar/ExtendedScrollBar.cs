namespace CustomControls;

using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

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
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedScrollBar"/> class.
    /// </summary>
    public ExtendedScrollBar()
        : base()
    {
    }
    #endregion

    #region Implementation
    /// <summary>
    /// Binds the Minimum, Maximum and ViewportSize properties to the new ScrollViewer.
    /// </summary>
    private void UpdateBindings()
    {
        AddHandler(ScrollEvent, new ScrollEventHandler(OnScroll));
        BoundScrollViewer.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(BoundScrollChanged));

        Minimum = 0;

        bool IsHandled = false;

        switch (Orientation)
        {
            case Orientation.Horizontal:
                _ = SetBinding(MaximumProperty, new Binding("ScrollableWidth") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                _ = SetBinding(ViewportSizeProperty, new Binding("ViewportWidth") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                IsHandled = true;
                break;

            case Orientation.Vertical:
                _ = SetBinding(MaximumProperty, new Binding("ScrollableHeight") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                _ = SetBinding(ViewportSizeProperty, new Binding("ViewportHeight") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                IsHandled = true;
                break;
        }

        Debug.Assert(IsHandled);

        LargeChange = 242;
        SmallChange = 16;
    }

    /// <summary>
    /// Called when changes are detected to the scroll position, extent, or viewport size.
    /// </summary>
    /// <parameters>
    /// <param name="sender">This parameter is not used.</param>
    /// <param name="e">Describes a change in the scrolling state.</param>
    /// </parameters>
    private void BoundScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        ScrollChangedEventArgs args = Contracts.Contract.AssertNotNull(e);

        bool IsHandled = false;

        switch (Orientation)
        {
            case Orientation.Horizontal:
                Value = args.HorizontalOffset;
                IsHandled = true;
                break;

            case Orientation.Vertical:
                Value = args.VerticalOffset;
                IsHandled = true;
                break;
        }

        Debug.Assert(IsHandled);
    }

    /// <summary>
    /// Called as content scrolls in a scrollbar when the user moves the thumb by using the mouse.
    /// </summary>
    /// <parameters>
    /// <param name="sender">This parameter is not used.</param>
    /// <param name="e">Provides data for a System.Windows.Controls.Primitives.ScrollBar.Scroll event.</param>
    /// </parameters>
    private void OnScroll(object sender, ScrollEventArgs e)
    {
        ScrollEventArgs args = Contracts.Contract.AssertNotNull(e);

        bool IsHandled = false;

        switch (Orientation)
        {
            case Orientation.Horizontal:
                BoundScrollViewer.ScrollToHorizontalOffset(args.NewValue);
                IsHandled = true;
                break;

            case Orientation.Vertical:
                BoundScrollViewer.ScrollToVerticalOffset(args.NewValue);
                IsHandled = true;
                break;
        }

        Debug.Assert(IsHandled);
    }
    #endregion
}

namespace CustomControls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
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
    public class ExtendedScrollBar : ScrollBar
    {
        #region Custom properties and events
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
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            if (e.NewValue != null)
                UpdateBindings();
        }
        #endregion
        #endregion

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
            AddHandler(ScrollBar.ScrollEvent, new ScrollEventHandler(OnScroll));
            BoundScrollViewer.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(BoundScrollChanged));

            Minimum = 0;

            bool IsHandled = false;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    SetBinding(ScrollBar.MaximumProperty, new Binding("ScrollableWidth") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                    SetBinding(ScrollBar.ViewportSizeProperty, new Binding("ViewportWidth") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                    IsHandled = true;
                    break;

                case Orientation.Vertical:
                    SetBinding(ScrollBar.MaximumProperty, new Binding("ScrollableHeight") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
                    SetBinding(ScrollBar.ViewportSizeProperty, new Binding("ViewportHeight") { Source = BoundScrollViewer, Mode = BindingMode.OneWay });
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
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            bool IsHandled = false;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    Value = e.HorizontalOffset;
                    IsHandled = true;
                    break;

                case Orientation.Vertical:
                    Value = e.VerticalOffset;
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
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            bool IsHandled = false;

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    BoundScrollViewer.ScrollToHorizontalOffset(e.NewValue);
                    IsHandled = true;
                    break;

                case Orientation.Vertical:
                    BoundScrollViewer.ScrollToVerticalOffset(e.NewValue);
                    IsHandled = true;
                    break;
            }

            Debug.Assert(IsHandled);
        }
        #endregion
    }
}

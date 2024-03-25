namespace CustomControls;

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseLeftButtonDown"/> routed event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();

        base.OnPreviewMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonDown"/> routed event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnMouseLeftButtonDown(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseLeftButtonUp"/> routed event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseLeftButtonUp(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseLeftButtonUp"/> routed event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        DebugCall();
        base.OnMouseLeftButtonUp(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.PreviewMouseMove"/> routed event reaches an element in its route that is derived from this class.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
        DebugCall();
        base.OnPreviewMouseMove(e);
    }

    /// <summary>
    /// Invoked when an unhandled <see cref="UIElement.MouseMove"/> routed event is raised on this element.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
        DebugCall();
        DragAfterMouseMove(e);
        base.OnMouseMove(e);
    }
}

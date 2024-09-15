namespace CustomControls;

/// <summary>
/// A proxy to hold a reference to a <see cref="ExtendedToolBarButton"/> object.
/// </summary>
/// <param name="Button">Gets the stored <see cref="ExtendedToolBarButton"/> object.</param>
public record ExtendedToolBarItem(ExtendedToolBarButton Button)
{
    /// <summary>
    /// Gets the stored <see cref="ExtendedToolBarButton"/> object.
    /// </summary>
    public ExtendedToolBarButton Button { get; private set; } = Button;
}

namespace CustomControls;

/// <summary>
/// A proxy to hold a reference to a <see cref="ExtendedToolBarButton"/> object.
/// </summary>
public class ExtendedToolBarItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedToolBarItem"/> class.
    /// </summary>
    /// <param name="button">The <see cref="ExtendedToolBarButton"/> object to store.</param>
    public ExtendedToolBarItem(ExtendedToolBarButton button)
    {
        Button = button;
    }

    /// <summary>
    /// Gets the stored <see cref="ExtendedToolBarButton"/> object.
    /// </summary>
    /// <returns>
    /// The stored <see cref="ExtendedToolBarButton"/> object.
    /// </returns>
    public ExtendedToolBarButton Button { get; private set; }
}

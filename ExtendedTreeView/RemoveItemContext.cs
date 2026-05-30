namespace CustomControls;

/// <summary>
/// Represents a remove item context.
/// </summary>
public class RemoveItemContext : ModifyContext, IRemoveItemContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveItemContext"/> class.
    /// </summary>
    /// <param name="shownIndex">Index of the item.</param>
    public RemoveItemContext(int shownIndex)
        : base(shownIndex)
    {
    }

    /// <inheritdoc />
    public override void NextIndex()
    {
    }
}

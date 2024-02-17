namespace CustomControls;

/// <summary>
/// Represents the abstract interface for an insert item context.
/// </summary>
public interface IInsertItemContext : IModifyContext
{
}

/// <summary>
/// Represents an insert item context.
/// </summary>
public class InsertItemContext : ModifyContext, IInsertItemContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InsertItemContext"/> class.
    /// </summary>
    /// <param name="shownIndex">Index where insertion should take place.</param>
    public InsertItemContext(int shownIndex)
        : base(shownIndex)
    {
    }

    /// <summary>
    /// Moves to the next index.
    /// </summary>
    public override void NextIndex()
    {
        ShownIndex++;
    }
}

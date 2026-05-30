namespace CustomControls;

/// <summary>
/// Represents the abstract interface for a modify item context.
/// </summary>
public interface IModifyContext
{
    /// <summary>
    /// Gets the index of the item.
    /// </summary>
    int ShownIndex { get; }

    /// <summary>
    /// Starts the operation.
    /// </summary>
    void Start();

    /// <summary>
    /// Moves to the next index.
    /// </summary>
    void NextIndex();

    /// <summary>
    /// Complete the operation.
    /// </summary>
    void Complete();

    /// <summary>
    /// Closes the context.
    /// </summary>
    void Close();
}

/// <summary>
/// Represents a modify item context.
/// </summary>
/// <param name="shownIndex">Index of the item.</param>
public abstract class ModifyContext(int shownIndex) : IModifyContext
{
    /// <inheritdoc />
    public int ShownIndex { get; protected set; } = shownIndex;

    /// <inheritdoc />
    public virtual void Start()
    {
    }

    /// <inheritdoc />
    public abstract void NextIndex();

    /// <inheritdoc />
    public virtual void Complete()
    {
    }

    /// <inheritdoc />
    public virtual void Close()
    {
    }
}

namespace UndoRedo;

/// <summary>
/// Represents an operation that does nothing.
/// </summary>
internal class IdentityOperation : IReversibleOperation
{
    #region Init
    private IdentityOperation()
    {
    }

    /// <summary>
    /// Gets the singleton identity operation.
    /// </summary>
    public static IdentityOperation Default { get; } = new IdentityOperation();
    #endregion

    #region Properties
    /// <inheritdoc />
    public string Name { get; } = string.Empty;
    #endregion

    #region Client Interface
    /// <inheritdoc />
    public void Redo()
    {
    }

    /// <inheritdoc />
    public void Undo()
    {
    }
    #endregion
}

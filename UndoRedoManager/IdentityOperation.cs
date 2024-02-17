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
    /// <summary>
    /// Gets the operation name.
    /// </summary>
    public string Name { get; } = string.Empty;
    #endregion

    #region Client Interface
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public void Redo()
    {
    }

    /// <summary>
    /// Returns the system to the state before the operation was executed.
    /// </summary>
    public void Undo()
    {
    }
    #endregion
}

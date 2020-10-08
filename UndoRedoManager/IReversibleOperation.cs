namespace UndoRedo
{
    /// <summary>
    /// Interface that all reversible operation classes implement.
    /// </summary>
    public interface IReversibleOperation
    {
        /// <summary>
        /// Gets the operation name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes the operation.
        /// </summary>
        void Redo();

        /// <summary>
        /// Returns the system to the state before the operation was executed.
        /// </summary>
        void Undo();
    }
}

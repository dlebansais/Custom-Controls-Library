namespace UndoRedo
{
    /// <summary>
    /// Interface that all reversible operation classes implement.
    /// </summary>
    public interface IReversibleOperation
    {
        /// <summary>
        /// Name of the operation
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Execute the operation.
        /// </summary>
        void Redo();

        /// <summary>
        /// Return the system to the state before the operation was executed.
        /// </summary>
        void Undo();
    }
}

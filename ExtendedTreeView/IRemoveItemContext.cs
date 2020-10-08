namespace CustomControls
{
    /// <summary>
    /// Represents the abstract interface for a remove item context.
    /// </summary>
    public interface IRemoveItemContext : IModifyContext
    {
    }

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

        /// <summary>
        /// Moves to the next index.
        /// </summary>
        public override void NextIndex()
        {
        }
    }
}

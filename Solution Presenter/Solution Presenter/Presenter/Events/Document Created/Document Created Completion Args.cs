namespace CustomControls
{
    /// <summary>
    /// Represents the event data for a document created completion event.
    /// </summary>
    internal interface IDocumentCreatedCompletionArgs
    {
        /// <summary>
        /// Gets the path to the created document item.
        /// </summary>
        IItemPath NewItemPath { get; }

        /// <summary>
        /// Gets the path to the created document properties.
        /// </summary>
        IItemProperties NewItemProperties { get; }
    }

    /// <summary>
    /// Represents the event data for a document created completion event.
    /// </summary>
    internal class DocumentCreatedCompletionArgs : IDocumentCreatedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCreatedCompletionArgs"/> class.
        /// </summary>
        /// <param name="newItemPath">The path to the created document item.</param>
        /// <param name="newItemProperties">The path to the created document properties.</param>
        public DocumentCreatedCompletionArgs(IItemPath newItemPath, IItemProperties newItemProperties)
        {
            NewItemPath = newItemPath;
            NewItemProperties = newItemProperties;
        }

        /// <summary>
        /// Gets the path to the created document item.
        /// </summary>
        public IItemPath NewItemPath { get; private set; }

        /// <summary>
        /// Gets the path to the created document properties.
        /// </summary>
        public IItemProperties NewItemProperties { get; private set; }
    }
}

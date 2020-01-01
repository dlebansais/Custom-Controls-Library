namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents the event data for a document added completion event.
    /// </summary>
    internal interface IDocumentAddedCompletionArgs
    {
        /// <summary>
        /// Gets the list of added items.
        /// </summary>
        IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable { get; }

        /// <summary>
        /// Gets the list of added item properties.
        /// </summary>
        IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable { get; }
    }

    /// <summary>
    /// Represents the event data for a document added completion event.
    /// </summary>
    internal class DocumentAddedCompletionArgs : IDocumentAddedCompletionArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAddedCompletionArgs"/> class.
        /// </summary>
        public DocumentAddedCompletionArgs()
        {
            AddedItemTable = new Dictionary<IDocumentPath, IItemPath>();
            AddedPropertiesTable = new Dictionary<IDocumentPath, IItemProperties>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAddedCompletionArgs"/> class.
        /// </summary>
        /// <param name="addedItemTable">The list of added items.</param>
        /// <param name="addedPropertiesTable">The list of added item properties.</param>
        public DocumentAddedCompletionArgs(IReadOnlyDictionary<IDocumentPath, IItemPath> addedItemTable, IReadOnlyDictionary<IDocumentPath, IItemProperties> addedPropertiesTable)
        {
            AddedItemTable = addedItemTable;
            AddedPropertiesTable = addedPropertiesTable;
        }

        /// <summary>
        /// Gets the list of added items.
        /// </summary>
        public IReadOnlyDictionary<IDocumentPath, IItemPath> AddedItemTable { get; private set; }

        /// <summary>
        /// Gets the list of added item properties.
        /// </summary>
        public IReadOnlyDictionary<IDocumentPath, IItemProperties> AddedPropertiesTable { get; private set; }
    }
}

namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a context for the <see cref="DocumentClosedEventArgs"/> event data.
    /// </summary>
    public class DocumentClosedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentClosedEventContext"/> class.
        /// </summary>
        /// <param name="documentOperation">The operation.</param>
        /// <param name="closedDocumentList">The list of closed documents.</param>
        /// <param name="closedTree">The tree where documents are closed.</param>
        /// <param name="isUndoRedo">True if the operation can be undone.</param>
        /// <param name="clientInfo">The operation data.</param>
        public DocumentClosedEventContext(DocumentOperation documentOperation, IList<IDocument> closedDocumentList, IReadOnlyDictionary<ITreeNodePath, IPathConnection> closedTree, bool isUndoRedo, object? clientInfo)
        {
            DocumentOperation = documentOperation;
            ClosedDocumentList = closedDocumentList;
            ClosedTree = closedTree;
            IsUndoRedo = isUndoRedo;
            ClientInfo = clientInfo;
        }

        /// <summary>
        /// Gets the operation.
        /// </summary>
        public DocumentOperation DocumentOperation { get; private set; }

        /// <summary>
        /// Gets the list of closed documents.
        /// </summary>
        public IList<IDocument> ClosedDocumentList { get; private set; }

        /// <summary>
        /// Gets tree where documents are closed.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> ClosedTree { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the operation can be undone.
        /// </summary>
        public bool IsUndoRedo { get; private set; }

        /// <summary>
        /// Gets the operation data.
        /// </summary>
        public object? ClientInfo { get; private set; }
    }
}

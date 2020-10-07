namespace CustomControls
{
    using System.Windows.Media;

    /// <summary>
    /// Represents a routed command for documents.
    /// </summary>
    public class DocumentRoutedCommand : ExtendedRoutedCommand
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentRoutedCommand"/> class.
        /// </summary>
        /// <param name="documentType">The document type.</param>
        public DocumentRoutedCommand(IDocumentType documentType)
        {
            this.DocumentType = documentType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the document type.
        /// </summary>
        public IDocumentType DocumentType { get; private set; }

        /// <summary>
        /// Gets the menu header.
        /// </summary>
        public override string MenuHeader { get { return DocumentType.NewDocumentMenuHeader; } }

        /// <summary>
        /// Gets the button tooltip.
        /// </summary>
        public override string ButtonToolTip { get { return DocumentType.NewDocumentButtonToolTip; } }

        /// <summary>
        /// Gets the image source.
        /// </summary>
        public override ImageSource ImageSource { get { return DocumentType.NewDocumentIcon; } }
        #endregion
    }
}

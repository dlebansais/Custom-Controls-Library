namespace CustomControls
{
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents the type of a document.
    /// </summary>
    public interface IDocumentType
    {
        /// <summary>
        /// Gets a value indicating whether this is the preferred document type.
        /// </summary>
        bool IsPreferred { get; }

        /// <summary>
        /// Gets the document icon.
        /// </summary>
        ImageSource Icon { get; }

        /// <summary>
        /// Gets the header for document menus.
        /// </summary>
        string NewDocumentMenuHeader { get; }

        /// <summary>
        /// Gets the icon for a new document.
        /// </summary>
        ImageSource NewDocumentIcon { get; }

        /// <summary>
        /// Gets the tooltip for the button that creates new documents.
        /// </summary>
        string NewDocumentButtonToolTip { get; }

        /// <summary>
        /// Called when checking if a new documents can be added.
        /// </summary>
        /// <param name="e">The event data.</param>
        void CanAddNewDocument(CanExecuteRoutedEventArgs e);

        /// <summary>
        /// Called when a new document is added.
        /// </summary>
        /// <param name="e">The event data.</param>
        void OnAddNewDocument(ExecutedRoutedEventArgs e);
    }

    /// <summary>
    /// Represents the type of a document.
    /// </summary>
    public abstract class DocumentType : IDocumentType
    {
        /// <summary>
        /// Gets or sets a value indicating whether this is the preferred document type.
        /// </summary>
        public bool IsPreferred { get; set; }

        /// <summary>
        /// Gets the document icon.
        /// </summary>
        public abstract ImageSource Icon { get; }

        /// <summary>
        /// Gets the header for document menus.
        /// </summary>
        public abstract string NewDocumentMenuHeader { get; }

        /// <summary>
        /// Gets the icon for a new document.
        /// </summary>
        public abstract ImageSource NewDocumentIcon { get; }

        /// <summary>
        /// Gets the tooltip for the button that creates new documents.
        /// </summary>
        public abstract string NewDocumentButtonToolTip { get; }

        /// <summary>
        /// Called when checking if a new document can be created.
        /// </summary>
        /// <param name="e">The event data.</param>
        public abstract void CanCreateNewDocument(CanExecuteRoutedEventArgs e);

        /// <summary>
        /// Called when creating a new document.
        /// </summary>
        /// <param name="e">The event data.</param>
        public abstract void CreateNewDocument(ExecutedRoutedEventArgs e);

        /// <summary>
        /// Called when checking if a new documents can be added.
        /// </summary>
        /// <param name="e">The event data.</param>
        public abstract void CanAddNewDocument(CanExecuteRoutedEventArgs e);

        /// <summary>
        /// Called when a new document is added.
        /// </summary>
        /// <param name="e">The event data.</param>
        public abstract void OnAddNewDocument(ExecutedRoutedEventArgs e);
    }
}

namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Represents a window control containing documents.
    /// </summary>
    public partial class DocumentsWindow : Window
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsWindow"/> class.
        /// </summary>
        /// <param name="documentList">The list of documents.</param>
        public DocumentsWindow(ObservableCollection<IDocument> documentList)
        {
            DocumentList = documentList;

            InitializeComponent();
            DataContext = this;

            Loaded += OnLoaded;
        }

        /// <summary>
        /// Called when the window has been loaded.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            Icon = Owner.Icon;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the list of documents.
        /// </summary>
        public ObservableCollection<IDocument> DocumentList { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a document is activated.
        /// </summary>
        public event EventHandler<DocumentWindowEventArgs>? DocumentActivated;

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentActivated"/> event.
        /// </summary>
        /// <param name="document">The activated document.</param>
        protected void NotifyDocumentActivated(IDocument document)
        {
            DocumentActivated?.Invoke(this, new DocumentWindowEventArgs(document));
        }

        /// <summary>
        /// Occurs when a document is saved.
        /// </summary>
        public event EventHandler<DocumentWindowEventArgs>? DocumentSaved;

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentSaved"/> event.
        /// </summary>
        /// <param name="document">The saved document.</param>
        protected void NotifyDocumentSaved(IDocument document)
        {
            DocumentSaved?.Invoke(this, new DocumentWindowEventArgs(document));
        }

        /// <summary>
        /// Occurs when a document is closed.
        /// </summary>
        public event EventHandler<DocumentWindowEventArgs>? DocumentClosed;

        /// <summary>
        /// Invokes handlers of the <see cref="DocumentClosed"/> event.
        /// </summary>
        /// <param name="document">The closed document.</param>
        protected void NotifyDocumentClosed(IDocument document)
        {
            DocumentClosed?.Invoke(this, new DocumentWindowEventArgs(document));
        }
        #endregion

        #region Implementation
        private void CanActivate(object sender, CanExecuteRoutedEventArgs e)
        {
            if (listviewDocuments.SelectedItems.Count == 1)
                e.CanExecute = true;
        }

        private void OnActivate(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (IDocument Document in listviewDocuments.SelectedItems)
                NotifyDocumentActivated(Document);

            Close();
        }

        private void CanSave(object sender, CanExecuteRoutedEventArgs e)
        {
            foreach (IDocument Document in listviewDocuments.SelectedItems)
                if (Document.IsDirty)
                {
                    e.CanExecute = true;
                    break;
                }
        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (IDocument Document in listviewDocuments.SelectedItems)
                NotifyDocumentSaved(Document);
        }

        private void CanCloseWindows(object sender, CanExecuteRoutedEventArgs e)
        {
            if (listviewDocuments.SelectedItems.Count > 0)
                e.CanExecute = true;
        }

        private void OnCloseWindows(object sender, ExecutedRoutedEventArgs e)
        {
            List<IDocument> CloseList = new List<IDocument>();
            foreach (IDocument Document in listviewDocuments.SelectedItems)
                CloseList.Add(Document);

            foreach (IDocument Document in CloseList)
                NotifyDocumentClosed(Document);
        }

        private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (IDocument Document in listviewDocuments.SelectedItems)
                NotifyDocumentActivated(Document);

            Close();
        }

        private void OnOK(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}

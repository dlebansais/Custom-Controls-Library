namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;

    public partial class DocumentsWindow : Window
    {
        #region Init
        public DocumentsWindow(ObservableCollection<IDocument> documentList)
        {
            this.DocumentList = documentList;

            InitializeComponent();
            DataContext = this;

            Loaded += OnLoaded;
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            Icon = Owner.Icon;
        }
        #endregion

        #region Properties
        public ObservableCollection<IDocument> DocumentList { get; private set; }
        #endregion

        #region Events
        public event EventHandler<DocumentWindowEventArgs>? DocumentActivated;
        protected void NotifyDocumentActivated(IDocument document)
        {
            DocumentActivated?.Invoke(this, new DocumentWindowEventArgs(document));
        }

        public event EventHandler<DocumentWindowEventArgs>? DocumentSaved;
        protected void NotifyDocumentSaved(IDocument document)
        {
            DocumentSaved?.Invoke(this, new DocumentWindowEventArgs(document));
        }

        public event EventHandler<DocumentWindowEventArgs>? DocumentClosed;
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

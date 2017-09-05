using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace CustomControls
{
    public interface IDocumentType
    {
        bool IsPreferred { get; }
        ImageSource Icon { get; }
        string NewDocumentMenuHeader { get; }
        ImageSource NewDocumentIcon { get; }
        string NewDocumentButtonToolTip { get; }
        //void CanCreateNewDocument(CanExecuteRoutedEventArgs e);
        //void CreateNewDocument(ExecutedRoutedEventArgs e);
        void CanAddNewDocument(CanExecuteRoutedEventArgs e);
        void OnAddNewDocument(ExecutedRoutedEventArgs e);
    }

    public abstract class DocumentType : IDocumentType
    {
        public bool IsPreferred { get; set; }
        public abstract ImageSource Icon { get; }
        public abstract string NewDocumentMenuHeader { get; }
        public abstract ImageSource NewDocumentIcon { get; }
        public abstract string NewDocumentButtonToolTip { get; }
        public abstract void CreateNewDocument(ExecutedRoutedEventArgs e);
        public abstract void CanCreateNewDocument(CanExecuteRoutedEventArgs e);
        public abstract void OnAddNewDocument(ExecutedRoutedEventArgs e);
        public abstract void CanAddNewDocument(CanExecuteRoutedEventArgs e);
    }
}

using System.Windows.Media;

namespace CustomControls
{
    public class DocumentRoutedCommand : ExtendedRoutedCommand
    {
        #region Init
        public DocumentRoutedCommand(IDocumentType documentType)
        {
            this.DocumentType = documentType;
        }
        #endregion

        #region Properties
        public IDocumentType DocumentType { get; private set; }
        public override string MenuHeader { get { return DocumentType.NewDocumentMenuHeader; } }
        public override string ButtonToolTip { get { return DocumentType.NewDocumentButtonToolTip; } }
        public override ImageSource ImageSource { get { return DocumentType.NewDocumentIcon; } }
        #endregion
    }
}

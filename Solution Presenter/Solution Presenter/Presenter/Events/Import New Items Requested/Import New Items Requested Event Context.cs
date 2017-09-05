using System.Collections.Generic;

namespace CustomControls
{
    public class ImportNewItemsRequestedEventContext
    {
        public ImportNewItemsRequestedEventContext(Dictionary<object, IDocumentType> importedDocumentTable, IList<IDocumentPath> documentPathList)
        {
            this.ImportedDocumentTable = importedDocumentTable;
            this.DocumentPathList = documentPathList;
        }

        public Dictionary<object, IDocumentType> ImportedDocumentTable { get; private set; }
        public IList<IDocumentPath> DocumentPathList { get; private set; }
    }
}

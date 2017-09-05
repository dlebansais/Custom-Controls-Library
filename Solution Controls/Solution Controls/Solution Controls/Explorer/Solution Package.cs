using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomControls
{
    public class SolutionPackage
    {
        public SolutionPackage()
        {
            CreationTime = DateTime.MinValue;
            CreateFolderTable = new Collection<FolderPackage>();
            ReplaceContentTable = new Dictionary<string, IDocumentPath>();
            CreateContentTable = new Dictionary<string, object>();
            CreateItemList = new Collection<string>();
        }

        public DateTime CreationTime { get; private set; }
        public bool CreateSolution { get; private set; }
        public Collection<FolderPackage> CreateFolderTable { get; private set; }
        public Dictionary<string, IDocumentPath> ReplaceContentTable { get; private set; }
        public Dictionary<string, object> CreateContentTable { get; private set; }
        public Collection<string> CreateItemList { get; private set; }

        public void SetSolutionCreated()
        {
            CreateSolution = true;
        }

        public void AddCreatedFolder(IFolderPath folderPath, IFolderPath subfolderPath)
        {
            CreateFolderTable.Add(new FolderPackage(folderPath, subfolderPath));
        }

        public void AddCreatedContent(string exportId, object content)
        {
            CreateContentTable.Add(exportId, content);
        }

        public void AddReplacedContent(string exportId, IDocumentPath documentPath)
        {
            ReplaceContentTable.Add(exportId, documentPath);
        }

        public void AddCreatedItem(string exportId)
        {
            CreateItemList.Add(exportId);
        }

        public void InitCreationTime()
        {
            CreationTime = DateTime.UtcNow;
        }
    }
}

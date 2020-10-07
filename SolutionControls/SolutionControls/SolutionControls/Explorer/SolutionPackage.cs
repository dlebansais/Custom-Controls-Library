namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a solution package.
    /// </summary>
    public class SolutionPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionPackage"/> class.
        /// </summary>
        public SolutionPackage()
        {
            CreationTime = DateTime.MinValue;
            CreateFolderTable = new Collection<FolderPackage>();
            ReplaceContentTable = new Dictionary<string, IDocumentPath>();
            CreateContentTable = new Dictionary<string, object>();
            CreateItemList = new Collection<string>();
        }

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the solution should be created.
        /// </summary>
        public bool CreateSolution { get; private set; }

        /// <summary>
        /// Gets the table of folders to create.
        /// </summary>
        public Collection<FolderPackage> CreateFolderTable { get; private set; }

        /// <summary>
        /// Gets the table of item content to replace.
        /// </summary>
        public Dictionary<string, IDocumentPath> ReplaceContentTable { get; private set; }

        /// <summary>
        /// Gets the table of item content to create.
        /// </summary>
        public Dictionary<string, object> CreateContentTable { get; private set; }

        /// <summary>
        /// Gets the list of created items.
        /// </summary>
        public Collection<string> CreateItemList { get; private set; }

        /// <summary>
        /// Sets the <see cref="CreateSolution"/> flags.
        /// </summary>
        public void SetSolutionCreated()
        {
            CreateSolution = true;
        }

        /// <summary>
        /// Adds a newly created folder to the solution.
        /// </summary>
        /// <param name="folderPath">The parent folder path.</param>
        /// <param name="subfolderPath">The folder path.</param>
        public void AddCreatedFolder(IFolderPath folderPath, IFolderPath subfolderPath)
        {
            CreateFolderTable.Add(new FolderPackage(folderPath, subfolderPath));
        }

        /// <summary>
        /// Adds a newly created item content to the solution.
        /// </summary>
        /// <param name="exportId">The content ID.</param>
        /// <param name="content">The content data.</param>
        public void AddCreatedContent(string exportId, object content)
        {
            CreateContentTable.Add(exportId, content);
        }

        /// <summary>
        /// Adds a replaced content to the solution.
        /// </summary>
        /// <param name="exportId">The content ID.</param>
        /// <param name="documentPath">The path to the new item.</param>
        public void AddReplacedContent(string exportId, IDocumentPath documentPath)
        {
            ReplaceContentTable.Add(exportId, documentPath);
        }

        /// <summary>
        /// Adds a newly created item to the solution.
        /// </summary>
        /// <param name="exportId">The item ID.</param>
        public void AddCreatedItem(string exportId)
        {
            CreateItemList.Add(exportId);
        }

        /// <summary>
        /// Initializes the created time to the current time.
        /// </summary>
        public void InitCreationTime()
        {
            CreationTime = DateTime.UtcNow;
        }
    }
}

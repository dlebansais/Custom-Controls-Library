namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents an add or remove operation in a solution explorer.
    /// </summary>
    internal abstract class AddRemoveOperation : SolutionExplorerOperation
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="AddRemoveOperation"/> class.
        /// </summary>
        /// <param name="root">The root path.</param>
        /// <param name="pathTable">The table of paths.</param>
        protected AddRemoveOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            PathTable = pathTable;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the table of paths.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this operation is adding something.
        /// </summary>
        public abstract bool IsAdd { get; }
        #endregion

        #region Descendant Interface
        /// <summary>
        /// Creates a folder in a solution.
        /// </summary>
        /// <param name="parentFolder">The parent folder.</param>
        /// <param name="path">The folder path.</param>
        /// <param name="properties">The folder properties.</param>
        /// <returns>The created folder.</returns>
        protected virtual ISolutionFolder CreateSolutionFolder(ISolutionFolder parentFolder, IFolderPath path, IFolderProperties properties)
        {
            if (parentFolder == null)
                throw new ArgumentNullException(nameof(parentFolder));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return new SolutionFolder(parentFolder, path, properties);
        }

        /// <summary>
        /// Creates an item in a solution.
        /// </summary>
        /// <param name="parentFolder">The parent folder.</param>
        /// <param name="path">The item path.</param>
        /// <param name="properties">The item properties.</param>
        /// <returns>The created item.</returns>
        protected virtual ISolutionItem CreateSolutionItem(ISolutionFolder parentFolder, IItemPath path, IItemProperties properties)
        {
            if (parentFolder == null)
                throw new ArgumentNullException(nameof(parentFolder));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return new SolutionItem(path, parentFolder, properties);
        }

        /// <summary>
        /// Adds a table of path.
        /// </summary>
        /// <param name="pathTable">The table to add.</param>
        protected virtual void Add(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            ClearExpandedFolders();

            List<ISolutionTreeNodeCollection> ModifiedCollectionList = new List<ISolutionTreeNodeCollection>();

            List<ITreeNodePath> PathList = new List<ITreeNodePath>();
            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in pathTable)
                PathList.Add(Entry.Key);

            while (PathList.Count > 0)
            {
                IReadOnlyDictionary<IFolderPath, ISolutionFolder> FlatFolderTable = Root.FlatFolderChildren;

                int i = 0;
                while (i < PathList.Count)
                {
                    ITreeNodePath Path = PathList[i];
                    IPathConnection Connection = pathTable[Path];
                    IFolderPath? ParentPath = Connection.ParentPath;

                    if (ParentPath != null && FlatFolderTable.ContainsKey(ParentPath))
                    {
                        PathList.RemoveAt(i);

                        ISolutionFolder ParentFolder = FlatFolderTable[ParentPath];
                        ISolutionTreeNodeCollection ChildrenCollection = (ISolutionTreeNodeCollection)ParentFolder.Children;
                        bool IsHandled = false;

                        switch (Path)
                        {
                            case IFolderPath AsFolderPath:
                                IFolderProperties FolderProperties = (IFolderProperties)Connection.Properties;

                                ISolutionFolder NewFolder = CreateSolutionFolder(ParentFolder, AsFolderPath, FolderProperties);
                                ChildrenCollection.Add(NewFolder);

                                if (Connection.IsExpanded)
                                    AddExpandedFolder(NewFolder);

                                IsHandled = true;
                                break;

                            case IItemPath AsItemPath:
                                IItemProperties ItemProperties = (IItemProperties)Connection.Properties;

                                ISolutionItem NewItem = CreateSolutionItem(ParentFolder, AsItemPath, ItemProperties);
                                ChildrenCollection.Add(NewItem);

                                IsHandled = true;
                                break;
                        }

                        Debug.Assert(IsHandled);

                        if (!ModifiedCollectionList.Contains(ChildrenCollection))
                            ModifiedCollectionList.Add(ChildrenCollection);
                    }
                    else
                        i++;
                }
            }

            foreach (ISolutionTreeNodeCollection ChildrenCollection in ModifiedCollectionList)
                ChildrenCollection.Sort();
        }

        /// <summary>
        /// Removes a table of paths.
        /// </summary>
        /// <param name="pathTable">The table of paths.</param>
        protected virtual void Remove(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            IReadOnlyDictionary<IFolderPath, ISolutionFolder> FlatFolderTable = Root.FlatFolderChildren;
            List<ISolutionTreeNodeCollection> ModifiedCollectionList = new List<ISolutionTreeNodeCollection>();

            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in pathTable)
            {
                ITreeNodePath Path = Entry.Key;
                IPathConnection Connection = Entry.Value;
                IFolderPath? ParentPath = Connection.ParentPath;

                if (ParentPath != null)
                {
                    ISolutionFolder ParentFolder = FlatFolderTable[ParentPath];
                    ISolutionTreeNodeCollection ChildrenCollection = (ISolutionTreeNodeCollection)ParentFolder.Children;

                    foreach (ISolutionTreeNode Child in (IEnumerable<ISolutionTreeNode>)ChildrenCollection)
                        if (Child.Path.IsEqual(Path))
                        {
                            ChildrenCollection.Remove(Child);
                            break;
                        }

                    if (!ModifiedCollectionList.Contains(ChildrenCollection))
                        ModifiedCollectionList.Add(ChildrenCollection);
                }
            }

            foreach (ISolutionTreeNodeCollection ChildrenCollection in ModifiedCollectionList)
                ChildrenCollection.Sort();
        }
        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CustomControls
{
    internal abstract class AddRemoveOperation : SolutionExplorerOperation
    {
        #region Init
        protected AddRemoveOperation(ISolutionRoot root, IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
            : base(root)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            this.PathTable = pathTable;
        }
        #endregion

        #region Properties
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }
        public abstract bool IsAdd { get; }
        #endregion

        #region Descendant Interface
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
                    IFolderPath ParentPath = Connection.ParentPath;

                    if (FlatFolderTable.ContainsKey(ParentPath))
                    {
                        PathList.RemoveAt(i);

                        ISolutionFolder ParentFolder = FlatFolderTable[ParentPath];
                        ISolutionTreeNodeCollection ChildrenCollection = (ISolutionTreeNodeCollection)ParentFolder.Children;

                        IFolderPath AsFolderPath;
                        IItemPath AsItemPath;
                        bool IsHandled = false;

                        if ((AsFolderPath = Path as IFolderPath) != null)
                        {
                            IFolderProperties Properties = (IFolderProperties)Connection.Properties;

                            ISolutionFolder NewFolder = CreateSolutionFolder(ParentFolder, AsFolderPath, Properties);
                            ChildrenCollection.Add(NewFolder);

                            if (Connection.IsExpanded)
                                AddExpandedFolder(NewFolder);

                            IsHandled = true;
                        }

                        else if ((AsItemPath = Path as IItemPath) != null)
                        {
                            IItemProperties Properties = (IItemProperties)Connection.Properties;

                            ISolutionItem NewItem = CreateSolutionItem(ParentFolder, AsItemPath, Properties);
                            ChildrenCollection.Add(NewItem);

                            IsHandled = true;
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
                IFolderPath ParentPath = Connection.ParentPath;

                ISolutionFolder ParentFolder = FlatFolderTable[ParentPath];
                ISolutionTreeNodeCollection ChildrenCollection = (ISolutionTreeNodeCollection)ParentFolder.Children;

                foreach (ISolutionTreeNode Child in ChildrenCollection)
                    if (Child.Path.IsEqual(Path))
                    {
                        ChildrenCollection.Remove(Child);
                        break;
                    }

                if (!ModifiedCollectionList.Contains(ChildrenCollection))
                    ModifiedCollectionList.Add(ChildrenCollection);
            }

            foreach (ISolutionTreeNodeCollection ChildrenCollection in ModifiedCollectionList)
                ChildrenCollection.Sort();
        }
        #endregion
    }
}

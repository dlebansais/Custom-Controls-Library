using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CustomControls
{
    public interface IPathGroup
    {
        IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; }
        IFolderPath? GroupParentPath { get; }
    }

    public class PathGroup : IPathGroup
    {
        #region Init
        public PathGroup(ITreeNodePath path, IFolderPath parentPath, ITreeNodeProperties properties)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (parentPath == null)
                throw new ArgumentNullException(nameof(parentPath));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            Dictionary<ITreeNodePath, IPathConnection> SinglePathTable = new Dictionary<ITreeNodePath, IPathConnection>();
            SinglePathTable.Add(path, new PathConnection(parentPath, properties, false));

            PathTable = SinglePathTable;
            GroupParentPath = parentPath;
        }

        public PathGroup(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            Debug.Assert(HasCommonParent(pathTable));

            Dictionary<ITreeNodePath, IPathConnection> PathTable = new Dictionary<ITreeNodePath, IPathConnection>();
            IFolderPath? GroupParentPath = null;

            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in pathTable)
            {
                ITreeNodePath Path = Entry.Key;
                IPathConnection PathConnection = Entry.Value;
                IFolderPath? ParentPath = PathConnection.ParentPath;
                ITreeNodeProperties Properties = PathConnection.Properties;
                bool IsExpanded = PathConnection.IsExpanded;

                if (ParentPath != null && !pathTable.ContainsKey(ParentPath))
                {
                    if (GroupParentPath == null)
                        GroupParentPath = ParentPath;
 
                    ParentPath = null;
                }

                PathTable.Add(Path, new PathConnection(ParentPath, Properties, IsExpanded));
            }

            this.PathTable = PathTable;
            this.GroupParentPath = GroupParentPath;
        }

        public PathGroup(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable, IFolderPath groupParentPath)
        {
            if (groupParentPath == null)
                throw new ArgumentNullException(nameof(groupParentPath));
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            Debug.Assert(HasNullCommonParent(pathTable));

            Dictionary<ITreeNodePath, IPathConnection> PathTable = new Dictionary<ITreeNodePath, IPathConnection>();

            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in pathTable)
            {
                ITreeNodePath Path = Entry.Key;
                IPathConnection PathConnection = Entry.Value;
                IFolderPath? ParentPath = PathConnection.ParentPath;
                ITreeNodeProperties Properties = PathConnection.Properties;
                bool IsExpanded = PathConnection.IsExpanded;

                if (ParentPath == null)
                    ParentPath = GroupParentPath;

                PathTable.Add(Path, new PathConnection(ParentPath, Properties, IsExpanded));
            }

            this.PathTable = PathTable;
            GroupParentPath = groupParentPath;
        }

        public static bool HasCommonParent(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            IFolderPath? GroupParentPath = null;
            bool? IsNullParent = null;
            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in pathTable)
            {
                IPathConnection PathConnection = Entry.Value;
                IFolderPath? ParentPath = PathConnection.ParentPath;

                if (ParentPath != null && pathTable.ContainsKey(ParentPath))
                    continue;

                if (!IsNullParent.HasValue)
                    IsNullParent = (ParentPath == null);

                else if (IsNullParent.Value != (ParentPath == null))
                    return false;

                if (ParentPath != null)
                {
                    if (GroupParentPath == null)
                        GroupParentPath = ParentPath;

                    else if (GroupParentPath != ParentPath)
                        return false;
                }
            }

            return true;
        }

        public static bool HasNullCommonParent(IReadOnlyDictionary<ITreeNodePath, IPathConnection> pathTable)
        {
            if (pathTable == null)
                throw new ArgumentNullException(nameof(pathTable));

            foreach (KeyValuePair<ITreeNodePath, IPathConnection> Entry in pathTable)
            {
                IPathConnection PathConnection = Entry.Value;
                IFolderPath? ParentPath = PathConnection.ParentPath;

                if (ParentPath != null && !pathTable.ContainsKey(ParentPath))
                    return false;
            }

            return true;
        }
        #endregion

        #region Properties
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }
        public IFolderPath? GroupParentPath { get; private set; }
        #endregion
    }
}

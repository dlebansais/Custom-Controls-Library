namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Represents a group of paths.
    /// </summary>
    public interface IPathGroup
    {
        /// <summary>
        /// Gets the table of paths in the group.
        /// </summary>
        IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; }

        /// <summary>
        /// Gets the parent of paths in the group.
        /// </summary>
        IFolderPath? GroupParentPath { get; }
    }

    /// <summary>
    /// Represents a group of paths.
    /// </summary>
    public class PathGroup : IPathGroup
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="PathGroup"/> class.
        /// </summary>
        /// <param name="path">The single member of the group.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="properties">Properties of the associated item.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGroup"/> class.
        /// </summary>
        /// <param name="pathTable">The table of items in the group.</param>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PathGroup"/> class.
        /// </summary>
        /// <param name="pathTable">The table of items in the group.</param>
        /// <param name="groupParentPath">The specified parent path.</param>
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

        /// <summary>
        /// Checks whether paths in a table have a common parent.
        /// </summary>
        /// <param name="pathTable">The table of items.</param>
        /// <returns>True if items have a common parent; otherwise, false.</returns>
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
                    IsNullParent = ParentPath == null;
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

        /// <summary>
        /// Checks whether paths in a table have no parents.
        /// </summary>
        /// <param name="pathTable">The table of items.</param>
        /// <returns>True if items don't have any parent; otherwise, false.</returns>
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
        /// <summary>
        /// Gets the table of paths in the group.
        /// </summary>
        public IReadOnlyDictionary<ITreeNodePath, IPathConnection> PathTable { get; private set; }

        /// <summary>
        /// Gets the parent of paths in the group.
        /// </summary>
        public IFolderPath? GroupParentPath { get; private set; }
        #endregion
    }
}

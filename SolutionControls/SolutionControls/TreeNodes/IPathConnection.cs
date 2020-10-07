namespace CustomControls
{
    using System;

    /// <summary>
    /// Represents a path connection.
    /// </summary>
    public interface IPathConnection
    {
        /// <summary>
        /// Gets the parent path.
        /// </summary>
        IFolderPath? ParentPath { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        ITreeNodeProperties Properties { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is expanded.
        /// </summary>
        bool IsExpanded { get; }
    }

    /// <summary>
    /// Represents a path connection.
    /// </summary>
    [Serializable]
    public class PathConnection : IPathConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathConnection"/> class.
        /// </summary>
        /// <param name="parentPath">The parent path.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="isExpanded">True if the connection is expanded.</param>
        public PathConnection(IFolderPath? parentPath, ITreeNodeProperties properties, bool isExpanded)
        {
            ParentPath = parentPath;
            Properties = properties;
            IsExpanded = isExpanded;
        }

        /// <summary>
        /// Gets the parent path.
        /// </summary>
        public IFolderPath? ParentPath { get; }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        public ITreeNodeProperties Properties { get; }

        /// <summary>
        /// Gets a value indicating whether the connection is expanded.
        /// </summary>
        public bool IsExpanded { get; }
    }
}

using System;

namespace CustomControls
{
    public interface IPathConnection
    {
        IFolderPath? ParentPath { get; }
        ITreeNodeProperties Properties { get; }
        bool IsExpanded { get; }
    }

    [Serializable]
    public class PathConnection : IPathConnection
    {
        public PathConnection(IFolderPath? parentPath, ITreeNodeProperties properties, bool isExpanded)
        {
            ParentPath = parentPath;
            Properties = properties;
            IsExpanded = isExpanded;
        }

        public IFolderPath? ParentPath { get; }
        public ITreeNodeProperties Properties { get; }
        public bool IsExpanded { get; }
    }
}

using System;

namespace CustomControls
{
    public interface IPathConnection
    {
        IFolderPath ParentPath { get; }
        ITreeNodeProperties Properties { get; }
        bool IsExpanded { get; }
    }

    [Serializable]
    public class PathConnection : IPathConnection
    {
        public PathConnection(IFolderPath parentPath, ITreeNodeProperties properties, bool isExpanded)
        {
            this.ParentPath = parentPath;
            this.Properties = properties;
            this.IsExpanded = isExpanded;
        }

        public IFolderPath ParentPath { get; private set; }
        public ITreeNodeProperties Properties { get; private set; }
        public bool IsExpanded { get; private set; }
    }
}

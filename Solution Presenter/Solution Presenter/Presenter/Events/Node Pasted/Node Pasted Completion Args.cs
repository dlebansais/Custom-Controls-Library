using System.Collections.Generic;

namespace CustomControls
{
    internal interface INodePastedCompletionArgs
    {
        ITreeNodePath NewPath { get; }
        ITreeNodeProperties NewProperties { get; }
    }

    internal class NodePastedCompletionArgs : INodePastedCompletionArgs
    {
        public NodePastedCompletionArgs(ITreeNodePath NewPath, ITreeNodeProperties NewProperties)
        {
            this.NewPath = NewPath;
            this.NewProperties = NewProperties;
        }

        public ITreeNodePath NewPath { get; private set; }
        public ITreeNodeProperties NewProperties { get; private set; }
    }
}

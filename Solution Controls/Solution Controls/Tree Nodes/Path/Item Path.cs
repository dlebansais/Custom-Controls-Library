using System.Windows.Media;

namespace CustomControls
{
    public interface IItemPath : ITreeNodePath
    {
        ImageSource Icon { get; }
        IDocumentPath DocumentPath { get; }
    }
}

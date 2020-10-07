namespace CustomControls
{
    using System.Windows.Media;

    /// <summary>
    /// Represents the path of an item.
    /// </summary>
    public interface IItemPath : ITreeNodePath
    {
        /// <summary>
        /// Gets the item icon.
        /// </summary>
        ImageSource Icon { get; }

        /// <summary>
        /// Gets the document path for the item.
        /// </summary>
        IDocumentPath DocumentPath { get; }
    }
}

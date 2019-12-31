namespace CustomControls
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents the path of a folder.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Used to define a type hierarchy")]
    public interface IFolderPath : ITreeNodePath
    {
    }
}

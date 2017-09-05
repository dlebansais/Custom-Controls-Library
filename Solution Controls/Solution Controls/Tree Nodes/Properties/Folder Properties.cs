using System.Diagnostics.CodeAnalysis;

namespace CustomControls
{
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "Used to define a type hierarchy")]
    public interface IFolderProperties : ITreeNodeProperties
    {
    }
}

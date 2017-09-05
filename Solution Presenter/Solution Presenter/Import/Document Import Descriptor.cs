using System.Diagnostics.CodeAnalysis;

namespace CustomControls
{
    public interface IDocumentImportDescriptor
    {
        string FileExtension { get; }
        string FriendlyImportName { get; }
        bool IsDefault { get; }
        ImportedContentDescriptor Import(string fileName);
    }
}

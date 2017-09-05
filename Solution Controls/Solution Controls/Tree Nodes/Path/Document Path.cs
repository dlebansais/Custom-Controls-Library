namespace CustomControls
{
    public interface IDocumentPath
    {
        string HeaderName { get; }
        string ContentId { get; }
        string ExportId { get; }
        bool IsEqual(IDocumentPath other);
    }
}

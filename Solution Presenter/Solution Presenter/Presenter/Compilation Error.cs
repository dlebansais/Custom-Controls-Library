namespace CustomControls
{
    public interface ICompilationError
    {
        string Description { get; }
        IDocumentPath Source { get; }
        object Location { get; }
    }
}

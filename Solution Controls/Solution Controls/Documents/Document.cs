namespace CustomControls
{
    public interface IDocument
    {
        IDocumentType Type { get; }
        IDocumentPath Path { get; }
        bool IsDirty { get; }
        void SetViewGotFocus();
        void ClearIsDirty();
        bool CanUndo();
        void OnUndo();
        bool CanRedo();
        void OnRedo();
        bool CanSelectAll();
        void OnSelectAll();
        bool CanCut();
        void OnCut();
        bool CanCopy();
        void OnCopy();
        bool CanPaste();
        void OnPaste();
        bool CanDelete();
        void OnDelete();
    }
}

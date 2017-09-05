using System;

namespace CustomControls
{
    public interface IDocumentEditor
    {
        event EventHandler<EventArgs> CaretPositionChanged;
        int CaretLine { get; }
        int CaretColumn { get; }
        bool IsCaretOverride { get; }
    }
}

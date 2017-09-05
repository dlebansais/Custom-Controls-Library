namespace CustomControls
{
    public interface ITreeNodeProperties
    {
        ITreeNodePath Path { get; }
        bool IsDirty { get; }
        void ClearIsDirty();
        void UpdateString(string fieldName, string newText);
        void UpdateEnum(string fieldName, int newValue);
        string FriendlyPropertyName(string name);
    }
}

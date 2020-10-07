namespace CustomControls
{
    /// <summary>
    /// Represents the properties of a tree node.
    /// </summary>
    public interface ITreeNodeProperties
    {
        /// <summary>
        /// Gets the path to the tree node.
        /// </summary>
        ITreeNodePath Path { get; }

        /// <summary>
        /// Gets a value indicating whether the node is modified.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag.
        /// </summary>
        void ClearIsDirty();

        /// <summary>
        /// Updates a field name with a new value.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <param name="newText">The new value.</param>
        void UpdateString(string fieldName, string newText);

        /// <summary>
        /// Enumerates names of an enum.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <param name="newValue">The enum value.</param>
        void UpdateEnum(string fieldName, int newValue);

        /// <summary>
        /// Gets the friendly name of a property.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The friendly name of the property.</returns>
        string FriendlyPropertyName(string name);
    }
}

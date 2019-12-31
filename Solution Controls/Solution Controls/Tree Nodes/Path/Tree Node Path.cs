namespace CustomControls
{
    /// <summary>
    /// Represents the path to a node.
    /// </summary>
    public interface ITreeNodePath
    {
        /// <summary>
        /// Gets the friendly name.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Checks if two paths are equal.
        /// </summary>
        /// <param name="other">The other path.</param>
        /// <returns>True if they are equal; otherwise, false.</returns>
        bool IsEqual(ITreeNodePath other);

        /// <summary>
        /// Changes the path friendly name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        void ChangeFriendlyName(string newName);
    }
}

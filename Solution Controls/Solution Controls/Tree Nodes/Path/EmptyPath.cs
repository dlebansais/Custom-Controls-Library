namespace CustomControls
{
    /// <summary>
    /// Represents an empty path.
    /// </summary>
    public class EmptyPath : IRootPath
    {
        /// <summary>
        /// Gets the friendly name.
        /// </summary>
        public string FriendlyName { get; } = string.Empty;

        /// <summary>
        /// Checks if two paths are the same.
        /// </summary>
        /// <param name="other">The other path.</param>
        /// <returns>True if they are the same; otherwise, false.</returns>
        public bool IsEqual(ITreeNodePath other)
        {
            return false;
        }

        /// <summary>
        /// Changes the friendly name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        public void ChangeFriendlyName(string newName)
        {
        }
    }
}

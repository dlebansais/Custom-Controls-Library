namespace CustomControls
{
    /// <summary>
    /// Represents the properties of an item.
    /// </summary>
    public interface IItemProperties : ITreeNodeProperties
    {
        /// <summary>
        /// Gets a value indicating whether the item is special.
        /// </summary>
        bool IsSpecial { get; }
    }
}

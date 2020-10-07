namespace CustomControls
{
    /// <summary>
    /// Represents the source of a gesture.
    /// </summary>
    public interface IGestureSource
    {
        /// <summary>
        /// Gets the translator for gestures.
        /// </summary>
        IGestureTranslator GestureTranslator { get; }
    }
}

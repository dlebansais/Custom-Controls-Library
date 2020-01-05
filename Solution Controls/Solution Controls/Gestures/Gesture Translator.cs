namespace CustomControls
{
    /// <summary>
    /// Represents a translator for gesture text.
    /// </summary>
    public interface IGestureTranslator
    {
        /// <summary>
        /// Translates a gesture text.
        /// </summary>
        /// <param name="rawGestureText">The text to translate.</param>
        /// <returns>The translated text.</returns>
        string PostTranslate(string rawGestureText);
    }

    /// <summary>
    /// Represents a translator for gesture text.
    /// </summary>
    public class GestureTranslator : IGestureTranslator
    {
        /// <summary>
        /// Translates a gesture text.
        /// </summary>
        /// <param name="rawGestureText">The text to translate.</param>
        /// <returns>The translated text.</returns>
        public virtual string PostTranslate(string rawGestureText)
        {
            if (rawGestureText == "OemPlus")
                return "+";
            else if (rawGestureText == "OemMinus")
                return "-";
            else
                return rawGestureText;
        }
    }
}

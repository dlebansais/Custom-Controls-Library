namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a property entry for strings.
    /// </summary>
    public interface IStringPropertyEntry : IPropertyEntry
    {
        /// <summary>
        /// Gets or sets the property string.
        /// </summary>
        string Text { get; set; }
    }

    /// <summary>
    /// Represents a property entry for strings.
    /// </summary>
    public class StringPropertyEntry : PropertyEntry, IStringPropertyEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringPropertyEntry"/> class.
        /// </summary>
        /// <param name="sourcePropertiesList">The list of properties.</param>
        /// <param name="name">The property name.</param>
        /// <param name="friendlyName">The property friendly name.</param>
        /// <param name="text">The property string.</param>
        public StringPropertyEntry(IReadOnlyList<ITreeNodeProperties> sourcePropertiesList, string name, string friendlyName, string text)
            : base(sourcePropertiesList, name, friendlyName)
        {
            Text = text;
        }

        /// <summary>
        /// Gets or sets the property string.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Changes the property string.
        /// </summary>
        /// <param name="newText">The new string.</param>
        public virtual void UpdateText(string newText)
        {
            foreach (ITreeNodeProperties Properties in SourcePropertiesList)
                Properties.UpdateString(Name, newText);
        }
    }
}

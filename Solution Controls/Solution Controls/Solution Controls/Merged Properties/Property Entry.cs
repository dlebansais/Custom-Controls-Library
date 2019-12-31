namespace CustomControls
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a property entry.
    /// </summary>
    public interface IPropertyEntry
    {
        /// <summary>
        /// Gets the list of properties to which this entry belongs.
        /// </summary>
        IReadOnlyList<ITreeNodeProperties> SourcePropertiesList { get; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// Represents a property entry.
    /// </summary>
    public class PropertyEntry : IPropertyEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEntry"/> class.
        /// </summary>
        /// <param name="sourcePropertiesList">The list of properties to which this entry belongs.</param>
        /// <param name="name">The property name.</param>
        /// <param name="friendlyName">The property friendly name.</param>
        public PropertyEntry(IReadOnlyList<ITreeNodeProperties> sourcePropertiesList, string name, string friendlyName)
        {
            SourcePropertiesList = sourcePropertiesList;
            Name = name;
            FriendlyName = friendlyName;
        }

        /// <summary>
        /// Gets the list of properties to which this entry belongs.
        /// </summary>
        public IReadOnlyList<ITreeNodeProperties> SourcePropertiesList { get; private set; }

        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the property friendly name.
        /// </summary>
        public string FriendlyName { get; private set; }
    }
}

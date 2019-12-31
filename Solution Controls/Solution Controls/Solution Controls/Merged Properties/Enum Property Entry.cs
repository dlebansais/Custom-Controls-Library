namespace CustomControls
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a property entry for enums.
    /// </summary>
    public interface IEnumPropertyEntry : IPropertyEntry
    {
        /// <summary>
        /// Gets the list of enum names.
        /// </summary>
        IList<string> EnumNames { get; }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        int SelectedIndex { get; set; }
    }

    /// <summary>
    /// Represents a property entry for enums.
    /// </summary>
    public class EnumPropertyEntry : PropertyEntry, IEnumPropertyEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumPropertyEntry"/> class.
        /// </summary>
        /// <param name="sourcePropertiesList">The list of properties.</param>
        /// <param name="name">The property name.</param>
        /// <param name="friendlyName">The property friendly name.</param>
        /// <param name="enumNames">The list of enum names.</param>
        /// <param name="selectedIndex">The selected index.</param>
        public EnumPropertyEntry(IReadOnlyList<ITreeNodeProperties> sourcePropertiesList, string name, string friendlyName, string[] enumNames, int selectedIndex)
            : base(sourcePropertiesList, name, friendlyName)
        {
            if (enumNames == null)
                throw new ArgumentNullException(nameof(enumNames));

            this.EnumNames = new List<string>();
            foreach (string EnumName in enumNames)
                this.EnumNames.Add(EnumName);

            this.SelectedIndex = selectedIndex;
        }

        /// <summary>
        /// Gets the list of enum names.
        /// </summary>
        public IList<string> EnumNames { get; private set; }

        /// <summary>
        /// Gets or sets the selected index.
        /// </summary>
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Gets the selected enum.
        /// </summary>
        public string SelectedItem { get { return SelectedIndex >= 0 && SelectedIndex < EnumNames.Count ? EnumNames[SelectedIndex] : string.Empty; } }

        /// <summary>
        /// Changes the selected index.
        /// </summary>
        /// <param name="newSelectedIndex">The new index.</param>
        public virtual void UpdateSelectedIndex(int newSelectedIndex)
        {
            foreach (ITreeNodeProperties Properties in SourcePropertiesList)
                Properties.UpdateEnum(Name, newSelectedIndex);
        }
    }
}

namespace CustomControls
{
    /// <summary>
    /// Represents a filter for selecting documents by type.
    /// </summary>
    public class DocumentTypeFilter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTypeFilter"/> class.
        /// </summary>
        /// <param name="filterValue">The filter.</param>
        /// <param name="defaultExtension">The default extension for files of this type.</param>
        public DocumentTypeFilter(string filterValue, string defaultExtension)
        {
            FilterValue = filterValue;
            DefaultExtension = defaultExtension;
        }

        /// <summary>
        /// Gets the filter.
        /// </summary>
        public string FilterValue { get; private set; }

        /// <summary>
        /// Gets the default extension for files of this type.
        /// </summary>
        public string DefaultExtension { get; private set; }
    }
}

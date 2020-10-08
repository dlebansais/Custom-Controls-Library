namespace CustomControls
{
    /// <summary>
    /// Represents a context for the <see cref="RootPropertiesRequestedEventArgs"/> event data.
    /// </summary>
    public class RootPropertiesRequestedEventContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootPropertiesRequestedEventContext"/> class.
        /// </summary>
        /// <param name="properties">The properties of the root object.</param>
        public RootPropertiesRequestedEventContext(IRootProperties properties)
        {
            Properties = properties;
        }

        /// <summary>
        /// Gets the properties of the root object.
        /// </summary>
        public IRootProperties Properties { get; private set; }
    }
}

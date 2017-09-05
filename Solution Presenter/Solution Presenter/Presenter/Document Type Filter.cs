namespace CustomControls
{
    public class DocumentTypeFilter
    {
        public DocumentTypeFilter(string filterValue, string defaultExtension)
        {
            this.FilterValue = filterValue;
            this.DefaultExtension = defaultExtension;
        }

        public string FilterValue { get; private set; }
        public string DefaultExtension { get; private set; }
    }
}

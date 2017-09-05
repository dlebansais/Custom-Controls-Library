using CustomControls;
using System.Collections.Generic;

namespace CustomControls
{
    public interface IPropertyEntry
    {
        IReadOnlyList<ITreeNodeProperties> SourcePropertiesList { get; }
        string Name { get; }
    }

    public class PropertyEntry : IPropertyEntry
    {
        public PropertyEntry(IReadOnlyList<ITreeNodeProperties> sourcePropertiesList, string name, string friendlyName)
        {
            this.SourcePropertiesList = sourcePropertiesList;
            this.Name = name;
            this.FriendlyName = friendlyName;
        }

        public IReadOnlyList<ITreeNodeProperties> SourcePropertiesList { get; private set; }
        public string Name { get; private set; }
        public string FriendlyName { get; private set; }
    }
}

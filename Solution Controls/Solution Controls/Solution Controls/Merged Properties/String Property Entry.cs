using CustomControls;
using System.Collections.Generic;

namespace CustomControls
{
    public interface IStringPropertyEntry : IPropertyEntry
    {
        string Text { get; set; }
    }

    public class StringPropertyEntry : PropertyEntry, IStringPropertyEntry
    {
        public StringPropertyEntry(IReadOnlyList<ITreeNodeProperties> sourcePropertiesList, string name, string friendlyName, string text)
            : base(sourcePropertiesList, name, friendlyName)
        {
            this.Text = text;
        }

        public string Text { get; set; }

        public virtual void UpdateText(string newText)
        {
            foreach (ITreeNodeProperties Properties in SourcePropertiesList)
                Properties.UpdateString(Name, newText);
        }
    }
}

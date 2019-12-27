﻿using System;
using System.Collections.Generic;

namespace CustomControls
{
    public interface IEnumPropertyEntry : IPropertyEntry
    {
        IList<string> EnumNames { get; }
        int SelectedIndex { get; set; }
    }

    public class EnumPropertyEntry : PropertyEntry, IEnumPropertyEntry
    {
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

        public IList<string> EnumNames { get; private set; }
        public int SelectedIndex { get; set; }
        public string SelectedItem { get { return SelectedIndex >= 0 && SelectedIndex < EnumNames.Count ? EnumNames[SelectedIndex] : string.Empty; } }

        public virtual void UpdateSelectedIndex(int newSelectedIndex)
        {
            foreach (ITreeNodeProperties Properties in SourcePropertiesList)
                Properties.UpdateEnum(Name, newSelectedIndex);
        }
    }
}

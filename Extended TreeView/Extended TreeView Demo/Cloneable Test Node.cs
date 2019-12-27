using CustomControls;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExtendedTreeViewDemo
{
    public class CloneableTestNode : IExtendedTreeNode, ICloneable
    {
        public CloneableTestNode(CloneableTestNode? parent, int index)
        {
            Parent = parent;
            Index = index;

            Children = new CloneableTestNodeCollection(this);
        }

        public IExtendedTreeNode? Parent { get; private set; }
        public int Index { get; private set; }
        public IExtendedTreeNodeCollection Children { get; private set; }
        public string Text { get { return "CloneableTestNode #" + Index.ToString(CultureInfo.InvariantCulture); } }

        public void ChangeParent(IExtendedTreeNode newParent)
        {
            Parent = newParent;
        }

        public object Clone()
        {
            CloneableTestNode Clone = new CloneableTestNode((CloneableTestNode?)Parent, Index);
            foreach (CloneableTestNode Child in Children)
                Clone.Children.Add(Child.Clone());
            
            return Clone;
        }

        public override string ToString()
        {
            return base.ToString() + " #" + Index.ToString(CultureInfo.InvariantCulture);
        }
    }
}

namespace ExtendedTreeViewDemo
{
    using System.Globalization;
    using CustomControls;

    internal class TestNode : IExtendedTreeNode
    {
        public TestNode(TestNode? parent, int index)
        {
            Parent = parent;
            Index = index;

            Children = new TestNodeCollection(this);
        }

        public IExtendedTreeNode? Parent { get; private set; }
        public int Index { get; private set; }
        public IExtendedTreeNodeCollection Children { get; private set; }
        public string Text { get { return "TestNode #" + Index.ToString(CultureInfo.InvariantCulture); } }

        public void ChangeParent(IExtendedTreeNode newParent)
        {
            Parent = newParent;
        }

        public override string ToString()
        {
            return base.ToString() + " #" + Index.ToString(CultureInfo.InvariantCulture);
        }
    }
}

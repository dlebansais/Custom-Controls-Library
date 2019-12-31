namespace CustomControls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Represents a node in a solution.
    /// </summary>
    public interface ISolutionTreeNode : IExtendedTreeNode, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the node path.
        /// </summary>
        ITreeNodePath Path { get; }

        /// <summary>
        /// Gets the node properties.
        /// </summary>
        ITreeNodeProperties Properties { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the node is modified.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag.
        /// </summary>
        void ClearIsDirty();

        /// <summary>
        /// Changes the node name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        void ChangeName(string newName);
    }

    /// <summary>
    /// Represents a solution node.
    /// </summary>
    public abstract class SolutionTreeNode : ISolutionTreeNode
    {
        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionTreeNode"/> class.
        /// </summary>
        /// <param name="parent">The parent folder.</param>
        /// <param name="path">The node path.</param>
        /// <param name="properties">The node properties.</param>
        protected SolutionTreeNode(ISolutionFolder? parent, ITreeNodePath path, ITreeNodeProperties properties)
        {
            Parent = parent;
            Path = path;
            Properties = properties;

            UpdateName();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the node path.
        /// </summary>
        public ITreeNodePath Path { get; private set; }

        /// <summary>
        /// Gets the node parent.
        /// </summary>
        public IExtendedTreeNode? Parent { get; private set; }

        /// <summary>
        /// Gets the node properties.
        /// </summary>
        public ITreeNodeProperties Properties { get; private set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the item children.
        /// </summary>
        public abstract IExtendedTreeNodeCollection Children { get; }
        #endregion

        #region Client Interface
        /// <summary>
        /// Changes the node parent.
        /// </summary>
        /// <param name="newParent">The new parent.</param>
        public void ChangeParent(IExtendedTreeNode newParent)
        {
            Parent = newParent;
        }

        /// <summary>
        /// Changes the node name.
        /// </summary>
        /// <param name="newName">The new name.</param>
        public void ChangeName(string newName)
        {
            Path.ChangeFriendlyName(newName);
            UpdateName();

            IsDirty = true;
        }

        private void UpdateName()
        {
            Name = Path.FriendlyName;
            NotifyPropertyChanged(nameof(Name));
        }
        #endregion

        #region Dirty Flag
        /// <summary>
        /// Gets or sets a value indicating whether the node is modified.
        /// </summary>
        public bool IsDirty
        {
            get { return IsDirtyInternal; }
            set
            {
                if (IsDirtyInternal != value)
                {
                    IsDirtyInternal = value;
                    NotifyThisPropertyChanged();
                    NotifyIsDirtyChanged();
                }
            }
        }
        private bool IsDirtyInternal;

        /// <summary>
        /// Clears the <see cref="IsDirty"/> flag.
        /// </summary>
        public virtual void ClearIsDirty()
        {
            IsDirty = false;
        }

        /// <summary>
        /// Occurs when the <see cref="IsDirty"/> flag has changed.
        /// </summary>
        public event EventHandler<EventArgs>? IsDirtyChanged;

        /// <summary>
        /// Invokes handlers of the <see cref="IsDirtyChanged"/> event.
        /// </summary>
        protected virtual void NotifyIsDirtyChanged()
        {
            IsDirtyChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        /// Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invoke handlers of the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Invoke handlers of the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace CustomControls
{
    public interface ISolutionTreeNode : IExtendedTreeNode, INotifyPropertyChanged
    {
        ITreeNodePath Path { get; }
        ITreeNodeProperties Properties { get; }
        string Name { get; set; }
        bool IsDirty { get; }
        void ClearIsDirty();
        void ChangeName(string newName);
    }

    public abstract class SolutionTreeNode : ISolutionTreeNode
    {
        #region Init
        protected SolutionTreeNode(ISolutionFolder parent, ITreeNodePath path, ITreeNodeProperties properties)
        {
            this.Parent = parent;
            this.Path = path;
            this.Properties = properties;

            UpdateName();
            InitIsDirty();
        }
        #endregion

        #region Properties
        public ITreeNodePath Path { get; private set; }
        public IExtendedTreeNode Parent { get; private set; }
        public ITreeNodeProperties Properties { get; private set; }
        public string Name { get; set; }
        public abstract IExtendedTreeNodeCollection Children { get; }
        #endregion

        #region Client Interface
        public void ChangeParent(IExtendedTreeNode newParent)
        {
            Parent = newParent;
        }

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
        private void InitIsDirty()
        {
            _IsDirty = false;
        }

        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                if (_IsDirty != value)
                {
                    _IsDirty = value;
                    NotifyThisPropertyChanged();
                    NotifyIsDirtyChanged();
                }
            }
        }
        private bool _IsDirty;

        public virtual void ClearIsDirty()
        {
            IsDirty = false;
        }

        public event EventHandler<EventArgs> IsDirtyChanged;

        protected virtual void NotifyIsDirtyChanged()
        {
            if (IsDirtyChanged != null)
                IsDirtyChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

namespace CustomControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using UndoRedo;

    /// <summary>
    /// Represents a drop down control for undo and redo operations.
    /// </summary>
    public partial class DropDownList : Popup, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropDownList"/> class.
        /// </summary>
        public DropDownList()
        {
            InitializeComponent();
            DataContext = this;
        }

        /// <summary>
        /// Gets the list of operations.
        /// </summary>
        public ObservableCollection<IReversibleOperation> AssociatedList
        {
            get { return AssociatedListInternal; }
            private set
            {
                if (AssociatedListInternal != value)
                {
                    AssociatedListInternal = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private ObservableCollection<IReversibleOperation> AssociatedListInternal = new ObservableCollection<IReversibleOperation>();

        /// <summary>
        /// Gets the number of selected operations.
        /// </summary>
        public int SelectedCount
        {
            get
            {
                int Count = 0;
                for (int i = 0; i < listOperations.Items.Count; i++, Count++)
                {
                    ListBoxItem? Ctrl = listOperations.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (Ctrl == null || !Ctrl.IsSelected)
                        break;
                }

                return Count;
            }
        }

        /// <summary>
        /// Set the list of operations.
        /// </summary>
        /// <param name="associatedList">The list of operations.</param>
        public void SetAssociatedList(ObservableCollection<IReversibleOperation>? associatedList)
        {
            if (associatedList != null)
                this.AssociatedList = associatedList;
            else
                this.AssociatedList = new ObservableCollection<IReversibleOperation>();
        }

        /// <summary>
        /// Selects operations in the list.
        /// </summary>
        /// <param name="lastSelectedScreenCoordinates">The screen coordinates of the last selected operation.</param>
        public void SelectUpTo(Point lastSelectedScreenCoordinates)
        {
            bool SetSelected = true;

            for (int i = 0; i < listOperations.Items.Count; i++)
            {
                if (listOperations.ItemContainerGenerator.ContainerFromIndex(i) is ListBoxItem Ctrl)
                {
                    Point CtrlTopLeft = Ctrl.PointToScreen(new Point(0, 0));
                    if (CtrlTopLeft.Y >= lastSelectedScreenCoordinates.Y)
                        SetSelected = false;

                    Ctrl.IsSelected = SetSelected;
                }
            }
        }

        /// <summary>
        /// Checks whether a coordinate is within the drop down box.
        /// </summary>
        /// <param name="lastClickScreenCoordinates">The coordinates.</param>
        /// <returns>True if within the drop down box; otherwise, false.</returns>
        public bool IsWithin(Point lastClickScreenCoordinates)
        {
            Point TopLeft = listOperations.PointToScreen(new Point(0, 0));
            Point BottomRight = listOperations.PointToScreen(new Point(listOperations.ActualWidth, listOperations.ActualHeight));
            Rect rc = new Rect(TopLeft, BottomRight);

            return rc.Contains(lastClickScreenCoordinates);
        }

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

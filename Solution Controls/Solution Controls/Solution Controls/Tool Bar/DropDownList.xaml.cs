using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using UndoRedo;

namespace CustomControls
{
    public partial class DropDownList : Popup, INotifyPropertyChanged
    {
        public DropDownList()
        {
            InitializeComponent();
            DataContext = this;

            SetAssociatedList(null);
        }

        public ObservableCollection<IReversibleOperation> AssociatedList 
        {
            get { return _AssociatedList; }
            private set
            {
                if (_AssociatedList != value)
                {
                    _AssociatedList = value;
                    NotifyThisPropertyChanged();
                }
            }
        }
        private ObservableCollection<IReversibleOperation> _AssociatedList;

        public int SelectedCount
        {
            get
            {
                int Count = 0;
                for (int i = 0; i < listOperations.Items.Count; i++, Count++)
                {
                    ListBoxItem Ctrl = listOperations.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (Ctrl == null || !Ctrl.IsSelected)
                        break;
                }

                return Count;
            }
        }

        public void SetAssociatedList(ObservableCollection<IReversibleOperation> associatedList)
        {
            if (associatedList != null)
                this.AssociatedList = associatedList;
            else
                this.AssociatedList = new ObservableCollection<IReversibleOperation>();
        }

        public void SelectUpTo(Point lastSelectedScreenCoordinates)
        {
            bool SetSelected = true;

            for (int i = 0; i < listOperations.Items.Count; i++)
            {
                ListBoxItem Ctrl = listOperations.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (Ctrl != null)
                {
                    Point CtrlTopLeft = Ctrl.PointToScreen(new Point(0, 0));
                    if (CtrlTopLeft.Y >= lastSelectedScreenCoordinates.Y)
                        SetSelected = false;

                    Ctrl.IsSelected = SetSelected;
                }
            }
        }

        public bool IsWithin(Point lastClickScreenCoordinates)
        {
            Point TopLeft = listOperations.PointToScreen(new Point(0, 0));
            Point BottomRight = listOperations.PointToScreen(new Point(listOperations.ActualWidth, listOperations.ActualHeight));
            Rect rc = new Rect(TopLeft, BottomRight);

            return rc.Contains(lastClickScreenCoordinates);
        }

        #region Implementation of INotifyPropertyChanged
        /// <summary>
        ///     Implements the PropertyChanged event.
        /// </summary>
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Default parameter is mandatory with [CallerMemberName]")]
        internal void NotifyThisPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

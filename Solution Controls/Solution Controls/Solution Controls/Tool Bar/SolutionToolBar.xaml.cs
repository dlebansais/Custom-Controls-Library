using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using UndoRedo;
using Verification;

namespace CustomControls
{
    public partial class SolutionToolBar : UserControl
    {
        #region Custom properties and events
        #region Application Name
        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string), typeof(SolutionToolBar), new PropertyMetadata(null));

        public string ApplicationName
        {
            get { return (string)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }
        #endregion
        #region Undo Redo Manager
        public static readonly DependencyProperty UndoRedoManagerProperty = DependencyProperty.Register("UndoRedoManager", typeof(UndoRedoManager), typeof(SolutionToolBar), new PropertyMetadata(null));

        public UndoRedoManager UndoRedoManager
        {
            get { return (UndoRedoManager)GetValue(UndoRedoManagerProperty); }
            set { SetValue(UndoRedoManagerProperty, value); }
        }
        #endregion
        #region Open Documents
        public static readonly DependencyProperty OpenDocumentsProperty = DependencyProperty.Register("OpenDocuments", typeof(ICollection<IDocument>), typeof(SolutionToolBar), new PropertyMetadata(null));

        public IReadOnlyCollection<IDocument> OpenDocuments
        {
            get { return (IReadOnlyCollection<IDocument>)GetValue(OpenDocumentsProperty); }
            set { SetValue(OpenDocumentsProperty, value); }
        }
        #endregion
        #region Active Document
        public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(SolutionToolBar), new PropertyMetadata(null));

        public IDocument ActiveDocument
        {
            get { return (IDocument)GetValue(ActiveDocumentProperty); }
            set { SetValue(ActiveDocumentProperty, value); }
        }
        #endregion
        #region Main Menu Loaded
        public static readonly RoutedEvent MainMenuLoadedEvent = EventManager.RegisterRoutedEvent("MainMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionToolBar));

        public event RoutedEventHandler MainMenuLoaded
        {
            add { AddHandler(MainMenuLoadedEvent, value); }
            remove { RemoveHandler(MainMenuLoadedEvent, value); }
        }

        protected virtual void NotifyMainMenuLoaded(RoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            RaiseEvent(new RoutedEventArgs(MainMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Main ToolBar Loaded
        public static readonly RoutedEvent MainToolBarLoadedEvent = EventManager.RegisterRoutedEvent("MainToolBarLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionToolBar));

        public event RoutedEventHandler MainToolBarLoaded
        {
            add { AddHandler(MainToolBarLoadedEvent, value); }
            remove { RemoveHandler(MainToolBarLoadedEvent, value); }
        }

        protected virtual void NotifyMainToolBarLoaded(RoutedEventArgs e)
        {
            Assert.ValidateReference(e);

            RaiseEvent(new RoutedEventArgs(MainToolBarLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Document Activated
        public static readonly RoutedEvent DocumentActivatedEvent = EventManager.RegisterRoutedEvent("DocumentActivated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionToolBar));

        public event RoutedEventHandler DocumentActivated
        {
            add { AddHandler(DocumentActivatedEvent, value); }
            remove { RemoveHandler(DocumentActivatedEvent, value); }
        }

        protected virtual void NotifyDocumentActivated(IDocument activeDocument)
        {
            DocumentActivatedEventArgs Args = new DocumentActivatedEventArgs(DocumentActivatedEvent, activeDocument);
            RaiseEvent(Args);
        }
        #endregion
        #endregion

        #region Init
        public SolutionToolBar()
        {
            InitializeComponent();
            InitDropDownLists();
        }
        #endregion

        #region Properties
        public Separator DocumentMenuSeparator
        {
            get { return separatorMenuFirstNewDocument; }
        }

        public Separator DocumentToolBarSeparator
        {
            get { return separatorToolBarFirstNewDocument; }
        }

        public Separator FileCustomMenuSeparator
        {
            get { return separatorFileCustomMenuItem; }
        }

        public Separator EditCustomMenuSeparator
        {
            get { return separatorEditCustomMenuItem; }
        }

        public Separator FileToolBarSeparator
        {
            get { return separatorFileCustomButton; }
        }

        public Separator EditToolBarSeparator
        {
            get { return separatorEditCustomButton; }
        }

        public Menu MainMenu
        {
            get { return menuMain; }
        }
        #endregion

        #region Client Interface
        public virtual void Reset()
        {
            foreach (ExtendedToolBar ToolBar in toolbarMainTray.ToolBars)
                ToolBar.Reset();
        }

        public virtual string SerializeActiveButtons()
        {
            string XamlData = "";
            foreach (ExtendedToolBar ToolBar in toolbarMainTray.ToolBars)
            {
                if (XamlData.Length > 0)
                    XamlData += SerializedDataSeparator;

                XamlData += ToolBar.SerializeActiveButtons();
            }

            return XamlData;
        }

        public virtual void DeserializeActiveButtons(string xamlData)
        {
            Assert.ValidateReference(xamlData);

            string[] Splitted = xamlData.Split(SerializedDataSeparator);
            for (int i = 0; i < toolbarMainTray.ToolBars.Count && i < Splitted.Length; i++)
                if (Splitted[i].Length > 0)
                {
                    ExtendedToolBar ToolBar = (ExtendedToolBar)toolbarMainTray.ToolBars[i];
                    ToolBar.DeserializeActiveButtons(Splitted[i]);
                }
        }

        protected virtual char SerializedDataSeparator
        {
            get { return ','; }
        }
        #endregion

        #region Drop Down Lists
        private DropDownList DropDownPopup;

        private void InitDropDownLists()
        {
            UndoDropDown = new DropDownList();
            UndoDropDown.Placement = PlacementMode.Bottom;
            RedoDropDown = new DropDownList();
            RedoDropDown.Placement = PlacementMode.Bottom;
            DropDownPopup = null;
        }

        private void OnDropDownUndo(object sender, ExecutedRoutedEventArgs e)
        {
            UndoDropDown.SetAssociatedList(UndoRedoManager != null ? UndoRedoManager.UndoList : null);
            DropDownPopup = UndoDropDown;
            OnDropDown(e.OriginalSource as FrameworkElement);
        }

        private void OnDropDownRedo(object sender, ExecutedRoutedEventArgs e)
        {
            RedoDropDown.SetAssociatedList(UndoRedoManager != null ? UndoRedoManager.RedoList : null);
            DropDownPopup = RedoDropDown;
            OnDropDown(e.OriginalSource as FrameworkElement);
        }

        private void OnDropDown(FrameworkElement OriginalSource)
        {
            FrameworkElement PlacementTarget = OriginalSource;
            while (PlacementTarget != null)
            {
                if (PlacementTarget is ExtendedToolBarButton)
                    break;

                PlacementTarget = PlacementTarget.Parent as FrameworkElement;
            }

            DropDownPopup.PlacementTarget = PlacementTarget;

            StartCapture();
        }

        private void StartCapture()
        {
            DropDownPopup.IsOpen = true;

            MouseMove += OnMouseMove;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            Mouse.Capture(this);
        }

        private void StopCapture()
        {
            Mouse.Capture(null);
            MouseMove -= OnMouseMove;
            MouseDown -= OnMouseDown;
            MouseUp -= OnMouseUp;

            DropDownPopup.IsOpen = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            Point pt = PointToScreen(e.GetPosition(this));
            DropDownPopup.SelectUpTo(pt);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point pt = PointToScreen(e.GetPosition(this));
            if (!DropDownPopup.IsWithin(pt))
                StopCapture();
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pt = PointToScreen(e.GetPosition(this));
            bool IsWithin = DropDownPopup.IsWithin(pt);

            StopCapture();

            if (IsWithin)
            {
                int SelectedCount = DropDownPopup.SelectedCount;

                for (int i = 0; i < SelectedCount; i++)
                    if (DropDownPopup == UndoDropDown)
                        UndoRedoManager.Undo();
                    else if (DropDownPopup == RedoDropDown)
                        UndoRedoManager.Redo();
            }
        }

        public DropDownList UndoDropDown { get; private set; }
        public DropDownList RedoDropDown { get; private set; }
        #endregion

        #region Custom Menus
        public virtual void OnMainMenuLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainMenuLoaded(e);
        }

        protected virtual void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            PrettyItemsControl.MakeMenuPretty((ItemsControl)sender);
        }

        private void OnMainToolBarLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainToolBarLoaded(e);
            PrettyItemsControl.MakeToolBarTrayPretty((ToolBarTray)sender);
        }

        protected virtual void OnWindowMenuOpening(object sender, RoutedEventArgs e)
        {
            MenuItem WindowMenu;
            if ((WindowMenu = sender as MenuItem) != null)
            {
                MenuItem ListWindowMenu = WindowMenu.FindName("ListWindowsMenu") as MenuItem;
                if (ListWindowMenu != null)
                {
                    int WindowListIndex = WindowMenu.Items.IndexOf(ListWindowMenu);
                    if (WindowListIndex > 0)
                    {
                        int FirstWindow = WindowListIndex - 1;
                        while (FirstWindow > 0 && !(WindowMenu.Items[FirstWindow] is Separator))
                            FirstWindow--;

                        FirstWindow++;
                        for (int i = 0; i < WindowListIndex - FirstWindow; i++)
                            WindowMenu.Items.RemoveAt(FirstWindow);

                        if (OpenDocuments != null)
                        {
                            int i = 0;
                            foreach (IDocument Document in OpenDocuments)
                            {
                                MenuItem NewWindowItem = new MenuItem();
                                NewWindowItem.Header = (i + 1).ToString(CultureInfo.CurrentCulture) + " " + Document.Path.HeaderName + (Document.IsDirty ? "*" : "");
                                NewWindowItem.DataContext = Document;
                                NewWindowItem.Click += OnSelectWindowMenuClicked;

                                if (Document == ActiveDocument)
                                    NewWindowItem.IsChecked = true;

                                WindowMenu.Items.Insert(FirstWindow + i, NewWindowItem);

                                if (++i >= 10)
                                    break;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnSelectWindowMenuClicked(object sender, RoutedEventArgs e)
        {
            MenuItem AsMenuItem;
            if ((AsMenuItem = sender as MenuItem) != null)
            {
                IDocument ActiveDocument;
                if ((ActiveDocument = AsMenuItem.DataContext as IDocument) != null)
                    NotifyDocumentActivated(ActiveDocument);
            }
        }
        #endregion

        #region Buttons
        protected virtual void OnIsActiveChanged(object sender, RoutedEventArgs e)
        {
            FrameworkElement Element = sender as FrameworkElement;
            while (Element != null)
            {
                ToolBarTray AsToolBarTray;
                if ((AsToolBarTray = Element as ToolBarTray) != null)
                {
                    PrettyItemsControl.MakeToolBarTrayPretty(AsToolBarTray);
                    break;
                }

                Element = Element.Parent as FrameworkElement;
            }
        }
        #endregion
    }
}

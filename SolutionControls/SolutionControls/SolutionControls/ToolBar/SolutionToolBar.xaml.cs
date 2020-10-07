namespace CustomControls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using UndoRedo;

    /// <summary>
    /// Represents a toolbar control in a solution.
    /// </summary>
    public partial class SolutionToolBar : UserControl
    {
        #region Custom properties and events
        #region Application Name
        /// <summary>
        /// Identifies the <see cref="ApplicationName"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ApplicationNameProperty = DependencyProperty.Register("ApplicationName", typeof(string), typeof(SolutionToolBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        public string ApplicationName
        {
            get { return (string)GetValue(ApplicationNameProperty); }
            set { SetValue(ApplicationNameProperty, value); }
        }
        #endregion
        #region Undo Redo Manager
        /// <summary>
        /// Identifies the <see cref="UndoRedoManager"/> attached property.
        /// </summary>
        public static readonly DependencyProperty UndoRedoManagerProperty = DependencyProperty.Register("UndoRedoManager", typeof(UndoRedoManager), typeof(SolutionToolBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the undo redo manager.
        /// </summary>
        public UndoRedoManager UndoRedoManager
        {
            get { return (UndoRedoManager)GetValue(UndoRedoManagerProperty); }
            set { SetValue(UndoRedoManagerProperty, value); }
        }
        #endregion
        #region Open Documents
        /// <summary>
        /// Identifies the <see cref="OpenDocuments"/> attached property.
        /// </summary>
        public static readonly DependencyProperty OpenDocumentsProperty = DependencyProperty.Register("OpenDocuments", typeof(ICollection<IDocument>), typeof(SolutionToolBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the list of open documents.
        /// </summary>
        public IReadOnlyCollection<IDocument> OpenDocuments
        {
            get { return (IReadOnlyCollection<IDocument>)GetValue(OpenDocumentsProperty); }
            set { SetValue(OpenDocumentsProperty, value); }
        }
        #endregion
        #region Active Document
        /// <summary>
        /// Identifies the <see cref="ActiveDocument"/> attached property.
        /// </summary>
        public static readonly DependencyProperty ActiveDocumentProperty = DependencyProperty.Register("ActiveDocument", typeof(IDocument), typeof(SolutionToolBar), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the active document.
        /// </summary>
        public IDocument ActiveDocument
        {
            get { return (IDocument)GetValue(ActiveDocumentProperty); }
            set { SetValue(ActiveDocumentProperty, value); }
        }
        #endregion
        #region Main Menu Loaded
        /// <summary>
        /// Identifies the <see cref="MainMenuLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MainMenuLoadedEvent = EventManager.RegisterRoutedEvent("MainMenuLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionToolBar));

        /// <summary>
        /// Occurs when the main menu is loaded.
        /// </summary>
        public event RoutedEventHandler MainMenuLoaded
        {
            add { AddHandler(MainMenuLoadedEvent, value); }
            remove { RemoveHandler(MainMenuLoadedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="MainMenuLoaded"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void NotifyMainMenuLoaded(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            RaiseEvent(new RoutedEventArgs(MainMenuLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Main ToolBar Loaded
        /// <summary>
        /// Identifies the <see cref="MainToolBarLoaded"/> routed event.
        /// </summary>
        public static readonly RoutedEvent MainToolBarLoadedEvent = EventManager.RegisterRoutedEvent("MainToolBarLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionToolBar));

        /// <summary>
        /// Occurs when the main toolbar is loaded.
        /// </summary>
        public event RoutedEventHandler MainToolBarLoaded
        {
            add { AddHandler(MainToolBarLoadedEvent, value); }
            remove { RemoveHandler(MainToolBarLoadedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="MainToolBarLoaded"/> event.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected virtual void NotifyMainToolBarLoaded(RoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            RaiseEvent(new RoutedEventArgs(MainToolBarLoadedEvent, e.OriginalSource));
        }
        #endregion
        #region Document Activated
        /// <summary>
        /// Identifies the <see cref="DocumentActivated"/> routed event.
        /// </summary>
        public static readonly RoutedEvent DocumentActivatedEvent = EventManager.RegisterRoutedEvent("DocumentActivated", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SolutionToolBar));

        /// <summary>
        /// Occurs when a document is activated.
        /// </summary>
        public event RoutedEventHandler DocumentActivated
        {
            add { AddHandler(DocumentActivatedEvent, value); }
            remove { RemoveHandler(DocumentActivatedEvent, value); }
        }

        /// <summary>
        /// Invokes handlers of the <see cref="MainToolBarLoaded"/> event.
        /// </summary>
        /// <param name="activeDocument">The activated document.</param>
        protected virtual void NotifyDocumentActivated(IDocument activeDocument)
        {
            DocumentActivatedEventArgs Args = new DocumentActivatedEventArgs(DocumentActivatedEvent, activeDocument);
            RaiseEvent(Args);
        }
        #endregion
        #endregion

        #region Init
        /// <summary>
        /// Initializes a new instance of the <see cref="SolutionToolBar"/> class.
        /// </summary>
        public SolutionToolBar()
        {
            InitializeComponent();
            InitDropDownLists();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the separator for the document menu.
        /// </summary>
        public Separator DocumentMenuSeparator
        {
            get { return separatorMenuFirstNewDocument; }
        }

        /// <summary>
        /// Gets the separator for the document toolbar.
        /// </summary>
        public Separator DocumentToolBarSeparator
        {
            get { return separatorToolBarFirstNewDocument; }
        }

        /// <summary>
        /// Gets the separator for the file menu.
        /// </summary>
        public Separator FileCustomMenuSeparator
        {
            get { return separatorFileCustomMenuItem; }
        }

        /// <summary>
        /// Gets the separator for the file toolbar.
        /// </summary>
        public Separator FileToolBarSeparator
        {
            get { return separatorFileCustomButton; }
        }

        /// <summary>
        /// Gets the separator for the edit menu.
        /// </summary>
        public Separator EditCustomMenuSeparator
        {
            get { return separatorEditCustomMenuItem; }
        }

        /// <summary>
        /// Gets the separator for the edit toolbar.
        /// </summary>
        public Separator EditToolBarSeparator
        {
            get { return separatorEditCustomButton; }
        }

        /// <summary>
        /// Gets the main menu.
        /// </summary>
        public Menu MainMenu
        {
            get { return menuMain; }
        }
        #endregion

        #region Client Interface
        /// <summary>
        /// Resets the boolbar.
        /// </summary>
        public virtual void Reset()
        {
            foreach (ExtendedToolBar ToolBar in toolbarMainTray.ToolBars)
                ToolBar.Reset();
        }

        /// <summary>
        /// Gets a string representing active buttons.
        /// </summary>
        /// <returns>The serialization string.</returns>
        public virtual string SerializeActiveButtons()
        {
            string XamlData = string.Empty;
            foreach (ExtendedToolBar ToolBar in toolbarMainTray.ToolBars)
            {
                if (XamlData.Length > 0)
                    XamlData += SerializedDataSeparator;

                XamlData += ToolBar.SerializeActiveButtons();
            }

            return XamlData;
        }

        /// <summary>
        /// Updates button from a serialized string.
        /// </summary>
        /// <param name="xamlData">The serialized string.</param>
        public virtual void DeserializeActiveButtons(string xamlData)
        {
            if (xamlData == null)
                throw new ArgumentNullException(nameof(xamlData));

            string[] Splitted = xamlData.Split(SerializedDataSeparator);
            for (int i = 0; i < toolbarMainTray.ToolBars.Count && i < Splitted.Length; i++)
                if (Splitted[i].Length > 0)
                {
                    ExtendedToolBar ToolBar = (ExtendedToolBar)toolbarMainTray.ToolBars[i];
                    ToolBar.DeserializeActiveButtons(Splitted[i]);
                }
        }

        /// <summary>
        /// Gets the separator between serialized button.
        /// </summary>
        protected virtual char SerializedDataSeparator
        {
            get { return ','; }
        }
        #endregion

        #region Drop Down Lists
        private DropDownList DropDownPopup = new DropDownList();

        private void InitDropDownLists()
        {
            UndoDropDown.Placement = PlacementMode.Bottom;
            RedoDropDown.Placement = PlacementMode.Bottom;
        }

        private void OnDropDownUndo(object sender, ExecutedRoutedEventArgs e)
        {
            UndoDropDown.SetAssociatedList(UndoRedoManager?.UndoList);
            DropDownPopup = UndoDropDown;
            OnDropDown((FrameworkElement)e.OriginalSource);
        }

        private void OnDropDownRedo(object sender, ExecutedRoutedEventArgs e)
        {
            RedoDropDown.SetAssociatedList(UndoRedoManager?.RedoList);
            DropDownPopup = RedoDropDown;
            OnDropDown((FrameworkElement)e.OriginalSource);
        }

        private void OnDropDown(FrameworkElement originalSource)
        {
            Debug.Assert(DropDownPopup == UndoDropDown || DropDownPopup == RedoDropDown);

            FrameworkElement? PlacementTarget = originalSource;
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

        /// <summary>
        /// Gets the control for the list of operations to undo.
        /// </summary>
        public DropDownList UndoDropDown { get; private set; } = new DropDownList();

        /// <summary>
        /// Gets the control for the list of operations to redo.
        /// </summary>
        public DropDownList RedoDropDown { get; private set; } = new DropDownList();
        #endregion

        #region Custom Menus
        /// <summary>
        /// Called when the main menu is loaded.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        public virtual void OnMainMenuLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainMenuLoaded(e);
        }

        /// <summary>
        /// Called when a submenu is opened.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSubmenuOpened(object sender, RoutedEventArgs e)
        {
            PrettyItemsControl.MakeMenuPretty((ItemsControl)sender);
        }

        /// <summary>
        /// Called when the main toolbar is loaded.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        private void OnMainToolBarLoaded(object sender, RoutedEventArgs e)
        {
            NotifyMainToolBarLoaded(e);
            PrettyItemsControl.MakeToolBarTrayPretty((ToolBarTray)sender);
        }

        /// <summary>
        /// Called when a menu is opening.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnWindowMenuOpening(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem WindowMenu && WindowMenu.FindName("ListWindowsMenu") is MenuItem ListWindowMenu)
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
                            NewWindowItem.Header = (i + 1).ToString(CultureInfo.CurrentCulture) + " " + Document.Path.HeaderName + (Document.IsDirty ? "*" : string.Empty);
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

        /// <summary>
        /// Called when the selection window is clicked.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnSelectWindowMenuClicked(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem AsMenuItem && AsMenuItem.DataContext is IDocument ActiveDocument)
                NotifyDocumentActivated(ActiveDocument);
        }
        #endregion

        #region Buttons
        /// <summary>
        /// Called when the active button is changed.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">The event data.</param>
        protected virtual void OnIsActiveChanged(object sender, RoutedEventArgs e)
        {
            FrameworkElement? Element = sender as FrameworkElement;
            while (Element != null)
            {
                if (Element is ToolBarTray AsToolBarTray)
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

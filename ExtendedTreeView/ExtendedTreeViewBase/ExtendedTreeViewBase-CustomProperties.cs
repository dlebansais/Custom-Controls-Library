namespace CustomControls;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Contracts;

/// <summary>
/// Represents a control with a tree of nodes that can be moved around with Drag and Drop.
/// </summary>
public abstract partial class ExtendedTreeViewBase : MultiSelector
{
    #region Selection Mode
    /// <summary>
    /// Identifies the <see cref="SelectionMode"/> attached property.
    /// </summary>
    public static readonly DependencyProperty SelectionModeProperty = DependencyProperty.Register("SelectionMode", typeof(SelectionMode), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(SelectionMode.Single));

    /// <summary>
    /// Gets or sets the control selection mode.
    /// </summary>
    public SelectionMode SelectionMode
    {
        get { return (SelectionMode)GetValue(SelectionModeProperty); }
        set { SetValue(SelectionModeProperty, value); }
    }
    #endregion

    #region Is Root Always Expanded
    /// <summary>
    /// Identifies the <see cref="IsRootAlwaysExpanded"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IsRootAlwaysExpandedProperty = DependencyProperty.Register("IsRootAlwaysExpanded", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether the control root is always expanded.
    /// </summary>
    public bool IsRootAlwaysExpanded
    {
        get { return (bool)GetValue(IsRootAlwaysExpandedProperty); }
        set { SetValue(IsRootAlwaysExpandedProperty, value); }
    }
    #endregion

    #region Is Item Expanded At Start
    /// <summary>
    /// Identifies the <see cref="IsItemExpandedAtStart"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IsItemExpandedAtStartProperty = DependencyProperty.Register("IsItemExpandedAtStart", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether items should start expanded when the content changes.
    /// </summary>
    public bool IsItemExpandedAtStart
    {
        get { return (bool)GetValue(IsItemExpandedAtStartProperty); }
        set { SetValue(IsItemExpandedAtStartProperty, value); }
    }
    #endregion

    #region Allow Drag Drop
    /// <summary>
    /// Identifies the <see cref="AllowDragDrop"/> attached property.
    /// </summary>
    public static readonly DependencyProperty AllowDragDropProperty = DependencyProperty.Register("AllowDragDrop", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false, OnAllowDragDropChanged));

    /// <summary>
    /// Gets or sets a value indicating whether drag and drop is allowed.
    /// </summary>
    public bool AllowDragDrop
    {
        get { return (bool)GetValue(AllowDragDropProperty); }
        set { SetValue(AllowDragDropProperty, value); }
    }

    /// <summary>
    /// Handles changes of the <see cref="AllowDragDrop"/> property.
    /// </summary>
    /// <param name="modifiedObject">The modified object.</param>
    /// <param name="e">An object that contains event data.</param>
    protected static void OnAllowDragDropChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs e)
    {
        ExtendedTreeViewBase ctrl = Contract.AssertNotNull((ExtendedTreeViewBase)modifiedObject);
        ctrl.OnAllowDragDropChanged(e);
    }

    /// <summary>
    /// Handles changes of the <see cref="AllowDragDrop"/> property.
    /// </summary>
    /// <param name="e">An object that contains event data.</param>
    protected virtual void OnAllowDragDropChanged(DependencyPropertyChangedEventArgs e)
    {
        bool NewValue = (bool)e.NewValue;
        if (NewValue)
            AllowDrop = true;
    }
    #endregion

    #region Use Default Cursors
    /// <summary>
    /// Identifies the <see cref="UseDefaultCursors"/> attached property.
    /// </summary>
    public static readonly DependencyProperty UseDefaultCursorsProperty = DependencyProperty.Register("UseDefaultCursors", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false));

    /// <summary>
    /// Gets or sets a value indicating whether to use default cursors.
    /// </summary>
    public bool UseDefaultCursors
    {
        get { return (bool)GetValue(UseDefaultCursorsProperty); }
        set { SetValue(UseDefaultCursorsProperty, value); }
    }
    #endregion

    #region Cursor Forbidden
    /// <summary>
    /// Identifies the <see cref="CursorForbidden"/> attached property.
    /// </summary>
    public static readonly DependencyProperty CursorForbiddenProperty = DependencyProperty.Register("CursorForbidden", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(Cursors.None));

    /// <summary>
    /// Gets or sets the forbidden cursor.
    /// </summary>
    public Cursor CursorForbidden
    {
        get { return (Cursor)GetValue(CursorForbiddenProperty); }
        set { SetValue(CursorForbiddenProperty, value); }
    }
    #endregion

    #region Cursor Move
    /// <summary>
    /// Identifies the <see cref="CursorMove"/> attached property.
    /// </summary>
    public static readonly DependencyProperty CursorMoveProperty = DependencyProperty.Register("CursorMove", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(Cursors.None));

    /// <summary>
    /// Gets or sets the move cursor.
    /// </summary>
    public Cursor CursorMove
    {
        get { return (Cursor)GetValue(CursorMoveProperty); }
        set { SetValue(CursorMoveProperty, value); }
    }
    #endregion

    #region Cursor Copy
    /// <summary>
    /// Identifies the <see cref="CursorCopy"/> attached property.
    /// </summary>
    public static readonly DependencyProperty CursorCopyProperty = DependencyProperty.Register("CursorCopy", typeof(Cursor), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(Cursors.None));

    /// <summary>
    /// Gets or sets the copy cursor.
    /// </summary>
    public Cursor CursorCopy
    {
        get { return (Cursor)GetValue(CursorCopyProperty); }
        set { SetValue(CursorCopyProperty, value); }
    }
    #endregion

    #region Expand Button Width
    /// <summary>
    /// Identifies the <see cref="ExpandButtonWidth"/> attached property.
    /// </summary>
    public static readonly DependencyProperty ExpandButtonWidthProperty = DependencyProperty.Register("ExpandButtonWidth", typeof(double), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(0.0), new ValidateValueCallback(IsValidExpandButtonWidth));

    /// <summary>
    /// Gets or sets the expand button width.
    /// </summary>
    public double ExpandButtonWidth
    {
        get { return (double)GetValue(ExpandButtonWidthProperty); }
        set { SetValue(ExpandButtonWidthProperty, value); }
    }

    /// <summary>
    /// Checks if an expand button width is valid.
    /// </summary>
    /// <param name="value">The width to check.</param>
    /// <returns>True if valid; Otherwise, false.</returns>
    public static bool IsValidExpandButtonWidth(object value)
    {
        double Width = (double)value;
        return Width >= 0;
    }
    #endregion

    #region Expand Button Style
    /// <summary>
    /// Identifies the <see cref="ExpandButtonStyle"/> attached property.
    /// </summary>
    public static readonly DependencyProperty ExpandButtonStyleProperty = DependencyProperty.Register("ExpandButtonStyle", typeof(Style), typeof(ExtendedTreeViewBase));

    /// <summary>
    /// Gets or sets the expand button style.
    /// </summary>
    public Style ExpandButtonStyle
    {
        get { return (Style)GetValue(ExpandButtonStyleProperty); }
        set { SetValue(ExpandButtonStyleProperty, value); }
    }
    #endregion

    #region Indentation Width
    /// <summary>
    /// Identifies the <see cref="IndentationWidth"/> attached property.
    /// </summary>
    public static readonly DependencyProperty IndentationWidthProperty = DependencyProperty.Register("IndentationWidth", typeof(double), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(0.0), new ValidateValueCallback(IsValidIndentationWidth));

    /// <summary>
    /// Gets or sets the indentation width.
    /// </summary>
    public double IndentationWidth
    {
        get { return (double)GetValue(IndentationWidthProperty); }
        set { SetValue(IndentationWidthProperty, value); }
    }

    /// <summary>
    /// Checks if an indentation width is valid.
    /// </summary>
    /// <param name="value">The width to check.</param>
    /// <returns>True if valid; Otherwise, false.</returns>
    public static bool IsValidIndentationWidth(object value)
    {
        double Width = (double)value;
        return Width >= 0;
    }
    #endregion

    #region Has Context Menu Open
    /// <summary>
    /// Identifies the HasContextMenuOpen attached property.
    /// </summary>
    public static readonly DependencyProperty HasContextMenuOpenProperty = HasContextMenuOpenPropertyKey?.DependencyProperty !;
    private static readonly DependencyPropertyKey HasContextMenuOpenPropertyKey = DependencyProperty.RegisterAttachedReadOnly("HasContextMenuOpen", typeof(bool), typeof(ExtendedTreeViewBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    /// <summary>
    /// Gets the value of the HasContextMenuOpen property.
    /// </summary>
    /// <param name="element">The element with the property.</param>
    /// <returns>The property value.</returns>
    public static bool GetHasContextMenuOpen(DependencyObject element)
    {
        if (element is not null)
            return (bool)element.GetValue(HasContextMenuOpenProperty);
        else
            return false;
    }
    #endregion
}

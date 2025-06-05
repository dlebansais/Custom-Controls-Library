namespace CustomControls;

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents a text block that can be edited, for instance to rename a file.
/// Implemented as a normal, styleable TextBlock replaced by a TextBox when the user clicks on it.
/// Features:
/// . The delay between click and editing can be changed.
/// . The focus must be on a parent of the TextBlock for editing to occur.
/// . Reports events such as entering and leaving edit mode. User's actions can be canceled.
/// . The TextBlock and TextBox can be styled independently.
/// . Editing begins with the entire text selected.
/// . Editing mode is left if one of the following occurs:
///   . The TextBox looses the focus.
///   . The selector (listbox) hosting the control becomes inactive.
///   . The user press one of the following keys:
///   . Return, to validate the change.
///   . Escape, to cancel the change.
/// </summary>
public partial class EditableTextBlock : UserControl, IDisposable
{
    #region ClickDelay
    /// <summary>
    /// Identifies the <see cref="ClickDelay"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="ClickDelay"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty ClickDelayProperty = DependencyProperty.Register(nameof(ClickDelay), typeof(TimeSpan), typeof(EditableTextBlock), new FrameworkPropertyMetadata(DefaultClickDelay), new ValidateValueCallback(IsValidClickDelay));

    /// <summary>
    /// Gets or sets The delay between a click and the actual switch to editing mode.
    /// There is a minimum delay corresponding to the system double-click time.
    /// Only a time span greater than or equal to zero is valid.
    /// </summary>
    public TimeSpan ClickDelay
    {
        get => (TimeSpan)GetValue(ClickDelayProperty);
        set => SetValue(ClickDelayProperty, value);
    }

    /// <summary>
    /// Checks if a click delay is valid.
    /// </summary>
    /// <param name="value">The instance to check.</param>
    /// <returns>True if the delay is valid; Otherwise, false.</returns>
    internal static bool IsValidClickDelay(object value)
    {
        TimeSpan Delay = (TimeSpan)value;
        return Delay >= TimeSpan.Zero;
    }
    #endregion

    #region Editable
    /// <summary>
    /// Identifies the <see cref="Editable"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="Editable"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty EditableProperty = DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(true));

    /// <summary>
    /// Gets or sets a value indicating whether the user can click on the control to start editing.
    /// True, the user can click on the control to start editing (or the application can initiate it any other way).
    /// False, the control cannot be edited and the value of IsEditing is ignored.
    /// </summary>
    public bool Editable
    {
        get => (bool)GetValue(EditableProperty);
        set => SetValue(EditableProperty, value);
    }
    #endregion

    #region Is Editing
    /// <summary>
    /// Identifies the <see cref="IsEditing"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="IsEditing"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty IsEditingProperty = DependencyProperty.Register(nameof(IsEditing), typeof(bool), typeof(EditableTextBlock), new FrameworkPropertyMetadata(false, OnIsEditingChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the text is being edited.
    /// True, the text is being edited. The application can start editing by writing this value.
    /// False, the text is displayed as a normal TextBlock.
    /// </summary>
    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    /// <summary>
    /// Called when the <see cref="IsEditing"/> dependency property is changed on <paramref name="modifiedObject"/>.
    /// </summary>
    /// <param name="modifiedObject">The object that had its property modified.</param>
    /// <param name="args">Information about the change.</param>
    private static void OnIsEditingChanged(DependencyObject modifiedObject, DependencyPropertyChangedEventArgs args)
    {
        EditableTextBlock Ctrl = (EditableTextBlock)modifiedObject;
        Ctrl.OnIsEditingChanged();
    }

    /// <summary>
    /// Called when the <see cref="IsEditing"/> dependency property is changed.
    /// </summary>
    private void OnIsEditingChanged()
    {
        if (Editable)
            if (IsEditing)
                SwitchToTextBox();
            else
                SwitchToTextBlock();
    }
    #endregion

    #region Edit Enter event
    /// <summary>
    /// Identifies the <see cref="EditEnter"/> routed event.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="EditEnter"/> routed event.
    /// </returns>
    public static readonly RoutedEvent EditEnterEvent = EventManager.RegisterRoutedEvent(nameof(EditEnter), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBlock));

    /// <summary>
    /// Sent when the control is about to enter editing mode because of a user action (clicking the control).
    /// If canceled, the control does not enter editing mode and IsEditing remains false.
    /// </summary>
    public event RoutedEventHandler EditEnter
    {
        add { AddHandler(EditEnterEvent, value); }
        remove { RemoveHandler(EditEnterEvent, value); }
    }

    /// <summary>
    /// Sends a <see cref="EditEnter"/> event.
    /// </summary>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    protected virtual void NotifyEditEnter(CancellationTokenSource cancellation)
    {
        string Text = ctrlTextBlock.Text;
        EditableTextBlockEventArgs Args = CreateEditEnterEvent(Text, cancellation);
        Debug.Assert(Args.Text == Text);

        RaiseEvent(Args);
    }

    /// <summary>
    /// Creates arguments for the EditEnter routed event.
    /// </summary>
    /// <param name="textToEdit">The current content of the control.</param>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    /// <returns>The EditableTextBlockEventArgs object created.</returns>
    protected virtual EditableTextBlockEventArgs CreateEditEnterEvent(string textToEdit, CancellationTokenSource cancellation) => new(EditEnterEvent, this, textToEdit, cancellation);
    #endregion

    #region Edit Leave event
    /// <summary>
    /// Identifies the <see cref="EditLeave"/> routed event.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="EditLeave"/> routed event.
    /// </returns>
    public static readonly RoutedEvent EditLeaveEvent = EventManager.RegisterRoutedEvent(nameof(EditLeave), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBlock));

    /// <summary>
    /// Sent when the control is about to leave editing mode because of a user action (hitting the Return or Escape key, or changing the focus)
    /// If the user has validated the new text (with the Return key), IsEditCanceled is false, otherwise it is true.
    /// Leaving edit mode can only be canceled if IsEditCanceled is false.
    /// If canceled, the control does not leave editing mode and IsEditing remains true.
    /// </summary>
    public event RoutedEventHandler EditLeave
    {
        add { AddHandler(EditLeaveEvent, value); }
        remove { RemoveHandler(EditLeaveEvent, value); }
    }

    /// <summary>
    /// Sends a <see cref="EditLeave"/> event.
    /// </summary>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    /// <param name="isEditCanceled">A value that indicates if editing has been canceled.</param>
    protected virtual void NotifyEditLeave(CancellationTokenSource cancellation, bool isEditCanceled)
    {
        EditLeaveEventArgs Args = CreateEditLeaveEvent(ctrlTextBox.Text, cancellation, isEditCanceled);
        Debug.Assert(Args.IsEditCanceled == isEditCanceled);

        RaiseEvent(Args);
    }

    /// <summary>
    /// Creates arguments for the EditLeave routed event.
    /// </summary>
    /// <param name="newText">The current content of the control.</param>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    /// <param name="isEditCanceled">A value that indicates if editing has been canceled.</param>
    /// <returns>The EditableTextBlockEventArgs object created.</returns>
    protected virtual EditLeaveEventArgs CreateEditLeaveEvent(string newText, CancellationTokenSource cancellation, bool isEditCanceled) => new(EditLeaveEvent, this, newText, cancellation, isEditCanceled);
    #endregion

    #region Text
    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="Text"/> dependency property.
    /// </returns>
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata(string.Empty));

    /// <summary>
    /// Gets or sets the text displayed by the control. Does not change while the user is editing it.
    /// The new value is reported after the user has pressed the Return key.
    /// </summary>
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    #endregion

    #region Text Changed event
    /// <summary>
    /// Identifies the <see cref="TextChanged"/> routed event.
    /// </summary>
    /// <returns>
    /// The identifier for the <see cref="TextChanged"/> routed event.
    /// </returns>
    public static readonly RoutedEvent TextChangedEvent = EventManager.RegisterRoutedEvent(nameof(TextChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(EditableTextBlock));

    /// <summary>
    /// Reports that the user pressed the Return key to validate a change. The Text content may have not been modified.
    /// The control has left editing mode before this event is sent.
    /// If canceled, the previous text is not replaced.
    /// </summary>
    public event RoutedEventHandler TextChanged
    {
        add { AddHandler(TextChangedEvent, value); }
        remove { RemoveHandler(TextChangedEvent, value); }
    }

    /// <summary>
    /// Sends a <see cref="TextChanged"/> event.
    /// </summary>
    /// <param name="newText">The current content of the control.</param>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    protected virtual void NotifyTextChanged(string newText, CancellationTokenSource cancellation)
    {
        EditableTextBlockEventArgs Args = CreateTextChangedEvent(newText, cancellation);
        Debug.Assert(Args.Text == newText);

        RaiseEvent(Args);
    }

    /// <summary>
    /// Creates arguments for the TextChanged routed event.
    /// </summary>
    /// <param name="newText">The current content of the control.</param>
    /// <param name="cancellation">A token to hold cancellation information.</param>
    /// <returns>The EditableTextBlockEventArgs object created.</returns>
    protected virtual EditableTextBlockEventArgs CreateTextChangedEvent(string newText, CancellationTokenSource cancellation) => new(TextChangedEvent, this, newText, cancellation);
    #endregion
}

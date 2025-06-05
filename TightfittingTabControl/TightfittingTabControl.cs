namespace CustomControls;

using System;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Represents <see cref="TabControl"/> that starts with a size large enough to contains all its <see cref="TabItem"/> items.
/// <para>Implemented as a derived class of the <see cref="TabControl"/> parent.</para>
/// </summary>
public class TightfittingTabControl : TabControl
{
    #region Init
    /// <summary>
    /// Initializes a new instance of the <see cref="TightfittingTabControl"/> class.
    /// </summary>
    public TightfittingTabControl()
    {
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        InitCycling();
        SizeChanged += OnSizeChanged;
        _ = Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new SizeChangedEventHandler(OnSizeChanged), null, null);
    }
    #endregion

    #region Clycling through pages
    /// <summary>
    /// Event triggered when the control is resized to fit all pages.
    /// </summary>
    public event EventHandler? CyclingCompleted;

    private void InitCycling()
    {
        CycleDone = false;
        CycleIndex = -1;
        LargestWidth = 0;
        LargestHeight = 0;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        UpdateLargestSize(args);

        if (!CycleDone)
            UpdateCycle();
    }

    private void UpdateLargestSize(SizeChangedEventArgs args)
    {
        if (args.WidthChanged && LargestWidth < args.NewSize.Width)
            LargestWidth = args.NewSize.Width;

        if (args.HeightChanged && LargestHeight < args.NewSize.Height)
            LargestHeight = args.NewSize.Height;
    }

    private void UpdateCycle()
    {
        if (CycleIndex == -1)
            FirstCycle();

        NextCycle();

        if (CycleIndex != InitSelectedIndex)
            SelectedIndex = CycleIndex;
        else
            FinishCycle();
    }

    private void FirstCycle()
    {
        SaveSelectedIndex();

        if (SelectedIndex >= 0)
            CycleIndex = SelectedIndex;
        else
            CycleIndex = 0;
    }

    private void SaveSelectedIndex() => InitSelectedIndex = SelectedIndex;

    private void NextCycle()
    {
        CycleIndex = CycleIndex + 1;
        if (CycleIndex >= Items.Count)
            CycleIndex = 0;
    }

    private void FinishCycle()
    {
        CycleDone = true;

        RestoreSelectedIndex();
        SetMinSize();

        CyclingCompleted?.Invoke(this, new EventArgs());
    }

    private void RestoreSelectedIndex() => SelectedIndex = InitSelectedIndex;

    private void SetMinSize()
    {
        if (MinWidth < LargestWidth)
            MinWidth = LargestWidth;
        if (MinHeight < LargestHeight)
            MinHeight = LargestHeight;
    }

    private bool CycleDone;
    private int CycleIndex;
    private double LargestWidth;
    private double LargestHeight;
    private int InitSelectedIndex;
    #endregion
}

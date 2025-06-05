namespace CustomControls;

using System;
using System.Windows;
using System.Windows.Media;
using Contracts;

/// <summary>
/// Interaction logic for DisplayWindow.xaml.
/// </summary>
public partial class DisplayWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisplayWindow"/> class.
    /// </summary>
    /// <param name="lagMeter">The LagMeter control.</param>
    public DisplayWindow(DispatcherLagMeter lagMeter)
    {
        LagMeter = lagMeter;

        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Updates the displayed content of this window.
    /// </summary>
    internal void Update() => Dispatcher.Invoke(InvalidateVisual);

    /// <inheritdoc cref="UIElement.OnRender"/>
    [Access("protected", "override")]
    [RequireNotNull(nameof(drawingContext))]
    private void OnRenderVerified(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        double Length = Math.Min(ActualWidth, ActualHeight);
        Point Center = new(ActualWidth / 2, ActualHeight / 2);
        double RadiusX = Length / 8;
        double RadiusY = Length / 2;

        const double MaxAngle = 90;
        double Angle = (1 - (1 / (1 + (LagMeter.LastLag / Math.Max(LagMeter.DurationSensitivity, 0.01))))) * MaxAngle;

        const double BaseLength = 6.0;
        const double MaxReddening = 1.0;
        double Reddening = (1.0 - (1.0 / (1.0 + (Math.Max(0, LagMeter.LastQueueLength - BaseLength) / Math.Max(LagMeter.QueueLengthSensitivity, 1.0))))) * MaxReddening;

        double Red = 2.0 * Reddening;
        double Green = 2.0 * (1 - Reddening);
        const double Blue = 0;
        Brush Brush = new SolidColorBrush(Color.FromRgb(ToByteColor(Red), ToByteColor(Green), ToByteColor(Blue)));

        drawingContext.DrawRectangle(LagMeter.Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
        drawingContext.PushTransform(new TranslateTransform(Center.X, Center.Y));
        drawingContext.PushTransform(new RotateTransform(Angle));
        drawingContext.DrawEllipse(Brush, null, new Point(0, 0), RadiusX, RadiusY);
        drawingContext.Pop();
        drawingContext.Pop();
    }

    private static byte ToByteColor(double x) => (byte)Math.Max(Math.Min(255, x * 255), 0);

    private DispatcherLagMeter LagMeter { get; }
}

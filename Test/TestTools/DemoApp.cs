namespace TestTools;

using FlaUI.Core;
using FlaUI.Core.AutomationElements;

public class DemoApp
{
    /// <summary>
    /// Gets or sets the coverage application.
    /// </summary>
    public required Application CoverageApp { get; init; }

    /// <summary>
    /// Gets or sets the application.
    /// </summary>
    public required Application App { get; init; }

    /// <summary>
    /// Gets or sets the main window.
    /// </summary>
    public required Window MainWindow { get; init; }
}

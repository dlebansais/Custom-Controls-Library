namespace DispatcherLagMeter.Demo;

using System;
using System.Diagnostics;
using System.Windows;

/// <summary>
/// The DispatcherLagMeter demo program.
/// </summary>
internal partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
            if (args[1] == "escape1")
                DispatcherLagMeter.Demo.MainWindow.TestEscape = 1;
            else if (args[1] == "escape2")
                DispatcherLagMeter.Demo.MainWindow.TestEscape = 2;
            else if (args[1] == "escape3")
                DispatcherLagMeter.Demo.MainWindow.TestEscape = 3;
            else
                Process.GetCurrentProcess().Kill();
    }
}

namespace EnumRadioButtonDemo;

using System;
using System.Diagnostics;
using System.Windows;

/// <summary>
/// The EnumRadioButtonDemo program.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App()
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
            Process.GetCurrentProcess().Kill();
    }
}

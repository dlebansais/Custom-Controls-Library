namespace EditableTextBlock.Demo;

using System;
using System.Diagnostics;
using System.Windows;

/// <summary>
/// The EditableTextBlock demo program.
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
            if (args[1] == "escape1")
                EditableTextBlock.Demo.MainWindow.TestEscape = 1;
            else if (args[1] == "escape2")
                EditableTextBlock.Demo.MainWindow.TestEscape = 2;
            else if (args[1] == "escape3")
                EditableTextBlock.Demo.MainWindow.TestEscape = 3;
            else if (args[1] == "escape4")
                EditableTextBlock.Demo.MainWindow.TestEscape = 4;
            else if (args[1] == "escape5")
                EditableTextBlock.Demo.MainWindow.TestEscape = 5;
            else
                Process.GetCurrentProcess().Kill();
    }
}

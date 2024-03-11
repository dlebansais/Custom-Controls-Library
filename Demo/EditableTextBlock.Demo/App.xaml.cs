namespace EditableTextBlockDemo;

using System;
using System.Diagnostics;
using System.Windows;

/// <summary>
/// The EditableTextBlockDemo program.
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
                EditableTextBlockDemo.MainWindow.TestEscape = 1;
            else if (args[1] == "escape2")
                EditableTextBlockDemo.MainWindow.TestEscape = 2;
            else if (args[1] == "escape3")
                EditableTextBlockDemo.MainWindow.TestEscape = 3;
            else if (args[1] == "escape4")
                EditableTextBlockDemo.MainWindow.TestEscape = 4;
            else if (args[1] == "escape5")
                EditableTextBlockDemo.MainWindow.TestEscape = 5;
            else
                Process.GetCurrentProcess().Kill();
    }
}

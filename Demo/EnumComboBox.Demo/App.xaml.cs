﻿namespace EnumComboBox.Demo;

using System;
using System.Diagnostics;
using System.Windows;

/// <summary>
/// The EnumComboBox demo program.
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
            Process.GetCurrentProcess().Kill();
    }
}

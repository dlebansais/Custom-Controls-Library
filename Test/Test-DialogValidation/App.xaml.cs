﻿using System;
using System.Diagnostics;
using System.Windows;

namespace TestDialogValidation
{
    public partial class App : Application
    {
        public App()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                if (args[1] == "unset")
                    TestDialogValidation.MainWindow.TestUnset = true;
                else
                    Process.GetCurrentProcess().Kill();
        }
    }
}

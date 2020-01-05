using System;
using System.Diagnostics;
using System.Windows;

namespace TestEditableTextBlock
{
    public partial class App : Application
    {
        public App()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                Process.GetCurrentProcess().Kill();
        }
    }
}

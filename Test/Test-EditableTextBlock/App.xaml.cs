using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace TestEditableTextBlock
{
    public partial class App : Application
    {
        public App()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                if (args[1] == "escape1")
                    TestEditableTextBlock.MainWindow.TestEscape = 1;
                else if (args[1] == "escape2")
                    TestEditableTextBlock.MainWindow.TestEscape = 2;
                else if (args[1] == "escape3")
                    TestEditableTextBlock.MainWindow.TestEscape = 3;
                else if (args[1] == "escape4")
                    TestEditableTextBlock.MainWindow.TestEscape = 4;
                else if (args[1] == "escape5")
                    TestEditableTextBlock.MainWindow.TestEscape = 5;
                else
                    Process.GetCurrentProcess().Kill();
        }
    }
}

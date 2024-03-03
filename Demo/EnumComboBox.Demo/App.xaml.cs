namespace TestEnumComboBox
{
    using System;
    using System.Diagnostics;
    using System.Windows;

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

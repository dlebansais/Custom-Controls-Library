namespace TestTools;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Contracts;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using NuGet.Configuration;

public static class DemoApplication
{
    public static DemoApp? Launch(string demoAppName, string? arguments = null)
    {
        string? OpenCoverBasePath = GetPackagePath("opencover");

        string TestDirectory = AppDomain.CurrentDomain.BaseDirectory;

#if NETFRAMEWORK
        string AppDirectory = TestDirectory.Replace(@"\Test\", @"\Demo\").Replace(@".Test\", @".Demo\");
#else
        string AppDirectory = TestDirectory.Replace(@"\Test\", @"\Demo\", StringComparison.InvariantCulture).Replace(@".Test\", @".Demo\", StringComparison.InvariantCulture);
#endif
        string AppName = Path.Combine(AppDirectory, $"{demoAppName}.exe");
        string ResultFileName = Environment.GetEnvironmentVariable("RESULTFILENAME") ?? "result.xml";
        string CoverageAppName = @$"{OpenCoverBasePath}\tools\OpenCover.Console.exe";
        string CoverageAppArgs = @$"-register:user -target:""{AppName}"" -targetargs:""{arguments}"" ""-filter:+[*]* -[{demoAppName}*]*"" -output:""{TestDirectory}\{ResultFileName}""";

        if (Application.Launch(CoverageAppName, CoverageAppArgs) is Application CoverageApp)
        {
            Console.WriteLine($"{DateTime.Now} - CoverageAppName launched");
            Thread.Sleep(TimeSpan.FromSeconds(15));

            if (Application.Attach(AppName) is Application App)
            {
                Console.WriteLine($"{DateTime.Now} - AppName attached");
                _ = App.WaitWhileMainHandleIsMissing();
                Console.WriteLine($"{DateTime.Now} - AppName handle obtained");

                using FlaUI.UIA3.UIA3Automation Automation = new();

                if (App.GetMainWindow(Automation) is Window MainWindow)
                {
                    return new DemoApp { CoverageApp = CoverageApp, App = App, MainWindow = MainWindow };
                }

                Stop(CoverageApp, App);
            }
            else
                _ = CoverageApp.Close();
        }

        return null;
    }

    public static void Stop(DemoApp testApp)
    {
        Contract.RequireNotNull(testApp, out DemoApp TestApp);

        Stop(TestApp.CoverageApp, TestApp.App);
    }

    public static bool IsStopped(DemoApp testApp)
    {
        Contract.RequireNotNull(testApp, out DemoApp TestApp);

        return TestApp.CoverageApp.HasExited;
    }

    private static void Stop(Application coverageAppp, Application app)
    {
        _ = app.Close();

        Console.WriteLine($"{DateTime.Now} - Waiting");

        Stopwatch Watch = new();
        Watch.Start();
        while (!coverageAppp.HasExited && Watch.Elapsed < TimeSpan.FromSeconds(100.0))
            Thread.Sleep(1000);

        Thread.Sleep(2000);
        Console.WriteLine($"{DateTime.Now} - Coverage app closed");
    }

    private static int[]? GetVersion(string formattedVersion)
    {
        string[] Versions = formattedVersion.Split('.');
        int[] Version = new int[Versions.Length];

        for (int i = 0; i < Versions.Length; i++)
            if (int.TryParse(Versions[i], out int Value))
                Version[i] = Value;
            else
                return null;

        return Version;
    }

    private static int CompareVersion(int[] version1, int[] version2)
    {
        for (int i = 0; i < version1.Length && i < version2.Length; i++)
            if (version1[i] < version2[i])
                return 1;
            else if (version1[i] > version2[i])
                return -1;

        return version2.Length - version1.Length;
    }

    private static string? GetPackagePath(string packageName)
    {
        if (Settings.LoadDefaultSettings(null) is var NugetSettings && SettingsUtility.GetGlobalPackagesFolder(NugetSettings) is var NugetFolder)
        {
            string PackageBasePath = Path.Combine(NugetFolder, packageName);
            string[] Directories = Directory.GetDirectories(PackageBasePath);
            int[]? SelectedVersion = null;
            int SelectedVersionIndex = -1;

            for (int i = 0; i < Directories.Length; i++)
                if (GetVersion(Path.GetFileName(Directories[i])) is int[] Version && (SelectedVersion is null || CompareVersion(SelectedVersion, Version) > 0))
                {
                    SelectedVersion = Version;
                    SelectedVersionIndex = i;
                }

            if (SelectedVersionIndex >= 0)
            {
                string PackageVersionPath = Directories[SelectedVersionIndex];
                return PackageVersionPath;
            }
        }

        return null;
    }
}

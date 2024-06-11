namespace EnumComboBox.Test;

using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using NUnit.Framework;
using System;
using System.Threading;
using TestTools;

[TestFixture]
public class UnitTest
{
    private const string DemoAppName = "DispatcherLagMeter.Demo";

    [Test]
    public void TestDefault()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }

    [Test]
    public void TestEscape1()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName, "escape1");
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        Thread.Sleep(TimeSpan.FromSeconds(10));

        bool IsStopped = DemoApplication.IsStopped(DemoApp);
        Assert.That(IsStopped, Is.True);
    }

    [Test]
    public void TestEscape2()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName, "escape2");
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }
}

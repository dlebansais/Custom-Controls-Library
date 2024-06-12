namespace ExtendedPasswordBox.Test;

using System;
using System.Threading;
using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using TestTools;

[TestFixture]
public class UnitTest
{
    private const string DemoAppName = "ExtendedPasswordBox.Demo";

    [Test]
    public void TestDefault1()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }
}

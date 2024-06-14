namespace ExtendedPasswordBox.Test;

using System;
using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
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

    [Test]
    public void TestDefault2()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        TextBox TextBox = MainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit)).AsTextBox();
        Assert.That(TextBox, Is.Not.Null);

        TextBox.Text = "test";

        Thread.Sleep(TimeSpan.FromSeconds(2));

        TextBox.Text = string.Empty;

        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckBox CheckBox = MainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.CheckBox)).AsCheckBox();
        Assert.That(CheckBox, Is.Not.Null);
        Assert.That(CheckBox.IsChecked, Is.False);

        CheckBox.IsChecked = true;

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

        DemoApplication.Stop(DemoApp);
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

    [Test]
    public void TestEscape3()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName, "escape3");
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        TextBox TextBox = MainWindow.FindFirstDescendant(cf => cf.ByControlType(ControlType.Edit)).AsTextBox();
        Assert.That(TextBox, Is.Not.Null);

        TextBox.Text = "test";

        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }
}

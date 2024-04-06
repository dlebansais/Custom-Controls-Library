namespace DialogValidation.Test;

using System;
using System.Threading;
using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using TestTools;

[TestFixture]
public class UnitTest
{
    private const string DemoAppName = "DialogValidation.Demo";

    [Test]
    public void TestDefault1()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement ButtonOKElement = MainWindow.FindFirstDescendant(cf => cf.ByName("OK"));
        Assert.That(ButtonOKElement, Is.Not.Null);

        ButtonOKElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        bool IsStopped = DemoApplication.IsStopped(DemoApp);
        Assert.That(IsStopped, Is.True);
    }

    [Test]
    public void TestDefault2()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement ButtonCancelElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Cancel"));
        Assert.That(ButtonCancelElement, Is.Not.Null);

        ButtonCancelElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        bool IsStopped = DemoApplication.IsStopped(DemoApp);
        Assert.That(IsStopped, Is.True);
    }

    [Test]
    public void TestDefault3()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement CheckAddYesElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Add Yes"));
        Assert.That(CheckAddYesElement, Is.Not.Null);

        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        AutomationElement CheckIsLocalizedElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Is Localized"));
        Assert.That(CheckIsLocalizedElement, Is.Not.Null);

        CheckIsLocalizedElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckIsLocalizedElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckIsLocalizedElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckIsLocalizedElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        AutomationElement CheckIsHorizontalElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Horizontal"));
        Assert.That(CheckIsHorizontalElement, Is.Not.Null);

        CheckIsHorizontalElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckIsHorizontalElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckIsLocalizedElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        AutomationElement ButtonSetCustomCommandsElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Set Custom Commands"));
        Assert.That(ButtonSetCustomCommandsElement, Is.Not.Null);

        ButtonSetCustomCommandsElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }

    [Test]
    public void TestDefault4()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement ButtonSetCustomCommandsElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Set Custom Commands"));
        ButtonSetCustomCommandsElement.Click();

        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }
}

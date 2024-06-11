namespace EditableTextBlock.Test;

using System;
using System.Threading;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using NUnit.Framework;
using TestTools;

[TestFixture]
public class UnitTest
{
    private const string DemoAppName = "EditableTextBlock.Demo";

    [Test]
    public void TestDefault1()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        ClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }

    [Test]
    public void TestDefault2()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        ClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Mouse.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }

    [Test]
    public void TestDefault3()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        DoubleClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        AutomationElement CheckIsEditableElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Editable"));
        Assert.That(CheckIsEditableElement, Is.Not.Null);

        CheckIsEditableElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        ClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckIsEditableElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        ClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckIsEditableElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Thread.Sleep(TimeSpan.FromSeconds(10));

        DemoApplication.Stop(DemoApp);
    }

    [Test]
    [TestCase("escape1")]
    [TestCase("escape2")]
    [TestCase("escape3")]
    [TestCase("escape5")]
    public void TestEscape1(string escape)
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName, escape);
        Assert.That(DemoApp, Is.Not.Null);

        ClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Mouse.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Thread.Sleep(TimeSpan.FromSeconds(10));

        bool IsStopped = DemoApplication.IsStopped(DemoApp);
        Assert.That(IsStopped, Is.True);
    }

    [Test]
    public void TestEscape4()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName, "escape4");
        Assert.That(DemoApp, Is.Not.Null);

        ClickBox(DemoApp);
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Mouse.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        Thread.Sleep(TimeSpan.FromSeconds(10));

        DemoApplication.Stop(DemoApp);
    }

    private static void ClickBox(DemoApp demoApp)
    {
        MoveToClickablePoint(demoApp);
        Mouse.Click();
    }

    private static void DoubleClickBox(DemoApp demoApp)
    {
        MoveToClickablePoint(demoApp);
        Mouse.DoubleClick();
    }

    private static void MoveToClickablePoint(DemoApp demoApp)
    {
        Window MainWindow = demoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement TextElement = MainWindow.FindFirstDescendant(cf => cf.ByName("Init"));
        Assert.That(TextElement, Is.Not.Null);

        TextElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(1));

        TextElement = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("editableTextBlock"));
        Assert.That(TextElement, Is.Not.Null);

        var Point = TextElement.GetClickablePoint();
        Mouse.MoveTo(Point.X, Point.Y);
    }
}

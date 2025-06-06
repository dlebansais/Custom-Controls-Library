﻿namespace ExtendedCommandControls.Test;

using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using System;
using System.Threading;
using TestTools;

[TestFixture]
internal class UnitTest
{
    private const string DemoAppName = "ExtendedCommandControls.Demo";

    [Test]
    [TestCase("extendedToolBarButton1")]
    [TestCase("extendedToolBarButton2")]
    [TestCase("extendedToolBarButton3")]
    public void TestDefault1(string buttonId)
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement ComboElement = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId(buttonId));
        Assert.That(ComboElement, Is.Not.Null);

        ComboElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }

    [Test]
    [TestCase("extendedToolBarButton4")]
    [TestCase("extendedToolBarButton7")]
    public void TestContextMenu(string buttonId)
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement ComboElement = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId(buttonId));
        Assert.That(ComboElement, Is.Not.Null);

        ComboElement.RightClick();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        DemoApplication.Stop(DemoApp);
    }
}

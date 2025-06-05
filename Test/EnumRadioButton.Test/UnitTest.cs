namespace EnumRadioButton.Test;

using FlaUI.Core.AutomationElements;
using NUnit.Framework;
using System;
using System.Threading;
using TestTools;

[TestFixture]
internal class UnitTest
{
    private const string DemoAppName = "EnumRadioButton.Demo";

    [Test]
    public void TestDefault1()
    {
        DemoApp? DemoApp = DemoApplication.Launch(DemoAppName);
        Assert.That(DemoApp, Is.Not.Null);

        Window MainWindow = DemoApp.MainWindow;
        Assert.That(MainWindow, Is.Not.Null);

        AutomationElement ComboElement = MainWindow.FindFirstDescendant(cf => cf.ByAutomationId("enumRadioButtonX"));
        Assert.That(ComboElement, Is.Not.Null);

        AutomationElement CheckboxNullElement = MainWindow.FindFirstDescendant(cf => cf.ByText("Null"));
        Assert.That(CheckboxNullElement, Is.Not.Null);

        AutomationElement CheckboxBadElement = MainWindow.FindFirstDescendant(cf => cf.ByText("Bad"));
        Assert.That(CheckboxBadElement, Is.Not.Null);

        ComboElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckboxNullElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckboxNullElement.Click();

        CheckboxBadElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckboxBadElement.Click();

        DemoApplication.Stop(DemoApp);
    }
}

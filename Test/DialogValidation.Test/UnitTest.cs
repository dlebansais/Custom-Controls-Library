namespace TestDialogValidation;

using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;
using NUnit.Framework;

[TestFixture]
public class UnitTest
{
    [Test]
    public void TestDefault1()
    {
        WindowsDriver<WindowsElement> Session = LaunchApp();

        WindowsElement ButtonOKElement = Session.FindElementByName("OK");
        ButtonOKElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        WindowsElement ButtonCancelElement = Session.FindElementByName("Cancel");
        ButtonCancelElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        WindowsElement CheckAddYesElement = Session.FindElementByName("Add Yes");
        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        WindowsElement CheckIsLocalizedElement = Session.FindElementByName("Is Localized");
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

        WindowsElement CheckIsHorizontalElement = Session.FindElementByName("Horizontal");
        CheckIsHorizontalElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckIsHorizontalElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckIsLocalizedElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        WindowsElement ButtonSetCustomCommandsElement = Session.FindElementByName("Set Custom Commands");
        ButtonSetCustomCommandsElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        CheckAddYesElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        StopApp(Session);
    }

    [Test]
    public void TestDefault2()
    {
        WindowsDriver<WindowsElement> Session = LaunchApp();

        WindowsElement ButtonSetCustomCommandsElement = Session.FindElementByName("Set Custom Commands");
        ButtonSetCustomCommandsElement.Click();

        Thread.Sleep(TimeSpan.FromSeconds(2));

        StopApp(Session);
    }

    private static WindowsDriver<WindowsElement> LaunchApp()
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));

        AppiumOptions appiumOptions = new();
        appiumOptions.AddAdditionalCapability("app", @".\Test\DialogValidation.Test\bin\x64\Debug\net8.0-windows7.0\DialogValidation.Demo.exe");
        appiumOptions.AddAdditionalCapability("appArguments", "ignore");

        return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:9515"), appiumOptions);
    }

    private static void StopApp(WindowsDriver<WindowsElement> session)
    {
        Thread.Sleep(TimeSpan.FromSeconds(2));

        session.Close();
    }
}

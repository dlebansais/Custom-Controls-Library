﻿namespace EnumRadioButton.Test;

using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Threading;

[TestFixture]
public class UnitTest
{
    [Test]
    public void TestDefault1()
    {
        WindowsDriver<WindowsElement> Session = LaunchApp();

        WindowsElement ComboElement = Session.FindElementByAccessibilityId("enumRadioButtonX");
        ComboElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        WindowsElement CheckboxNullElement = Session.FindElementByName("Null");
        CheckboxNullElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckboxNullElement.Click();

        WindowsElement CheckboxBadElement = Session.FindElementByName("Bad");
        CheckboxBadElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        CheckboxBadElement.Click();

        StopApp(Session);
    }

    private static WindowsDriver<WindowsElement> LaunchApp()
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));

        AppiumOptions appiumOptions = new();
        appiumOptions.AddAdditionalCapability("app", @".\Test\Test-EnumRadioButton\bin\x64\Debug\Test-EnumRadioButton.exe");
        appiumOptions.AddAdditionalCapability("appArguments", "ignore");

        return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
    }

    private static void StopApp(WindowsDriver<WindowsElement> session)
    {
        Thread.Sleep(TimeSpan.FromSeconds(2));

        using WindowsDriver<WindowsElement> DeletedSession = session;
    }
}

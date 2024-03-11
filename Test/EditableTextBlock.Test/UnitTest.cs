namespace EditableTextBlock.Test;

using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;

[TestFixture]
public class UnitTest
{
    [Test]
    public void TestDefault1()
    {
        WindowsDriver<WindowsElement> Session = LaunchApp();

        ClickBox(Session);

        StopApp(Session);
    }

    [Test]
    public void TestDefault2()
    {
        WindowsDriver<WindowsElement> Session = LaunchApp();

        ClickBox(Session);
        ClickBox(Session);

        StopApp(Session);
    }

    [Test]
    public void TestDefault3()
    {
        WindowsDriver<WindowsElement> Session = LaunchApp();

        DoubleClickBox(Session);

        WindowsElement CheckIsEditableElement = Session.FindElementByName("Editable");
        CheckIsEditableElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        ClickBox(Session);

        CheckIsEditableElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        ClickBox(Session);

        CheckIsEditableElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(2));

        StopApp(Session);
    }

    private static WindowsDriver<WindowsElement> LaunchApp()
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));

        AppiumOptions appiumOptions = new();
        appiumOptions.AddAdditionalCapability("app", @".\Test\Test-EditableTextBlock\bin\x64\Debug\Test-EditableTextBlock.exe");
        appiumOptions.AddAdditionalCapability("appArguments", "ignore");

        return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
    }

    private static void ClickBox(WindowsDriver<WindowsElement> session)
    {
        WindowsElement TextElement = session.FindElementByName("Init");
        TextElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        TextElement = session.FindElementByAccessibilityId("editableTextBlock");

        Actions action = new(session);
        _ = action.MoveToElement(TextElement, 10, 10);
        _ = action.Click();
        action.Perform();

        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    private static void DoubleClickBox(WindowsDriver<WindowsElement> session)
    {
        WindowsElement TextElement = session.FindElementByName("Init");
        TextElement.Click();
        Thread.Sleep(TimeSpan.FromSeconds(1));
        TextElement = session.FindElementByAccessibilityId("editableTextBlock");

        Actions action = new(session);
        _ = action.MoveToElement(TextElement, 10, 10);
        _ = action.DoubleClick();
        action.Perform();

        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    private static void StopApp(WindowsDriver<WindowsElement> session)
    {
        Thread.Sleep(TimeSpan.FromSeconds(2));

        using WindowsDriver<WindowsElement> DeletedSession = session;
    }
}

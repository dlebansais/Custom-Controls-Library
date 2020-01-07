namespace TestEditableTextBlock
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;
    using OpenQA.Selenium.Interactions;
    using System;
    using System.Threading;

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestDefault1()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            ClickBox(Session);

            StopApp(Session);
        }

        [TestMethod]
        public void TestDefault2()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            ClickBox(Session);
            ClickBox(Session);

            StopApp(Session);
        }

        [TestMethod]
        public void TestDefault3()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            DoubleClickBox(Session);

            WindowsElement CheckIsEditableElement = Session.FindElementByName("Editable");
            CheckIsEditableElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            ClickBox(Session);

            StopApp(Session);
        }

        private WindowsDriver<WindowsElement> LaunchApp()
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));

            AppiumOptions appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", @"C:\Projects\Custom-Controls-Library\Test\Test-EditableTextBlock\bin\x64\Debug\Test-EditableTextBlock.exe");
            appiumOptions.AddAdditionalCapability("appArguments", "ignore");

            return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
        }

        private void ClickBox(WindowsDriver<WindowsElement> session)
        {
            WindowsElement TextElement = session.FindElementByName("Init");
            TextElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            TextElement = session.FindElementByAccessibilityId("editableTextBlock");

            Actions action = new Actions(session);
            action.MoveToElement(TextElement, 10, 10);
            action.Click();
            action.Perform();

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private void DoubleClickBox(WindowsDriver<WindowsElement> session)
        {
            WindowsElement TextElement = session.FindElementByName("Init");
            TextElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            TextElement = session.FindElementByAccessibilityId("editableTextBlock");

            Actions action = new Actions(session);
            action.MoveToElement(TextElement, 10, 10);
            action.DoubleClick();
            action.Perform();

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        private void StopApp(WindowsDriver<WindowsElement> session)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));

            using WindowsDriver<WindowsElement> DeletedSession = session;
        }
    }
}

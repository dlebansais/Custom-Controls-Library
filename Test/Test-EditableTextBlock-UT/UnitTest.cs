namespace TestEditableTextBlock
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;

    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void TestDefault1()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            WindowsElement TextElement = Session.FindElementByName("Init");
            TextElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(20));

            StopApp(Session);
        }

        [TestMethod]
        public void TestDefault2()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            WindowsElement TextElement = Session.FindElementByName("Init");
            TextElement.Click();

            StopApp(Session);
        }

        [TestMethod]
        public void TestDefault3()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            WindowsElement TextElement = Session.FindElementByName("Init");
            TextElement.Click();
            TextElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            WindowsElement CheckIsEditableElement = Session.FindElementByName("Editable");
            CheckIsEditableElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            TextElement = Session.FindElementByName("Init");
            TextElement.Click();

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

        private void StopApp(WindowsDriver<WindowsElement> session)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));

            using WindowsDriver<WindowsElement> DeletedSession = session;
        }
    }
}

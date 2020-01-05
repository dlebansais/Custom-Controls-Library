namespace TestDialogValidation
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Appium;
    using OpenQA.Selenium.Appium.Windows;

    [TestClass]
    public class UnitTest
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Context = context;
        }

        private static TestContext Context;

        [TestMethod]
        public void TestDefault1()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            WindowsElement ButtonOKElement = Session.FindElementByName("OK");
            ButtonOKElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            WindowsElement ButtonCancelElement = Session.FindElementByName("Cancel");
            ButtonCancelElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            WindowsElement CheckIsLocalizedElement = Session.FindElementByName("Is Localized");
            CheckIsLocalizedElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));
            CheckIsLocalizedElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

            WindowsElement CheckAddYesElement = Session.FindElementByName("Add Yes");
            CheckAddYesElement.Click();
            Thread.Sleep(TimeSpan.FromSeconds(2));

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

        [TestMethod]
        public void TestDefault2()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            WindowsElement ButtonSetCustomCommandsElement = Session.FindElementByName("Set Custom Commands");
            ButtonSetCustomCommandsElement.Click();

            Thread.Sleep(TimeSpan.FromSeconds(2));

            StopApp(Session);
        }

        private WindowsDriver<WindowsElement> LaunchApp()
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));

            AppiumOptions appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", @"C:\Projects\Custom-Controls-Library\Test\Test-DialogValidation\bin\x64\Debug\Test-DialogValidation.exe");
            appiumOptions.AddAdditionalCapability("appArguments", "ignore");

            return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
        }

        private void StopApp(WindowsDriver<WindowsElement> session)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));

            session.Close();
        }
    }
}

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

            WindowsElement ButtonNoElement = Session.FindElementByName("OK");
            ButtonNoElement.Click();

            Thread.Sleep(TimeSpan.FromSeconds(2));

            StopApp(Session);
        }

        [TestMethod]
        public void TestDefault2()
        {
            WindowsDriver<WindowsElement> Session = LaunchApp();

            WindowsElement ButtonNoElement = Session.FindElementByName("Cancel");
            ButtonNoElement.Click();

            StopApp(Session);
        }

        private WindowsDriver<WindowsElement> LaunchApp()
        {
            AppiumOptions appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", @".\Test\Test-DialogValidation\bin\x64\Debug\Test-DialogValidation.exe");

            return new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), appiumOptions);
        }

        private void StopApp(WindowsDriver<WindowsElement> session)
        {
            Thread.Sleep(TimeSpan.FromSeconds(2));

            session.Close();
        }
    }
}

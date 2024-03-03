namespace TestEnumRadioButton
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

        private WindowsDriver<WindowsElement> LaunchApp()
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));

            AppiumOptions appiumOptions = new AppiumOptions();
            appiumOptions.AddAdditionalCapability("app", @".\Test\Test-EnumRadioButton\bin\x64\Debug\Test-EnumRadioButton.exe");
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

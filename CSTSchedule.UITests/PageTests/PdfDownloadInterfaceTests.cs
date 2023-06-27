using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Blazorise;
using CSTScheduling.Pages;
using NUnit.Framework;
using Microsoft.AspNetCore.Http;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using AngleSharp.Dom;
using System.IO;
using Microsoft.EntityFrameworkCore;
using CSTScheduling.Shared;
using CSTScheduling.Data.Services;
using CSTSchedule.UITests.fixtures;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Threading;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Interactions;

namespace CSTScheduling.UITests
{
    public class PdfDownloadInterfaceTests
    {

        EdgeDriver driver;
        WebDriverWait wait;
        CstScheduleDbContext testContext;
        const string insSelect = "body > div.page > div.main > div.content.px-4 > div:nth-child(4) > div:nth-child(1) > div > select";
        const string roomSelect = "body > div.page > div.main > div.content.px-4 > div:nth-child(4) > div:nth-child(2) > div > select";
        const string btnSelect = "#btnDownload";
        public const int TEST_WAIT = 250;
        public fixtures fix;

        [OneTimeSetUp]
        public void Setup()
        {
            // Set db source
            string dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".UITests") - 1) + "ing" + @"\CstScheduleDb.db";
            var contextOpts = new DbContextOptionsBuilder<CstScheduleDbContext>()
             .UseSqlite("Data Source=" + dbDirectory)
             .Options;


            // Create a new database context
            testContext = new CstScheduleDbContext(contextOpts);
            // Wipe the database
            testContext.Database.EnsureDeleted();
            // Ensure new database is made
            testContext.Database.EnsureCreated();
            // Populate db with test data
            fix = new CSTSchedule.UITests.fixtures.fixtures();
            fix.Load(testContext); //loads all test data into the test database


            // Make MSEdge driver with disabled cookies
            EdgeOptions options = new EdgeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);
            options.AddArgument("-inprivate");
            options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            driver = new EdgeDriver(options);
            // Make Wait driver
            wait = new WebDriverWait(driver, new System.TimeSpan(0, 0, 8));
            // Maximize window
            driver.Manage().Window.Maximize();
            SelectProgramAndSemester();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Close();
            testContext.Dispose();
        }

        #region Instructor       

        [Test]
        public void Test_OneInsForSem_Valid()
        {
            RefreshPage();

            // Count the number of items in the instructor dropdown
            // It should exactly be 5
            Assert.AreEqual(5, GetMultipleWhen("visible", insSelect + " > option").Count);

            // select first instructor
            GetWhen("clickable", insSelect + " > option:nth-child(1)").Click();

            // Find the href of the "Download" button
            GetWhen("clickable", btnSelect).GetAttribute("href");

            Assert.AreEqual("http://localhost:5000/PdfDownload?sem=18&ins=3&rms=", GetWhen("clickable", btnSelect).GetAttribute("href"));
        }

        [Test]
        public void Test_MultipleInsForSem_Valid()
        {
            RefreshPage();

            // hold ctrl down
            Actions a = new Actions(driver);
            //enter text with keyDown() SHIFT key ,keyUp() then build() ,perform()
            a.KeyDown(Keys.LeftControl);

            // select first instructor
            GetWhen("clickable", insSelect + " > option:nth-child(1)").Click();
            
            // select second instructor
            GetWhen("clickable", insSelect + " > option:nth-child(2)").Click();

            a.KeyUp(Keys.LeftControl);

            // Find the href of the "Download" button
            GetWhen("clickable", btnSelect).GetAttribute("href");

            Assert.AreEqual("http://localhost:5000/PdfDownload?sem=18&ins=3,1&rms=", GetWhen("clickable", btnSelect).GetAttribute("href"));
        }
        #endregion

        #region Room

        [Test]
        public void Test_OneRoomForSem_Valid()
        {
            RefreshPage();


            // select first room
            GetWhen("clickable", roomSelect + " > option:nth-child(1)").Click();

            // Find the href of the "Download" button
            GetWhen("clickable", btnSelect).GetAttribute("href");

            Assert.AreEqual("http://localhost:5000/PdfDownload?sem=18&ins=&rms=1", GetWhen("clickable", btnSelect).GetAttribute("href"));
        }

        [Test]
        public void Test_MultipleRoomForSem_Valid()
        {
            RefreshPage();
            // hold ctrl down
            Actions a = new Actions(driver);
            //enter text with keyDown() SHIFT key ,keyUp() then build() ,perform()
            a.KeyDown(Keys.LeftControl);

            // select first room
            GetWhen("clickable", roomSelect + " > option:nth-child(1)").Click();

            // select second room
            GetWhen("clickable", roomSelect + " > option:nth-child(2)").Click();

            a.KeyUp(Keys.LeftControl);

            // Find the href of the "Download" button
            GetWhen("clickable", btnSelect).GetAttribute("href");

            Assert.AreEqual("http://localhost:5000/PdfDownload?sem=18&ins=&rms=1,2", GetWhen("clickable", btnSelect).GetAttribute("href"));
        }
        #endregion

        #region Instructor + Room     

        [Test]
        public void Test_OneInsOneRoomForSem_Valid()
        {
            RefreshPage();

            // Count the number of items in the instructor dropdown

            // select first instructor
            GetWhen("clickable", insSelect + " > option:nth-child(1)").Click();

            // select first room
            GetWhen("clickable", roomSelect + " > option:nth-child(1)").Click();

            // Find the href of the "Download" button
            GetWhen("clickable", btnSelect).GetAttribute("href");

            Assert.AreEqual("http://localhost:5000/PdfDownload?sem=18&ins=3&rms=1", GetWhen("clickable", btnSelect).GetAttribute("href"));
        }

        [Test]
        public void Test_MultipleInsRoom_Valid()
        {
            RefreshPage();
            // hold ctrl down
            Actions a = new Actions(driver);
            //enter text with keyDown() SHIFT key ,keyUp() then build() ,perform()
            a.KeyDown(Keys.LeftControl);

            // select first room
            GetWhen("clickable", roomSelect + " > option:nth-child(1)").Click();

            // select second room
            GetWhen("clickable", roomSelect + " > option:nth-child(2)").Click();

            // select first instructor
            GetWhen("clickable", insSelect + " > option:nth-child(1)").Click();

            // select second instructor
            GetWhen("clickable", insSelect + " > option:nth-child(2)").Click();

            a.KeyUp(Keys.LeftControl);

            // Find the href of the "Download" button
            GetWhen("clickable", btnSelect).GetAttribute("href");

            Assert.AreEqual("http://localhost:5000/PdfDownload?sem=18&ins=3,1&rms=1,2", GetWhen("clickable", btnSelect).GetAttribute("href"));
        }

        [Test]
        public void Test_NoInsNoRmsNoSem_Invalid()
        {
            RefreshPage();
            Assert.AreEqual("btn btn-info w-100 disabled", GetWhen("clickable", btnSelect).GetAttribute("class"));
        }

        [Test]
        public void Test_NoSemOnlyInsRoom_Invalid()
        {
            driver.Url = "http://localhost:5000/Report";

            zzZ(200);
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(btnSelect)).Count);
        }
        #endregion

        #region Helper Methods
        private void zzZ(int i)
        {
            Thread.Sleep(i);
        }

        /// <summary>
        /// This helper method clears the existing text in an input field
        /// before patiently entering the given text string into the field.
        /// </summary>
        /// <param name="text">The text to be entered</param>
        /// <param name="selector">The selector for the input field</param>
        private void InputWhen(string text, string selector)
        {
            Thread.Sleep(TEST_WAIT);
            GetWhen("exists", selector).Clear();
            Thread.Sleep(TEST_WAIT);
            GetWhen("exists", selector).SendKeys(text + Keys.Tab);
            //GetWhen("exists", selector).SendKeys(text);
        }


        /// <summary>
        /// This helper method waits until the given selector meets the given condition
        /// before returning the requested IWebElement.
        /// </summary>
        /// <param name="condition">The criteria to wait for: "exists", "visible", "clickable", or a text string to wait for in the element.</param>
        /// <param name="selector">The selector to find this IWebElement on the HTML page.</param>
        /// <returns></returns>
        private IWebElement GetWhen(string condition, string selector)
        {
            IWebElement element = null;
            Thread.Sleep(TEST_WAIT);
            switch (condition)
            {
                case "exists":
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
                    element = driver.FindElement(By.CssSelector(selector));
                    break;
                case "visible":
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
                    element = driver.FindElement(By.CssSelector(selector));
                    break;
                case "clickable":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(selector)));
                    element = driver.FindElement(By.CssSelector(selector));
                    break;
                default:
                    wait.Until(ExpectedConditions.TextToBePresentInElement(driver.FindElement(By.CssSelector(selector)), condition));
                    element = driver.FindElement(By.CssSelector(selector));
                    break;
            }
            Thread.Sleep(TEST_WAIT);
            return element;
        }

        /// <summary>
        /// This helper method waits until the given selector meets the given condition
        /// before returning the requested IWebElement.
        /// </summary>
        /// <param name="condition">The criteria to wait for: "exists", "visible", "clickable", or a text string to wait for in the element.</param>
        /// <param name="selector">The selector to find this IWebElement on the HTML page.</param>
        /// <returns></returns>
        private ReadOnlyCollection<IWebElement> GetMultipleWhen(string condition, string selector)
        {
            ReadOnlyCollection<IWebElement> elements = null;
            Thread.Sleep(TEST_WAIT);
            switch (condition)
            {
                case "exists":
                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
                    elements = driver.FindElements(By.CssSelector(selector));
                    break;
                case "visible":
                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
                    elements = driver.FindElements(By.CssSelector(selector));
                    break;
                case "clickable":
                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(selector)));
                    elements = driver.FindElements(By.CssSelector(selector));
                    break;
                default:
                    wait.Until(ExpectedConditions.TextToBePresentInElement(driver.FindElement(By.CssSelector(selector)), condition));
                    elements = driver.FindElements(By.CssSelector(selector));
                    break;
            }
            Thread.Sleep(TEST_WAIT);
            return elements;
        }


        /// <summary>
        /// This procedure navigates to the main page, selects CST as the program,
        /// then selects "Year 1, Semester 1, Group A" on the Schedule creation page.
        /// </summary>
        private void SelectProgramAndSemester()
        {
            Thread.Sleep(TEST_WAIT);
            // Render Programs page
            driver.Url = "http://localhost:5000/";
            Thread.Sleep(TEST_WAIT);
            // Select "CST" as the current department
            var CST_button = GetWhen("exists", "body > div.page > div.main > div.content.px-4 > div > table > tbody > tr:nth-child(1) > td:nth-child(6) > button");
            if (CST_button.Enabled)
            {
                // Click if "CST" is not already selected
                CST_button.Click();
            }

            RefreshPage();
        }

        private void RefreshPage()
        {
            // Render ScheduleCreate page
            driver.Url = "http://localhost:5000/Report";
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            Thread.Sleep(TEST_WAIT);
            // Select the Semester "Year 1, Sem 1, Group A", and then wait.
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("select")));
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
            semester_dropdown.SelectByIndex(1); // Select child <option> by index
        }
        #endregion
    }
}

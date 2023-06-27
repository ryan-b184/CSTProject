﻿using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace CSTSchedule.UITests.PageTests
{
    class RoomPageDeleteTests
    {
        EdgeDriver driver;
        public CSTSchedule.UITests.fixtures.fixtureDatePoint fix;
        CstScheduleDbContext testContext;
        public const int TEST_WAIT = CSTSchedule.UITests.fixtures.fixtures.TEST_WAIT;
        WebDriverWait wait;

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
            // Wipe the database between tests
            testContext.Database.EnsureDeleted();
            // Ensure new database is made
            testContext.Database.EnsureCreated();
            // Make MSEdge driver with disabled cookies
            EdgeOptions options = new EdgeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);
            options.AddArgument("-inprivate");
            options.AddArgument("headless");
            options.AddArgument("disable-gpu");
            driver = new EdgeDriver(options);
            // Make Wait driver
            wait = new WebDriverWait(driver, new System.TimeSpan(0, 0, 15));
            // Maximize window
            driver.Manage().Window.Maximize();

            // Populate db with test data
            fix = new CSTSchedule.UITests.fixtures.fixtureDatePoint();
            fix.Load(testContext); //loads all test data into the test database

        }

        [OneTimeTearDown]
        public void Teardown()
        {
            driver.Close(); //close the driver after all tests are done 
        }

        [Test]
        public void Test_RoomConfirmation()
        {
            string roomDeletebtnSelector = "body > div.page > div.main > div.content.px-4 > div > table > tbody > tr:nth-child(4) > td:nth-child(5) > button.btn.btn-outline-danger";
            string deleteConfirmBtnSelector = "#divButtons > button:nth-child(1)";
            string monday8CourseSelector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";
            string monday8RoomSelectory = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(2) > input";
            string trashCanSelector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > span";
            #region delete room
            loadPage("http://localhost:5000/room");

            // click the delete button 
            getWebElement(roomDeletebtnSelector).Click();
            Thread.Sleep(TEST_WAIT);

            var inputField = getWebElement(deleteConfirmBtnSelector);
            // assert that the modal is visible
            Assert.IsTrue(inputField.Displayed);

            // click ok button to confirm deletion
            getWebElement(deleteConfirmBtnSelector).Click();
            Thread.Sleep(TEST_WAIT);



            #region loop through table to check for room 239B instructor page
            //assert that the room 239B is no longer available
            var table = driver.FindElement(By.TagName("table"));
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
            var trList = table.FindElements(By.TagName("tr"));

            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));

           
            for(int i = 1; i > trList.Count(); i++)
            {
                var room = trList[i].FindElement(By.CssSelector("td:nth-child(0)"));
                Assert.IsTrue(room.GetAttribute("value") != "239B");
            }

            #endregion

            #endregion

            #region check a CISR object with 239B room
            //loadPage("http://localhost:5000/ScheduleCreate");
            // navigate to schedule create page and select the first index of the semester dropdown
            this.SelectProgramAndSemester();


            //click trash can and wait
            getWebElement(trashCanSelector).Click();
            Thread.Sleep(TEST_WAIT);

            // inputs the course "endEarlyCourse" into monday at 8 am which has a default room of 239B
            InputWhen("[endEarlyCourse] endEarlyCourse", monday8CourseSelector);

            //check the room of monday at 8 and see that the field is empty
            Assert.IsTrue(getWebElement(monday8RoomSelectory).GetAttribute("value") == "");
            #endregion
        }

        #region Helper Method
        /// <summary>
        /// This helper method clears the existing text in an input field
        /// before patiently entering the given text string into the field.
        /// </summary>
        /// <param name="text">The text to be entered</param>
        /// <param name="selector">The selector for the input field</param>
        private void InputWhen(string text, string selector)
        {
            GetWhen("exists", selector).Clear();
            Thread.Sleep(TEST_WAIT);
            GetWhen("exists", selector).SendKeys(text + Keys.Tab);
            Thread.Sleep(TEST_WAIT);
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


        public void loadPage(string url)
        {
            Thread.Sleep(TEST_WAIT);
            driver.Url = url;
            Thread.Sleep(TEST_WAIT);
        }

        public IWebElement getWebElement(string selector)
        {
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
            var element = driver.FindElement(By.CssSelector(selector));
            Thread.Sleep(TEST_WAIT);

            return element;
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
            string selector = "body > div.page > div.main > div.content.px-4 > div > table > tbody > tr:nth-child(1) > td:nth-child(6) > button";
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
            var CST_button = driver.FindElement(By.CssSelector(selector));
            if (CST_button.Enabled)
            {
                // Click if "CST" is not already selected
                CST_button.Click();
            }

            // Render ScheduleCreate page
            driver.Url = "http://localhost:5000/ScheduleCreate";
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

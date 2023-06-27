using CSTScheduling.Data.Context;
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
    class CoursePageDeleteTest
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
        public void Test_CourseConfirmation()
        {
            string courseDeleteBtn = "#CourseTable > tbody > tr:nth-child(3) > td:nth-child(11) > button";
            string courseConfirmDeleteBtn = "#divButtons > button:nth-child(1)";
            string secondPageSelector = "body > div.page > div.main > div.content.px-4 > ul > li:nth-child(3) > a";
            string secondPageSelectorActive = "body > div.page > div.main > div.content.px-4 > ul > li.page-item.active > a";
            string datePointList = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul";

            #region course page 
            //load course page
            loadPage("http://localhost:5000/Course");

            //click second page button
            getWebElement(secondPageSelector).Click();

            // click the delete button starLateCourse
            getWebElement(courseDeleteBtn).Click();
            Thread.Sleep(TEST_WAIT);


            // assert that the modal and btn is visible
            Assert.IsTrue(getWebElement(courseConfirmDeleteBtn).Displayed);

            // click ok button to confirm deletion
            getWebElement(courseConfirmDeleteBtn).Click();
            Thread.Sleep(TEST_WAIT);

            //check that page 2 is still has the class "active"
            IWebElement secondPage = getWebElement(secondPageSelectorActive);
            //Assert.IsTrue(secondPage.GetAttribute("class").Contains("active"));
            Assert.IsTrue(secondPage.Displayed);


            #region loop through table to check for course COSC292
            //assert that the instructor bryce is no longer available
            var table = driver.FindElement(By.TagName("table"));
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
            var trList = table.FindElements(By.TagName("tr"));

            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));


            //foreach (var tr in trList)
            for (int i=1; i<trList.Count; i++)
            {//loops through each table row
             //for each row check the course abbr isn't startLateCourse
                var courseAbbr = trList[i].FindElement(By.CssSelector("td:nth-child(1)"));
                Assert.IsTrue(courseAbbr.Text != "startLateCourse");
            }

            #endregion



            #endregion

            #region ScheduleCreate no startLateCourse course available
            //navigation to schedule create page
            this.SelectProgramAndSemester();

            //check that the third date point is no longer there
            IWebElement ul = getWebElement(datePointList);
            var liList = ul.FindElements(By.TagName("li"));

            Assert.IsTrue(liList.Count == 2);
            

            
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

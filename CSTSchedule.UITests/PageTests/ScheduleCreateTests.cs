using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.IO;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net.NetworkInformation;
using System.Text;
using System.Net;
using System;
using CSTSchedule.UITests.fixtures;
using System.Collections.ObjectModel;

namespace CSTScheduling.UITests
{
    public class ScheduleCreateTests
    {
        EdgeDriver driver;
        public CSTSchedule.UITests.fixtures.fixtureDatePoint fix;
        CstScheduleDbContext testContext;
        public const int TEST_WAIT = fixtures.TEST_WAIT;
        WebDriverWait wait;
        List<Semester> semList;
        List<Instructor> insList;
        List<Room> roomList;
        List<Course> courseList;
        List<CISR> cisrList;


        #region Story10 Global Variables
        private const string semesterSelectBox = "";
        private const string courseSelectBox = "";

        /// <summary>
        /// Used for all UI tests to select IWebElements.
        /// </summary>
        // Monday (8:00AM) Elements
        string Monday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";
        string Monday8Room_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(2) > input";
        string Monday8Ins1_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div >div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > input";
        string Monday8Ins2_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(4) > input";
        string Monday8Copy_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(2) > span";
        string Monday8Delete_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div >div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > span";
        // Monday (10:00AM) Elements
        string Monday10_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(3) > td:nth-child(2) > div > div";
        string Monday10Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(3) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";
        // Tuesday (8:00AM) Elements
        string Tuesday8_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(3) > div > div";
        string Tuesday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(3) > div > div > form > div:nth-child(1) > input";


        #endregion

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

            // Fetch from the database
            semList = testContext.Semester.ToList();
            insList = testContext.Instructor.ToList();
            roomList = testContext.Room.ToList();
            courseList = testContext.Course.ToList();
            cisrList = testContext.CISR.ToList();

        }

        [OneTimeTearDown]
        public void Teardown()
        {
            driver.Close(); //close the driver after all tests are done 
        }


        #region Original Tests
        /// <summary>
        /// This Test will Determin If a given semester has 3 Date Breakpoints available
        /// <br />It will then check to ensure that each date breakpoint is displaying appropriate CISR objects for its date range and semester
        /// </summary>
        [Test]
        public void Test_ScheduleWithThreeCoursesFirstBreakpoint_Valid()
        {
            // Navigate to the page
            Thread.Sleep(TEST_WAIT);
            driver.Url = "http://localhost:5000/ScheduleCreate";
            Thread.Sleep(TEST_WAIT);

            //Wait for the semester selection dropdown to appear
            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));

            // Select the first element in the list
            semester_dropdown.SelectByIndex(1);
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("table")));


            /**
             * Checking first date point for three CISRs
             */

            // Checks for the CISR COSC 292
            // Spans the entire semester, should be in every date breakpoint
            Thread.Sleep(TEST_WAIT);
            string selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
            Thread.Sleep(TEST_WAIT);
            var course1 = driver.FindElement(By.CssSelector(selector));
            Assert.AreEqual(course1.GetAttribute("value"), "[COSC292] COSC");

            // Checks for the CISR COSC 292
            // Spans the entire semester, should be in every date breakpoint
            Thread.Sleep(TEST_WAIT);
            string selector2 = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(5) > td:nth-child(4) > div > div > form > div:nth-child(1) > input";
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector2)));
            Thread.Sleep(TEST_WAIT);
            var course2 = driver.FindElement(By.CssSelector(selector2));
            Assert.AreEqual(course2.GetAttribute("value"), "[COSC292] COSC");

            //Checks for the CISR endEarlyCourse
            // This CISR ends early, and should not be in the next date range
            Thread.Sleep(TEST_WAIT);
            string selector3 = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(6) > td:nth-child(5) > div > div > form > div:nth-child(1) > input";
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector3)));
            Thread.Sleep(TEST_WAIT);
            var course3 = driver.FindElement(By.CssSelector(selector3));
            Assert.AreEqual(course3.GetAttribute("value"), "[endEarlyCourse] endEarlyCourse");


            // Switching date range
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("ul")));
            var ul = driver.FindElement(By.CssSelector("body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul"));
            var listItems = ul.FindElements(By.TagName("li"));
            Thread.Sleep(TEST_WAIT);
            listItems[2].Click();

            //Checks the seond date point for the two semester spanning CISR objects
            //Also ensures that the CISR that spanned the previous range is no longer being displayed

            //Checks for the course COSC 292
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
            var course4 = driver.FindElement(By.CssSelector(selector));
            Assert.AreEqual(course4.GetAttribute("value"), "[COSC292] COSC");

            // Checks for the course COSC 292
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector2)));
            var course5 = driver.FindElement(By.CssSelector(selector2));
            Assert.AreEqual(course5.GetAttribute("value"), "[COSC292] COSC");

            // Ensure that a CISR from a previous date range is not being displayed, as the course has ended
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector3)));
            var course6 = driver.FindElement(By.CssSelector(selector3));
            Assert.AreEqual(course6.GetAttribute("value"), "");



            // Switch Date Range
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("ul")));
            listItems[2].Click();

            //Checks the third date range for the two semester spanning CISR objects
            //Also checks for a new CISR object that only spans the final date range

            // Check for the first CISR, ensure that it is displaying the course COSC292
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
            var course7 = driver.FindElement(By.CssSelector(selector));
            Assert.AreEqual(course7.GetAttribute("value"), "[COSC292] COSC");
            Thread.Sleep(TEST_WAIT);

            //Check the second CISR to ensure it is displaying the course COSC292
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector2)));
            var course8 = driver.FindElement(By.CssSelector(selector2));
            Assert.AreEqual(course8.GetAttribute("value"), "[COSC292] COSC");
            Thread.Sleep(TEST_WAIT);

            //Check for the CISR with the name startLateCourse to ensure it is being displayed when the appropriate date range is clicked
            var selector4 = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(4) > div > div > form > div:nth-child(1) > input";
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector4)));
            var course9 = driver.FindElement(By.CssSelector(selector4));
            Assert.AreEqual(course9.GetAttribute("value"), "[startLateCourse] startLateCourse");

        }

        /// <summary>
        /// This test checks to ensure that the SemesterPage component, when given a semester, displays the appropriate number of date ranges for selection
        /// </summary>
        [Test] ///passing
        public void Test_ScheduleWithThreeCourseBreakPoints_Valid()
        {
            // Navigate to the schedule create page
            Thread.Sleep(TEST_WAIT);
            driver.Url = "http://localhost:5000/ScheduleCreate";
            Thread.Sleep(TEST_WAIT);

            // Locate the semester select dropdown and select the first semester
            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
            semester_dropdown.SelectByIndex(1);

            //Wait for the table to appear
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("table")));

            //Wait for the date breakpoint elements to appear
            wait.Until(ExpectedConditions.ElementExists(By.TagName("ul")));
            Thread.Sleep(TEST_WAIT);

            //Locate the number of elements within the date breakpoint select list
            var ul = driver.FindElement(By.CssSelector("body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul"));
            var listItems = ul.FindElements(By.TagName("li"));

            // Ensure there are the appropriate number of date breakpoints available
            Assert.AreEqual(listItems.Count(), 3);

        }

        /// <summary>
        /// This Test checks to ensure the ScheduleComponent displays a single date range for selection when given a semester with a single range available
        /// </summary>
        [Test]
        public void Test_NoBreakPointsTabs_Valid()
        {
            // Navigate to the schedule page
            Thread.Sleep(TEST_WAIT);
            driver.Url = "http://localhost:5000/ScheduleCreate";

            // Wait until the semester select dropdown is available
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));

            // Select the third semester in the list
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
            semester_dropdown.SelectByIndex(3);

            // Wait for the BreakPoint list to appear
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.CssSelector("body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul")));

            //Locate the number of elements within the date breakpoint select list
            Thread.Sleep(TEST_WAIT);
            var ul = driver.FindElement(By.CssSelector("body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul"));
            var listItems = ul.FindElements(By.TagName("li"));

            // Ensure that there is exactly one element available
            Assert.AreEqual(listItems.Count(), 1);
        }

        #endregion

        #region Box Size and margins test

        /// <summary>
        /// This test ensures "Cells" within the ScheduleComponet are the appropriate size
        /// </summary>
        [Test]//passes
        public void Test_CellWidthsAndHeightsWithinSchedule_Valid()
        {
            // Navigate to the page
            Thread.Sleep(TEST_WAIT);
            driver.Url = "http://localhost:5000/ScheduleCreate";
            Thread.Sleep(TEST_WAIT);

            //Wait until the semester dropdown exists, then select the first semester
            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
            semester_dropdown.SelectByIndex(1);
            Thread.Sleep(TEST_WAIT);

            //Wait until the ScheduleComponent displays a table
            wait.Until(ExpectedConditions.ElementExists(By.TagName("table")));
            var table = driver.FindElement(By.TagName("table"));
            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
            var trList = table.FindElements(By.TagName("tr"));

            Thread.Sleep(TEST_WAIT);
            wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));

            //Check to ensure that each cell is the appropraite size in PX
            foreach (var trs in trList)
            {//loops through each table row
                foreach (var td in trs.FindElements(By.TagName("td")))
                {//loops through each table cell
                    var size = td.Size;
                    //height: 7.5rem; width: 13.2rem; 
                    Assert.True(size.Height == 107); 
                    Assert.True(size.Width == 200);

                }
            }

        }


        #endregion

        #region Alignment Tests

        /// <summary>
        /// This Test ensures that the Semester selection dropdown is created with the proper paddigns and margins
        /// </summary>
        [Test]//passes
        public void Test_SemesterDropDownGaps_Valid()
        {
            // Navigate to the page
            Thread.Sleep(TEST_WAIT);
            driver.Url = "http://localhost:5000/ScheduleCreate";
            Thread.Sleep(TEST_WAIT);

            //Wait for the semester dropdown to become available
            wait.Until(ExpectedConditions.ElementExists(By.TagName("select")));
            var sem = driver.FindElement(By.TagName("select"));
            Thread.Sleep(TEST_WAIT);

            //checking bottom margins and paddings of the semester selection dropdown
            var padbot = int.Parse(sem.GetCssValue("padding-bottom").Substring(0, 2));
            var margbot = int.Parse(sem.GetCssValue("margin-bottom").Substring(0, 1));
            Assert.True(padbot + margbot == 12);

            Thread.Sleep(TEST_WAIT);
            //checking top margins and paddings of the semester selection dropdown
            var padtop = int.Parse(sem.GetCssValue("padding-top").Substring(0, 2));
            var margtop = int.Parse(sem.GetCssValue("margin-top").Substring(0, 1));
            Assert.True(padbot + margtop == 12);


        }

        #endregion


        #region Helper Methods
        /******** HELPER METHODS ********/


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


        #endregion





    }
}
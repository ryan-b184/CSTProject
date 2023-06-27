using NUnit.Framework;
using System.Collections.Generic;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Context;
using CSTSchedule.UITests.fixtures;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;
using System.Collections.ObjectModel;

namespace CSTScheduling.UITests
{
    public class ScheduleEdit
    {
        /// <summary>
        /// Fixture, WebDriver, WaitDriver, dbContext, etc.
        /// </summary>
        #region Global dbData and Drivers
        EdgeDriver driver;
        WebDriverWait wait;
        public fixtures fix;
        // Length of timeout, changed in fixtures
        public const int TEST_WAIT = fixtures.TEST_WAIT;
        CstScheduleDbContext testContext;
        List<Semester> semList;
        List<Instructor> insList;
        List<Room> roomList;
        List<Course> courseList;
        List<CISR> cisrList;
        private const string semesterSelectBox = "";
        private const string courseSelectBox = "";
        #endregion


        /// <summary>
        /// Used for all UI tests to select IWebElements.
        /// </summary>
        #region Global IWebElement Selectors
        // Monday (8:00AM) Elements
        string Monday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";
        //string Monday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(7) > div > div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";

        string Monday8Room_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(2) > input";
        string Monday8Ins1_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div >div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > input";
        string Monday8Ins2_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(4) > input";
        string Monday8Copy_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(2) > span";
        string Monday8Delete_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div >div > table > tbody > tr:nth-child(1) > td:nth-child(2) > div > div > form > div:nth-child(1) > span";
        // Monday (10:00AM) Elements
        string Monday10_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(3) > td:nth-child(2) > div > div";
        string Monday10Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(3) > td:nth-child(2) > div > div > form > div:nth-child(1) > input";
        string Monday10Ins2_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(3) > td:nth-child(2) > div > div > form > div:nth-child(4) > input";
        // Tuesday (8:00AM) Elements
        string Tuesday8_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(3) > div > div";
        string Tuesday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(3) > div > div > form > div:nth-child(1) > input";
        string Tuesday8Ins1_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(3) > div > div > form > input";
        string Tuesday8Room_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(3) > div > div > form > div:nth-child(2) > input";
        // Wednesday (8:00AM) elements
        string Wednesday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div >table > tbody > tr:nth-child(1) > td:nth-child(4) > div > div > form > div:nth-child(1) > input";

        // Friday (8:00AM) Elements
        string Friday8Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(1) > td:nth-child(6) > div > div > form > div:nth-child(1) > input";
        string Friday9Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(2) > td:nth-child(6) > div > div > form > div:nth-child(1) > input";
        string Friday10Course_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > div > div > div > table > tbody > tr:nth-child(3) > td:nth-child(6) > div > div > form > div:nth-child(1) > input";
        // Modal
        string Modal_selector = "#mErrMsg";
        string ModalBody_selector = "#mErrMsg ul > li";
        string Allow_selector = "#btnAllow";
        string Deny_selector = "#btnDeny";
        string Breakpoint1_1A_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul > li:nth-child(1) > button";
        string Breakpoint3_1A_selector = "body > div.page > div.main > div.content.px-4 > div:nth-child(3) > ul > li:nth-child(3) > button";
        #endregion

        /// <summary>
        /// The setup specifies the path to the db we are loading our fixture data into.
        /// Instantiates a new DB context to interact with the database.
        /// Finally, launches a full-screen Microsoft Edge driver in headless mode, without cookies.
        /// </summary>
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
            // Fetch from the database
            semList = testContext.Semester.ToList();
            insList = testContext.Instructor.ToList();
            roomList = testContext.Room.ToList();
            courseList = testContext.Course.ToList();
            cisrList = testContext.CISR.ToList();


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
        }


        /// <summary>
        /// Teardown closes the Microsoft Edge driver.
        /// </summary>
        [OneTimeTearDown]
        public void Teardown()
        {
            driver.Close();
            testContext.Dispose();
        }

        #region Schedule Edit Tests

        /// <summary>
        /// This test is performed entirely on the cell for Monday @ 8:00AM.
        /// 
        /// Checks that the user is able to effectively move the cell data from
        /// one time-block to another using the copy/delete buttons; in this case
        /// it is from 8:00 AM to 10:00 AM.
        /// </summary>
        [Test]
        public void Test_ChangeTime()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Enter "[COSC292] Java" into Monday8AM course field
                InputWhen("[COSC292] COSC", Monday8Course_selector);

                // Click COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();

                // Paste (single-click) into cell Monday10AM
                GetWhen("exists", Monday10_selector).Click();

                // Deselect COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();

                // Delete (click X button) on cell Monday8AM 
                GetWhen("visible", Monday8Delete_selector).Click();

                // Assert that Tuesday8AM input now contains the course
                Assert.AreEqual("[COSC292] COSC", GetWhen("exists", Monday10Course_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test is performed on the cells for Monday @ 8:00AM,
        /// and Tuesday @ 8:00 AM.
        /// 
        /// Checks that the user is able to effectively move the cell data from
        /// a time-block on one day, to a time-block on another day using the
        /// copy/delete buttons; in this case it's from Monday 8AM to Tuesday 8AM.
        /// </summary>
        [Test]
        public void Test_ChangeDay()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Enter "[COSC292] Java" into Monday8AM course field
                InputWhen("[COSC292] COSC", Monday8Course_selector);

                // Click COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();

                // Paste (single-click) into cell Tuesday_8AM
                GetWhen("exists", Tuesday8_selector).Click();

                // Deselect COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();

                // Delete (click X button) on cell Monday8AM 
                GetWhen("visible", Monday8Delete_selector).Click();

                // Assert that Tuesday8AM input now contains the course
                Assert.AreEqual("[COSC292] COSC", GetWhen("exists", Tuesday8Course_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test is performed entirely on the cell for Monday @ 8:00AM.
        /// 
        /// Checks that a default room value loads for "[COSC292] COSC",
        /// then checks that changing the room to "[Main] 430" creates a
        /// persistent change.
        /// </summary>
        [Test]
        public void Test_ChangeRoom()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Assert that entering "[COSC292] COSC" loads room "[Main] 230a"
                InputWhen("[COSC292] COSC", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 230a", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));

                // Assert that modifying to "[Main] 430" creates a persistent change
                InputWhen("[Main] 430", Monday8Room_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 430", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test is performed entirely on the cell for Monday @ 8:00AM.
        /// 
        /// Checks that a default primary instructor loads for "[COSC292] COSC",
        /// then checks that changing it to "Barrie, Bryce" creates a
        /// persistent change.
        /// </summary>
        [Test]
        public void Test_ChangeInstructor()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Assert that entering "[COSC292] COSC" loads primary instructor "Basoalto, Ernesto"
                InputWhen("[COSC292] COSC", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("Basoalto, Ernesto", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));

                // Assert that modifying to "Barrie, Bryce" creates a persistent change
                InputWhen("Barrie, Bryce", Monday8Ins1_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("Barrie, Bryce", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test is performed entirely on the cell for Monday @ 8:00AM.
        /// 
        /// Checks that a default secondary instructor loads for "[secInsCourseA] secInsCourseA",
        /// then checks that changing it to "Barrie, Bryce" creates a persistent change.
        /// </summary>
        [Test]
        public void Test_ChangeSecondaryInstructor()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Assert that entering "[secInsCourseA] secInsCourseA" loads secondary instructor "New, Ron"
                InputWhen("[secInsCourseA] secInsCourseA", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("New, Ron", GetWhen("exists", Monday8Ins2_selector).GetAttribute("value"));

                // Assert that modifying to "Barrie, Bryce" creates a persistent change
                InputWhen("Barrie, Bryce", Monday8Ins2_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("Barrie, Bryce", GetWhen("exists", Monday8Ins2_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test is performed entirely on the cell for Monday @ 8:00AM.
        /// 
        /// Checks that when a user types a new course for a cell, that course
        /// and its defaults will load, and overwrite the previous cell data.
        /// </summary>
        [Test]
        public void Test_ChangeCourse()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Assert that entering "[COSC292] Java" loads that course's default room and primary instructor.
                InputWhen("[COSC292] COSC", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 230a", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));
                Assert.AreEqual("Basoalto, Ernesto", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));

                // Assert that changing to "[startLateCourse] startLateCourse" loads that course's default room and primary instructor.
                InputWhen("[endEarlyCourse] endEarlyCourse", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 239B", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));
                Assert.AreEqual("Barrie, Bryce", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test is performed entirely on the cell for Monday @ 8:00AM.
        /// 
        /// Checks that all Instructors and Rooms from the system are available
        /// in the datalists that appear below their respective input boxes.
        /// </summary>
        [Test]
        public void Test_ShowAllInsAndRooms()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Enter "[COSC292] Java" into the Course field
                InputWhen("[COSC292] COSC", Monday8Course_selector);

                // Get the Room datalist and count it.
                var RoomData_list = GetWhen("exists", "#roomDataList");
                int RoomData_count = RoomData_list.FindElements(By.CssSelector("option")).Count;

                // Get the Instructor datalist and count it.
                var InsData_list = GetWhen("exists", "#instructorDataList");
                int InsData_count = InsData_list.FindElements(By.CssSelector("option")).Count;

                // Get the fixture's Rooms and Instructors
                List<Room> RoomData_fix = fix.makeRoom();
                List<Instructor> InsData_fix = fix.makeIns();

                // Assert list counts are matching the database
                Assert.AreEqual(InsData_count, InsData_fix.Count);
                Assert.AreEqual(RoomData_count, RoomData_fix.Count);
            }
            finally
            {
                fix.Reload(testContext);
            }
        }


        /// <summary>
        /// This test checks that all <th> headers displaying the day of week
        /// are centered within the span of each column, as requested by the
        /// client.
        /// </summary>
        [Test]
        public void Test_ScheduleDaysCentered()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Find the <thead> style, since it applies to the <th> children.
                var thead = driver.FindElement(By.CssSelector("thead"));

                // Assert that "text-center" is being applied.
                Assert.AreEqual(thead.GetAttribute("style"), "text-align: center;");
            }
            finally
            {
                fix.Reload(testContext);
            }

        }
        #endregion


        #region Edited tests for story 13
        [Test]
        public void Test_ChangeTimeCheck()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Enter "[COSC292] Java" into Monday8AM course field
                InputWhen("[COSC292] COSC", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);

                // Click COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();
                Thread.Sleep(TEST_WAIT);

                // Paste (single-click) into cell Monday10AM
                GetWhen("exists", Monday10_selector).Click();
                Thread.Sleep(TEST_WAIT);

                // Deselect COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();
                Thread.Sleep(TEST_WAIT);

                // Delete (click X button) on cell Monday8AM 
                GetWhen("visible", Monday8Delete_selector).Click();
                Thread.Sleep(TEST_WAIT);

                // Assert that Monday at 10 input now contains the course
                Assert.AreEqual("[COSC292] COSC", GetWhen("exists", Monday10Course_selector).GetAttribute("value"));

                //navigate to another page
                Thread.Sleep(TEST_WAIT);
                driver.Url = "http://localhost:5000/semester";

                //naviagte back to the schedule create page
                Thread.Sleep(TEST_WAIT);
                this.SelectProgramAndSemester();
                Thread.Sleep(TEST_WAIT);

                //assert that the COSC coruse still exists in the monday10 slot
                Assert.AreEqual("[COSC292] COSC", GetWhen("exists", Monday10Course_selector).GetAttribute("value"));

            }
            finally
            {
                fix.Reload(testContext);
            }
        }

        [Test]
        public void Test_ChangeDayCheck()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Enter "[COSC292] Java" into Monday8AM course field
                InputWhen("[COSC292] COSC", Monday8Course_selector);

                // Click COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();

                // Paste (single-click) into cell Tuesday_8AM
                GetWhen("exists", Tuesday8_selector).Click();

                // Deselect COPY on Monday8AM
                GetWhen("visible", Monday8Copy_selector).Click();

                // Delete (click X button) on cell Monday8AM 
                GetWhen("visible", Monday8Delete_selector).Click();

                // Assert that Tuesday8AM input now contains the course
                Assert.AreEqual("[COSC292] COSC", GetWhen("exists", Tuesday8Course_selector).GetAttribute("value"));

                //navigate to another page
                Thread.Sleep(TEST_WAIT);
                driver.Url = "http://localhost:5000/semester";

                //naviagte back to the schedule create page
                Thread.Sleep(TEST_WAIT);
                this.SelectProgramAndSemester();
                Thread.Sleep(TEST_WAIT);

                // Assert that Tuesday8AM input still contains the course
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[COSC292] COSC", GetWhen("exists", Tuesday8Course_selector).GetAttribute("value"));

                Assert.AreEqual("", GetWhen("exists", Monday8Course_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }

        [Test]
        public void Test_ChangeRoomCheck()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Assert that entering "[COSC292] COSC" loads room "[Main] 230a"
                Thread.Sleep(TEST_WAIT);
                InputWhen("[COSC292] COSC", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 230a", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));
                Thread.Sleep(TEST_WAIT);

                // Assert that modifying to "[Main] 430" creates a persistent change
                InputWhen("[Main] 430", Monday8Room_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 430", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));

                Thread.Sleep(TEST_WAIT);
                driver.Url = "http://localhost:5000/semester";

                //naviagte back to the schedule create page
                Thread.Sleep(TEST_WAIT);
                this.SelectProgramAndSemester();
                Thread.Sleep(TEST_WAIT);

                // Assert that the saved room carries over
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("[Main] 430", GetWhen("exists", Monday8Room_selector).GetAttribute("value"));
            }
            finally
            {
                fix.Reload(testContext);
            }
        }
        [Test]
        public void Test_ChangeInstructorCheck()
        {
            try
            {
                // To begin, select "CST" on the Programs page, then "Year 1: Semester 1A" on the ScheduleCreate page.
                this.SelectProgramAndSemester();

                // Assert that entering "[COSC292] COSC" loads primary instructor "Basoalto, Ernesto"
                InputWhen("[COSC292] COSC", Monday8Course_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("Basoalto, Ernesto", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));

                // Assert that modifying to "Barrie, Bryce" creates a persistent change
                InputWhen("Barrie, Bryce", Monday8Ins1_selector);
                Thread.Sleep(TEST_WAIT);
                Assert.AreEqual("Barrie, Bryce", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));

                Thread.Sleep(TEST_WAIT);
                driver.Url = "http://localhost:5000/semester";

                //naviagte back to the schedule create page
                Thread.Sleep(TEST_WAIT);
                this.SelectProgramAndSemester();
                Thread.Sleep(TEST_WAIT);

                Assert.AreEqual("Barrie, Bryce", GetWhen("exists", Monday8Ins1_selector).GetAttribute("value"));

            }
            finally
            {
                fix.Reload(testContext);
            }
        }
        #endregion


        #region ErrorTests
        [Test]
        public void Test_PrimaryInstructorErrorThrowing()
        {
            fix.Reload(testContext);

            // Select "CST" as the Program then render ScheduleCreate page
            this.SelectProgram();

            // Ensure modal doesn't already exist on page
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(".modal-open")).Count);

            #region PrimaryToPrimaryErrorThrowing
            // Select Semester: "Year 1, Sem 1, Group A" (the first student group)
            this.SelectSemester(1);

            // Enter "fulllengthcourseA" with primary instructor 'a' from 8-9 on monday
            InputWhen("[fullLengthCourseA] fullLengthCourseA", Monday8Course_selector);

            // nav to second student group
            this.SelectSemester(2);

            // select fulllengthcourseb with primary instructor 'a' from 8-9 on monday
            InputWhen("[fullLengthCourseB] fullLengthCourseB", Monday8Course_selector);

            // assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);

            // test the text content of element
            Assert.AreEqual("Barrie, B already teaches fullLengthCourseA from Wednesday, September 01 to Thursday, December 30",
                    GetWhen("visible", ModalBody_selector).Text);


            // ----- Test_AbleToDenyPrimaryInstructorConflict
            // Ensure buttons exist
            Assert.AreEqual("Allow", driver.FindElement(By.CssSelector(Allow_selector)).Text);
            Assert.AreEqual("Deny", driver.FindElement(By.CssSelector(Deny_selector)).Text);
            // Select to deny above conflict
            GetWhen("clickable", Deny_selector).Click();
            // Assert that no update occured

            zzZ(200);
            Assert.AreEqual("", driver.FindElement(By.CssSelector(Monday8Course_selector)).GetAttribute("value"));

            // ----- Test_AbleToAllowPrimaryInstructorConflict
            // Trigger error again but allow it...
            // Select fulllengthcourseb with primary instructor 'a' from 8-9 on monday
            InputWhen("[fullLengthCourseB] fullLengthCourseB", Monday8Course_selector);
            GetWhen("clickable", Allow_selector).Click();

            // Ensure primary instructor displays in the cell
            zzZ(200);
            Assert.AreEqual("Barrie, Bryce", driver.FindElement(By.CssSelector(Monday8Ins1_selector)).GetAttribute("value"));

            // ----- Test_PrimaryToPrimaryInstructorsBreakpointedWillNotDisplayWarningIfNotOverlapping
            // NOTE: NEED TO ADJUST FOR THE ACTUAL BREAKPOINTS WHEN IMPLEMENTED
            // Nav back to first student group
            this.SelectSemester(1);
            // Select endearlycourse with primary instructor 'a' from 8-9 on tuesday
            InputWhen("[endEarlyCourse] endEarlyCourse", Tuesday8Course_selector);
            // Select the third breakpoint for Semester 1A
            GetWhen("clickable", Breakpoint3_1A_selector).Click();
            // Select startlatecourse with primary instructor 'a' from 8-9 on tuesday
            InputWhen("[startLateCourse] startLateCourse", Tuesday8Course_selector);
            // If no error is thrown, primary instructor should now appear on schedule

            zzZ(200);
            Assert.AreEqual("Barrie, Bryce", GetWhen("visible", Tuesday8Ins1_selector).GetAttribute("value"));
            #endregion


            #region SecondaryToPrimaryErrorThrowing
            // ----- Test_SecondaryToPrimaryInstructorConflictsWillDisplayWarning
            // nav to second student group, primary ins in group a already primed

            this.SelectSemester(1);
            InputWhen("[fullLengthCourseA] fullLengthCourseA", Monday10Course_selector);

            this.SelectSemester(2);
            InputWhen("[COSC295] COSC", Monday10Course_selector);

            // assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);

            // test the text content of element
            Assert.AreEqual("Barrie, B already teaches fullLengthCourseA from Wednesday, September 01 to Thursday, December 30",
                    GetWhen("visible", ModalBody_selector).Text);

            // select to deny above conflict
            GetWhen("clickable", Deny_selector).Click();

            // assert that no update occured
            zzZ(200);
            Assert.AreEqual("", driver.FindElement(By.CssSelector(Monday10Course_selector)).GetAttribute("value"));

            // fire conflict again
            InputWhen("[COSC295] COSC", Monday10Course_selector);

            // select allow
            GetWhen("clickable", Allow_selector).Click();

            // check that update occured on card
            zzZ(200);
            Assert.AreEqual("[COSC295] COSC", driver.FindElement(By.CssSelector(Monday10Course_selector)).GetAttribute("value"));
            #endregion
        }

        [Test]
        public void Test_SecondaryInstructorErrorThrowing()
        {
            fix.Reload(testContext);

            this.SelectProgram();
            // Ensure modal doesn't already exist on page
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(".modal-open")).Count);

            #region PrimaryToSecondaryErrorThrowing
            // ----- Test_PrimaryToSecondaryInstructorConflictsWillDisplayWarning
            // nav to first student group
            this.SelectSemester(1);

            // select secInsCourseA with secondary instructor 'a' from 8-9 on monday
            InputWhen("secInsCourseA", Monday8Course_selector);

            // nav to second student group
            this.SelectSemester(2);

            //select secInsCourseB with primary instructor 'a' from 8-9 on monday
            InputWhen("secInsCourseB", Monday8Course_selector);

            // assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);

            // test the text content of element
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(".mErrMsg")).Count);
            Assert.AreEqual("New, R already teaches secInsCourseA from Wednesday, September 01 to Thursday, December 30",
                    GetWhen("visible", ModalBody_selector).Text);

            // select to deny above conflict
            GetWhen("clickable", Deny_selector).Click();
            // assert that no update occured
            zzZ(200);
            Assert.AreEqual("", GetWhen("visible", Monday8Course_selector).GetAttribute("value"));

            // fire conflict again but allow it...
            InputWhen("secInsCourseB", Monday8Course_selector);
            GetWhen("clickable", Allow_selector).Click();
            // Ensure secondary instructor now displays in the cell
            zzZ(200);
            Assert.AreEqual("New, Ron", GetWhen("visible", Monday8Ins1_selector).GetAttribute("value"));
            #endregion


            #region SecondaryToSecondaryErrorThrowing
            //nav to first student group
            this.SelectSemester(1);
            //select secInsCourseA with secondary instructor 'a' from 8-9 on monday
            InputWhen("secInsCourseA", Monday10Course_selector);

            //nav to 2nd student group
            this.SelectSemester(2);
            //select secInsCourseC with secondary instructor 'a' from 8-9 on monday
            InputWhen("secInsCourseC", Monday10Course_selector);


            // assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);

            // test the text content of element
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(".mErrMsg")).Count);
            Assert.AreEqual("New, R already teaches secInsCourseA from Wednesday, September 01 to Thursday, December 30",
                   GetWhen("visible", ModalBody_selector).Text);

            // select to deny above conflict
            GetWhen("clickable", Deny_selector).Click();
            // assert that no update occured
            zzZ(200);
            Assert.AreEqual("", GetWhen("visible", Monday10Course_selector).GetAttribute("value"));

            // fire conflict again but allow it...
            InputWhen("secInsCourseC", Monday10Course_selector);
            GetWhen("clickable", Allow_selector).Click();
            // Ensure secondary instructor now displays in the cell
            zzZ(200);
            Assert.AreEqual("New, Ron", GetWhen("visible", Monday10Ins2_selector).GetAttribute("value"));
            #endregion    

        }

        [Test]
        public void Test_RoomAndMiscErrorThrowing()
        {
            fix.Reload(testContext);
            this.SelectProgram();

            // Ensure modal doesn't already exist on page
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(".modal-open")).Count);

            #region RoomConflictErrorThrowing
            // ----- Test_RoomConflictsWillDisplayWarning
            // nav to first student group
            this.SelectSemester(1);
            // select roomTestCourseA with room 1 from 8-9 on monday
            InputWhen("roomTestCourseA", Monday8Course_selector);
            // nav to second student group
            this.SelectSemester(2);
            // select roomTestCourseB with room 1 from 8-9 on monday
            InputWhen("roomTestCourseB", Monday8Course_selector);

            // assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);

            // test the text content of element
            Assert.AreEqual("230a scheduled with roomTestCourseA from Wednesday, September 01 to Thursday, December 30",
                    GetWhen("visible", ModalBody_selector).Text);

            // select to deny above conflict
            GetWhen("clickable", Deny_selector).Click();
            zzZ(200);

            // assert that no update occured
            Assert.AreEqual("", GetWhen("visible", Monday8Course_selector).GetAttribute("value"));

            // fire conflict again but allow it ...
            InputWhen("roomTestCourseB", Monday8Course_selector);
            GetWhen("clickable", Allow_selector).Click();
            zzZ(200);

            // Assert the conflicted room now displays in the cell
            Assert.AreEqual("[Main] 230a", GetWhen("visible", Monday8Room_selector).GetAttribute("value"));

            // ----- Test_RoomsBreakpointedWillNotDisplayWarningIfNotOverlapping
            // nav back to first student group
            this.SelectSemester(1);
            // select endearlycourse with room 1 from 8-9 on tuesday
            InputWhen("endEarlyCourse", Tuesday8Course_selector);
            Assert.AreEqual("[Main] 239B", GetWhen("visible", Tuesday8Room_selector).GetAttribute("value"));

            // select startlatecourse with room 1 from 8-9 on tuesday
            GetWhen("clickable", Breakpoint3_1A_selector).Click();
            zzZ(200);
            InputWhen("startLateCourse", Tuesday8Course_selector);

            // no error is thrown, should appear on schedule
            Assert.AreEqual("[Main] 239B", GetWhen("visible", Tuesday8Room_selector).GetAttribute("value"));
            Assert.AreEqual(0, driver.FindElements(By.CssSelector(".modal-open")).Count);
            #endregion

            #region CourseErrorThrowing
            // ---- Test_SchedulingACourseForMoreThanTheAllotedHoursDisplaysError
            // nav to first student group
            this.SelectSemester(1);
            InputWhen("twoHourCourse", Monday10Course_selector);
            //add twohourcourse with room 1 from 8-9 on friday
            InputWhen("twoHourCourse", Friday8Course_selector);
            //add twohourcourse with room 1 from 9-10 on friday
            InputWhen("twoHourCourse", Friday9Course_selector);

            //assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);
            // test the text content of element
            Assert.AreEqual("twoHourCourse overbooking, 3 / 2 hours", GetWhen("visible", ModalBody_selector).Text);
            GetWhen("clickable", Allow_selector).Click();
            zzZ(200);

            //add twohourcourse with room 1 from 10-11 on friday
            InputWhen("twoHourCourse", Friday10Course_selector);
            zzZ(200);
            Assert.AreEqual("twoHourCourse overbooking, 4 / 2 hours", GetWhen("visible", ModalBody_selector).Text);
            GetWhen("clickable", Deny_selector).Click();
            zzZ(200);

            // ------ Test_AbleToDenyBasedOnCourseExceedingMaxHours
            // assert nothing changed
            Assert.AreEqual("", GetWhen("visible", Friday10Course_selector).GetAttribute("value"));
            #endregion

            #region MiscTests
            // ----- Test_MultipleConflictsAppearInSingleMessage
            // nav to first student group
            this.SelectSemester(1);
            // add manyConflictCourseA with ins 'a' and room 1 from 8-9 on wednesday
            InputWhen("manyConflictCourseA", Wednesday8Course_selector);
            // nav to second student group
            this.SelectSemester(2);
            // add manyConflictCourseB with ins 'a' and room 1 from 8-9 on wednesday
            InputWhen("manyConflictCourseB", Wednesday8Course_selector);

            //assert error should throw here
            Assert.IsTrue(GetWhen("exists", Modal_selector).Displayed);
            Assert.AreEqual(1, GetMultipleWhen("visible", Modal_selector).Count);

            // assert both ins 'a', b  and 'room 1' message appeared
            Assert.AreEqual(3, GetMultipleWhen("visible", ModalBody_selector).Count);
            GetWhen("clickable", Deny_selector).Click();
            zzZ(200);
            #endregion
        }
        #endregion


        #region HELPER METHODS

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

            // Render ScheduleCreate page
            driver.Url = "http://localhost:5000/ScheduleCreate";
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            Thread.Sleep(TEST_WAIT);
            // Select the Semester "Year 1, Sem 1, Group A", and then wait.
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("select")));
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
            semester_dropdown.SelectByIndex(1); // Select child <option> by index
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
        /// 
        /// Takes in the index of the Semester List item to select.
        /// 
        /// </summary>
        private void SelectProgram()
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

            // Render ScheduleCreate page
            driver.Url = "http://localhost:5000/ScheduleCreate";
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            Thread.Sleep(TEST_WAIT);
        }

        private void SelectSemester(int index)
        {
            Thread.Sleep(TEST_WAIT);
            // Select the Semester "Year 1, Sem 1, Group A", and then wait.
            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("select")));
            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
            semester_dropdown.SelectByIndex(index); // Select child <option> by index
        }

        private void zzZ(int i)
        {
            Thread.Sleep(i);
        }
        #endregion

    }
}
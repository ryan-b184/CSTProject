//using CSTScheduling.Data.Context;
//using CSTScheduling.Data.Models;
//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Edge;
//using OpenQA.Selenium.Support.UI;
//using SeleniumExtras.WaitHelpers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading;

//namespace CSTSchedule.UITests.PageTests
//{
//    public class ProgramPageDeleteTests
//    {
//        EdgeDriver driver;
//        public CSTSchedule.UITests.fixtures.fixtureDatePoint fix;
//        CstScheduleDbContext testContext;
//        public const int TEST_WAIT = CSTSchedule.UITests.fixtures.fixtures.TEST_WAIT;
//        WebDriverWait wait;

//        [OneTimeSetUp]
//        public void Setup()
//        {


//            // Set db source
//            string dbDirectory = Directory.GetCurrentDirectory();
//            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".UITests") - 1) + "ing" + @"\CstScheduleDb.db";
//            var contextOpts = new DbContextOptionsBuilder<CstScheduleDbContext>()
//             .UseSqlite("Data Source=" + dbDirectory)
//             .Options;

//            // Create a new database context
//            testContext = new CstScheduleDbContext(contextOpts);
//            // Wipe the database between tests
//            testContext.Database.EnsureDeleted();
//            // Ensure new database is made
//            testContext.Database.EnsureCreated();
//            // Make MSEdge driver with disabled cookies
//            EdgeOptions options = new EdgeOptions();
//            options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);
//            options.AddArgument("-inprivate");
//            options.AddArgument("headless");
//            options.AddArgument("disable-gpu");
//            driver = new EdgeDriver(options);
//            // Make Wait driver
//            wait = new WebDriverWait(driver, new System.TimeSpan(0, 0, 15));
//            // Maximize window
//            driver.Manage().Window.Maximize();

//            // Populate db with test data
//            fix = new CSTSchedule.UITests.fixtures.fixtureDatePoint();
//            fix.Load(testContext); //loads all test data into the test database

//        }


//        [OneTimeTearDown]
//        public void Teardown()
//        {
//            driver.Close(); //close the driver after all tests are done 
//        }

//        [Test]
//        public void Test_ProgramConfirmation()
//        {
//            //selector variables
//            string departmentBtnDelete = ""; // to bring up delete modal
//            string confirmationModalInputField = "";
//            string deleteConfirmBtnSelector = "";

//            #region Text Entered Invalid
//            //navigate to the depart page
//            loadPage("http://localhost:5000/");

//            //click delete button
//            getWebElement(departmentBtnDelete).Click();

//            //grab the input field
//            var inputField = getWebElement(confirmationModalInputField);
//            // Assert that the modal is visible
//            Assert.IsTrue(inputField.Displayed);
//            // enter valid text
//            inputField.SendKeys("CST");
//            //grab the delete confirm button
//            var deleteConfirmBtn = getWebElement(deleteConfirmBtnSelector);

//            //click the confirm delete button
//            deleteConfirmBtn.Click();
//            Thread.Sleep(TEST_WAIT);

//            //var firstIndexDepartment = getWebElement(firstIndexDepartmentSelector);
//            //// assert that the index where CST department was, is changed
//            //Assert.IsTrue(firstIndexDepartment.GetAttribute("value") != "CST");

//            //Wait until the Progrma page displays a table
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("table")));
//            var table = driver.FindElement(By.TagName("table"));
//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
//            var trList = table.FindElements(By.TagName("tr"));

//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));

//            Thread.Sleep(TEST_WAIT);


//            //Check to ensure that each cell is the appropraite size in PX
//            foreach (var trs in trList)
//            {//loops through each table row
//                foreach (var td in trs.FindElements(By.TagName("td")))
//                {   //loops through each table cell
//                    Assert.IsTrue(td.GetAttribute("value") != "CST");
//                }
//            }

//            #endregion
//        }
//        [Test]
//        public async void Test_EditProgramSemester()
//        {
//            //selector variables
//            string departmentBtnEdit = ""; // to bring up delete modal
//            string semesterCountSelector = "";
//            string updateBtnSelector = "";
//            string confirmationModalInputField = "";
//            string deleteConfirmBtnSelector = "";
//            string semesterPageDropdownSelector = "";
//            string schedulePageDropdownSelector = "";

//            #region edit department semester count
//            //navigate to the depart page
//            loadPage("http://localhost:5000/");

//            //click delete button
//            getWebElement(departmentBtnEdit).Click();
//            // change the semester count from 3 to 2
//            getWebElement(semesterCountSelector).SendKeys("2");

//            getWebElement(updateBtnSelector).Click();

//            var inputfield = getWebElement(confirmationModalInputField);
//            // assert that the confirmation modal is visible
//            Assert.IsTrue(inputfield.Displayed);

//            // enter valid text
//            inputfield.SendKeys("CST");
//            Thread.Sleep(TEST_WAIT);
//            //grab the delete confirm button
//            var deleteConfirmBtn = getWebElement(deleteConfirmBtnSelector);

//            //click the confirm delete button
//            deleteConfirmBtn.Click();
//            Thread.Sleep(TEST_WAIT);

//            #endregion

//            #region semester page dropdown
//            // navigate to semester page
//            loadPage("http://localhost:5000/semester");

//            // click semester dropdown
//            var semesterDropdownList = getWebElement(semesterPageDropdownSelector);
//            semesterDropdownList.Click();
//            Thread.Sleep(TEST_WAIT);
//            // count the semesters in the dropdown
//            int semesterCount = semesterDropdownList.FindElements(By.CssSelector("option")).Count();

//            // get the semester list of this department from the database to compare
//            List<Semester> semesterList = await testContext.Semester.Where(x => x.deptID == 1).ToListAsync();
//            // assert that the count of options are the same as the count of semester in the database
//            Assert.AreEqual(semesterCount, semesterList.Count());

//            #endregion

//            #region schedule page dropdown
//            // navigate to schedule page
//            loadPage("http://localhost:5000/ScheduleCreate");

//            var scheduleDropdown = getWebElement(schedulePageDropdownSelector);
//            scheduleDropdown.Click();
//            Thread.Sleep(TEST_WAIT);

//            // count the semesters in the dropdown at the schedule page
//            semesterCount = scheduleDropdown.FindElements(By.CssSelector("option")).Count();

//            semesterList = await testContext.Semester.Where(x => x.deptID == 1).ToListAsync();
//            // assert that the count of options are the same as the count of semester in the database
//            Assert.AreEqual(semesterCount, semesterList.Count());
//            #endregion
//        }


//        #region Helper Method
//        public void loadPage(string url)
//        {
//            Thread.Sleep(TEST_WAIT);
//            driver.Url = url;
//            Thread.Sleep(TEST_WAIT);
//        }

//        public IWebElement getWebElement(string selector)
//        {
//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
//            var element = driver.FindElement(By.CssSelector(selector));
//            Thread.Sleep(TEST_WAIT);

//            return element;
//        }

//        #endregion
//    }
//}

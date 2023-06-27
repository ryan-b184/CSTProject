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
//    class DeleteTests
//    {


//        EdgeDriver driver;
//        public CSTSchedule.UITests.fixtures.fixtureDatePoint fix;
//        CstScheduleDbContext testContext;
//        public const int TEST_WAIT = CSTSchedule.UITests.fixtures.fixtures.TEST_WAIT;
//        WebDriverWait wait;
//        List<Semester> semList;
//        List<Instructor> insList;
//        List<Room> roomList;
//        List<Course> courseList;
//        List<CISR> cisrList;

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

//            // Fetch from the database
//            semList = testContext.Semester.ToList();
//            insList = testContext.Instructor.ToList();
//            roomList = testContext.Room.ToList();
//            courseList = testContext.Course.ToList();
//            cisrList = testContext.CISR.ToList();

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
//            string errorMessage = "";
//            string firstIndexDepartmentSelector = "";

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
//        [Test]
//        public void Test_RoomConfirmation()
//        {
//            string roomDeletebtnSelector = "";
//            string deleteConfirmBtnSelector = "";
//            string monday8CourseSelector = "";
//            string monday8RoomSelectory = "";
//            #region delete room
//            loadPage("http://localhost:5000/room");

//            // click the delete button 
//            getWebElement(roomDeletebtnSelector).Click();
//            Thread.Sleep(TEST_WAIT);

//            var inputField = getWebElement(deleteConfirmBtnSelector);
//            // assert that the modal is visible
//            Assert.IsTrue(inputField.Displayed);

//            // click ok button to confirm deletion
//            getWebElement(deleteConfirmBtnSelector).Click();
//            Thread.Sleep(TEST_WAIT);

//            #region loop through table to check for room 239B instructor page
//            //assert that the room 239B is no longer available
//            var table = driver.FindElement(By.TagName("table"));
//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
//            var trList = table.FindElements(By.TagName("tr"));

//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));

            
//            foreach (var tr in trList)
//            {//loops through each table row
//             //for each row check the room number and make sure it isn't room 239B
//                var room = tr.FindElement(By.CssSelector("td:nth-child(0)"));
//                Assert.IsTrue(room.Text != "239B");
//            }

//            #endregion

//            #endregion

//            #region check a CISR object with 239B room
//            //loadPage("http://localhost:5000/ScheduleCreate");
//            // navigate to schedule create page and select the first index of the semester dropdown
//            this.SelectProgramAndSemester();

//            //inputs the course "endEarlyCourse" into monday at 8 am which has a default room of 239B
//            InputWhen("[endEarlyCourse] endEarlyCourse", monday8CourseSelector);

//            //check the room of monday at 8 and see that the field is empty
//            Assert.IsTrue(getWebElement(monday8RoomSelectory).GetAttribute("value") == "");
//            #endregion
//        }
//        [Test]
//        public void Test_InstructorConformation()
//        {
//            string instructorDeleteBtnSelector = "";
//            string modalDeleteConfirmBtn = "";
//            string instructorSelector = "";
//            string monday8CourseSelector = "";
//            string monday8PrimarySelector = "";
//            string monday8SecondarySelector = "";

//            #region instructor page 
//            //load instructor page
//            loadPage("http://localhost:5000/instructor");

//            // click the delete button 
//            getWebElement(instructorDeleteBtnSelector).Click();
//            Thread.Sleep(TEST_WAIT);

           
//            // assert that the modal and btn is visible
//            Assert.IsTrue(getWebElement(modalDeleteConfirmBtn).Displayed);

//            // click ok button to confirm deletion
//            getWebElement(modalDeleteConfirmBtn).Click();
//            Thread.Sleep(TEST_WAIT);


//            #region loop through table to check for bryce instructor page
//                //assert that the instructor bryce is no longer available
//                var table = driver.FindElement(By.TagName("table"));
//                Thread.Sleep(TEST_WAIT);
//                wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
//                var trList = table.FindElements(By.TagName("tr"));

//                Thread.Sleep(TEST_WAIT);
//                wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));

//                foreach (var tr in trList)
//                {//loops through each table row
//                    //for each row check the first name of the instructor is not bryce
//                    var instructorFirstName = tr.FindElement(By.CssSelector("td:nth-child(1)"));
//                    Assert.IsTrue(instructorFirstName.Text != "Bryce");
//                }

//            #endregion

//            #endregion

//            #region check a CISR object with Bryce instructor primary no secondary
//            //loadPage("http://localhost:5000/ScheduleCreate");
//            // navigate to schedule create page and select the first index of the semester dropdown
//            this.SelectProgramAndSemester();

//            //inputs the course "endEarlyCourse" into monday at 8 am which has a default Instructor of Bryce Barrie
//            InputWhen("[endEarlyCourse] endEarlyCourse", monday8CourseSelector);

//            //check the room of monday at 8 and see that the primary instructor field is empty
//            Assert.IsTrue(getWebElement(monday8PrimarySelector).GetAttribute("Value") == "");
//            //check the room of monday at 8 and see that the secondary instructor field is empty
//            Assert.IsTrue(getWebElement(monday8SecondarySelector).GetAttribute("Value") == "");
//            #endregion

//            #region check a CISR object with Bryce instructor secondary 
//            //loadPage("http://localhost:5000/ScheduleCreate");
//            // navigate to schedule create page and select the first index of the semester dropdown
//            this.SelectProgramAndSemester();

//            //inputs the course "[secInsBusyCourse] secInsBusyCourse" into monday at 8 am which has a default secondary ins of bryce
//            InputWhen("[secInsBusyCourse] secInsBusyCourse", monday8CourseSelector);

//            //check the room of monday at 8 and see that the field is empty
//            Assert.IsTrue(getWebElement(monday8SecondarySelector).GetAttribute("Value") == "");
//            #endregion

//            #region check a CISR object with Bryce Primary and secondary 
//            //loadPage("http://localhost:5000/ScheduleCreate");
//            // navigate to schedule create page and select the first index of the semester dropdown
//            this.SelectProgramAndSemester();

//            //inputs the course "[fullLengthCourseB] fullLengthCourseB" into monday at 8 am which has a default room of 239B
//            InputWhen("[fullLengthCourseB] fullLengthCourseB", monday8CourseSelector);

//            //check the room of monday at 8 and see that the field is empty
//            Assert.IsTrue(getWebElement(monday8PrimarySelector).GetAttribute("Value") == "Ron new");
//            //check that the secondary isntructor slot is now empty
//            Assert.IsTrue(getWebElement(monday8SecondarySelector).GetAttribute("Value") == "");
//            #endregion



//        }
//        [Test]
//        public void Test_CourseConfirmation()
//        {
//            string courseDeleteBtn = "";
//            string courseConfirmDeleteBtn = "";
//            string secondPageSelector = "";
//            int currentPageNumber = 2;

//            string thirdDatePoint = "";

//            #region course page 
//            //load course page
//            loadPage("http://localhost:5000/Course");

//            //click second page button
//            getWebElement(secondPageSelector).Click();

//            // click the delete button 
//            getWebElement(courseDeleteBtn).Click();
//            Thread.Sleep(TEST_WAIT);


//            // assert that the modal and btn is visible
//            Assert.IsTrue(getWebElement(courseConfirmDeleteBtn).Displayed);

//            // click ok button to confirm deletion
//            getWebElement(courseConfirmDeleteBtn).Click();
//            Thread.Sleep(TEST_WAIT);

//            //check that page 2 is still has the class "active"
//            IWebElement secondPage = getWebElement(secondPageSelector);
//            Assert.IsTrue(secondPage.GetAttribute("class").Contains("active"));


//            #region loop through table to check for course COSC292
//            //assert that the instructor bryce is no longer available
//            var table = driver.FindElement(By.TagName("table"));
//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("tr")));
//            var trList = table.FindElements(By.TagName("tr"));

//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.TagName("td")));

           
//            foreach (var tr in trList)
//            {//loops through each table row
//             //for each row check the course abbr isn't startLateCourse
//                var courseAbbr = tr.FindElement(By.CssSelector("td:nth-child(1)"));
//                Assert.IsTrue(courseAbbr.Text != "startLateCourse");
//            }

//            #endregion



//            #endregion

//            #region ScheduleCreate no startLateCourse course available
//            //navigation to schedule create page
//            this.SelectProgramAndSemester();

//            //click third datepoint
//            getWebElement(thirdDatePoint).Click();

//            //get the list of options for courses
//            var courseDataList = GetWhen("exists", "#courseDataList");
//            var courseOptions = courseDataList.FindElements(By.CssSelector("option"));

//            foreach(var course in courseOptions)
//            {
//                Assert.IsTrue(course.GetAttribute("value") != "startLateCourse");
//            }
//            #endregion
//        }

//        [Test]
//        public void Test_incorrectConfirmationMessage()
//        {
//            //selector variables
//            string departmentBtnDelete = ""; // to bring up delete modal
//            string confirmationModalInputField = "";
//            string deleteConfirmBtnSelector = "";
//            string errorMessage = "";
//            string errorColor = "";


//            #region No Text entered
//            //navigate to the depart page
//            loadPage("http://localhost:5000/");

//            //click delete button
//            getWebElement(departmentBtnDelete).Click();

//            //grab the input field
//            var inputField = getWebElement(confirmationModalInputField);
//            //grab the delete confirm button
//            var deleteConfirmBtn = getWebElement(deleteConfirmBtnSelector);


//            //click the confirm delete button
//            deleteConfirmBtn.Click();
//            Thread.Sleep(TEST_WAIT);

//            //check the warning message and border color after incorrect entry
//            checkWarnings(errorMessage, confirmationModalInputField, "Please enter valid Program Name");
//            #endregion

//            #region Text Entered Invalid
//            //navigate to the depart page
//            loadPage("http://localhost:5000/");

//            //click delete button
//            getWebElement(departmentBtnDelete).Click();

//            //grab the input field
//            inputField = getWebElement(confirmationModalInputField);

//            // enter invalid text
//            inputField.SendKeys("CST123");
//            //grab the delete confirm button
//            deleteConfirmBtn = getWebElement(deleteConfirmBtnSelector);


//            //click the confirm delete button
//            deleteConfirmBtn.Click();
//            Thread.Sleep(TEST_WAIT);

//            //check the warning message and border color after incorrect entry
//            checkWarnings(errorMessage, confirmationModalInputField, "Please enter valid Program Name");
//            #endregion

//        }


//        /******** HELPER METHODS ********/


//        /// <summary>
//        /// This helper method clears the existing text in an input field
//        /// before patiently entering the given text string into the field.
//        /// </summary>
//        /// <param name="text">The text to be entered</param>
//        /// <param name="selector">The selector for the input field</param>
//        private void InputWhen(string text, string selector)
//        {
//            GetWhen("exists", selector).Clear();
//            Thread.Sleep(TEST_WAIT);
//            GetWhen("exists", selector).SendKeys(text + Keys.Tab);
//            Thread.Sleep(TEST_WAIT);
//        }
//        /// <summary>
//        /// This helper method waits until the given selector meets the given condition
//        /// before returning the requested IWebElement.
//        /// </summary>
//        /// <param name="condition">The criteria to wait for: "exists", "visible", "clickable", or a text string to wait for in the element.</param>
//        /// <param name="selector">The selector to find this IWebElement on the HTML page.</param>
//        /// <returns></returns>
//        private IWebElement GetWhen(string condition, string selector)
//        {
//            IWebElement element = null;
//            Thread.Sleep(TEST_WAIT);
//            switch (condition)
//            {
//                case "exists":
//                    wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
//                    element = driver.FindElement(By.CssSelector(selector));
//                    break;
//                case "visible":
//                    wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(selector)));
//                    element = driver.FindElement(By.CssSelector(selector));
//                    break;
//                case "clickable":
//                    wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(selector)));
//                    element = driver.FindElement(By.CssSelector(selector));
//                    break;
//                default:
//                    wait.Until(ExpectedConditions.TextToBePresentInElement(driver.FindElement(By.CssSelector(selector)), condition));
//                    element = driver.FindElement(By.CssSelector(selector));
//                    break;
//            }
//            Thread.Sleep(TEST_WAIT);
//            return element;
//        }

//        public void loadPage(string url)
//        {
//            Thread.Sleep(TEST_WAIT);
//            driver.Url = url;
//            Thread.Sleep(TEST_WAIT);
//        }

//        public void clickButton(string selector)
//        {
//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
//            var btn = driver.FindElement(By.CssSelector(selector));
//            btn.Click(); //click the delete button
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

//        public void checkWarnings(string messageSelector, string inputFieldSelector , string errMessage)
//        {
//            //check for error message and warning bootstrap border
//            Thread.Sleep(TEST_WAIT);
//            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(messageSelector)));
//            Assert.IsTrue(driver.FindElement(By.CssSelector(messageSelector)).Text == errMessage);

//            Thread.Sleep(TEST_WAIT);

//            //checks the border color to see if it is red now
//            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(inputFieldSelector)));
//            Assert.IsTrue(driver.FindElement(By.CssSelector(inputFieldSelector)).GetCssValue("border-color") == "background: rgba(232, 170, 0, 1)");
//            Thread.Sleep(TEST_WAIT);
//        }



//        /// <summary>
//        /// This procedure navigates to the main page, selects CST as the program,
//        /// then selects "Year 1, Semester 1, Group A" on the Schedule creation page.
//        /// </summary>
//        private void SelectProgramAndSemester()
//        {
//            Thread.Sleep(TEST_WAIT);
//            // Render Programs page
//            driver.Url = "http://localhost:5000/";
//            Thread.Sleep(TEST_WAIT);
//            // Select "CST" as the current department
//            string selector = "body > div.page > div.main > div.content.px-4 > div > table > tbody > tr:nth-child(1) > td:nth-child(6) > button";
//            wait.Until(ExpectedConditions.ElementExists(By.CssSelector(selector)));
//            var CST_button = driver.FindElement(By.CssSelector(selector));
//            if (CST_button.Enabled)
//            {
//                // Click if "CST" is not already selected
//                CST_button.Click();
//            }

//            // Render ScheduleCreate page
//            driver.Url = "http://localhost:5000/ScheduleCreate";
//            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
//            Thread.Sleep(TEST_WAIT);
//            // Select the Semester "Year 1, Sem 1, Group A", and then wait.
//            wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("select")));
//            SelectElement semester_dropdown = new SelectElement(driver.FindElement(By.TagName("select")));
//            semester_dropdown.SelectByIndex(1); // Select child <option> by index
//        }

//    }
//}

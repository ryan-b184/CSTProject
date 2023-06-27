using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Services;
using CSTScheduling.Pages;
using CSTScheduling.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSTScheduling.Test.UnitTests.Services
{
    class CourseServiceTests
    {
        public Department testDept;
        public Semester testSemester;
        public Instructor ins1;
        public Instructor ins2;
        public Room testRoom;
        public CstScheduleDbContext testContext;
        public List<Course> courseList;
        Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {

            String dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
             .UseSqlite("Data Source=" + dbDirectory)
             .Options;

            // Create a new database context
            testContext = new CstScheduleDbContext(options);
            // Wipe the database between tests
            testContext.Database.EnsureDeletedAsync();
            //make sure new database is made
            testContext.Database.EnsureCreatedAsync();

            ctx = new Bunit.TestContext();
            ctx.Services.AddDbContextFactory<CstScheduleDbContext>(opt =>
              opt.UseSqlite("Data Source=" + dbDirectory));
            ctx.Services.AddSingleton<CstScheduleDbService>();
            ctx.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            testDept = new Department
            {
                ID = 1,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3
            };

            testSemester = new Semester
            {
                SemesterID = testDept.ID + ",1,1,1",
                deptID = testDept.ID,
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 02),
                StartTime = 8,
                EndTime = 15,
                HasBreak = false,
                BreakStart = null,
                BreakEnd = null
            };
            testRoom = new Room
            {
                ID = 1,
                roomNumber = "123",
                city = "Saskatoon",
                campus = "SIAST",
                capacity = 25
            };
            ins1 = new Instructor
            {
                ID = 1,
                email = "TestIns1@test.com",
                lName = "Gibson",
                fName = "Eric",
                officeNum = "306-153-1234",
                phoneNum = "306-412-3556",
                note = "This is a test note"
            };
            ins2 = new Instructor
            {
                ID = 2,
                email = "TestIns2@test.com",
                lName = "Sierra",
                fName = "John",
                officeNum = "306-666-4444",
                phoneNum = "306-888-7777",
            };
            testContext.Department.Add(testDept);
            testContext.Semester.Add(testSemester);
            testContext.Room.Add(testRoom);
            testContext.Instructor.Add(ins1);
            testContext.Instructor.Add(ins2);
            testContext.SaveChanges();
        }

        [Test]
        public void test_listEmptywhenDbEmpty_Valid()
        {

            courseList = testContext.Course.ToList();

            // there should be no course in the database
            Assert.AreEqual(courseList.Count, 0);

            ctx.RenderComponent<MainLayout>();
            // render the course display page
            var cut = ctx.RenderComponent<CourseDisplay>();

            // there should be no courses to display
            var trlist = cut.FindAll("tbody tr");
            Assert.AreEqual(trlist.Count, 0);


            ctx.Dispose();
            ctx = null;
        }

        [Test]
        public void Test_ListOneWhenDbHasOne_Valid()
        {
            AddCoursesToDb(1);

            courseList = testContext.Course.ToList();
            // There should be exactly 1 course in the database
            Assert.AreEqual(1, courseList.Count);

            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            var trList = cut.FindAll("tbody tr");
            // The page should only return 1 record
            Assert.AreEqual(1, trList.Count);


            ctx.Dispose();
            ctx = null;
        }
        [Test]
        public void Test_ListTenWhenDbHasTen_Valid()
        {
            AddCoursesToDb(10);

            courseList = testContext.Course.ToList();
            // There should be exactly 10 course in the database
            Assert.AreEqual(courseList.Count, 10);

            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            var trList = cut.FindAll("tbody tr");
            // The page should only return 10 records
            Assert.AreEqual(10, trList.Count);


            ctx.Dispose();
            ctx = null;
        }

        [Test]
        public async Task Test_ListMax10CoursesPerPageWhenDbHas11_Valid()
        {

            List<Course> coursesToAdd = new List<Course>();

            for (int i = 0; i < 11; i++)
            {
                coursesToAdd.Add(
                    new Course
                    {
                        ID = i + 1,
                        courseName = "Course Test",
                        courseAbbr = "corse123",
                        startDate = testSemester.StartDate,
                        endDate = testSemester.EndDate,
                        hoursPerWeek = 5,
                        semesterID = testSemester.SemesterID,
                        creditUnits = 5,
                        classroomIDBindable = testRoom.ID,
                        primaryInstructorIDBindable = ins1.ID,
                        secondaryInstructorIDBindable = ins2.ID
                    }
                );
            }

            await testContext.Course.AddRangeAsync(coursesToAdd);
            await testContext.SaveChangesAsync();

            courseList = testContext.Course.ToList();
            // There should be exactly 11 course in the database
            Assert.AreEqual(courseList.Count, 11);

            // Render the Course Display Page
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            var trList = cut.FindAll("tbody tr");
            // The 1st page should only return 10 records
            Assert.AreEqual(10, trList.Count);

            var bNext = cut.Find("#next");
            // Click and go to the next page
            bNext.Click();

            trList = cut.FindAll("tbody tr");

            // The 2nd page should only return 1 record
            Assert.AreEqual(1, trList.Count);

            ctx.Dispose();
            ctx = null;
        }

        [Test]
        public void Test_HasFivePagesWith50Courses_Valid()
        {
            AddCoursesToDb(50);

            courseList = testContext.Course.ToList();
            // There should be exactly 50 course in the database
            Assert.AreEqual(courseList.Count, 50);

            // Render the Course Display Page
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            var trList = cut.FindAll("tbody tr");
            // The first page should only return 10 records
            Assert.AreEqual(10, trList.Count);

            for (int i = 0; i < 4; i++)
            {
                var bNext = cut.Find("#next");
                // Go to the next page as long as there is a next page
                bNext.Click();

                trList = cut.FindAll("tbody tr");
                // Every page should return 10 records
                Assert.AreEqual(10, trList.Count);
            }

            ctx.Dispose();
            ctx = null;
        }

        /// <summary>
        /// This test is to ensure that when 27 semesters are added that the items per page is 10, 10, then lastly 7.
        /// It checks this by navigating by the 'next' button, then backtracking with the 'back' button
        /// </summary>
        [Test]
        public void Test_27semestersBreaksPagesInto_10_10_7_Valid()
        {
            AddCoursesToDb(27);

            courseList = testContext.Course.ToList();
            // There should be exactly 50 course in the database
            Assert.AreEqual(courseList.Count, 27);

            // Render the Course Display Page
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            int trList;

            for (int i = 0; i < 2; i++)
            {
                trList = cut.FindAll("tbody tr").Count;
                // Every page should return 10 records
                Assert.AreEqual(10, trList);

                var bNext = cut.Find("#next");
                // Go to the next page as long as there is a next page
                bNext.Click();
            }

            //should be 7 on next page now
            trList = cut.FindAll("tbody tr").Count;
            Assert.AreEqual(7, trList);

            for (int i = 0; i < 2; i++)
            {
                var bNext = cut.Find("#back");
                // Go to the next page as long as there is a next page
                bNext.Click();

                trList = cut.FindAll("tbody tr").Count;
                // Every page should return 10 records
                Assert.AreEqual(10, trList);
            }

            ctx.Dispose();
            ctx = null;
        }

        /// <summary>
        /// This test ensures that semesters are returning in a sorted fashion. This is accomplished by inserting them in descending order
        /// then checking them all in ascending order
        /// </summary>
        [Test]
        public void Test_SemestersReturnSorted_Valid()
        {
            for (int i = 9; i >= 0; i--)
            {
                testContext.Course.Add(
                    new Course
                    {
                        courseName = "courseTest" + i,
                        courseAbbr = "course" + i,
                        startDate = testSemester.StartDate,
                        endDate = testSemester.EndDate,
                        hoursPerWeek = 5,
                        semesterID = testSemester.SemesterID,
                        creditUnits = 5,
                        classroomIDBindable = testRoom.ID,
                        primaryInstructorIDBindable = ins1.ID,
                        secondaryInstructorIDBindable = ins2.ID
                    });
            }
            testContext.SaveChanges();

            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            for (int i = 0; i < 10; i++)
            {
                var temp = cut.Find($"tbody tr:nth-of-type({i + 1}) td:first-of-type").TextContent;
                Assert.AreEqual("courseTest" + i, temp);
            }
        }

        /// <summary>
        /// This test is to ensure that the 'next', 'back' and '1', '2', '3', ... 'n' buttons work properly
        /// </summary>
        [Test]
        public void Test_AssertNextBackAndSelectorButtonsWork_Valid()
        {
            AddCoursesToDb(57);

            //should have 8 buttons .. 'back', 1, 2, ... 6, 'next'
            //going to check them all

            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            //getting all courses straight from the page here
            List<Course> courseList = cut.Instance.allCourses;

            //test that 8 links are available within pagnation
            Assert.AreEqual(8, cut.FindAll("ul.pagination li.page-item").Count);

            //original display - make sure first item matches up here
            string currentTestString = cut.Find($"tbody tr:nth-of-type({1}) td:first-of-type").TextContent;
            Assert.AreEqual(courseList[0].courseName, currentTestString);

            //check that the active button is '1'
            currentTestString = cut.Find("li.page-item.active").TextContent;
            Assert.AreEqual("1", currentTestString);

            //check that clicking '1' when already on '1' doesn't change anything
            var btn = cut.Find("li.page-item.active a.page-link");
            btn.Click();
            currentTestString = cut.Find($"tbody tr:nth-of-type({1}) td:first-of-type").TextContent;
            Assert.AreEqual(courseList[0].courseName, currentTestString);

            //check that the disabled button is 'back'
            currentTestString = cut.Find("li.page-item.disabled").TextContent;
            Assert.AreEqual("Back", currentTestString);

            //check that clicking 'back' when already on first page doesn't do anything
            btn = cut.Find("#back");
            btn.Click();
            currentTestString = cut.Find($"tbody tr:nth-of-type({1}) td:first-of-type").TextContent;
            Assert.AreEqual(courseList[0].courseName, currentTestString);

            //click 4
            btn = cut.Find("li.page-item:nth-of-type(5) a.page-link");
            btn.Click();

            //check that new data has been added, corrosponding to 30th item returned
            currentTestString = cut.Find($"tbody tr:nth-of-type({1}) td:first-of-type").TextContent;
            Assert.AreEqual(courseList[30].courseName, currentTestString);

            //check that 'back' button is now enabled
            currentTestString = cut.Find("li.page-item #back").TextContent;
            Assert.AreEqual("Back", currentTestString);

            //check that '4' is now selected
            currentTestString = cut.Find("li.page-item.active").TextContent;
            Assert.AreEqual("4", currentTestString);

            //navigate to '6'
            btn = cut.Find("li.page-item:nth-of-type(7) a.page-link");
            btn.Click();

            //check that 7 items are loaded, corrosponding with last elements in db
            for (int i = 1; i <= 7; i++)
            {
                currentTestString = cut.Find($"tbody tr:nth-of-type({i}) td:first-of-type").TextContent;
                Assert.AreEqual(courseList[49 + i].courseName, currentTestString);
            }

            //check that 'next' button is now disabled
            currentTestString = cut.Find("li.page-item.disabled").TextContent;
            Assert.AreEqual("Next", currentTestString);
        }

        /// <summary>
        /// Less than 10 items has no pagnation available
        /// </summary>
        [Test]
        public void Test_10ItemsHasNoPagnation_Valid()
        {
            AddCoursesToDb(10);

            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            //no pagination should be present on page
            Assert.AreEqual(0, cut.FindAll("ul.pagination").Count);
        }

        /// <summary>
        /// 11 items gives pagnation options
        /// </summary>
        [Test]
        public void Test_11ItemsGivesPagination_Valid()
        {
            AddCoursesToDb(11);

            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<CourseDisplay>();

            //pagination should be available now
            Assert.AreEqual(1, cut.FindAll("ul.pagination").Count);
        }

        /// <summary>
        /// Helper method to add courses to database, adds in ascending order with all information added
        ///     courseName = "courseTest" + i,
        ///     courseAbbr = "course" + i,
        /// </summary>
        /// <param name="numCourses">Number of courses to add</param>
        private void AddCoursesToDb(int numCourses)
        {
            for (int i = 0; i < numCourses; i++)
            {
                testContext.Course.Add(
                    new Course
                    {
                        courseName = "courseTest" + i,
                        courseAbbr = "course" + i,
                        startDate = testSemester.StartDate,
                        endDate = testSemester.EndDate,
                        hoursPerWeek = 5,
                        semesterID = testSemester.SemesterID,
                        creditUnits = 5,
                        classroomIDBindable = testRoom.ID,
                        primaryInstructorIDBindable = ins1.ID,
                        secondaryInstructorIDBindable = ins2.ID
                    }
                );
            }
            testContext.SaveChanges();
        }

    }
}

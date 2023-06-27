using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using Bunit.TestDoubles;
using NUnit.Framework;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Services;
using CSTScheduling.Pages;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using CSTScheduling.Shared;

namespace CSTScheduling.Test.E2ETests
{
    class SemesterServiceTests
    {
        public List<Course> courseList = new List<Course>();
        public List<Instructor> insList = new List<Instructor>();
        public List<Room> roomList = new List<Room>();
        public Semester semTest;
        public Department programTest;
        CstScheduleDbContext testContext;
        Bunit.TestContext testService;


        [SetUp]
        public void setup()
        {

            String dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
             .UseSqlite("Data Source=" + dbDirectory)
             .Options;

            // Create a new database context
            testContext = new CstScheduleDbContext(options);
            // Wipe the database between tests
            testContext.Database.EnsureDeleted();
            //make sure new database is made
            testContext.Database.EnsureCreated();

            roomList = new List<Room>();

            //Initiliaze variables for test here
            this.programTest = new Department
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3

            };

            testContext.Department.Add(programTest);

            semTest = new Semester
            {
                SemesterID = programTest.ID + ",1,1,1",
                deptID = programTest.ID,
                StartDate = new DateTime(2020, 01, 01),
                EndDate = new DateTime(2020, 01, 02),
                StartTime = 8,
                EndTime = 15,
                HasBreak = false,
                BreakStart = null,
                BreakEnd = null
            };

            Room testRoom = new Room
            {
                ID = 1,
                roomNumber = "123",
                city = "Saskatoon",
                campus = "SIAST",
                capacity = 25

            };

            Instructor test1 = new Instructor
            {
                ID = 1,
                email = "TestIns1@test.com",
                lName = "Gibson",
                fName = "Eric",
                officeNum = "306-153-1234",
                phoneNum = "306-412-3556",
                note = "This is a test note"
            };

            Instructor test2 = new Instructor
            {
                ID = 2,
                email = "TestIns2@test.com",
                lName = "Sierra",
                fName = "John",
                officeNum = "306-666-4444",
                phoneNum = "306-888-7777",
            };

            Course testCourse = new Course
            {
                ID = 1,
                courseName = "Course Test",
                courseAbbr = "corse123",
                startDate = semTest.StartDate,
                endDate = semTest.EndDate,
                hoursPerWeek = 5,
                semesterID = semTest.SemesterID,
                creditUnits = 5,
                classroomIDBindable = testRoom.ID,
                primaryInstructorIDBindable = test1.ID,
                secondaryInstructorIDBindable = test2.ID
            };

            courseList.Add(testCourse);

            testContext.Semester.Add(semTest);
            testContext.Room.Add(testRoom);
            testContext.Instructor.Add(test1);
            testContext.Instructor.Add(test2);
            testContext.Add(testCourse);

            testContext.SaveChanges();

            testService = new Bunit.TestContext();

            testService.Services.AddDbContextFactory<CstScheduleDbContext>(opt =>
              opt.UseSqlite("Data Source=" + dbDirectory));
            testService.Services.AddScoped<CstScheduleDbService>();
            testService.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            
        }


        [Test]
        public async Task Test_ViewSemesterInfo_Valid()
        {
            using var context = new Bunit.TestContext();
            testService.RenderComponent<MainLayout>();

            var cut = testService.RenderComponent<SemesterInfo>();//context.RenderComponent<SemesterInfo>();
            
            var markup = cut.Markup;

            var test = cut.Find($"#DepartmentYear").GetDescendants().Count();
            Assert.AreEqual(4, test);
            test = cut.Find($"#DepartmentSemester").GetDescendants().Count();
            Assert.AreEqual(test, 6);
            test = cut.Find($"#DepartmentGroup").GetDescendants().Count();
            Assert.AreEqual(test, 4);

            // Test Start and End Date

            DateTime startTest = new DateTime(2020, 01, 01);
            DateTime endTest = new DateTime(2020, 01, 02);

            Assert.AreEqual(startTest, cut.Instance.CurrentSemester.StartDate);
            Assert.AreEqual(endTest, cut.Instance.CurrentSemester.EndDate);

        }

        [Test]
        public void Test_ViewSemesterNoInfo_Valid()
        {
            testContext.Dispose();
            testService.RenderComponent<MainLayout>();
            var cut = testService.RenderComponent<SemesterInfo>();

            Assert.AreNotEqual(semTest, cut.Instance.CurrentSemester);
        }

        [Test]
        public void Test_ViewAllCourses_Valid()
        {
            testService.RenderComponent<MainLayout>();

            for (int i = 2; i <= 5; i++)
            {
                Course testCourse = new Course
                {
                    ID = i,
                    courseName = "Course Test" + i,
                    courseAbbr = "corse123" + i,
                    startDate = semTest.StartDate,
                    endDate = semTest.EndDate,
                    hoursPerWeek = 5,
                    semesterID = semTest.SemesterID,
                    creditUnits = 5
                };

                testContext.Course.Add(testCourse);
                testContext.SaveChanges();
                courseList.Add(testCourse);
            }

            var cut = testService.RenderComponent<SemesterInfo>();
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(courseList[i].ID, cut.Instance.CurrentCourses[i].ID);
            }

        }

        [Test]
        public void Test_ViewCoursesNoCourses_Valid() 
        {
            testService.RenderComponent<MainLayout>();
            var cut = testService.RenderComponent<SemesterInfo>();

            var beforeWipe = cut.Instance.CurrentCourses.Count;

            cut.Instance.CurrentCourses = new List<Course>();
            var afterWipe = cut.Instance.CurrentCourses.Count;
            Assert.AreNotEqual(beforeWipe, afterWipe);
        }

        [Test]
        public void Test_ViewAllInscturcors() 
        {
            testService.RenderComponent<MainLayout>();

            insList = new List<Instructor>();

            Instructor test1 = new Instructor
            {
                ID = 1,
                email = "TestIns1@test.com",
                lName = "Gibson",
                fName = "Eric",
                officeNum = "306-153-1234",
                phoneNum = "306-412-3556",
                note = "This is a test note"
            };

            Instructor test2 = new Instructor
            {
                ID = 2,
                email = "TestIns2@test.com",
                lName = "Sierra",
                fName = "John",
                officeNum = "306-666-4444",
                phoneNum = "306-888-7777",
            };

            insList.Add(test1);
            insList.Add(test2);

            var cut = testService.RenderComponent<SemesterInfo>();
            for (int i = 0; i <= insList.Count -1; i++)
            {
                Assert.AreEqual(insList[i].ID, cut.Instance.CurrentInstructors[i].ID);
            }
        }

        [Test]
        public void Test_ViewInstructorsNoInsctuctors_Valid() 
        {

            testService.RenderComponent<MainLayout>();
            var cut = testService.RenderComponent<SemesterInfo>();

            var beforeWipe = cut.Instance.CurrentInstructors.Count;
            cut.Instance.CurrentInstructors = new List<Instructor>();
            var afterWipe = cut.Instance.CurrentInstructors.Count;

            Assert.AreNotEqual(beforeWipe, afterWipe);

        }

    }
}

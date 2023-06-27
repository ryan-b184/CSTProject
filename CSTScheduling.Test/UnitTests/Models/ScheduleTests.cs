using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using Intro.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSTScheduling.Utilities;
using System.IO;

namespace CSTScheduling.Test.UnitTests.Models
{
    class ScheduleTests
    {
        #region Global Props and Setup/Teardown
        Semester testSemester;
        Department testDept;
        Room testRoom;
        Instructor ins1;
        Instructor ins2;
        Course testCourse;
        private List<CISR> CISRList;
        CstScheduleDbContext testContext;
        private Bunit.TestContext ctx;
        ValidationHelper vhelper;
        DbContextOptions<CstScheduleDbContext> options;

        [SetUp]
        public void Setup()
        {
            //Initiliaze variables for test here

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

            testCourse = new Course
            {
                ID = 1,
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
            };



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

            vhelper = new ValidationHelper();
        }

        [TearDown]
        public void TearDown()
        {
            testContext.Dispose();
            vhelper = null;
        }
        #endregion

        #region 'Adding Course' tests


        [Test]
        public void Test_SaveScheduleOneCourse_Valid()
        {
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                Time = 8
            };
            //check for errors before adding
            testContext.CISR.Add(testCISR);
            testContext.SaveChanges();
            var test2 = testContext.CISR.ToList<CISR>();

            Assert.IsTrue(test2[0].course.courseName == testCISR.course.courseName);

        }

        [Test]
        public async Task Test_SaveScheduleMultipleCourses_Valid()
        {
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                primaryInstructor = null,
                secondaryInstructor = null,
                semester = null,
                Time = 8
            };
            //check for errors before adding
            await testContext.CISR.AddAsync(testCISR);
            await testContext.SaveChangesAsync();

            var test2 = testContext.CISR.ToList<CISR>();

            Assert.IsTrue(test2[0].course.courseName == testCISR.course.courseName);

            //SECOND CISR ADDED
            CISR testCISR2 = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                primaryInstructor = null,
                secondaryInstructor = null,
                semester = null,
                Time = 8
            };
            //check for errors before adding
            await testContext.CISR.AddAsync(testCISR2); //breaking here, 
            await testContext.SaveChangesAsync();

            var test3 = testContext.CISR.ToList<CISR>();
            Assert.IsTrue(test3[0].course.courseName == testCISR2.course.courseName);
        }
        #endregion

        #region 'Courses with/without Instructors/Rooms assigned' tests
        [Test]
        public void Test_AddCourseWithInstructorAndRoom_Valid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            Instructor tempIns = new();
            Room tempRoom = new();
            //adding all required fields
            CISR testCISR = new CISR
            {
                ID = 1,
                course = testCourse,
                primaryInstructor = ins1,
                room = testRoom,
                Day = DayOfWeek.Monday,
                Time = 8
            };

            //Check for errors
            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsEmpty(errors);
        }
        #endregion

        #region CISR Day and Time required tests
        [Test]
        public void Test_DayAndTimeAdded_Valid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            tempCourse.courseName = "COSC180";

            //adding all required fields
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                Time = 8
            };

            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_OnlyDayAdded_Invalid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            tempCourse.courseName = "COSC180";

            //adding all required fields
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday
            };

            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("You must select a time between 0:00 and 24:00", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_OnlyTimeAdded_Invalid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            tempCourse.courseName = "COSC180";

            //adding all required fields
            CISR testCISR = new CISR
            {
                course = testCourse,
                Time = 8
            };

            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("You must have a day selected before saving a course", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_TimeBetweenZeroAnd24_Valid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            tempCourse.courseName = "COSC180";

            //adding all required fields
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                Time = 12
            };

            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsTrue(errors.Count() == 0);
        }

        [Test]
        public void Test_TimeZeroOr24_Valid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            tempCourse.courseName = "COSC180";

            //adding all required fields
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                Time = 0
            };

            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsEmpty(errors);

            //Same test but with the time being set to 24
            testCISR.Time = 24;

            //checking for errors
            errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_TimeNegOneOr25_Invalid()
        {
            //defining temporary objects to test the CISR object
            Course tempCourse = new();
            tempCourse.courseName = "COSC180";

            //adding all required fields
            CISR testCISR = new CISR
            {
                course = testCourse,
                Day = DayOfWeek.Monday,
                Time = -1
            };

            var errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("You must select a time between 0:00 and 24:00", errors[0].ErrorMessage);

            // Same test but for other outside other boundary
            testCISR.Time = 25;
            errors = ValidationHelper.Validate(testCISR);

            // assert against errors
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("You must select a time between 0:00 and 24:00", errors[0].ErrorMessage);
        }
        #endregion
    }
}

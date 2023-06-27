using System;
using NUnit.Framework;
using System.Linq;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Context;
using Intro.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CSTScheduling.Test.UnitTests.Models.CourseTest
{
    class CourseTests
    {
        //Set up null/default variables here

        private Bunit.TestContext testContext;
        //private IRenderedComponent<CourseAddEdit> component;
        CstScheduleDbContext context;
        private Course testCourse;

        [SetUp]
        public void Setup()
        {
            //Initiliaze variables for test here
            testContext = new Bunit.TestContext();
            //component = testContext.RenderComponent<CourseAddEdit>();
            testCourse = new Course
            {
                ID = 1,
                courseAbbr = "COSC180",
                courseName = "Introduction to Programming",
                hoursPerWeek = 4,
                creditUnits = 6,
                classroomIDBindable = 1,
                //classroom = new Room { ID = 1 },
                startDate = new DateTime?(new DateTime(year: 2021, month: 09, day: 15)),
                endDate = new DateTime?(new DateTime(year: 2022, month: 04, day: 15)),
                primaryInstructorIDBindable = 1,
                semesterID = "1,1,1,1",
                // primaryInstructor = new Data.Models.Instructor { ID = 1 },
                secondaryInstructorIDBindable = 2,
                //  secondaryInstructor = new Data.Models.Instructor { ID = 2 }
            };

            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDatabase")
                          .Options;

            context = new CstScheduleDbContext(options);

            //Wipe the database between tests
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown()
        {
            // get rid of test code here/stop test code
            testContext.Dispose();
            testCourse = null;

        }

        #region CourseNameTests
        [Test]
        public void Test_CourseNameInputExceeds80Chars_Invalid()
        {
            testCourse.courseName = new string('a', 81);
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 2);
            Assert.AreEqual("Specified name is longer than the limit of 80 characters", errors[0].ErrorMessage);

        }
        [Test]
        public void Test_CourseNameInput_Valid()
        {
            testCourse.courseName = "Advanced Programming";
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_CourseNameInputEmpty_Invalid()
        {
            testCourse.courseName = "";
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Course Name is required", errors[0].ErrorMessage);

        }
        [Test]
        public void Test_CourseNameInputBounds30Chars_Valid()
        {
            testCourse.courseName = new string('a', 30);
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        #endregion

        #region CourseAbbrTests
        [Test]
        public void Test_CourseAbbreviationsPass_Valid()
        {
            testCourse.courseAbbr = "COSC180";
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_CourseAbbreviationsExceeds20Chars_Invalid()
        {
            testCourse.courseAbbr = new string('a', 21);
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 2);
            //Assert.AreEqual("Course abbreviations can only be 20 characters long", errors[0].ErrorMessage);

        }
        [Test]
        public void Test_CourseAbbreviationUpperBounds20Char_Valid()
        {
            testCourse.courseAbbr = new string('a', 20);
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_CourseAbbreviationEmpty_Invalid()
        {
            testCourse.courseAbbr = "";
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Course abbreviation is required", errors[0].ErrorMessage);

        }
        #endregion

        #region HoursePerWeekTests
        [Test]
        public void Test_HoursPerWeekErrorOutOfRange35_Invalid()
        {
            testCourse.hoursPerWeek = 36;

            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Hours per week must be at least 0 hours and a maximum of 35 hours", errors[0].ErrorMessage);

        }

        [Test]
        public void Test_HoursPerWeekUpperBounds30Hours_Valid()
        {
            testCourse.hoursPerWeek = 30;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_HoursPerWeekLowerBounds_Valid()
        {
            testCourse.hoursPerWeek = 0;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        #endregion

        #region PrimaryInstructorTests
        [Test]
        public void Test_PrimaryInstructorRequired_Valid()
        {
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_Primary_Instructor_Required_Invalid()
        {
            testCourse.primaryInstructorIDBindable = -1;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("A primary instructor is required for this course", errors[0].ErrorMessage);

        }
        #endregion

        #region SecondaryInstructorTests
        [Test]
        public void Test_SecondaryInstructorNotEqualToPrimaryInstructor_Invalid()
        {
            testCourse.primaryInstructor = new Data.Models.Instructor { ID = 1 };
            testCourse.secondaryInstructor = new Data.Models.Instructor { ID = 1 }; /***************************** must be different object than the primary instructor*/
            testCourse.primaryInstructorIDBindable = 1;
            testCourse.secondaryInstructorIDBindable = 1;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Secondary instructor cannot be the same as Primary Instructor", errors[0].ErrorMessage);

        }

        [Test]
        public void Test_SecondaryInstructorInputButNoPrimaryInstructor_Invalid()
        {
            testCourse.primaryInstructorIDBindable = -1;
            var errors = ValidationHelper.Validate(testCourse);


            Assert.AreEqual("A primary instructor is required for this course", errors[0].ErrorMessage);
            Assert.IsTrue(errors.Count() == 1);
        }

        #endregion

        #region ClassroomTests
        [Test]
        public void Test_ClassroomRequired_Invalid()
        {
            testCourse.classroomIDBindable = -1;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Classroom is required", errors[0].ErrorMessage);

        }
        [Test]
        public void TestClassroomAdded_Valid()
        {
            testCourse.classroomIDBindable = 5;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }

        #endregion

        #region CreditUnitTests
        [Test]
        public void Test_CreditUnitsUpperBounds10_Valid()
        {
            testCourse.creditUnits = 10;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_CreditUnitsLowerBounds0_Valid()
        {
            testCourse.creditUnits = 0;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsEmpty(errors);

        }
        [Test]
        public void Test_CreditUnitsLessThan0_Invalid()
        {
            testCourse.creditUnits = -1;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Credit units must be between 0 and 10", errors[0].ErrorMessage);

        }
        [Test]
        public void Test_CreditUnitsHigherThan10_Invalid()
        {
            testCourse.creditUnits = 15;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Credit units must be between 0 and 10", errors[0].ErrorMessage);

        }

        [Test]
        public void Test_CreditUnitsEmpty_Invalid()
        {
            testCourse.creditUnits = -1;
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Credit units must be between 0 and 10", errors[0].ErrorMessage);

        }
        #endregion

        //ADD TEST FOR START AND END DATE

        [Test]
        public void Test_EndDateBeforeStartDate_Invalid()
        {

            testCourse.startDate = new DateTime?(new DateTime(year: 2001, month: 10, day: 5));
            testCourse.endDate = new DateTime?(new DateTime(year: 2001, month: 10, day: 4));
            var errors = ValidationHelper.Validate(testCourse);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("End date cannot be before start date", errors[0].ErrorMessage);

        }

        #region Test if Course is already added in the database
        [Test]
        public void Test_CourseAddedDoesNotExistInDB_Valid()
        {
            context.Course.Add(new Course
            {
                ID = 2,
                courseName = "IT Development Project",
                courseAbbr = "COSA",
                hoursPerWeek = 4,
                creditUnits = 6,
                classroomIDBindable = 2,
                startDate = new DateTime?(new DateTime(year: 2021, month: 09, day: 15)),
                endDate = new DateTime?(new DateTime(year: 2022, month: 04, day: 15)),
                //classroom = new Room { ID = 2 },
                primaryInstructorIDBindable = 3,
                //primaryInstructor = new Data.Models.Instructor { ID = 3, email = "testdata@sample.com" },
                secondaryInstructorIDBindable = 2,
                //secondaryInstructor = new Data.Models.Instructor { ID = 4, email = "testdata2@sample.com" }
            });

            context.SaveChanges();

            var TestValue = context.Course.ToList<Course>();

            Assert.AreEqual(1, TestValue.Count);

            context.Dispose();
        }

        [Test]
        public void Test_CourseAddedWhenAlreadyInDB_Invalid()
        {
            context.Course.Add(new Course
            {
                ID = 2,
                courseName = "IT Development Project",
                courseAbbr = "COSA",
                hoursPerWeek = 4,
                creditUnits = 6,
                classroomIDBindable = 2,
                //classroom = new Room { ID = 2 },
                startDate = new DateTime?(new DateTime(year: 2021, month: 09, day: 15)),
                endDate = new DateTime?(new DateTime(year: 2022, month: 04, day: 15)),
                primaryInstructorIDBindable = 3,
                //primaryInstructor = new Data.Models.Instructor { ID = 3, email = "testdata@sample.com" },
                secondaryInstructorIDBindable = 2,
                //secondaryInstructor = new Data.Models.Instructor { ID = 4, email = "testdata2@sample.com" }
            });

            context.SaveChanges();

            Assert.Throws<InvalidOperationException>(() =>
            {
                context.Course.Add(new Course
                {
                    ID = 2,
                    courseName = "IT Development Project",
                    courseAbbr = "COSA",
                    hoursPerWeek = 4,
                    creditUnits = 6,
                    classroomIDBindable = 2,
                    //classroom = new Room { ID = 2 },
                    startDate = new DateTime?(new DateTime(year: 2021, month: 09, day: 15)),
                    endDate = new DateTime?(new DateTime(year: 2022, month: 04, day: 15)),
                    primaryInstructorIDBindable = 3,
                    //primaryInstructor = new Data.Models.Instructor { ID = 3 },
                    secondaryInstructorIDBindable = 4,
                    //secondaryInstructor = new Data.Models.Instructor { ID = 4 }
                });

            });

            context.Dispose();
        }
        #endregion
    }
}
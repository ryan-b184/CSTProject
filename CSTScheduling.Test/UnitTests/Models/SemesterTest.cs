using System;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using CSTScheduling.Data.Models;
using Intro.Tests.Helpers;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using CSTScheduling.Data.Context;
using System.Collections.Generic;
using System.IO;

namespace CSTScheduling.Test.UnitTests.Models
{
    class SemesterTest
    {

        #region Attributes / Setup / Teardown

        //Create new semester entity to be tested
        Semester TestSemester = new Semester();
        CstScheduleDbContext context;

        [SetUp]
        public void Setup()
        {
            TestSemester.SemesterID = ("123,2,3,2");

            TestSemester.StartDate = new DateTime(2020, 01, 01);
            TestSemester.EndDate = new DateTime(2020, 01, 02);

            TestSemester.deptID = 123;

            TestSemester.StartTime = 8;
            TestSemester.EndTime = 15;

            TestSemester.HasBreak = false;


            // DB Context Options
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
                          .UseInMemoryDatabase(databaseName: "TestDatabase")
                          .Options;




            //String dbDirectory = Directory.GetCurrentDirectory();
            //dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";
            //// DB Context Options
            //var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
            //             .UseSqlite("Data Source=" + dbDirectory)
            //             .Options;

            // Create a new database context
            context = new CstScheduleDbContext(options);

            // Wipe the database between tests
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown(){}
        #endregion


        #region SemesterID Tests


        /// <summary>
        /// Asserts that using an ID that does not match the REGEX Throws an error
        /// </summary>
        [Test]
        public void Test_SemesterWithInvalidID_Invalid()
        {

            TestSemester.SemesterID = "0000000";

            var errors = ValidationHelper.Validate(TestSemester);


            Assert.IsTrue(errors.Count() == 1);

            Assert.AreEqual("Internal Error, Semester ID does not match REGEX", errors[0].ErrorMessage);


        }


        /// <summary>
        /// Asserts that a Semester with an ID that matches the regex does not throw an error
        /// </summary>
        [Test]
        public void Test_SemesterWithValidID_Valid()
        {
            TestSemester.SemesterID = ("123,2,3,2");
            var errors = ValidationHelper.Validate(TestSemester);

            Assert.Zero(errors.Count);

        }


        #endregion


        #region Semester Start/End Date Tests


        /// <summary>
        /// Asserts that a semester without start and end dates set throws two errors
        /// </summary>
        [Test]
        public void Test_SemesterStartAndEndDateNotSet_Invalid()
        {

            TestSemester.StartDate = null;
            TestSemester.EndDate = null;

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.AreEqual(errors.Count, 2);
            Assert.AreEqual("Start Date is Required", errors[0].ErrorMessage);
            Assert.AreEqual("End Date is Required", errors[1].ErrorMessage);
        }


        /// <summary>
        /// Asserts that setting the semester end date before the start date throws an error
        /// </summary>
        [Test]
        public void Test_SemesterEndDateSetBeforeStartDate_Invalid()
        {

            TestSemester.StartDate = new DateTime(2020,01,01);
            TestSemester.EndDate = new DateTime(2019, 12, 31);

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.IsTrue(errors.Count == 1);

            Assert.AreEqual("Semester End Date must be after the semester Start Date", errors[0].ErrorMessage);
        }


        /// <summary>
        /// Asserts that setting the semester break start and end dates to the same day throws an error
        /// </summary>
        [Test]
        public void Test_SemesterBreakStartEndOnSameDate_Invalid()
        {
            TestSemester.StartDate = new DateTime(2020, 01, 01);
            TestSemester.EndDate = new DateTime(2020, 01, 01);

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.IsTrue(errors.Count == 1);

            Assert.AreEqual("Semester End Date must be after the semester Start Date", errors[0].ErrorMessage);
        }


        /// <summary>
        /// Asserts that setting the semester end date after the semester start date throws no error messages.
        /// </summary>
        [Test]
        public void Test_SemesterStartAndEndDatesValid_Valid()
        {

            TestSemester.StartDate = new DateTime(2020, 01, 01);
            TestSemester.EndDate = new DateTime(2020, 01, 02);

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.Zero(errors.Count);
        }


        #endregion


        #region Start/End Time Tests


        /// <summary>
        /// Asserts that Setting the semester start and end time outside of the range 0-23 throws two errors.
        /// </summary>
        [Test]
        public void Test_SemesterStartTimeEndTimeOutsideOfRange_Invalid()
        {
            TestSemester.StartTime = -1;
            TestSemester.EndTime = 24;

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.IsTrue(errors.Count() == 2);

            Assert.AreEqual("Internal Error, day start time must be an INT between 0 and 23 inclusive", errors[0].ErrorMessage);

            Assert.AreEqual("Internal Error, day end time must be an INT between 0 and 23 inclusive", errors[1].ErrorMessage);

        }


        /// <summary>
        /// Asserts that setting the semester End Time to less than the start time throws an error
        /// </summary>
        [Test]
        public void Test_SemesterEndTimeLessThanStartTime_Invalid()
        {
            TestSemester.StartTime = 23;
            TestSemester.EndTime = 0;

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.IsTrue(errors.Count() == 1);

            Assert.AreEqual("The day must end after it has started", errors[0].ErrorMessage);
        }


        /// <summary>
        /// Assert that setting the semester start and end times to valid values throws no errors
        /// </summary>
        [Test]
        public void Test_SemesterStartAndEndTimeValid_Valid()
        {
            TestSemester.StartTime = 8;
            TestSemester.EndTime = 15;

            var errors = ValidationHelper.Validate(TestSemester);

            Assert.Zero(errors.Count);
        }


        #endregion


        #region Break Attributes Tests


        /// <summary>
        /// Asserts that when a valid Semester object is created HasBreak defaults to null, and throws no errors for misssing break date fields.
        /// </summary>
        [Test]
        public void Test_SemesterHasBreakDefaultFalse_Valid()
        {
            Semester TSemester = createNewTestSemester();

            var errors = ValidationHelper.Validate(TSemester);

            Assert.Zero(errors.Count);

            Assert.IsFalse(TSemester.HasBreak);
        }


        /// <summary>
        /// Asserts that when HasBreak has been set to true, not setting the BreakDate field will throw errors.
        /// </summary>
        [Test]
        public void Test_SemesterBreaksRequired_Invalid()
        {
            Semester TSemester = createNewTestSemester();

            TSemester.HasBreak = true;

            var errors = ValidationHelper.Validate(TSemester);

            Assert.IsTrue(errors.Count == 2);

            Assert.AreEqual("'Has Break' checkbox has been checked, this field is required", errors[0].ErrorMessage);

            Assert.AreEqual("'Has Break' checkbox has been checked, this field is required", errors[1].ErrorMessage);
        }


        /// <summary>
        /// Asserts that Break dates set before the semester start date will throw two errors.
        /// </summary>
        [Test]
        public void Test_SemesterBreakDatesBeforeSemesterStart_Invalid()
        {
            Semester TSemester = createNewTestSemester();

            TSemester.HasBreak = true;

            TSemester.BreakStart = new DateTime(2019, 01, 01);
            TSemester.BreakEnd = new DateTime(2019, 01, 02);

            var errors = ValidationHelper.Validate(TSemester);

            Assert.IsTrue(errors.Count == 2);

            Assert.AreEqual("The break start date must fall between the Semester Start and End dates", errors[0].ErrorMessage);

            Assert.AreEqual("The break end date must fall between the Semester Start and End dates", errors[1].ErrorMessage);
        }


        /// <summary>
        /// Asserts that the break dates being set after the semesters end date will throw two errors.
        /// </summary>
        [Test]
        public void Test_SemesterBreakDatesAfterSemesterEnd_Invalid()
        {
            Semester TSemester = createNewTestSemester();

            TSemester.HasBreak = true;

            TSemester.BreakStart = new DateTime(2021, 01, 01);
            TSemester.BreakEnd = new DateTime(2021, 01, 02);

            var errors = ValidationHelper.Validate(TSemester);

            Assert.IsTrue(errors.Count == 2);

            Assert.AreEqual("The break start date must fall between the Semester Start and End dates", errors[0].ErrorMessage);

            Assert.AreEqual("The break end date must fall between the Semester Start and End dates", errors[1].ErrorMessage);
        }


        /// <summary>
        /// Asserts that a break start date beong set before the break end date will throw an error.
        /// </summary>
        [Test]
        public void Test_SemesterBreakStartAfterBreakEnd_Invalid()
        {
            Semester TSemester = createNewTestSemester();

            TSemester.HasBreak = true;

            TSemester.BreakStart = new DateTime(2020, 01, 31);
            TSemester.BreakEnd = new DateTime(2020, 01, 10);

            var errors = ValidationHelper.Validate(TSemester);

            Assert.IsTrue(errors.Count == 1);

            Assert.AreEqual("The break end must occur after the break start", errors[0].ErrorMessage);
        }


        /// <summary>
        /// Asserts that valid dates for semester breaks will return no errors
        /// </summary>
        [Test]
        public void Test_SemesterBreaksAreValid_Valid()
        {
            Semester TSemester = createNewTestSemester();

            TSemester.HasBreak = true;

            TSemester.BreakStart = new DateTime(2020, 01, 31);
            TSemester.BreakEnd = new DateTime(2020, 02, 14);

            var errors = ValidationHelper.Validate(TSemester);

            Assert.Zero(errors.Count);
        }


        #endregion


        #region Database Methods Tests


        /// <summary>
        /// Asserts that semester that does not exist in the database will be successfully added
        /// </summary>
        [Test]
        public void Test_SemesterAddedWhenNotAlreadyInDb_Valid()
        {

            context.Semester.Add(new Semester{
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5
            });

            context.SaveChanges();

            var returnVal = context.Semester.ToList<Semester>();

            Assert.AreEqual(1, returnVal.Count );

            context.Dispose();
        }


        /// <summary>
        /// Asserts that addigna semester that alreaday exists in the databse will throw an exception
        /// </summary>
        /// 

        //No longer matters I think? --Logan 
        //[Test]
        //public void Test_SemesterAddWhenAlreadyInDb_Invalid()
        //{

        //    context.Semester.Add(new Semester
        //    {
        //        SemesterID = "123,1,1,1",
        //        StartDate = new DateTime(10, 12, 21),
        //        EndDate = new DateTime(30, 12, 21),
        //        StartTime = 1,
        //        EndTime = 5
        //    });

        //    context.SaveChanges();


        //    Assert.Throws<InvalidOperationException>( () =>
        //    {
        //            context.Semester.Add(new Semester
        //            {
        //                SemesterID = "123,1,1,1",
        //                StartDate = new DateTime(10, 12, 21),
        //                EndDate = new DateTime(30, 12, 21),
        //                StartTime = 1,
        //                EndTime = 5
        //            });
        //    });

        //    context.Dispose();
        //}


        /// <summary>
        /// Asserts that calling update on a semester object will change the values
        /// </summary>
        [Test]
        public void Test_SemesterUpdatedWhenAlreadyInDb_Valid()
        {

            // Create a semester object
            Semester testSemester = new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5
            };

            // Add the semester object to the database
            context.Semester.Add(testSemester);
            context.SaveChanges();

            // Modify an attribute of the semester object and update
            testSemester.StartTime = 4;

            context.Semester.Update(testSemester);
            context.SaveChanges();

            // Clear the reference to the object, and instead pull it from the database
            testSemester = null;
            testSemester = context.Semester.Find(1);

            // Assert that the changes to the data have been made
            Assert.AreEqual(4, testSemester.StartTime);

            context.Dispose();

        }

        #endregion

        /// <summary>
        /// Helper Method, creates a valid instance of a Semester object for use with the Break Attribute Tests
        /// </summary>
        /// <returns></returns>
        private Semester createNewTestSemester()
        {
            Semester TSemester = new Semester();

            TSemester.SemesterID = ("123,2,3,2");

            TSemester.deptID = 123;

            TSemester.StartDate = new DateTime(2020, 01, 01);
            TSemester.EndDate = new DateTime(2020, 12, 31);

            TSemester.StartTime = 8;
            TSemester.EndTime = 15;

            return TSemester;
        }

        #region View Semester Tests


        #region Test View Semester With Courses
        /// <summary>
        /// Assert that we are able to retrieve Courses In a Semester
        /// </summary>
        [Test]
        public void Test_ViewSemesterWithCourses_Valid()
        {
            Semester testSemester = new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5,
                HasBreak = false
            };

            context.Semester.Add(testSemester);

            context.Course.Add(new Course
            {
                ID = 1,
                courseAbbr = "COSC180",
                courseName = "Introduction to Programming",
                hoursPerWeek = 4,
                creditUnits = 6,
                classroomIDBindable = 1,
                //semester = testSemester,
                semesterID = testSemester.SemesterID,
                //classroom = new Room { ID = 1 },
                primaryInstructorIDBindable = 1,
                //primaryInstructor = new Data.Models.Instructor { ID = 1, email = "testdata@sample.com" },
                secondaryInstructorIDBindable = 2,
                //secondaryInstructor = new Data.Models.Instructor { ID = 2, email = "testdata2@sample.com" }
            });

            context.SaveChanges();

            //Look for all the courses that are in testSemester
            List<Course> testCourse = context.Course.Where(course => course.semesterID == testSemester.SemesterID).ToList<Course>();

            //Assert that the testCourse list has 1 Course
            Assert.AreEqual(testCourse.Count, 1);


            context.Dispose();
        }
        #endregion

        #region Test View Semester with Start and End Date Set valid

        /// <summary>
        /// Assert that we are able to retrieve Semester Start and End Date
        /// </summary>
        [Test]
        public void Test_ViewSemesterWithStartAndEndDateSet_Valid()
        {
            context.Semester.Add(new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5
            });

            
            context.SaveChanges();

            
            Semester testSemester = context.Semester.Find(1); //Find a semester with 123,1,1,1 semesterID

            Assert.IsNotNull(testSemester);  //Test that we are able to retrieve a semester using the id
            Assert.AreEqual(testSemester.StartDate, new DateTime(10, 12, 21)); //Test that we are able to retrieve a semester startdate equal to the test data
            Assert.AreEqual(testSemester.EndDate, new DateTime(30, 12, 21)); //Test that we are able to retrieve a semester enddate equal to the test data


            context.Dispose();

        }

        #endregion

        #region Test View Semester with Day Start and End Date Set valid
        /// <summary>
        /// Assert that we are able to retrieve Semester Day's Start and End time
        /// </summary>
        [Test]
        public void Test_ViewSemesterWithDayStartAndEndDateSet_Valid()
        {
            context.Semester.Add(new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5
            });

            context.SaveChanges();

            Semester testSemester = context.Semester.Find(1); //Find a semester with 123,1,1,1 semesterID

            Assert.IsNotNull(testSemester); //Test that we are able to retrieve a semester using the id
            Assert.AreEqual(testSemester.StartTime, 1); //Test that we are able to retrieve a semester day start time equal to the test data
            Assert.AreEqual(testSemester.EndTime, 5); //Test that we are able to retrieve a semester day end time equal to the test data

            context.Dispose();
        }

        #endregion

        #region Test View Semester with Break Start and End Date Set valid
        /// <summary>
        /// Assert that we are able to retrieve Semester's Break Start and End Date
        /// </summary>
        [Test]
        public void Test_ViewSemesterWithBreakStartAndEndDateSet_Valid()
        {
            context.Semester.Add(new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5,
                HasBreak = true,
                BreakStart = new DateTime(15,12,21),
                BreakEnd = new DateTime(20,12,21)
            });

            context.SaveChanges();
            
            Semester testSemester = context.Semester.Find(1); //Find a semester with 123,1,1,1 semesterID
            
            Assert.IsNotNull(testSemester); //Test that we are able to retrieve a semester using the id
            Assert.AreEqual(testSemester.BreakStart, new DateTime(15, 12, 21)); //Test that we are able to retrieve semester break start date equal to the test data
            Assert.AreEqual(testSemester.BreakEnd, new DateTime(20, 12, 21)); //Test that we are able to retrieve semester break end date equal to the test data
            Assert.IsTrue(testSemester.HasBreak); //Test that we are able to retreive Semester break status equal to the test data

            context.Dispose();
        }

        #endregion

        #region Semester With No Courses

        /// <summary>
        /// Assert that we are able to retrieve Semester without Courses in it
        /// </summary>
        [Test]
        public void Test_ViewSemesterWithNoCourses_Valid()
        {
            Semester testSemester = new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5,
                HasBreak = false
            };

            context.Semester.Add(testSemester);

            context.SaveChanges();

            //Look for all the courses that are in testSemester
            List<Course> testCourse = context.Course.Where(course => course.semesterID == testSemester.SemesterID).ToList<Course>();

            Assert.AreEqual(testCourse.Count, 0); //Test that no Course returns 

            context.Dispose();
        }

        #endregion

        #region Semester With No Instructors

        /// <summary>
        /// Assert that we are able to retreive Semester without Instructors in it
        /// </summary>
        //[Test]
        //public void Test_ViewSemesterWithNoInstructors_Valid()
        //{
        //    Semester testSemester = new Semester
        //    {
        //        SemesterID = "123,1,1,1",
        //        StartDate = new DateTime(10, 12, 21),
        //        EndDate = new DateTime(30, 12, 21),
        //        StartTime = 1,
        //        EndTime = 5,
        //        HasBreak = false
        //    };

        //    context.Semester.Add(testSemester);

        //    context.SaveChanges();

        //    //Look for Courses that are in the semester
        //    List<Course> testCourse = context.Course.Where(course => course.semesterID == testSemester.SemesterID).ToList<Course>();

        //    List<Instructor> instructorList = new List<Instructor>(); 

        //    //retrieve all the instructors that are in all of the Courses
        //    foreach (Course x in testList)
        //    {
        //        if (!instructorList.Contains(x.primaryInstructor))
        //        {
        //            instructorList.Add(x.primaryInstructor);
        //        }

        //        if (!instructorList.Contains(x.secondaryInstructor))
        //        {
        //            instructorList.Add(x.secondaryInstructor);
        //        }
        //    }

        //    Assert.AreEqual(instructorList.Count, 0); // Test that there are no instructors returning

        //    context.Dispose();
        //}

        #endregion

        #region Semester With Instructors
        /// <summary>
        /// Assert that we are able to retreive all the instructors in a semester
        /// </summary>
        [Test]
        public void Test_ViewSemesterWithInstructors_Valid()
        {
            Semester testSemester = new Semester
            {
                SemesterID = "123,1,1,1",
                StartDate = new DateTime(10, 12, 21),
                EndDate = new DateTime(30, 12, 21),
                StartTime = 1,
                EndTime = 5,
                HasBreak = false
            };

            context.Semester.Add(testSemester);

            context.Course.Add(new Course
            {
                ID = 1,
                courseAbbr = "COSC180",
                courseName = "Introduction to Programming",
                hoursPerWeek = 4,
                creditUnits = 6,
                classroomIDBindable = 1,
                //semester = testSemester,
                semesterID = testSemester.SemesterID,
                //classroom = new Room { ID = 1 },
                primaryInstructorIDBindable = 1,
                //primaryInstructor = new Data.Models.Instructor { ID = 1, email = "testdata@sample.com" },
                secondaryInstructorIDBindable = 2,
                //secondaryInstructor = new Data.Models.Instructor { ID = 2, email = "testdata2@sample.com" }
            });

            context.SaveChanges();

            // Look for all the Courses within the testSemester
            List<Course> courseList = context.Course.Where(course => course.semesterID == testSemester.SemesterID).ToList<Course>();

            //List<Instructor> instructorList = new List<Instructor>();
            List<int> instructorList = new List<int>();
            //retrieve all the instructors that are in all of the Courses
            foreach (Course x in courseList)
            {
                if (!instructorList.Contains(x.primaryInstructorIDBindable))
                {
                    instructorList.Add(x.primaryInstructorIDBindable);
                }

                if (!instructorList.Contains(x.secondaryInstructorIDBindable))
                {
                    instructorList.Add(x.secondaryInstructorIDBindable);
                }
            }

            Assert.AreEqual(2, instructorList.Count); // Test that we are able to retrieve instructors in the semester

            context.Dispose();
        }

    }
    #endregion

        #endregion

}





//using System;
//using System.Collections.Generic;
//using Blazorise;
//using Blazorise.Bootstrap;
//using Blazorise.Icons.FontAwesome;
//using Bunit;
//using CSTScheduling.Data.Context;
//using CSTScheduling.Data.Models;
//using CSTScheduling.Data.Services;
//using CSTScheduling.Pages;
//using Intro.Tests.Helpers;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using NUnit.Framework;
//using System;

//using System.Collections.Generic;
//using System.IO;
//using System.Linq;

//namespace CSTScheduling.Test.UnitTests.Services
//{
//    class ScheduleViewTests
//    {
//        CstScheduleDbContext testContext;
//        Bunit.TestContext ctx;

//        [SetUp]
//        public void Setup()
//        {
            
//            //sets up the textDB for use during tests
//            String dbDirectory = Directory.GetCurrentDirectory();
//            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";
//            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
//             .UseSqlite("Data Source=" + dbDirectory)
//             .Options;

//            // Create a new database context
//            testContext = new CstScheduleDbContext(options);
//            // Wipe the database between tests
//            testContext.Database.EnsureDeleted();
//            //make sure new database is made
//            testContext.Database.EnsureCreated();


//            ctx = new Bunit.TestContext();
//            ctx.Services.AddDbContextFactory<CstScheduleDbContext>(opt =>
//              opt.UseSqlite("Data Source=" + dbDirectory));
//            ctx.Services.AddSingleton<CstScheduleDbService>();
//            ctx.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            
//        }


//        [TearDown]
//        public void Teardown()
//        {
//            testContext.Dispose();
            
//        }

//        #region schedule course display tests

//        [Test]
//        public void Test_ScheduleOneCourseAdded_Valid()
//        {
//            var cut = ctx.RenderComponent<DepartmentPage>();

//            //grab the lists from the page that will be displayed
//            List<Semester> semList = cut.Instance.semList;
//            List<CISR> cisrList = cut.Instance.cisrList;

//            Assert.Equals(cisrList.Count, 45);//check the count of the list
            
//            //check values of list
//            //compare list values to displayed values using for loop

//        }
//        [Test]
//        public void Test_Schedule45CoursesAdded_Valid()
//        {
//            var cut = ctx.RenderComponent<DepartmentPage>();

//            //grab the lists from the page that will be displayed
//            List<Semester> semList = cut.Instance.semList;
//            List<CISR> cisrList = cut.Instance.cisrList;

//            Assert.Equals(cisrList.Count, 45);//check the count of the list

//            //check values of list
//            //compare list values to displayed values using for loop
//        }
//        [Test]
//        public void Test_2CoursesDisplayInRightSlots_Valid()
//        {
//            Semester semTest = new Semester
//            {
//                SemesterID = "1,1,1,1",
//                deptID = 1,
//                StartDate = new DateTime(2020, 01, 01),
//                EndDate = new DateTime(2020, 01, 02),
//                StartTime = 8,
//                EndTime = 15,
//                HasBreak = false,
//                BreakStart = null,
//                BreakEnd = null
//            };
//            testContext.Semester.Add(semTest);
//            testContext.Course.Add(
//                new Course
//                {
//                    ID = 1,
//                    courseName = "Course Test",
//                    courseAbbr = "corse123",
//                    startDate = semTest.StartDate,
//                    endDate = semTest.EndDate,
//                    hoursPerWeek = 5,
//                    semesterID = semTest.SemesterID,
//                    creditUnits = 5,
//                    classroomIDBindable = 1,
//                    primaryInstructorIDBindable = 1,
//                    secondaryInstructorIDBindable = 0
//                }
//            );
//            for (int i=0; i< 44; i++)
//            {
//                testContext.Course.Add(
//                    new Course
//                    {
//                        ID = 0,
//                        courseName = "Course Test",
//                        courseAbbr = "corse123",
//                        startDate = semTest.StartDate,
//                        endDate = semTest.EndDate,
//                        hoursPerWeek = 5,
//                        semesterID = semTest.SemesterID,
//                        creditUnits = 5,
//                        classroomIDBindable = 1,
//                        primaryInstructorIDBindable = 1,
//                        secondaryInstructorIDBindable = 0
//                    }
//                );
//            }


//            var cut = ctx.RenderComponent<DepartmentPage>();

//            //grab the lists from the page that will be displayed
//            List<Semester> semList = cut.Instance.semList;
//            List<CISR> cisrList = cut.Instance.cisrList;
//            List<CISR> dbcisrList = testContext.CISR.ToListAsync();

//            Assert.Equals(cisrList.Count, 45);//check the count of the list

//            //check values of list
//            //compare list values to displayed values
//        }

//        #endregion
//        #region List of added semesters display
//        [Test]
//        public void Test_SemesterDropDownTwoAdded_Valid()
//        {

//        }
//        [Test]
//        public void Test_YearDropDownTwoAdded_Valid()
//        {

//        }
//        [Test]
//        public void Test_StudentGropuDropDownTwoAdded_Valid()
//        {

//        }
//        [Test]
//        public void Test_NoSemestersAdded_Valid()
//        {

//        }


//        #endregion

//        #region Schedule Component Display
//        [Test]
//        public void Test_2SchedulesAddedAppear_Valid()
//        {

//        }
//        [Test]
//        public void Test_TestNoSchedulesMade_Valid()
//        {

//        }
        
        
//        #endregion
//    }
//}

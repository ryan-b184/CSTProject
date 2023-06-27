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
using CSTScheduling.Test.Fixtures;

namespace CSTScheduling.Test.UnitTests.Services
{
    public class DeleteTests
    {

        Bunit.TestContext ctx;
        public CstScheduleDbContext testContext;
        public deleteFixture fix;
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

            // Populate db with test data
            fix = new deleteFixture();
            fix.Unload(testContext);
            fix.Load(testContext); //loads all test data into the test database
        }

        [TearDown]
        public void Teardown()
        {
            testContext.Dispose();
            
        }


        [Test]
        public async Task Test_Course_Delete()
        {
            // render the main layout
            ctx.RenderComponent<MainLayout>();

            // render the course display page
            var cut = ctx.RenderComponent<CourseDisplay>();

            int courseCountBefore = cut.Instance.allCourses.Count();
            Course course = cut.Instance.allCourses.Find(i => i.courseAbbr == "COSC292");

            List<CISR> cisrBeforeDelete = await testContext.CISR.Where(i => i.CourseID == course.ID).ToListAsync();

            cut.Instance.confirmDeleteCourse(course);
            cut.Instance.deleteCourse();

            List<CISR> cisrAfterDelete = await testContext.CISR.Where(i => i.CourseID == course.ID).ToListAsync();

            // count of courses after delete. expected to be 1 less course
            int courseCountAfter = cut.Instance.allCourses.Count();

            //assert that the before count is greater than the after count
            Assert.IsTrue(courseCountBefore > courseCountAfter);
            //assert that the course deleted is not in the course list anymore
            Assert.IsTrue(cut.Instance.allCourses.Find(i => i.ID == course.ID) == null);
            Assert.IsTrue(cisrBeforeDelete.Count() > cisrAfterDelete.Count());
        }

        [Test]
        public async Task Test_Room_Delete()
        {
            // render the main layout
            ctx.RenderComponent<MainLayout>();

            // render the room display page
            var cut = ctx.RenderComponent<RoomPage>();

            // count the list of rooms before delete
            int roomCountBefore = cut.Instance.roomList.Count();
            Room room = cut.Instance.roomList.Find(i => i.roomNumber == "239A");

            

            // get the cisrs with room we will delete.
            List<CISR> cisrBeforeDelete = await testContext.CISR.Where(i => i.RoomID == room.ID).ToListAsync();
            List<Course> courseBeforeDelete = await testContext.Course.Where(i => i.classroomIDBindable == room.ID).ToListAsync();
            //call the method to open modal
            cut.Instance.confirmDeleteRoom(room);
            cut.Instance.deleteRoom();

            // get the cisrs with deleted room, expected to return null
            List<CISR> cisrAfterDelete = await testContext.CISR.Where(i => i.RoomID == room.ID).ToListAsync();
            List<Course> courseAfterDelete = await testContext.Course.Where(i => i.classroomIDBindable == room.ID).ToListAsync();
            // count the list of rooms after delete, expected to be 1 less
            int roomCountAfter = cut.Instance.roomList.Count();

            // assert that the count of lists before is greater than the count of lists after deletion
            Assert.IsTrue(roomCountBefore > roomCountAfter);
            // assert that room page does not have the deleted room anymore
            Assert.IsTrue(cut.Instance.roomList.Find(i => i.ID == room.ID) == null);
            // assert that courses with the deleted room as default have gone
            Assert.IsTrue(courseBeforeDelete.Count() > courseAfterDelete.Count());
            // assert that all the courses scheduled with the room are with null value for room
            Assert.IsTrue(cisrBeforeDelete.Count() > cisrAfterDelete.Count());

        }

        [Test]
        public async Task Test_Instructor_Delete()
        {
            // render the main layout
            ctx.RenderComponent<MainLayout>();

            // render the room display page
            var cut = ctx.RenderComponent<InstructorPage>();

            Instructor ins = cut.Instance.Instructors.Find(i => i.ToString() == "Basoalto, Ernesto");

            // count the list of instructors on the page
            int insListBefore = cut.Instance.Instructors.Count();
            // get the cisrs with instructors we will delete.
            List<CISR> cisrOfInstructorPrimary = await testContext.CISR.Where(i => i.PrimaryInstructorID == ins.ID).ToListAsync();
            List<CISR> cisrOfInstructorSecondary = await testContext.CISR.Where(i => i.SecondaryInstructorID == ins.ID).ToListAsync();

            cut.Instance.confirmDeleteInstructor(ins);
            cut.Instance.deleteInstructor();

            // count the list of instructors on the page
            int insListAfter = cut.Instance.Instructors.Count();
            // compare the course list before and after
            Assert.IsTrue(insListBefore > insListAfter );

            //comparing secondary instructor count to the list before

            #region CISR checks

           
            //_______________________________ compare  CISRs
            List<CISR> cisrOfInstructorPrimaryAfter = await testContext.CISR.Where(i => i.PrimaryInstructorID == ins.ID).ToListAsync();
            List<CISR> cisrOfInstructorSecondaryAfter = await testContext.CISR.Where(i => i.SecondaryInstructorID == ins.ID).ToListAsync();
            
            //assert that count after is one less
            Assert.IsTrue(cisrOfInstructorSecondary.Count > cisrOfInstructorSecondaryAfter.Count );
            Assert.IsTrue(cisrOfInstructorPrimary.Count > cisrOfInstructorPrimaryAfter.Count );

            #endregion
        }

        //[Test]
        //public async void Test_Program_Delete()
        //{
        //    #region Setup
        //    //will load in lists for program, courses, Cisrs, semesters
        //    List<Semester> semList = await testContext.Semester.ToListAsync();
        //    List<Department> departmentList = await testContext.Department.ToListAsync();
        //    List<Course> courseList = await testContext.Course.ToListAsync();
        //    List<CISR> cisrList = await testContext.CISR.ToListAsync();



        //    //Load pages
        //    ctx.RenderComponent<MainLayout>();
        //    var cut = ctx.RenderComponent<DepartmentPage>();
        //    List<Department> depPageList = cut.Instance.depList;

        //    //delete the program
        //    Department depToRemove = depPageList.Find(x => x.departmentName == "CST");
        //    //await cut.Instance.deleteDepartment(depToRemove);
        //    List<Department> depPageListAfter = cut.Instance.depList;

        //    //Load in lists after deletion
        //    List<Semester> semListAfter = await testContext.Semester.ToListAsync();
        //    List<Department> departmentListAfter = await testContext.Department.ToListAsync();
        //    List<Course> courseListAfter = await testContext.Course.ToListAsync();
        //    List<CISR> cisrListAfter = await testContext.CISR.ToListAsync();

        //    #endregion


        //    #region Department checks
        //    Assert.IsTrue(depPageList.Count == depPageListAfter.Count - 1);
        //    Assert.IsTrue(departmentListAfter.Find(x => x.departmentName == depToRemove.departmentName) == null);
        //    #endregion

        //    #region Semester checks
        //    List<Semester> oldSemesters = semList.Where(x => x.deptID == depToRemove.ID).ToList();
        //    Assert.IsTrue(semList.Count - oldSemesters.Count == semListAfter.Count);
 
        //    #endregion

        //    #region Course checks
        //    foreach(Semester sem in oldSemesters)
        //    {//checks that every course under a semester is now gone
        //        List<CISR> semesterCISR = cisrListAfter.Where(x => x.semester == sem).ToList();
        //        List<Course> courseForSem = courseListAfter.Where(x => x.semesterID == sem.SemesterID).ToList();
        //        Assert.IsTrue(courseForSem.Count == 0); // check that all courses in every semester are deleted
        //        Assert.IsTrue(semesterCISR.Count == 0); // check that all cisr in every semester are deleted
        //    }
        //    #endregion

          
        //}

        //[Test]
        //public async void Test_Reduce_DepartmentSemester_Count()
        //{
        //   //Load pages
        //    ctx.RenderComponent<MainLayout>();
        //    var cut = ctx.RenderComponent<DepartmentPage>();

        //    Department depToBeReduced = cut.Instance.depList.Where(x => x.departmentName == "CST").FirstOrDefault();

        //    int semesterCountBeforeReduced = testContext.Semester.Where(x => x.deptID == depToBeReduced.ID).Count();

        //    // reduce the semster count by 1, expected the 3rd semester to be deleted. keep first and second semesters
        //    depToBeReduced.semesterCount -= 1;

        //    //cut.Instance.Edit(depToBeReduced);
        //    //cut.Instance.saveDepartment();

        //    int semesterCountAfterReduced = testContext.Semester.Where(x => x.deptID == depToBeReduced.ID).Count();
        //    List<Semester> allSemesterInDepartment = await testContext.Semester.Where(x => x.deptID == depToBeReduced.ID).ToListAsync();
        //    // check that the amount of semesters are reduced
        //    // 4 because 2 group of a semester in a year
        //    Assert.IsTrue(semesterCountBeforeReduced  - 4 ==  semesterCountAfterReduced);
        //    foreach (Semester sem in allSemesterInDepartment)
        //    {
        //        // loop through the list of semester after deletion
        //        // test that the 3rd semesters are deleted
        //        Assert.IsTrue(getSemesterString(sem) != "3");

        //    }

        //    // get all of the CISR that are on 3rd semester
        //    List<CISR> allCISRinDeletedSemester = await testContext.CISR.Where(x => getSemesterString(x.semester) == "3" && x.semester.deptID == depToBeReduced.ID).ToListAsync();
        //    Assert.IsTrue(allCISRinDeletedSemester.Count() == 0);


        //}

        //public async void Test_Reduce_DepartmentYear_Count()
        //{
        //    //Load pages
        //    ctx.RenderComponent<MainLayout>();
        //    var cut = ctx.RenderComponent<DepartmentPage>();

        //    Department depToBeReduced = cut.Instance.depList.Where(x => x.departmentName == "CST").FirstOrDefault();

        //    int semesterCountBeforeReduced = testContext.Semester.Where(x => x.deptID == depToBeReduced.ID).Count();

        //    // reduce the semster count by 1, expected the 3rd semester to be deleted. keep first and second semesters
        //    depToBeReduced.lengthInYears -= 1;

        //    //cut.Instance.Edit(depToBeReduced);
        //    //cut.Instance.saveDepartment();

        //    // count expected to be 6 less
        //    int semesterCountAfterReduced = testContext.Semester.Where(x => x.deptID == depToBeReduced.ID).Count();
        //    Assert.IsTrue(semesterCountBeforeReduced - 6 == semesterCountAfterReduced);

        //    List<Semester> allSemesterInDepartment = await testContext.Semester.Where(x => x.deptID == depToBeReduced.ID).ToListAsync();
        //    foreach (Semester sem in allSemesterInDepartment)
        //    {
        //        // loop through the list of semester after deletion
        //        // test that the 2nd year semesters are deleted
        //        Assert.IsTrue(getYearString(sem) != "2");

        //    }

        //    // get all of the CISR that are on 2nd year
        //    List<CISR> allCISRinDeletedSemester = await testContext.CISR.Where(x => getYearString(x.semester) == "2" && x.semester.deptID == depToBeReduced.ID).ToListAsync();
        //    Assert.IsTrue(allCISRinDeletedSemester.Count() == 0);
        //}

        /*
         * Helper method to parse the semesterID of semester and return the year
         */
        public string getYearString(Semester sem)
        {
            return sem.SemesterID.Split(",")[1];
        }

        /*
         * Helper method to parse the semesterID of semester and return the semester
         */
        public string getSemesterString(Semester sem)
        {
            return sem.SemesterID.Split(",")[2];
        }
    }
}

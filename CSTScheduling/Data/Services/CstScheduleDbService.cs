using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace CSTScheduling.Data.Services
{
    public class CstScheduleDbService
    {
        #region Private Attributes
        public IDbContextFactory<CstScheduleDbContext> dbContextFactory; //= new ServiceCollection().BuildServiceProvider().GetRequiredService<IDbContextFactory<CstScheduleDbContext>>();
        public CstScheduleDbContext dbContext;
        public List<Room> roomList = new List<Room>();
        public List<Instructor> InstructorList = new List<Instructor>();

        public Department tempDep = null;
        public List<Department> departmentList;
        public List<Room> unassaignedRooms;
        public List<CISR> CISRList = new();

        public Boolean depSuccess = false;
        public Boolean roomSuccess = false;

        private string errFormat = "dddd, MMMM dd";
        #endregion

        /*
         * 'Cache' temporary objects in the DbService so all components can access them.
         */

        #region GlobalAttrs For Component-Access

        public Department CurrentDepartment = new();
        public bool hasLoaded = false;
        public string currentMode = "READ_MODE";

        #region SemesterInfo Attributes
        public Semester CurrentSemester;
        public int YearSelected { get; set; } = 1;
        public int SemesterSelected { get; set; } = 1;
        public int StudentGroupSelected { get; set; } = 1;
        public List<Course> CurrentCourses { get; set; }
        public List<Instructor> CurrentInstructors { get; set; }

        #endregion

        #region CourseDisplay Attributes
        public List<Course> AllCourses { get; set; }
        #endregion
        #endregion

        /// <summary>
        /// Asyncronous Method
        /// <br />This method is called once on startup to initialise the service
        /// <br />It will Attempt to fetch a List:Department from the Database, and use the first one (this.departmentList[0]) as the currently selected department (this.currentDepartment)
        /// <br />If no Departments are found in the Database, it will create a temoporary Department object to signal the application that one must be created
        /// </summary>
        /// <returns></returns>
        public async Task Initialize()
        {
            await GetDepartmentAsync();
            if (this.departmentList.Count > 0)
            {
                this.CurrentDepartment = this.departmentList[0];
            }
            else
            {
                this.CurrentDepartment = new Department();
                CurrentDepartment.departmentName = "No Programs Created";
            }
        }

        #region PDF
        public List<DateTime> GetDatesForIns(Instructor ins, int semID)
        {
            dbContext = dbContextFactory.CreateDbContext();
            var dateList = new List<DateTime>();
            var semester = dbContext.Semester.FirstOrDefault(s => s.ID == semID);
            if (semester != null)
            {
                var courseList = dbContext.Course.Where(c => c.semesterID == semester.SemesterID && c.startDate.HasValue
                                                        && c.endDate.HasValue && (c.primaryInstructorIDBindable == ins.ID || c.secondaryInstructorIDBindable == ins.ID))
                                                    .ToList();

                dateList = courseList.SelectMany(c => new[] { c.startDate.Value, c.endDate.Value })
                                        .ToList<DateTime>();

                // For the instructors that doesn't teach whole semester
                dateList.Add(semester.StartDate.Value);
                dateList.Add(semester.EndDate.Value);
                dateList = dateList.Distinct().OrderBy(d => d).ToList<DateTime>();
            }

            return dateList;
        }

        public List<CISR> GetInsCISR(Instructor ins, DateTime startDate, DateTime endDate)
        {
            dbContext = dbContextFactory.CreateDbContext();

            var cisrList = dbContext.CISR.Include(c => c.course).Include(c => c.primaryInstructor).Include(c => c.secondaryInstructor).Include(c => c.room)
                .Where(c => (c.PrimaryInstructorID.Value == ins.ID || c.SecondaryInstructorID.Value == ins.ID) && (c.course.endDate.Value >= startDate && c.course.startDate.Value <= endDate))
                .ToList<CISR>();

            dbContext.Dispose();

            return cisrList;
        }

        public List<DateTime> GetDatesForRoom(Room room, int semID)
        {
            dbContext = dbContextFactory.CreateDbContext();
            var dateList = new List<DateTime>();
            var semester = dbContext.Semester.FirstOrDefault(s => s.ID == semID);

            if (semester != null)
            {
                var courseList = dbContext.Course.Where(c => c.semesterID == semester.SemesterID && c.classroomIDBindable == room.ID
                                                    && c.startDate.HasValue && c.endDate.HasValue)
                                            .ToList();

                dateList = courseList.SelectMany(c => new[] { c.startDate.Value, c.endDate.Value })
                                        .ToList<DateTime>();

                // For the instructors that doesn't teach whole semester
                dateList.Add(semester.StartDate.Value);
                dateList.Add(semester.EndDate.Value);
                dateList = dateList.Distinct().OrderBy(d => d).ToList<DateTime>();
            }

            return dateList;
        }

        public List<CISR> GetRoomCISR(Room room, DateTime startDate, DateTime endDate)
        {
            dbContext = dbContextFactory.CreateDbContext();
            var cisrList = dbContext.CISR.Include(c => c.course).Include(c => c.primaryInstructor).Include(c => c.secondaryInstructor).Include(c => c.room)
                .Where(c => (c.RoomID.Value == room.ID) && (c.course.endDate.Value >= startDate && c.course.startDate.Value <= endDate))
                .ToList<CISR>();

            dbContext.Dispose();

            return cisrList;
        }
        #endregion

        #region Constructor
        public CstScheduleDbService(IDbContextFactory<CstScheduleDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }
        #endregion


        #region CRUD Instructor
        /// <summary>
        /// This method returns a list of Instructors.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Instructor>> GetInstructorsAsync()
        {
            dbContext = dbContextFactory.CreateDbContext();

            List<Instructor> returnList = new List<Instructor>();

            returnList = await dbContext.Instructor.OrderBy(e => e.lName).ThenBy(e => e.fName).ToListAsync();
            InstructorList = returnList;
            await dbContext.DisposeAsync();

            return returnList;
        }

        public async Task<Instructor> GetInstructorsAsync(int ID)
        {
            var instructor = new Instructor();
            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                instructor = await dbContext.Instructor.FirstOrDefaultAsync(c => c.ID.Equals(ID));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await dbContext.DisposeAsync();
            return instructor;
        }


        /// <summary>
        /// This method adds a new Instructor to the DbContext and saves it
        /// </summary>
        /// <param name="instructor"></param>
        /// <returns></returns>
        public async Task<Instructor> AddInstructorAsync(Instructor instructor)
        {
            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                await dbContext.Instructor.AddAsync(instructor);
                await dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await dbContext.DisposeAsync();
            return instructor;
        }

        /// <summary>
        /// This method update and existing instructor and saves the changes
        /// </summary>
        /// <param name="instructor"></param>
        /// <returns></returns>
        public async Task<Instructor> UpdateInstructorAsync(Instructor instructor)
        {
            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                var instructorExist = dbContext.Instructor.FirstOrDefaultAsync(p => p.ID == instructor.ID);
                await dbContext.DisposeAsync();
                //need to dispose and recreate apparently?
                dbContext = dbContextFactory.CreateDbContext();
                if (instructorExist != null)
                {
                    await dbContext.DisposeAsync();
                    dbContext = dbContextFactory.CreateDbContext();
                    dbContext.Instructor.Update(instructor);
                    await dbContext.SaveChangesAsync();
                    await GetInstructorsAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return instructor;
        }


        /// <summary>
        /// This method removes an existing Instructor from the DbContext and saves it
        /// </summary>
        /// <param name="instructor"></param>
        /// <returns></returns>
        //    public async Task DeleteInstructorAsync(Instructor instructor)
        //    {
        //        try
        //        {
        //            dbContext.Instructor.Remove(instructor);
        //            await dbContext.SaveChangesAsync();
        //        }
        //        catch (Exception)
        //        {
        //            throw;
        //        }
        //    }

        /// <summary>
        /// This method is to delete instructor from the database
        /// This method will also set the CISR object primary or secondary instructor to null depending on the instrcutor passed in
        /// This method will also set the Course object primary or secondary instructor to null depending on the instrcutor passed in
        /// </summary>
        /// <param name="instructor">Instructor to remove</param>
        /// <returns>Lists of instructors</returns>
        public async Task<List<Instructor>> deleteInstructorAsync(Instructor ins)
        {
            List<Instructor> insList = new List<Instructor>();
            try
            {
                dbContext = dbContextFactory.CreateDbContext();

                // set the CISR room to null
                deleteInstructorCisr(ins);
                await dbContext.SaveChangesAsync();
                // set the Courses room to null
                deleteInstructorInCourse(ins);
                await dbContext.SaveChangesAsync();
                dbContext.Remove(ins);
                await dbContext.SaveChangesAsync();

                insList = dbContext.Instructor.ToList();
                await dbContext.DisposeAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Instructor not deleted successfully");
            }

            return insList;
        }
        /// <summary>
        /// This method is to delete Instructors in CISR
        /// This method will set the primary and secondary instructor of the course to null if the instructor passed in is the primary instructor
        /// This method will set the secondary instructor of the course to null if the instructor is secondary instructor
        /// </summary>
        /// <param name="ins">Instructor to remove from the Scheduled course</param>
        public void deleteInstructorCisr(Instructor ins)
        {
            List<CISR> PrimarycisrToChange = new List<CISR>();
            List<CISR> SecondarycisrToChange = new List<CISR>();
            dbContext = dbContextFactory.CreateDbContext();

            // get all the list of cisr that had the passed in instructor as primary instructoor
            PrimarycisrToChange = dbContext.CISR.Where(c => c.PrimaryInstructorID == ins.ID).ToList();
            // get all the list of cisr that had the passed in instructor as secondary instructoor
            SecondarycisrToChange = dbContext.CISR.Where(c => c.SecondaryInstructorID == ins.ID).ToList();

            // iterate through the list of cisr that had this instructor as primary and set the primary and secondary to null
            foreach (CISR cisr in PrimarycisrToChange)
            {
                cisr.PrimaryInstructorID = null;
                cisr.SecondaryInstructorID = null;
                dbContext.Update(cisr);
                
            }

            // iterate through the list of cisr that had this instructor as secondary and set the secondary to null
            foreach (CISR cisr in SecondarycisrToChange)
            {
                cisr.SecondaryInstructorID = null;
                dbContext.Update(cisr);
            }

        }

        /// <summary>
        /// This method is to delete Instructors in Course
        /// This method will set the primary and secondary instructor of the course to null if the instructor passed in is the primary instructor
        /// This method will set the secondary instructor of the course to null if the instructor is secondary instructor
        /// </summary>
        /// <param name="ins">Instructor to remove from the course</param>
        public void deleteInstructorInCourse(Instructor ins)
        {
            dbContext = dbContextFactory.CreateDbContext();
            List<Course> insAsPrimary = new List<Course>();
            List<Course> insAsSecondary = new List<Course>();

            // get all the list of course that had the passed in instructor as primary instructoor
            insAsPrimary = dbContext.Course.Where(c => c.primaryInstructorIDBindable == ins.ID).ToList();
            // get all the list of course that had the passed in instructor as primary instructoor
            insAsSecondary = dbContext.Course.Where(c => c.secondaryInstructorIDBindable == ins.ID).ToList();

            // iterate through the list of course that had the passed in instructor as default and set primary and secondary id to 0
            foreach (Course course in insAsPrimary)
            {
                course.primaryInstructorIDBindable  = 0;
                course.secondaryInstructorIDBindable = 0;
                dbContext.Update(course);
            }

            // iterate through the list of course that had the passed in instructor as default secondary and set the secondary id to 0
            foreach (Course course in insAsSecondary)
            {
                course.secondaryInstructorIDBindable = 0;
                dbContext.Update(course);
            }

        }


        #endregion


        #region CRUD Course
        /// <summary>
        /// Asynchronous Method
        /// <br />Adds the given Course object to the Database
        /// </summary>
        /// <param name="course">The Course to add</param>
        /// <returns>Course</returns>
        public async Task<Course> AddCourseAsync(Course course)
        {
            course.semesterID = this.CurrentSemester.SemesterID;

            for (int i = 0; i < roomList.Count; i++)
            {
                if (roomList[i].ID == course.classroomIDBindable)
                {
                    course.classroomIDBindable = roomList[i].ID;
                }
            }

            for (int i = 0; i < InstructorList.Count; i++)
            {
                if (InstructorList[i].ID == course.primaryInstructorIDBindable)
                {
                    course.primaryInstructorIDBindable = InstructorList[i].ID;
                }

                if (InstructorList[i].ID == course.secondaryInstructorIDBindable)
                {
                    course.secondaryInstructorIDBindable = InstructorList[i].ID;
                }
            }

            DateTime curStartDate = (DateTime)course.startDate;
            DateTime curEndDate = (DateTime)course.endDate;

            course.numOfWeeks = calculateTotalWeeks(course);
            course.totalHours = calculateTotalHours(course.hoursPerWeek, course.numOfWeeks);

            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                await dbContext.Course.AddAsync(course);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await dbContext.DisposeAsync();
            return course;
        }



        /// <summary>
        /// Update course in database
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        public async Task<Course> UpdateCourseAsync(Course course)
        {
            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                dbContext.Course.Update(course);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await dbContext.DisposeAsync();
            return course;
        }

        public async Task<Course> GetCourseAsync(int id)
        {
            Course retVal = new();

            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                retVal = await dbContext.Course.FindAsync(id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return retVal;
        }

        public async Task<List<Course>> GetCourseBySemester(Semester sem)
        {
            List<Course> retVal = new();

            dbContext = dbContextFactory.CreateDbContext();
            retVal = await dbContext.Course.Where(e => e.semesterID == sem.SemesterID)
                .OrderBy(e => e.courseAbbr).ToListAsync<Course>();
            //retVal = dbContext.Set<Course>().Where(c => c.semesterID == CurrentSemester.SemesterID).ToList<Course>();
            await dbContext.DisposeAsync();
            return retVal; //.FindAll(e => e.semesterID == sem.SemesterID);
        }

        public async Task<List<Course>> GetCourseBySemesterDatePoint(Semester sem, DateTime start, DateTime end)
        {
            List<Course> retVal = new();
            dbContext = dbContextFactory.CreateDbContext();

            retVal = await dbContext.Course.Where(e => (e.semesterID == sem.SemesterID)
                     && (e.startDate <= start && e.endDate >= end))
                     .OrderBy(e => e.courseAbbr).ToListAsync<Course>();

            await dbContext.DisposeAsync();
            return retVal;
        }

        public async Task<List<Course>> GetCourseBySemesterWithEndDate(Semester sem, DateTime end)
        {
            List<Course> retVal = new();

            dbContext = dbContextFactory.CreateDbContext();
            retVal = await dbContext.Course.Where(e => e.semesterID == sem.SemesterID)
                .Where(c => c.startDate <= end && c.endDate >= end)
                .OrderBy(e => e.courseAbbr).ToListAsync<Course>();
            //retVal = dbContext.Set<Course>().Where(c => c.semesterID == CurrentSemester.SemesterID).ToList<Course>();
            await dbContext.DisposeAsync();
            return retVal; //.FindAll(e => e.semesterID == sem.SemesterID);
        }
        public async Task<List<Course>> GetAllDepartmentCoursesAsync()
        {
            dbContext.Dispose();

            if (CurrentDepartment != null)
            {
                AllCourses = new List<Course>();
                dbContext = dbContextFactory.CreateDbContext();

                List<Semester> tempSemesterList = await dbContext.Set<Semester>().Where(s => s.deptID == CurrentDepartment.ID).ToListAsync<Semester>();

                foreach (Semester sem in tempSemesterList)
                {
                    List<Course> tempCourses = await dbContext.Set<Course>().Where(c => c.semesterID == sem.SemesterID).OrderBy(e => e.courseAbbr).ToListAsync<Course>();
                    AllCourses.AddRange(tempCourses);
                }

                foreach (Course curCourse in AllCourses)
                {

                    if (curCourse.primaryInstructorIDBindable != 0)
                    {
                        var tempIns = await dbContext.Instructor.FindAsync(curCourse.primaryInstructorIDBindable);
                        curCourse.primaryInstructor = tempIns;
                    }

                    if (curCourse.secondaryInstructorIDBindable != 0)
                    {
                        var tempIns = await dbContext.Instructor.FindAsync(curCourse.secondaryInstructorIDBindable);
                        curCourse.secondaryInstructor = tempIns;
                    }
                }
            }
            dbContext.Dispose();

            this.AllCourses = AllCourses;

            return AllCourses;

        }

        /// <summary>
        /// This method is to find all of the Courses with the Room passed in then Set the room to 0
        /// </summary>
        /// <param name="room"></param>
        public async void deleteRoomInCourse(Room room)
        {
            List<Course> courseToChange = new List<Course>();
            dbContext = dbContextFactory.CreateDbContext();

            // get the list of course that had the passed in room as default value
            courseToChange = dbContext.Course.Where(c => c.classroomIDBindable == room.ID).ToList();

            // iterate through the list of courses that had the passed in room as default value and set the classroomID to 0
            foreach (Course course in courseToChange)
            {
                course.classroomIDBindable = 0;
                dbContext.Update(course);
            }
        }

        /// <summary>
        /// This method is to delete the course passed in from the database
        /// This method will also delete the CISR object of this course
        /// </summary>
        /// <param name="course">Course to delete</param>
        /// <returns>List of courses</returns>
        public async Task<List<Course>> deleteCourseAsync(Course course)
        {
            List<Course> retVal = new List<Course>();
            try
            {
                dbContext = dbContextFactory.CreateDbContext();

                // delete all the Scheduled CISR of this course
                deleteCourseCISRS(course);
                await dbContext.SaveChangesAsync();
                dbContext.Remove(course);
                await dbContext.SaveChangesAsync();

                retVal = await GetAllDepartmentCoursesAsync();
                await dbContext.DisposeAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Course not deleted successfully");
            }



            return retVal;
        }

        #region calculator for course's calculated fields
        public int calculateTotalHours(int hoursperweek, int numOfWeeks)
        {
            int totalHours = 0;
            return totalHours = hoursperweek * numOfWeeks;
        }

        private int calculateTotalWeeks(Course newCourse)
        {
            int totalWeeks = 0;

            if (newCourse.startDate != null && newCourse.endDate != null)
            {
                DateTime start = (DateTime)newCourse.startDate;
                DateTime end = (DateTime)newCourse.endDate;

                TimeSpan lengthInWeeks = end.Subtract(start);

                totalWeeks = (int)(Math.Round(lengthInWeeks.TotalDays) / 7);

                if (CurrentSemester.HasBreak)
                {
                    DateTime breakStart = (DateTime)CurrentSemester.BreakStart;
                    DateTime breakEnd = (DateTime)CurrentSemester.BreakEnd;

                    TimeSpan breakTime = breakEnd.Subtract(breakStart);

                    totalWeeks -= (int)(Math.Round(breakTime.TotalDays) / 7);
                }
            }
            return totalWeeks;
        }
        #endregion

        #endregion


        #region CRUD Semester
        /// <summary>
        /// Asynchronous Method
        /// <br />This method returns a List:Semester containing all of the Semesters from the currently selected Department (this.CurrentDepartment)
        /// </summary>
        /// <returns>List:Semester</returns>
        public async Task<List<Semester>> GetAllSemesters()
        {
            dbContext = dbContextFactory.CreateDbContext();
            List<Semester> tempSemesterList = dbContext.Set<Semester>().Where(s => s.deptID == CurrentDepartment.ID).OrderBy(s => s.SemesterID).ToList<Semester>();
            await dbContext.DisposeAsync();
            return tempSemesterList;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />Returns a Semester that matches the given ID
        /// </summary>
        /// <param name="semID">The ID of the Semester for Find</param>
        /// <returns>Semester</returns>
        public async Task<Semester> GetSemesterById(string semID)
        {
            dbContext = dbContextFactory.CreateDbContext();
            Semester semester = dbContext.Semester.Find(semID);
            await dbContext.DisposeAsync();
            return semester;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />Returns a List:Instructor containing all of the Instructors in the Databse
        /// </summary>
        /// <returns>List:Instructor</returns>
        public async Task<List<Instructor>> GetSemesterInstructorsAsync()
        {
            List<Instructor> returnInstructors = new List<Instructor>();

            if (CurrentDepartment.ID != 0)
            {

                if (this.CurrentCourses == null)
                {
                    await GetSemesterCoursesAsync();
                }

                await dbContext.DisposeAsync();
                dbContext = dbContextFactory.CreateDbContext();
                foreach (Course curCourse in CurrentCourses)
                {

                    if (returnInstructors.Find(x => x.ID == curCourse.primaryInstructorIDBindable) == default && curCourse.primaryInstructorIDBindable != 0)
                    {
                        var tempIns = dbContext.Instructor.Find(curCourse.primaryInstructorIDBindable);
                        curCourse.primaryInstructor = tempIns;
                        returnInstructors.Add(tempIns);
                    }
                    if (returnInstructors.Find(x => x.ID == curCourse.secondaryInstructorIDBindable) == default && curCourse.secondaryInstructorIDBindable != 0)
                    {
                        var tempIns = dbContext.Instructor.Find(curCourse.secondaryInstructorIDBindable);
                        curCourse.secondaryInstructor = tempIns;
                        returnInstructors.Add(tempIns);
                    }
                }

                await dbContext.DisposeAsync();
            }
            CurrentInstructors = returnInstructors;

            return returnInstructors;
        }
        /// <summary>
        /// Asynchronous Method - Void Return, Do Not Await
        /// <br />Updates the semester currently selected in the service (this.CurrentSemester) in the Database
        /// </summary>
        public async void UpdateSemesterAsync()
        {
            await dbContext.DisposeAsync();
            dbContext = dbContextFactory.CreateDbContext();

            dbContext.Semester.Update(CurrentSemester);

            await dbContext.SaveChangesAsync();

            await dbContext.DisposeAsync();
        }

        /// <summary>
        /// Asynchronous Method - Void Return, Do Not Await
        /// <br />This method Creates the given Semester object in the Database
        /// <br />Called by CreateDepartmentSemesters()
        /// </summary>
        /// <param name="newSemester">The semester to create</param>
        private async void CreateSemesterAsync(Semester newSemester)
        {
            await dbContext.DisposeAsync();
            dbContext = dbContextFactory.CreateDbContext();
            var semesterExists = dbContext.Semester.Where(x => x.SemesterID == newSemester.SemesterID).Count() == 0 ? false : true;

            if (!semesterExists)
            {
                await dbContext.Semester.AddAsync(newSemester);
            }
            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        /// <summary>
        /// This method creates the Semesters for a given Department based on the number of
        /// <br />Years, Semesters, and Student Groups in the given Department
        /// </summary>
        /// <param name="newDept">The Department to create Semesters for</param>
        private void CreateDepartmentSemesters(Department newDept)
        {
            int year;
            int semester;
            int group;

            List<Semester> allSemesters = new List<Semester>();
            this.CurrentDepartment = newDept;

            for (int y = 0; y < newDept.lengthInYears; y++)
            {
                year = y + 1;
                for (int s = 0; s < newDept.semesterCount; s++)
                {
                    semester = s + 1;
                    for (int g = 0; g < newDept.ProgramNumberOfGroups; g++)
                    {
                        group = g + 1;

                        Semester newSemester = new Semester
                        {
                            deptID = newDept.ID,
                            SemesterID = newDept.ID + "," + year + "," + semester + "," + group,
                            StartDate = newDept.startDate,
                            EndDate = newDept.EndDate,
                            StartTime = 8,
                            EndTime = 15,
                            HasBreak = false,
                            BreakStart = null,
                            BreakEnd = null
                        };
                        CreateSemesterAsync(newSemester);
                    }
                }
            }
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />This method returns a Semester based on the currently selected
        /// <br />Department, Year, Semester, and Student Group within the service
        /// </summary>
        /// <returns>Senester</returns>
        public async Task<Semester> GetSemesterAsync()
        {
            if (this.CurrentDepartment == null)
            {
                await GetDepartmentAsync();
                this.CurrentDepartment = this.departmentList[0];
            }

            Semester returnSemester = new Semester();

            if (dbContext != null)
            {
                await dbContext.DisposeAsync();
            }

            dbContext = dbContextFactory.CreateDbContext();

            if (CurrentDepartment.ID != 0)
            {
                string sKey = CurrentDepartment.ID + ","
                    + this.YearSelected + ","
                    + this.SemesterSelected + ","
                    + this.StudentGroupSelected;

                // The semester to find
                returnSemester = await dbContext.Semester.SingleAsync(x => x.SemesterID == sKey);

                await dbContext.DisposeAsync();

                CurrentSemester = returnSemester;
            }

            return returnSemester;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />This method returns a List:Course that contains all of the courses for the currently selected Semester in the service (this.CurrentSemester)
        /// </summary>
        /// <returns>List:Course</returns>
        public async Task<List<Course>> GetSemesterCoursesAsync()
        {
            if (CurrentSemester == null)
            {
                await GetSemesterAsync();
            }

            List<Course> returnCourses = new List<Course>();
            List<Instructor> tempInstructors = new List<Instructor>();


            if (CurrentDepartment.ID != 0)
            {
                await dbContext.DisposeAsync();
                dbContext = dbContextFactory.CreateDbContext();

                returnCourses = dbContext.Set<Course>().Where(c => c.semesterID == CurrentSemester.SemesterID).OrderBy(e => e.courseAbbr).ToList<Course>();

                CurrentCourses = returnCourses;

                foreach (Course curCourse in CurrentCourses)
                {
                    var tempIns1 = tempInstructors.Find(x => x.ID == curCourse.primaryInstructorIDBindable);
                    if (tempIns1 == default && curCourse.primaryInstructorIDBindable != 0)
                    {
                        tempIns1 = dbContext.Instructor.Find(curCourse.primaryInstructorIDBindable);
                        curCourse.primaryInstructor = tempIns1;
                        tempInstructors.Add(tempIns1);
                    }
                    else
                    {
                        curCourse.primaryInstructor = tempIns1;
                    }

                    var tempIns2 = tempInstructors.Find(x => x.ID == curCourse.secondaryInstructorIDBindable);
                    if (tempIns2 == default && curCourse.secondaryInstructorIDBindable != 0)
                    {
                        tempIns2 = dbContext.Instructor.Find(curCourse.secondaryInstructorIDBindable);
                        curCourse.secondaryInstructor = tempIns2;
                        tempInstructors.Add(tempIns2);
                    }
                    else
                    {
                        curCourse.secondaryInstructor = tempIns2;
                    }
                }

                await dbContext.DisposeAsync();
            }
            return returnCourses;
        }

        #endregion


        #region CRUD CISR

        /// <summary>
        /// This asynchronous method will return a List of CISR objects that are
        /// 1) Linked to the given semester
        /// 2) Are between both the start and end paramateters
        /// </summary>
        /// <param name="start"><DateTime>The start of the range to look for CISR objects</param>
        /// <param name="end"><DateTime>The end of the range to look for CISR objects</param>
        /// <param name="sem"><Semester>The semester for the relevant CISR objects</param>
        /// <returns>List:CISR</returns>
        public async Task<List<CISR>> GetDatePointCISR(DateTime start, DateTime end, Semester sem)
        {
            List<CISR> returnList = new List<CISR>();

            dbContext = dbContextFactory.CreateDbContext();

            returnList = await dbContext.Course.Where(c => c.semesterID == sem.SemesterID)
                .Where(c => c.startDate >= start && c.endDate <= end)
                .Include(c => c.cisrList)
                .SelectMany(c => c.cisrList)
                .ToListAsync();

            await dbContext.DisposeAsync();

            return returnList;
        }

        /// <summary>
        /// This asynchronous method returns all of the CISR objects from the given semester
        /// AND that have Start and End dates that match the semesters start and end dates
        /// </summary>
        /// <param name="sem"></param>
        /// <returns>List:CISR</returns>
        public async Task<List<CISR>> GetSemesterCISR(Semester sem)
        {
            List<CISR> returnList = new List<CISR>();

            dbContext = dbContextFactory.CreateDbContext();

            returnList = await dbContext.Course.Where(c => c.semesterID == sem.SemesterID)
                .Where(c => c.startDate == sem.StartDate && c.endDate == sem.EndDate)
                .Include(c => c.cisrList)
                .SelectMany(c => c.cisrList)
                .ToListAsync();

            await dbContext.DisposeAsync();

            return returnList;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />This method will take an action in the databasea with the given CISR object
        /// <br />1) If the CISR has already been created in the database, but all of the other fields are set to 0 (null), delete the CISR
        /// <br />2) If The CISR exists (ID > 0) and at least one field is not 0 (null), update the CISR
        /// <br />3) If the CISR does not exist (ID == 0), and at least one field is not 0 (null), create the CISR
        /// </summary>
        /// <param name="tempCisr">CISR</param>
        public async void SaveCISR(CISR tempCisr)
        {

            dbContext = dbContextFactory.CreateDbContext();
            if (tempCisr.ID != 0 && tempCisr.roomIDBindable == 0 && tempCisr.primaryInstructorIDBindable == 0 && tempCisr.secondaryInstructorIDBindable == 0 && tempCisr.courseIDBindable == 0)
            {
                dbContext.Remove(tempCisr);
            }
            else if (tempCisr.ID > 0 && (tempCisr.roomIDBindable != 0 || tempCisr.primaryInstructorIDBindable != 0 || tempCisr.secondaryInstructorIDBindable != 0 || tempCisr.courseIDBindable != 0))
            {
                dbContext.Update(tempCisr);
            }
            else if (tempCisr.roomIDBindable != 0 || tempCisr.primaryInstructorIDBindable != 0 || tempCisr.secondaryInstructorIDBindable != 0 || tempCisr.courseIDBindable != 0)
            {
                await dbContext.AddAsync(tempCisr);
            }

            await dbContext.SaveChangesAsync();
            await dbContext.DisposeAsync();
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />This method will return a List of unique Datetimes for a semester based on that semesters courses
        /// <br />This list will include the semesters start and end date
        /// </summary>
        /// <param name="sem">The semester to retrieve the dates from</param>
        /// <returns>List:DateTime</returns>
        public async Task<List<DateTime?>> GetSemesterDatePoints(Semester sem)
        {
            List<DateTime?> returnList = new List<DateTime?>();

            dbContext = dbContextFactory.CreateDbContext();


            returnList = dbContext.Course.Where(c => c.semesterID == sem.SemesterID)
                    .Select(d => d.endDate)
                    .Distinct()
                    .ToList();

            returnList.AddRange(dbContext.Course.Where(c => c.semesterID == sem.SemesterID)
                    .Select(d => d.startDate)
                    .Distinct()
                    .ToList());

            await dbContext.DisposeAsync();

            returnList.Add(sem.StartDate);
            returnList.Add(sem.EndDate);

            returnList.Sort();

            return returnList.Distinct().ToList();
        }
        /// <summary>
        /// Find CISR that previously occupied time that passed in cisr is trying to use
        /// </summary>
        /// <param name="cisr">CISR to check agains</param>
        /// <returns>Previous CISR in that timeslot, else default CISR</returns>
        public async Task<CISR> GetCisrOldAsync(CISR cisr)
        {
            if (cisr.ID != 0)
            {
                try
                {
                    dbContext = dbContextFactory.CreateDbContext();
                    return await dbContext.CISR.FirstOrDefaultAsync(e => e.ID == cisr.ID);
                }
                catch (Exception e)
                {
                    throw new ApplicationException("Error while retrieving old CISR.");
                }
                finally
                {
                    await dbContext.DisposeAsync();
                }
            }
            return new CISR
            {
                ID = 0,
                Day = cisr.Day,
                Time = cisr.Time,
                primaryInstructorIDBindable = 0,
                secondaryInstructorIDBindable = 0,
                courseIDBindable = 0,
                roomIDBindable = 0
            };
        }

        /// <summary>
        /// This method is to find all the CISR with the passed in room as default then set the room of the CISR to null
        /// </summary>
        /// <param name="room"></param>
        public async void deleteRoomCISRS(Room room)
        {
            List<CISR> cisrToChange = new List<CISR>();
            dbContext = dbContextFactory.CreateDbContext();
            // get all the cisr that had this room as default
            cisrToChange = dbContext.CISR.Where(c => c.RoomID == room.ID).ToList();
                
            // iterate through the list of cisr that had this room as default value and set the roomID and room to null
            foreach(CISR cisr in cisrToChange)
            {
                cisr.RoomID = null;
                cisr.room = null;
                dbContext.Update(cisr);
            }
        }

        /// <summary>
        /// This method is to delete all Scheduled Courses of the course passed in
        /// </summary>
        /// <param name="course">Scheduled course to delete</param>
        public async void deleteCourseCISRS(Course course)
        {
            dbContext.CISR.RemoveRange(dbContext.CISR.Where(c => c.CourseID == course.ID).ToList());
        }



            #endregion

            #region CRUD Program ('Department')

            /// <summary>
            /// Returns The currently selected department within the database service, CurrentDepartment
            /// <br />If there is no selected department, runs the method GetDepartmentsAsync()
            /// </summary>
            /// <returns>Department</returns>
            public Department GetCurrentDepartment()
        {
            if (this.CurrentDepartment != null)
            {
                return this.CurrentDepartment;
            }
            else
            {
                GetDepartmentAsync();
                this.CurrentDepartment = this.departmentList[0];
                return this.CurrentDepartment;
            }
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />Returns a List of Departments in the database, AND sets the value of this.departmentList to the returned list
        /// <br />This method can return a specific number of Departments if the variables Start:Int and Count:Int are supplies
        /// </summary>
        /// <param name="start">Offset to use (not required - default of 0)</param>
        /// <param name="count">Number of items to return (not required - default of 0)</param>
        /// <returns>List:Department</returns>
        public async Task<List<Department>> GetDepartmentAsync(int start = 0, int count = 0)
        {
            List<Department> returnVal;
            dbContext = dbContextFactory.CreateDbContext();
            if (count <= 0)
            {
                returnVal = await dbContext.Department.OrderBy(x => x.departmentName).ToListAsync();
            }
            else
            {
                returnVal = await dbContext.Department.OrderBy(x => x.departmentName).Skip(start).Take(count).ToListAsync();
            }

            await dbContext.DisposeAsync();
            this.departmentList = returnVal;
            return returnVal;
        }
        /// <summary>
        /// Asyncrhonous Method
        /// <br />Adds the given Department object to the Database
        /// </summary>
        /// <param name="department"></param>
        /// <returns>Department</returns>
        public async Task<Department> AddDepartmentAsync(Department department)
        {
            dbContext = dbContextFactory.CreateDbContext();
            await dbContext.Department.AddAsync(department);
            await dbContext.SaveChangesAsync();
            this.departmentList.Add(department);
            CreateDepartmentSemesters(department);
            depSuccess = true;

            await dbContext.DisposeAsync();

            return department;
        }

        /// <summary>
        /// Updates the given Department object in the Database
        /// </summary>
        /// <param name="dep"></param>
        /// <returns>Department</returns>
        public async Task<Department> EditDepartmentAsync(Department dep)
        {
            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                dbContext.Update(dep);
                await dbContext.SaveChangesAsync();

            }
            catch (Exception e)
            {
                throw new ApplicationException("error editing department");
            }
            finally
            {
                await dbContext.DisposeAsync();
            }

            return dep;
        }
        /// <summary>
        /// Asynchronous method
        /// <br />Returns a Department object from the Databse with the given ID
        /// <br />Sets the service variable this.tempDep to the returned Department 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Department</returns>
        public async Task<Department> GetDepartmentOnIdAsync(int id)
        {
            await dbContext.DisposeAsync();
            dbContext = dbContextFactory.CreateDbContext();
            Department d = await dbContext.Department.FindAsync(id);
            this.tempDep = d;
            await dbContext.DisposeAsync();
            return d;
        }
        #endregion


        #region CRUD Program ('Room')

        /// <summary>
        /// Asyncrhonous Method
        /// <br /> Returns a List of Rooms from the Database
        /// <br /> Sets the service variable this.roomList to the returned List
        /// </summary>
        /// <returns>List:Room</returns>
        public async Task<List<Room>> GetRoomAsync()
        {

            this.roomList.Clear();
            dbContext = dbContextFactory.CreateDbContext();

            this.roomList = await dbContext.Room.OrderBy(e => e.roomNumber).ToListAsync();

            await dbContext.DisposeAsync();

            return this.roomList;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />Returns a Room object with the given ID
        /// variable
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Room</returns>
        public async Task<Room> GetRoomOnIdAsync(int id)
        {
            await dbContext.DisposeAsync();
            dbContext = dbContextFactory.CreateDbContext();
            Room depRoom = await dbContext.Room.FindAsync(id);
            await dbContext.DisposeAsync();
            return depRoom;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />Adds the given Room object to the Databse
        /// </summary>
        /// <param name="room"></param>
        /// <returns>Room</returns>
        public async Task<Room> AddRoomAsync(Room room)
        {

            try
            {
                dbContext = dbContextFactory.CreateDbContext();
                await dbContext.Room.AddAsync(room);
                await dbContext.SaveChangesAsync();
                await dbContext.DisposeAsync();

                //calls get room to reset roomList and unassignedRooms
                GetRoomAsync();

                //sets to true
                roomSuccess = true;
                return room;
            }
            catch (Exception e)
            {
                //if something goes wrong, make sure success = false
                roomSuccess = false;
            }
            roomSuccess = false;

            return room;
        }

        /// <summary>
        /// Asynchronous Method
        /// <br />Updates the given Room object in the Database
        /// </summary>
        /// <param name="room"></param>
        /// <returns>Room</returns>
        public async Task<Room> EditRoomAsync(Room room)
        {

            try
            {
                dbContext = dbContextFactory.CreateDbContext();


                dbContext.Update(room);
                await dbContext.SaveChangesAsync();
                GetRoomAsync();

                roomSuccess = true;

            }
            catch (Exception e)
            {
                roomSuccess = false;
            }



            await dbContext.DisposeAsync();
            return room;
        }


        /// <summary>
        /// This method is to delete the passed in Room from the database
        /// The method will also set the CISR object that had this room as default to null
        /// The method will also set the Course object that had this room as default to null
        /// </summary>
        /// <param name="room">Room to delete</param>
        /// <returns>list of rooms</returns>
        public async Task<List<Room>> deleteRoomAsync(Room room)
        {
            List<Room> retRoom = new List<Room>();
            try
            {
                dbContext = dbContextFactory.CreateDbContext();

                // set the CISR room to null
                deleteRoomCISRS(room);
                await dbContext.SaveChangesAsync();
                // set the Courses room to null
                deleteRoomInCourse(room);
                await dbContext.SaveChangesAsync();
                dbContext.Remove(room);
                await dbContext.SaveChangesAsync();

                retRoom = dbContext.Room.ToList();
                await dbContext.DisposeAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Room not deleted successfully");
            }

            return retRoom;
        }
        #endregion
        #region ScheduleErrorChecking
        /// <summary>
        /// Checks room, instructor, and over hour conflict/errors for a given CISR against current database
        /// This will assume that previous errors should no longer display. Remove if checks to disable this functionality
        /// </summary>
        /// <param name="cisr">Item to check for conflicts</param>
        /// <returns>List of all conflicts</returns>
        public async Task<List<string>> CheckAllConflicts(CISR cisr)
        {
            List<string> retList = new();

            #region ExtraChecks, Old
            //CISR oldCisr = await GetCisrOldAsync(cisr);

            //// compare vs old cisr, if null or default returned we do not have to do any checking
            //// only need to check for conflicts if something changed and is not null/empty
            //if(oldCisr != null && oldCisr.ID > 0)
            //{
            //    // if both had a primary instructor and they are not the same
            //    if(oldCisr.primaryInstructor != null && cisr.primaryInstructor != null && oldCisr.primaryInstructor.ID != cisr.primaryInstructor.ID)
            //    {
            //        retList = await CheckPrimaryInstructorConflicts(cisr, retList);
            //    }

            //    // if both had a secondary instructor and they are not the same
            //    if(oldCisr.secondaryInstructor != null && cisr.secondaryInstructor != null && oldCisr.secondaryInstructor.ID != cisr.secondaryInstructor.ID)
            //    {
            //        retList = await CheckSecondaryInstructorConflicts(cisr, retList);
            //    }

            //    // if both had a room set
            //    if(oldCisr.room != null && cisr.room != null && oldCisr.room.ID != cisr.room.ID)
            //    {
            //        retList = await CheckRoomConflicts(cisr, retList);
            //    }
            //}

            //// should do overhour check if different cisr id
            //if(oldCisr != null && oldCisr.ID != cisr.ID)
            //{
            //    retList = await CheckOverHours(cisr, retList);
            //}
            #endregion

            retList = await CheckPrimaryInstructorConflicts(cisr, retList);
            retList = await CheckSecondaryInstructorConflicts(cisr, retList);
            retList = await CheckRoomConflicts(cisr, retList);
            retList = await CheckOverHours(cisr, retList);

            return retList;
        }

        /// <summary>
        /// Adds all primary instructor conflicts to list passed in
        /// </summary>
        /// <param name="cisr">Item to check for instructor conflicts</param>
        /// <param name="retList">List passed in</param>
        /// <returns>Original list with any new conflicts appended</returns>
        public async Task<List<string>> CheckPrimaryInstructorConflicts(CISR cisr, List<string> retList)
        {
            // Only check conflicts if there is a Primary Instructor
            /** TO DO: maybe check if instructor has even changed **/
            if (cisr.primaryInstructorIDBindable != 0)
            {
                /** make sure to explicitly load, and only taking in the ID's, not the whole cisr **/

                // Get Course from the passed in CISR
                dbContext = dbContextFactory.CreateDbContext();
                Course cFromCISR = await GetCourseAsync(cisr.courseIDBindable);

                // Get Instructor from the passed in CISR
                dbContext = dbContextFactory.CreateDbContext();
                Instructor curIns = await GetInstructorsAsync(cisr.primaryInstructorIDBindable);

                // Get ALL CISRs using this Instructor, ignoring date
                dbContext = dbContextFactory.CreateDbContext();
                List<CISR> cisrList = await dbContext.CISR
                    .Where(e => e.Day == cisr.Day && e.Time == cisr.Time &&
                                (cisr.primaryInstructorIDBindable == e.PrimaryInstructorID.Value ||
                                cisr.primaryInstructorIDBindable == e.SecondaryInstructorID.Value) &&
                                cisr.ID != e.ID)
                    // Include course from query because it doesn't by default
                    .Include(e => e.course).ToListAsync();
                await dbContext.DisposeAsync();

                // Only keep the CISRs whose dates overlap.
                retList.AddRange(
                    cisrList.Where(c => c.course.startDate <= cFromCISR.endDate && c.course.endDate >= cFromCISR.startDate)
                    // Build a List<string> of conflicts to return
                    .Select(c =>
                    {
                        // Loop through, find conflicts, add conflicts to the List<string>
                        DateTime d1 = (DateTime)(c.course.startDate < cFromCISR.startDate ? cFromCISR.startDate : c.course.startDate);
                        DateTime d2 = (DateTime)(c.course.endDate > cFromCISR.endDate ? cFromCISR.endDate : c.course.endDate);
                        return $"{ curIns.calcName } already teaches { c.course.courseAbbr } from { d1.ToString(errFormat) } to { d2.ToString(errFormat) }";
                    }).ToArray<string>());
            }
            return retList;
        }

        /// <summary>
        /// Adds all secondary conflicts to list passed in
        /// </summary>
        /// <param name="cisr">Item to check for instructor conflicts</param>
        /// <param name="retList">List passed in</param>
        /// <returns>Original list with any new conflicts appended</returns>
        public async Task<List<string>> CheckSecondaryInstructorConflicts(CISR cisr, List<string> retList)
        {
            // Only check conflicts if there is a secondary Instructor
            /** TO DO: maybe check if instructor has even changed **/
            if (cisr.secondaryInstructorIDBindable != 0)
            {
                /** make sure to explicitly load, and only taking in the ID's, not the whole cisr **/

                // Get Course from the passed in CISR
                dbContext = dbContextFactory.CreateDbContext();
                Course cFromCISR = await GetCourseAsync(cisr.courseIDBindable);

                // Get Instructor from the passed in CISR
                dbContext = dbContextFactory.CreateDbContext();
                Instructor curIns = await GetInstructorsAsync(cisr.secondaryInstructorIDBindable);

                // Get ALL CISRs using this Instructor, ignoring date
                dbContext = dbContextFactory.CreateDbContext();
                List<CISR> cisrList = await dbContext.CISR
                    .Where(e => e.Day == cisr.Day && e.Time == cisr.Time &&
                                (cisr.secondaryInstructorIDBindable == e.PrimaryInstructorID.Value ||
                                cisr.secondaryInstructorIDBindable == e.SecondaryInstructorID.Value) &&
                                cisr.ID != e.ID)
                    // Include course from query because it doesn't by default
                    .Include(e => e.course).ToListAsync();
                await dbContext.DisposeAsync();

                // Only keep the CISRs whose dates overlap.
                retList.AddRange(
                    cisrList.Where(c => c.course.startDate <= cFromCISR.endDate && c.course.endDate >= cFromCISR.startDate)
                    // Build a List<string> of conflicts to return
                    .Select(c =>
                    {
                        // Loop through, find conflicts, add conflicts to the List<string>
                        DateTime d1 = (DateTime)(c.course.startDate < cFromCISR.startDate ? cFromCISR.startDate : c.course.startDate);
                        DateTime d2 = (DateTime)(c.course.endDate > cFromCISR.endDate ? cFromCISR.endDate : c.course.endDate);
                        return $"{ curIns.calcName } already teaches { c.course.courseAbbr } from { d1.ToString(errFormat) } to { d2.ToString(errFormat) }";
                    }).ToArray<string>());
            }
            return retList;
        }


        /// <summary>
        /// Adds all room conflicts to list passed in
        /// </summary>
        /// <param name="cisr">Item to check for room conflicts</param>
        /// <param name="retList">List passed in</param>
        /// <returns>Original list with any new conflicts appended</returns>
        public async Task<List<string>> CheckRoomConflicts(CISR cisr, List<string> retList)
        {
            if (cisr.roomIDBindable != 0)
            {
                dbContext = dbContextFactory.CreateDbContext();
                List<CISR> cisrList = await dbContext.CISR
                    .Where(e => e.Day == cisr.Day && e.Time == cisr.Time &&
                                cisr.roomIDBindable == e.RoomID &&
                                cisr.ID != e.ID)
                    .Include(e => e.course).ToListAsync();

                await dbContext.DisposeAsync();

                Course cFromCISR = await GetCourseAsync(cisr.courseIDBindable);
                Room curRoom = await GetRoomOnIdAsync(cisr.roomIDBindable);

                // Only keep the CISRs whose dates overlap.
                retList.AddRange(
                    cisrList.Where(c => c.course.startDate <= cFromCISR.endDate && c.course.endDate >= cFromCISR.startDate)
                    // Build a List<string> of conflicts to return
                    .Select(c =>
                    {
                        // Loop through, find conflicts, add conflicts to the List<string>
                        DateTime d1 = (DateTime)(c.course.startDate < cFromCISR.startDate ? cFromCISR.startDate : c.course.startDate);
                        DateTime d2 = (DateTime)(c.course.endDate > cFromCISR.endDate ? cFromCISR.endDate : c.course.endDate);
                        return $"{ curRoom.roomNumber } scheduled with { c.course.courseAbbr } from { d1.ToString(errFormat) } to { d2.ToString(errFormat) }";
                    }).ToArray<string>());
            }
            return retList;
        }
        /// <summary>
        /// Adds all over hour errors to list of conflicts
        /// </summary>
        /// <param name="cisr">Item to check for course hour errors</param>
        /// <param name="retList">List passed in</param>
        /// <returns>Original list with any new conflicts appended</returns>
        public async Task<List<string>> CheckOverHours(CISR cisr, List<string> retList)
        {
            if (cisr.courseIDBindable != 0)
            {
                // so currently a course can only exist within its parent semester, not just within the same time frame as a semester
                // due to this, I am only needing to check the count
                dbContext = dbContextFactory.CreateDbContext();
                //get instance of course from cisr
                Course curCourse = await dbContext.Course
                    .Include(e => e.cisrList)
                    .FirstOrDefaultAsync(e => e.ID == cisr.courseIDBindable);

                // if this cisr is already in the db, do not add one to potential count!
                bool isPreviouslySaved = false;
                foreach (CISR c in curCourse.cisrList)
                {
                    if (c.ID == cisr.ID)
                    {
                        isPreviouslySaved = true;
                        break;
                    }
                }

                // potential hours if added
                int hourCountIfAdded = curCourse.cisrList.Count() + (isPreviouslySaved ? 0 : 1);
                await dbContext.DisposeAsync();


                if (!isPreviouslySaved && hourCountIfAdded > curCourse.hoursPerWeek)
                {
                    retList.Add($"{ curCourse.courseAbbr } overbooking, { hourCountIfAdded } / { curCourse.hoursPerWeek } hours");
                }
            }
            return retList;
        }

        #endregion

    }
}
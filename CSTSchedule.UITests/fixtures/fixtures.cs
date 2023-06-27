
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace CSTSchedule.UITests.fixtures
{
    public class fixtures
    {
        // Change this to change the Sleep time used within the UI tests
        public const int TEST_WAIT = 500;
        //make course
        public List<Course> makeCourses()
        {
            return new List<Course>()
            {
				//users are added out of sequence to test sorting logic in controller
                new Course(){
                    ID=0,
                    classroomIDBindable=1,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=1,
                    courseName="COSC",
                    courseAbbr="COSC292",
                    hoursPerWeek=23,
                    creditUnits=4,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course(){
                    ID=0,
                    classroomIDBindable=2,
                    semesterID="1,1,1,2",
                    primaryInstructorIDBindable=2,
                    secondaryInstructorIDBindable=3, // added for secondary to primary error throwing
                    courseName="COSC",
                    courseAbbr="COSC295",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=3,
                    semesterID="1,1,2,1",
                    primaryInstructorIDBindable=3,
                    courseName="COOS",
                    courseAbbr="COOS293",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=4,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=3,
                    courseName="fullLengthCourseA",
                    courseAbbr="fullLengthCourseA",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=5,
                    semesterID="1,1,1,2",
                    primaryInstructorIDBindable=3,
                    courseName="fullLengthCourseB",
                    courseAbbr="fullLengthCourseB",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=5,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=3,
                    courseName="endEarlyCourse",
                    courseAbbr="endEarlyCourse",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 9, 5),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=5,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=3,
                    courseName="startLateCourse",
                    courseAbbr="startLateCourse",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 8),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=1,
                    secondaryInstructorIDBindable=3,
                    courseName="secInsBusyCourse",
                    courseAbbr="secInsBusyCourse",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=1,
                    secondaryInstructorIDBindable=4,
                    courseName="secInsCourseA",
                    courseAbbr="secInsCourseA",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    semesterID="1,1,1,2",
                    primaryInstructorIDBindable=4,
                    courseName="secInsCourseB",
                    courseAbbr="secInsCourseB",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    semesterID="1,1,1,2",
                    primaryInstructorIDBindable=2,
                    secondaryInstructorIDBindable=4,
                    courseName="secInsCourseC",
                    courseAbbr="secInsCourseC",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=1,
                    semesterID="1,1,1,1",
                    courseName="roomTestCourseA",
                    courseAbbr="roomTestCourseA",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=1,
                    semesterID="1,1,1,2",
                    courseName="roomTestCourseB",
                    courseAbbr="roomTestCourseB",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    semesterID="1,1,1,1",
                    courseName="twoHourCourse",
                    courseAbbr="twoHourCourse",
                    hoursPerWeek=2,
                    creditUnits=2,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=5,
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=3,
                    secondaryInstructorIDBindable=4,
                    courseName="manyConflictCourseA",
                    courseAbbr="manyConflictCourseA",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
                new Course()
                {
                    ID=0,
                    classroomIDBindable=5,
                    semesterID="1,1,1,2",
                    primaryInstructorIDBindable=3,
                    secondaryInstructorIDBindable=4,
                    courseName="manyConflictCourseB",
                    courseAbbr="manyConflictCourseB",
                    hoursPerWeek=23,
                    creditUnits=5,
                    startDate= new DateTime(2021, 09, 1),
                    endDate=new DateTime(2021, 12, 30),
                },
            };
        }

        public List<Semester> makeSemesters(List<Department> department)
        {
            List<Semester> semList = new();
            for (int i = 0; i < department.Count(); i++)
            {
                for (int j = 1; j <= department[i].semesterCount + 1; j++)
                {
                    Semester sem = new Semester()
                    {
                        SemesterID = department[i].ID + ",1," + j + ",1",
                        deptID = department[i].ID,
                        StartTime = 8,
                        EndTime = 15,
                        HasBreak = false,
                        StartDate = new DateTime(2021, 09, 1),
                        EndDate = new DateTime(2021, 12, 30)
                    };
                    semList.Add(sem);

                    sem = new Semester()
                    {
                        SemesterID = department[i].ID + ",1," + j + ",2",
                        deptID = department[i].ID,
                        StartTime = 8,
                        EndTime = 15,
                        HasBreak = false,
                        StartDate = new DateTime(2021, 09, 1),
                        EndDate = new DateTime(2021, 12, 30)
                    };
                    semList.Add(sem);

                    sem = new Semester()
                    {
                        SemesterID = department[i].ID + ",2," + j + ",1",
                        deptID = department[i].ID,
                        StartTime = 8,
                        EndTime = 15,
                        HasBreak = false,
                        StartDate = new DateTime(2022, 01, 1),
                        EndDate = new DateTime(2022, 03, 30)
                    };
                    semList.Add(sem);
                    sem = new Semester()
                    {
                        SemesterID = department[i].ID + ",2," + j + ",2",
                        deptID = department[i].ID,
                        StartTime = 8,
                        EndTime = 15,
                        HasBreak = false,
                        StartDate = new DateTime(2022, 01, 1),
                        EndDate = new DateTime(2022, 03, 30)
                    };
                    semList.Add(sem);
                    sem = new Semester()
                    {
                        SemesterID = department[i].ID + ",3," + j + ",1",
                        deptID = department[i].ID,
                        StartTime = 8,
                        EndTime = 15,
                        HasBreak = false,
                        StartDate = new DateTime(2022, 04, 1),
                        EndDate = new DateTime(2022, 05, 30)
                    };
                    semList.Add(sem);
                    sem = new Semester()
                    {
                        SemesterID = department[i].ID + ",3," + j + ",2",
                        deptID = department[i].ID,
                        StartTime = 8,
                        EndTime = 15,
                        HasBreak = false,
                        StartDate = new DateTime(2022, 04, 1),
                        EndDate = new DateTime(2022, 05, 30),
                    };
                    semList.Add(sem);
                }
            }
            return semList;
        }
        //make CISR
        public List<CISR> makeCISR(List<Semester> semList)
        {
            List<CISR> list = new List<CISR>();

            return list;
        }

        //Make Departments
        public List<Department> makeDepartment()
        {
            return new List<Department>()
            {
                new Department()
                {
                    ID=1,
                    departmentName="CST",
                    semesterCount=2,
                    startDate= new DateTime(2021, 09, 1),
                    EndDate=new DateTime(2022, 6, 1),
                },
                new Department()
                {
                    ID=2,
                    departmentName="Electrical Engineering",
                    semesterCount=2,
                    startDate= new DateTime(2021, 09, 1),
                    EndDate=new DateTime(2022, 6, 1),
                },
                new Department()
                {
                    ID=3,
                    departmentName="DMAT",
                    semesterCount=2,
                    startDate= new DateTime(2021, 09, 1),
                    EndDate=new DateTime(2022, 6, 1),
                }
            };
        }

        //Make Instructors
        public List<Instructor> makeIns()
        {
            return new List<Instructor>()
            {
				//uaers are added out of sequence to test sorting logic in controller
                new Instructor(){
                    ID=1,
                    fName="Ernesto",
                    lName="Basoalto"
                },
                new Instructor(){
                    ID=2,
                    fName="Wade",
                    lName="Lahoda"
                },
                new Instructor(){
                    ID=3,
                    fName="Bryce",
                    lName="Barrie"
                },
                new Instructor(){
                    ID=4,
                    fName="Ron",
                    lName="New"
                },
                new Instructor(){
                    ID=5,
                    fName="Rob",
                    lName="Miller"
                }
            };
        }

        //Make Rooms
        public List<Room> makeRoom()
        {
            return new List<Room>()
            {
				//uaers are added out of sequence to test sorting logic in controller
                new Room(){
                    ID=1,
                    roomNumber="230a",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=2,
                    roomNumber="230b",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=3,
                    roomNumber="430",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=4,
                    roomNumber="239A",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=5,
                    roomNumber="239B",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                }
            };
        }

        public void Load(CstScheduleDbContext dbcontext)
        {


            List<Department> departments = makeDepartment();
            List<Instructor> instructors = makeIns();
            List<Room> rooms = makeRoom();
            List<Course> courses = makeCourses();
            List<Semester> semesters = makeSemesters(departments);
            List<CISR> cisr = makeCISR(semesters);


            dbcontext.Department.AddRange(departments);
            dbcontext.Semester.AddRange(semesters);
            dbcontext.Instructor.AddRange(instructors);
            dbcontext.Room.AddRange(rooms);
            dbcontext.Course.AddRange(courses);
            dbcontext.CISR.AddRange(cisr);
            dbcontext.SaveChanges();
        }

        //PREFERRED - clear out old/modified data before starting new tests
        public void Reload(CstScheduleDbContext dbcontext)
        {
            Unload(dbcontext);
            Load(dbcontext);
        }

        public void Unload(CstScheduleDbContext dbcontext)
        {
            dbcontext.Instructor.RemoveRange(dbcontext.Instructor);
            dbcontext.Room.RemoveRange(dbcontext.Room);
            dbcontext.Course.RemoveRange(dbcontext.Course);
            dbcontext.CISR.RemoveRange(dbcontext.CISR);
            dbcontext.Semester.RemoveRange(dbcontext.Semester);
            dbcontext.Department.RemoveRange(dbcontext.Department);
            dbcontext.SaveChanges();
        }
    }
}
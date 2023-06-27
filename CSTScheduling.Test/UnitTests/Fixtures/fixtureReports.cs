using CSTScheduling.Data.Models;
using CSTScheduling.Data.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSTScheduling.Test.UnitTests.fixtures
{
    class fixtureReports
    {
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
                    semesterID="1,1,1,1",
                    primaryInstructorIDBindable=2,
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
                    semesterID="1,1,1,1",
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
                    courseName="COOS",
                    courseAbbr="COOS291",
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
                    courseName="COOS",
                    courseAbbr="COOS292",
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
                    courseName="TCOM",
                    courseAbbr="TCOM202",
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
            };
        }

        public List<Semester> makeSemesters(List<Department> department)
        {
            List<Semester> semList = new();
            Semester sem = new Semester()
            {
                SemesterID = department[0].ID + ",1," + "1" + ",1",
                deptID = department[0].ID,
                StartTime = 8,
                EndTime = 9,
                HasBreak = false,
                StartDate = new DateTime(2021, 09, 1),
                EndDate = new DateTime(2021, 12, 30)
            };
            semList.Add(sem);

            sem = new Semester()
            {
                SemesterID = department[0].ID + ",1," + "1" + ",1",
                deptID = department[0].ID,
                StartTime = 9,
                EndTime = 10,
                HasBreak = false,
                StartDate = new DateTime(2021, 09, 1),
                EndDate = new DateTime(2021, 12, 30)
            };
            semList.Add(sem);

            sem = new Semester()
            {
                SemesterID = department[0].ID + ",1," + "1" + ",1",
                deptID = department[0].ID,
                StartTime = 10,
                EndTime = 11,
                HasBreak = false,
                StartDate = new DateTime(2022, 01, 1),
                EndDate = new DateTime(2022, 03, 30)
            };
            semList.Add(sem);

            sem = new Semester()
            {
                SemesterID = department[0].ID + ",1," + "1" + ",1",
                deptID = department[0].ID,
                StartTime = 11,
                EndTime = 12,
                HasBreak = false,
                StartDate = new DateTime(2022, 01, 1),
                EndDate = new DateTime(2022, 03, 30)
            };
            semList.Add(sem);

            sem = new Semester()
            {
                SemesterID = department[0].ID + ",1," + "1" + ",1",
                deptID = department[0].ID,
                StartTime = 13,
                EndTime = 14,
                HasBreak = false,
                StartDate = new DateTime(2022, 01, 1),
                EndDate = new DateTime(2022, 03, 30)
            };
            semList.Add(sem);

            sem = new Semester()
            {
                SemesterID = department[0].ID + ",1," + "1" + ",1",
                deptID = department[0].ID,
                StartTime = 14,
                EndTime = 15,
                HasBreak = false,
                StartDate = new DateTime(2022, 01, 1),
                EndDate = new DateTime(2022, 03, 30)
            };
            semList.Add(sem);

            return semList;
        }
        //make CISR
        public List<CISR> makeCISR(List<Semester> semList, List<Course> courseList, List<Instructor> insLIst, List<Room> rmsList)
        {

            List<CISR> list = new List<CISR>();

            list.Add(new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Monday,
                Time = 8,
                semester = semList[0],
                courseIDBindable = courseList[0].ID,
                primaryInstructorIDBindable = insLIst[0].ID,
                secondaryInstructorIDBindable = insLIst[1].ID,
                roomIDBindable = rmsList[0].ID
            });

            list.Add(new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Tuesday,
                Time = 9,
                semester = semList[0],
                courseIDBindable = courseList[1].ID,
                primaryInstructorIDBindable = insLIst[0].ID,
                secondaryInstructorIDBindable = insLIst[1].ID,
                roomIDBindable = rmsList[0].ID
            });

            list.Add(new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Wednesday,
                Time = 10,
                semester = semList[0],
                courseIDBindable = courseList[2].ID,
                primaryInstructorIDBindable = insLIst[0].ID,
                secondaryInstructorIDBindable = insLIst[1].ID,
                roomIDBindable = rmsList[0].ID
            });

            list.Add(new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Thursday,
                Time = 11,
                semester = semList[0],
                courseIDBindable = courseList[3].ID,
                primaryInstructorIDBindable = insLIst[0].ID,
                secondaryInstructorIDBindable = insLIst[1].ID,
                roomIDBindable = rmsList[0].ID
            });

            list.Add(new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Friday,
                Time = 13,
                semester = semList[0],
                courseIDBindable = courseList[4].ID,
                primaryInstructorIDBindable = insLIst[0].ID,
                secondaryInstructorIDBindable = insLIst[1].ID,
                roomIDBindable = rmsList[0].ID
            });

            list.Add(new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Friday,
                Time = 14,
                semester = semList[0],
                courseIDBindable = courseList[5].ID,
                primaryInstructorIDBindable = insLIst[0].ID,
                secondaryInstructorIDBindable = insLIst[1].ID,
                roomIDBindable = rmsList[0].ID
            });

            return list;
        }

        //Make Departments
        public List<Department> makeDepartment()
        {
            return new List<Department>()
            {
                new Department()
                {
                    ID=0,
                    departmentName="CST",
                    semesterCount=2,
                    startDate= new DateTime(2021, 09, 1),
                    EndDate=new DateTime(2022, 6, 1),
                },
                new Department()
                {
                    ID=0,
                    departmentName="Electrical Engineering",
                    semesterCount=2,
                    startDate= new DateTime(2021, 09, 1),
                    EndDate=new DateTime(2022, 6, 1),
                },
                new Department()
                {
                    ID=0,
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
                    ID=0,
                    fName="Ernesto",
                    lName="Basoalto"
                },
                new Instructor(){
                    ID=0,
                    fName="Wade",
                    lName="Lahoda"
                },
                new Instructor(){
                    ID=0,
                    fName="Bryce",
                    lName="Barrie"
                },
                new Instructor(){
                    ID=0,
                    fName="Ron",
                    lName="New"
                },
                new Instructor(){
                    ID=0,
                    fName="Rob",
                    lName="Miller"
                },
                new Instructor(){
                    ID=0,
                    fName="Jason",
                    lName="Schmidt"
                },
                new Instructor(){
                    ID=0,
                    fName="Rick",
                    lName="Caron"
                },
            };
        }

        //Make Rooms
        public List<Room> makeRoom()
        {
            return new List<Room>()
            {
				//users are added out of sequence to test sorting logic in controller
                new Room(){
                    ID=0,
                    roomNumber="230a",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="302a",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="430b",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="239a",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="239b",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="242b",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="241a",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                },
                new Room(){
                    ID=0,
                    roomNumber="241b",
                    capacity=40,
                    city="Saskatoon",
                    campus="Main",
                }
            };
        }

        public void Load(CstScheduleDbContext dbcontext)
        {
            List<Department> departments = makeDepartment();
            dbcontext.Department.AddRange(departments);
            dbcontext.SaveChanges();

            List<Instructor> instructors = makeIns();
            dbcontext.Instructor.AddRange(instructors);
            dbcontext.SaveChanges();

            List<Room> rooms = makeRoom();
            dbcontext.Room.AddRange(rooms);
            dbcontext.SaveChanges();

            List<Course> courses = makeCourses();
            dbcontext.Course.AddRange(courses);
            dbcontext.SaveChanges();

            List<Semester> semesters = makeSemesters(departments);

            dbcontext.Semester.AddRange(semesters);
            dbcontext.SaveChanges();

            List<CISR> cisr = makeCISR(semesters, courses, instructors, rooms);
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
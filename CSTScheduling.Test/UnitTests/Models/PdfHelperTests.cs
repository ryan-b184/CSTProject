using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using CSTScheduling.Pages;
using CSTScheduling.Test.UnitTests.fixtures;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTScheduling.Test.UnitTests.Models.PdfHelperTests
{    
    class PdfHelperTests
    {
        public fixtureReports fix = new();
        private PdfHelper pf = new();

        static Department dept = new Department()
        {
            ID = 0,
            departmentName = "CST",
            semesterCount = 2,
            startDate = new DateTime(2021, 09, 1),
            EndDate = new DateTime(2022, 6, 1),
        };

        static Course course = new Course()
        {
            ID = 0,
            classroomIDBindable = 1,
            semesterID = "1,1,1,1",
            primaryInstructorIDBindable = 1,
            courseName = "COSC",
            courseAbbr = "COSC292",
            hoursPerWeek = 23,
            creditUnits = 4,
            startDate = new DateTime(2021, 09, 1),
            endDate = new DateTime(2021, 12, 30),
        };

        static Instructor ins1 = new Instructor()
        {
            ID = 0,
            fName = "Ernesto",
            lName = "Basoalto"
        };
        static Instructor ins2 = new Instructor()
        {
            ID = 0,
            fName = "Wade",
            lName = "Lahoda"
        };

        static Room rms = new Room()
        {
            ID = 0,
            roomNumber = "230a",
            capacity = 40,
            city = "Saskatoon",
            campus = "Main",
        };

        static Semester sem = new Semester()
        {
            SemesterID = dept.ID + ",1," + "1" + ",1",
            deptID = dept.ID,
            StartTime = 8,
            EndTime = 9,
            HasBreak = false,
            StartDate = new DateTime(2021, 09, 1),
            EndDate = new DateTime(2021, 12, 30)
        };

        private List<CISR> cisrList = new List<CISR>(new CISR[]
        {
            new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Monday,
                Time = 8,
                semester = sem,
                course = course,
                primaryInstructor = ins1,
                secondaryInstructor = ins2,
                room = rms
            },
            new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Tuesday,
                Time = 9,
                semester = sem,
                course = course,
                primaryInstructor = ins1,
                secondaryInstructor = ins2,
                room = rms
            },
            new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Wednesday,
                Time = 10,
                semester = sem,
                course = course,
                primaryInstructor = ins1,
                secondaryInstructor = ins2,
                room = rms
            },
            new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Thursday,
                Time = 11,
                semester = sem,
                course = course,
                primaryInstructor = ins1,
                secondaryInstructor = ins2,
                room = rms
            },
            new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Friday,
                Time = 13,
                semester = sem,
                course = course,
                primaryInstructor = ins1,
                secondaryInstructor = ins2,
                room = rms
            },
            new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Friday,
                Time = 14,
                semester = sem,
                course = course,
                primaryInstructor = ins1,
                secondaryInstructor = ins2,
                room = rms
            }

        });

        #region Unit Tests
        [Test]
        public void Test_GetHTMLForOneCISR_Valid()
        {
            List<CISR> cisrLst = new List<CISR>
            {
                cisrList[0],
            };

            // Get the HTML string from the list of CISR
            String str = pf.ConvertToHtml(cisrLst);

            // the output html string
            String html = "<table><thead><th>Time</th><th>Monday</th><th>Tuesday</th><th>Wednesday</th><th>Thrusday</th><th>Friday</th></thead><tbody><tr>" +
                "<th>8:00</th><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>9:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>10:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>11:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>12:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>1:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>2:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>3:00</th><td></td><td></td><td></td><td></td><td></td></tr></tbody></table>";

            // The string we get from the cisrList and output html string should be same
            Assert.AreEqual(html, str);
        }        

        [Test]
        public void Test_GetHTMLFromCISRDiagonal_Valid()
        {
            // Get the HTML string from the list of CISR
            String str = pf.ConvertToHtml(cisrList);

            // the output html string
            String html = "<table><thead><th>Time</th><th>Monday</th><th>Tuesday</th><th>Wednesday</th><th>Thrusday</th><th>Friday</th></thead><tbody><tr>" +
                "<th>8:00</th><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>9:00</th><td></td><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td><td></td><td></td><td></td></tr><tr>" +
                "<th>10:00</th><td></td><td></td><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td><td></td><td></td></tr><tr>" +
                "<th>11:00</th><td></td><td></td><td></td><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td><td></td></tr><tr>" +
                "<th>12:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                "<th>1:00</th><td></td><td></td><td></td><td></td><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td></tr><tr>" +
                "<th>2:00</th><td></td><td></td><td></td><td></td><td><p><b>COSC292</b></p><p>230a</p><p>Basoalto, E</p><p>Lahoda, W</p></td></tr><tr>" +
                "<th>3:00</th><td></td><td></td><td></td><td></td><td></td></tr></tbody></table>";

            // The string we get from the cisrList and output html string should be same
            Assert.AreEqual(html, str);
        }

        [Test]
        public void Test_GetHTMLForOneCISR_Invalid()
        {
            CISR cisr = new CISR()
            {
                ID = 0,
                Day = DayOfWeek.Monday,
                Time = 8
            };

            List<CISR> cisrLst = new List<CISR>();
            cisrLst.Add(cisr);

            // Get the HTML string from the list of CISR
            String str = pf.ConvertToHtml(cisrLst);

            // the output html string
            String html = "<table><thead><th>Time</th><th>Monday</th><th>Tuesday</th><th>Wednesday</th><th>Thrusday</th><th>Friday</th></thead>" +
                    "<tbody><tr><th>8:00</th><td><p><b></b></p><p></p><p></p></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>9:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>10:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>11:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>12:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>1:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>2:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>3:00</th><td></td><td></td><td></td><td></td><td></td></tr></tbody></table>";

            // The string we get from the cisrList and output html string should be same
            Assert.AreEqual(str, html);
        }

        [Test]
        public void Test_GetPdfFromHtml_Valid()
        {
            // Create an HTML string to pass into function and get the pdf file back
            String html = "<table><thead><th>Time</th><th>Monday</th><th>Tuesday</th><th>Wednesday</th><th>Thrusday</th><th>Friday</th></thead>" +
                    "<tbody><tr><th>8:00</th><td><p><b>COSC292</b></p><p>239a</p><p>Basoalto, E</p><p>Basoalto, E</p></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>9:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>10:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>11:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>12:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>1:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>2:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>3:00</th><td></td><td></td><td></td><td></td><td></td></tr></tbody></table>";

            // Get the pdf file back from HTML string
            byte[] file = pf.GetPdfFromHtml(html);

            // Length of the returned pdf file should be beween below range
            Assert.True(file.Length >= 49545 && file.Length <= 49565);
        }

        [Test]
        public void Test_GetPdfFromHtml_Invalid()
        {
            // Create an HTML string to pass into function and get the pdf file back
            String html = "<table><thead><th>Time</th><th>Monday</th><th>Tuesday</th><th>Wednesday</th><th>Thrusday</th><th>Friday</th></thead>" +
                    "<tbody><tr><th>8:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>9:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>10:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>11:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>12:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>1:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr><th>2:00</th><td></td><td></td><td></td><td></td><td></td></tr><tr>" +
                    "<th>3:00</th><td></td><td></td><td></td><td></td><td></td></tr></tbody>";

            // Get the pdf file back from HTML string
            byte[] file = pf.GetPdfFromHtml(html);

            // Length of the returned pdf file should be beween below range
            Assert.True(file.Length >= 26460 && file.Length <= 26480);
        }

        [Test]
        public void Test_GetEmptyPdfFromHtml_Invalid()
        {
            // Pass in an empty html string
            String html = "";

            // Get the pdf file back from empty HTML string
            byte[] file = pf.GetPdfFromHtml(html);

            // Length of the returned pdf file should be beween below range
            Assert.True(file.Length >= 1570 && file.Length <= 1590);
        }
        #endregion
    }
}

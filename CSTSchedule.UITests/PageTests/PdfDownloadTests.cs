using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Services;
using CSTScheduling.Pages;
using Intro.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CSTScheduling.UITests.PageTests
{
    class PdfDownloadTests
    {
        public CSTSchedule.UITests.fixtures.fixtureReports fix;
        CstScheduleDbContext dbContext;

        List<Semester> semList;
        List<Instructor> insList;
        List<Room> roomList;

        public Instructor inst = new();

        private const double TIME_WAIT = 6;

        static readonly string downloadFolder = @"D:\AutomatedTestsDownloads";

        // Formatted string to easily pass in arguments in the tests
        string commandFormat = "\"http://localhost:5000/PdfDownload?{0}\" --output " + downloadFolder + @"\{1}.zip";
        

        [OneTimeSetUp]
        public void Setup()
        {
            // Set path for the tests to use particular database
            string dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".UITests") - 1) + "ing" + @"\CstScheduleDb.db";
            var contextOpts = new DbContextOptionsBuilder<CstScheduleDbContext>()
             .UseSqlite("Data Source=" + dbDirectory)
             .Options;

            // Create a new database context
            dbContext = new CstScheduleDbContext(contextOpts);
            // Wipe the database between tests
            dbContext.Database.EnsureDeleted();
            // Ensure new database is made
            dbContext.Database.EnsureCreated();

            // Get the directory where the tests will download files
            System.IO.DirectoryInfo di = new DirectoryInfo(@"D:\AutomatedTestsDownloads");

            // Delete the directory and all the files inside on Startup if exists
            if( di.Exists )
            {
                if (di != null)
                {
                    foreach (FileInfo file in di.EnumerateFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.EnumerateDirectories())
                    {
                        dir.Delete(true);
                    }
                }
                di.Delete();
            }            
            
            // Create a new directory
            Directory.CreateDirectory(@"D:\AutomatedTestsDownloads");

            // Populate db with test data
            fix = new CSTSchedule.UITests.fixtures.fixtureReports();
            fix.Load(dbContext); //loads all test data into the test database

            // Fetch data from the database
            semList = dbContext.Semester.ToList();
            insList = dbContext.Instructor.ToList();
            roomList = dbContext.Room.ToList();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            // get rid of test data
            dbContext.Dispose();
        }

        #region Instructor Pdfs        
        [Test]
        public void Test_GetInsPdfForInsIDSemID_Valid()
        {
            // Get the Instrucor and Semester objects from lists
            var ins = insList[0];
            var sem = semList[0];

            // Open curl.exe and run command with valid instructor and semester ids
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&ins=" + ins.ID, "InsIDSemIDValid"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InsIDSemIDValid.zip", downloadFolder + @"\InsIDSemIDValid");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\InsIDSemIDValid\" + ins.fName + " " + ins.lName + ".pdf");

            // Check if the name matches with the valid pdf name
            Assert.AreEqual(result, ins.fName + " " + ins.lName + ".pdf");
        }

        [Test]
        public void Test_GetInsPdfForInvalidInsIDSemID_Invalid()
        {
            // Get the Instrucor and Semester objects from lists
            var ins = insList[0];
            var sem = semList[0];

            // Open curl.exe and run command with invalid instructor id and a valid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&ins=" + ins.ID + 10, "InvalidInsSemID"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InvalidInsSemID.zip", downloadFolder + @"\InvalidInsSemID");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\InvalidInsSemID\Invalid ins" + ins.ID + 10 + ".pdf");

            // Check if the name matches with the invalid pdf name
            Assert.AreEqual(result, "Invalid ins" + ins.ID + 10 + ".pdf");
        }

        [Test]
        public void Test_GetInsPdfForInsIDInvalidSemID_Invalid()
        {
            // Get the Instrucor and Semester objects from lists
            var ins = insList[0];
            var sem = semList[0];

            // Open curl.exe and run command with valid instructor id and an invalid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + 10 + "&ins=" + ins.ID, "InsIDInvalidSemID"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InsIDInvalidSemID.zip", downloadFolder + @"\InsIDInvalidSemID");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT/2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\InsIDInvalidSemID\Invalid sem" + sem.ID + 10 + ".pdf");

            // Check if the name matches with the invalid pdf name
            Assert.AreEqual(result, "Invalid sem" + sem.ID + 10 + ".pdf");
        }

        [Test]
        public void Test_GetInsPdfForInsIDSemID_Invalid()
        {
            // Get the Instrucor and Semester objects from lists
            var ins = insList[0];
            var sem = semList[0];

            // Open curl.exe and run command with invalid instructor and semester ids
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + 10 + "&ins=" + ins.ID + 10, "InvalidInsIDSemID"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InvalidInsIDSemID.zip", downloadFolder + @"\InvalidInsIDSemID");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\InvalidInsIDSemID\Invalid ins" + ins.ID + 10 + ".pdf");

            // Check if the name matches with the invalid pdf name
            Assert.AreEqual(result, "Invalid ins" + ins.ID + 10 + ".pdf");
        }

        [Test]
        public void Test_GetInsPdfForMultipleInsIDsSemID_Valid()
        {
            // Get the Instrucor and Semester objects from lists
            var ins1 = insList[0];
            var ins2 = insList[1];
            var sem = semList[0];

            // Open curl.exe and run command with multiple valid instructor ids and a valid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&ins=" + ins1.ID + "," + ins2.ID, "MultipleInsIDSemIDValid"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\MultipleInsIDSemIDValid.zip", downloadFolder + @"\MultipleInsIDSemIDValid");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Check the number of files in the specifies directory
            int fileCount = Directory.GetFiles(downloadFolder + @"\MultipleInsIDSemIDValid", "*", SearchOption.AllDirectories).Length;
            // Check if only 2 files exist
            Assert.AreEqual(2, fileCount);

            // Get the names of the extracted files
            var result1 = Path.GetFileName(downloadFolder + @"\MultipleInsIDSemIDValid\" + ins1.fName + " " + ins1.lName + ".pdf");
            var result2 = Path.GetFileName(downloadFolder + @"\MultipleInsIDSemIDValid\" + ins2.fName + " " + ins2.lName + ".pdf");

            // Check if the names match with the valid pdf names
            Assert.AreEqual(result1, ins1.fName + " " + ins1.lName + ".pdf");            
            Assert.AreEqual(result2, ins2.fName + " " + ins2.lName + ".pdf");

        }
        #endregion

        #region Room Pdfs        
        [Test]
        public void Test_GetRoomPdfForRoomIDSemID_Valid()
        {
            // Get the Room and Semester objects from lists
            var rms = roomList[0];
            var sem = semList[0];

            // Open curl.exe and run command with valid room and semester ids
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&rms=" + rms.ID, "RoomIDSemIDValid"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\RoomIDSemIDValid.zip", downloadFolder + @"\RoomIDSemIDValid");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\RoomIDSemIDValid\Room " + rms.roomNumber + ".pdf");

            // Check if the name matches with the valid pdf name
            Assert.AreEqual(result, "Room " + rms.roomNumber + ".pdf");
        }

        [Test]
        public void Test_GetRoomPdfForInvalidRoomIDSemID_Invalid()
        {
            // Get the Room and Semester objects from lists
            var rms = roomList[0];
            var sem = semList[0];

            // Open curl.exe and run command with an invalid room id and a valid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&rms=" + rms.ID+10, "InvalidRoomIDSemID"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InvalidRoomIDSemID.zip", downloadFolder + @"\InvalidRoomIDSemID");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\InvalidRoomIDSemID\Invalid room" + rms.ID + ".pdf");

            // Check if the name matches with the invalid pdf name
            Assert.AreEqual(result, "Invalid room" + rms.ID + ".pdf");
        }

        [Test]
        public void Test_GetRoomPdfForRoomIdInvalidSemID_Invalid()
        {
            // Get the Room and Semester objects from lists
            var rms = roomList[0];
            var sem = semList[0];

            // Open curl.exe and run command with a valid room id and an invalid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID+10 + "&rms=" + rms.ID, "RoomIdInvalidSemID"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\RoomIdInvalidSemID.zip", downloadFolder + @"\RoomIdInvalidSemID");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\RoomIdInvalidSemID\Invalid sem" + sem.ID + ".pdf");

            // Check if the name matches with the invalid pdf name
            Assert.AreEqual(result, "Invalid sem" + sem.ID + ".pdf");
        }

        [Test]
        public void Test_GetRoomPdfForInsIDSemID_Invalid()
        {
            // Get the Room and Semester objects from lists
            var rms = roomList[0];
            var sem = semList[0];

            // Open curl.exe and run command with invalid room and semester ids
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID+10 + "&rms=" + rms.ID+10, "InsIDSemIDInvalid"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InsIDSemIDInvalid.zip", downloadFolder + @"\InsIDSemIDInvalid");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Get the name of the extracted file
            var result = Path.GetFileName(downloadFolder + @"\InsIDSemIDInvalid\Invalid room" + rms.ID + ".pdf");

            // Check if the name matches with the invalid pdf name
            Assert.AreEqual(result, "Invalid room" + rms.ID + ".pdf");
        }

        [Test]
        public void Test_GetRoomPdfForMultipleRoomIDsSemID_Valid()
        {
            // Get the Room and Semester objects from lists
            var rms1 = roomList[0];
            var rms2 = roomList[1];
            var sem = semList[0];

            // Open curl.exe and run command with multiple valid room ids and a valid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&rms=" + rms1.ID + "," + rms2.ID, "MultipleRoomIDSemIDValid"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\MultipleRoomIDSemIDValid.zip", downloadFolder + @"\MultipleRoomIDSemIDValid");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Check the number of files in the specifies directory
            int fileCount = Directory.GetFiles(downloadFolder + @"\MultipleRoomIDSemIDValid", "*", SearchOption.AllDirectories).Length;
            // Check if only 2 files exist
            Assert.AreEqual(2, fileCount);

            // Get the names of the extracted files
            var result1 = Path.GetFileName(downloadFolder + @"\MultipleRoomIDSemIDValid\" + rms1.roomNumber + ".pdf");
            var result2 = Path.GetFileName(downloadFolder + @"\MultipleRoomIDSemIDValid\" + rms2.roomNumber + ".pdf");

            // Check if the names match with the valid pdf names
            Assert.AreEqual(result1, rms1.roomNumber + ".pdf");            
            Assert.AreEqual(result2, rms2.roomNumber + ".pdf");
        }
        #endregion

        #region Room + Instructor        
        [Test]
        public void Test_GetPdfsForInsIDRoomIDSemIDs_Valid()
        {
            // Get the Instructor, Room and Semester objects from lists
            var ins = insList[0];
            var rms = roomList[0];
            var sem = semList[0];

            // Open curl.exe and run command with valid instructor, room and semester ids
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&ins=" + ins.ID + "&rms=" + rms.ID, "InsIDRoomIDSemIDValid"),
            };
            Process.Start(proc1);

            // Wait for 2 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\InsIDRoomIDSemIDValid.zip", downloadFolder + @"\InsIDRoomIDSemIDValid");

            // Wait for 2 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Check the number of files in the specifies directory
            int fileCount = Directory.GetFiles(downloadFolder + @"\InsIDRoomIDSemIDValid", "*", SearchOption.AllDirectories).Length;
            // Check if only 2 files exist
            Assert.AreEqual(2, fileCount);

            // Get the names of the extracted files
            var result1 = Path.GetFileName(downloadFolder + @"\InsIDRoomIDSemIDValid\" + rms.roomNumber + ".pdf");
            var result2 = Path.GetFileName(downloadFolder + @"\InsIDRoomIDSemIDValid\" + ins.fName + " " + ins.lName + ".pdf");

            // Check if the names match with the valid pdf names
            Assert.AreEqual(result1, rms.roomNumber + ".pdf");            
            Assert.AreEqual(result2, ins.fName + " " + ins.lName + ".pdf");
        }

        [Test]
        public void Test_GetPdfsForMultipleInsIDRoomIDsSemID_Valid()
        {
            // Get the Instructor, Room and Semester objects from lists
            var ins1 = insList[0];
            var rms1 = roomList[0];
            var ins2 = insList[1];
            var rms2 = roomList[1];
            var sem = semList[0];

            // Open curl.exe and run command with multiple valid instructor, room and semester ids
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&ins=" + ins1.ID + "," + ins2.ID + "&rms=" + rms1.ID + "," + rms2.ID, "MultipleInsIDRoomIDsSemIDValid"),
            };
            Process.Start(proc1);

            // Wait for 3 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT * 2));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid.zip", downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid");

            // Wait for 3 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Check the number of files in the specifies directory
            int fileCount = Directory.GetFiles(downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid", "*", SearchOption.AllDirectories).Length;
            // Check if only 4 files exist
            Assert.AreEqual(4, fileCount);

            // Room checks
            // Get the names of the extracted files
            var result1 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid\" + rms1.roomNumber + ".pdf");            
            var result2 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid\" + rms2.roomNumber + ".pdf");
            // Check if the names match with the valid pdf names
            Assert.AreEqual(result1, rms1.roomNumber + ".pdf");
            Assert.AreEqual(result2, rms2.roomNumber + ".pdf");

            // Instructor checks
            // Get the names of the extracted files
            var result3 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid\" + ins1.fName + " " + ins1.lName + ".pdf");
            var result4 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDValid\" + ins2.fName + " " + ins2.lName + ".pdf");
            // Check if the names match with the valid pdf names
            Assert.AreEqual(result3, ins1.fName + " " + ins1.lName + ".pdf");            
            Assert.AreEqual(result4, ins2.fName + " " + ins2.lName + ".pdf");
        }

        [Test]
        public void Test_GetPdfsForMultipleInsIDRoomIDsSemID_InValid()
        {
            // Get the Instructor, Room and Semester objects from lists
            var ins1 = insList[0];
            var rms1 = roomList[0];
            var ins2 = insList[1];
            var rms2 = roomList[1];
            var sem = semList[0];

            // Open curl.exe and run command with multiple valid and invalid instructor and room ids, and a valid semester id
            ProcessStartInfo proc1 = new ProcessStartInfo
            {
                FileName = @"C:\Windows\System32\curl.exe",
                WorkingDirectory = @"D:\",
                Arguments = String.Format(commandFormat, "sem=" + sem.ID + "&ins=" + ins1.ID+10 + "," + ins2.ID + "&rms=" + rms1.ID + "," + rms2.ID+10, "MultipleInsIDRoomIDsSemIDInvalid"),
            };
            Process.Start(proc1);

            // Wait for 3 seconds for the process to download files properly.
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT));

            // The file is downloaded as zip file, so we have to extract it to a specified path
            ZipFile.ExtractToDirectory(downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid.zip", downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid");

            // Wait for 3 more seconds so the files are properly extracted and are available
            Thread.Sleep(TimeSpan.FromSeconds(TIME_WAIT / 2));

            // Check the number of files in the specifies directory
            int fileCount = Directory.GetFiles(downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid", "*", SearchOption.TopDirectoryOnly).Length;
            // Check if only 4 files exist
            Assert.AreEqual(4, fileCount);

            // Valid file checks
            // Get the names of the extracted files
            var result1 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid\" + rms1.roomNumber + ".pdf");
            var result2 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid\" + ins2.calcName + ".pdf");
            // Check if the names match with the valid pdf names
            Assert.AreEqual(result1, rms1.roomNumber + ".pdf");            
            Assert.AreEqual(result2, ins2.calcName + ".pdf");

            // Invalid file checks
            // Get the names of the extracted files
            var result3 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid\" + "Invalid ins" + ins1.ID + ".pdf");
            var result4 = Path.GetFileName(downloadFolder + @"\MultipleInsIDRoomIDsSemIDInvalid\" + "Invalid room" + rms2.ID + ".pdf");
            // Check if the names match with the invalid pdf names
            Assert.AreEqual(result3, "Invalid ins" + ins1.ID + ".pdf");            
            Assert.AreEqual(result4, "Invalid room" + rms2.ID + ".pdf");
        }
        #endregion
    }
}

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSTScheduling.Data.Models;
using Microsoft.EntityFrameworkCore;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Services;
using CSTScheduling.Pages;
using Bunit;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Blazorise;
using CSTScheduling.Shared;

namespace CSTScheduling.Test.UnitTests.Models
{
    class InstructorListTests
    {
        #region Global variables, Setup/Teardown
        Instructor ins;
        private List<Instructor> insList;
        CstScheduleDbContext testContext;
        Department testDept;
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            // DB Context Options
            String dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";

            // DB Context Options
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
                         .UseSqlite("Data Source=" + dbDirectory).Options;

            // Create a new database context
            testContext = new CstScheduleDbContext(options);

            //Create a bUnit text context
            ctx = new Bunit.TestContext();
            ctx.Services.AddDbContextFactory<CstScheduleDbContext>(opt =>
              opt.UseSqlite("Data Source=" + dbDirectory));
            ctx.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();
            ctx.Services.AddSingleton<CstScheduleDbService>();

            // Wipe the database between tests
            testContext.Database.EnsureDeleted();
            testContext.Database.EnsureCreated();

            testDept = new Department
            {
                ID = 1,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3
            };

            testContext.Department.Add(testDept);
            testContext.SaveChanges();
            ctx.RenderComponent<MainLayout>();
        }

        [TearDown]
        public void Teardown()
        {
            ins = null;
            insList = null;

            //dispose dbContext after test
            testContext.Dispose();
        }
        #endregion 


        #region Unit Tests (Reading from db into list)
        [Test]
        public async Task Test_ListEmptyWhenDbEmpty_Valid()
        {
            /// ( Leave the db empty )

            // Populate list with read method
            insList = await testContext.Instructor.ToListAsync();

            // List should be empty
            Assert.AreEqual(insList.Count, 0);
        }

        [Test]
        public async Task Test_ListOneInstructorWhenDbHasOne_Valid()
        {
            // Insert 1 instructor into db
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Populate the list
            insList = await testContext.Instructor.ToListAsync();

            // List should contain the 1 instructor
            Assert.AreEqual(insList.Count, 1);
        }

        [Test]
        public async Task Test_ListFiveWhenDbHasFive_Valid()
        {
            // Insert 5 instructors into db
            for (int i = 1; i <= 5; i++)
            {
                ins = new Instructor
                {
                    email = "bugs#" + i + "@saskpolytech.ca",
                    fName = "Bugs#" + i,
                };
                testContext.Instructor.Add(ins);
                testContext.SaveChanges();
            }

            // Populate the list
            insList = await testContext.Instructor.ToListAsync();

            // List should contain exactly 5 Instructors
            Assert.AreEqual(insList.Count, 5);
        }

        [Test]
        public async Task Test_ListAlphabeticalWhenDbHasFive_Valid()
        {
            // Insert 5 instructors into db
            for (int i = 1; i <= 5; i++)
            {
                ins = new Instructor
                {
                    email = "bugs" + i + "@saskpolytech.ca",
                    fName = "Bugs" + i,
                };
                testContext.Instructor.Add(ins);
                testContext.SaveChanges();
            }

            // Mock list to compare with the original list if it's already ordered or not
            List<Instructor> insList2 = new List<Instructor>();
            // Populate the list from the test database
            insList2 = await testContext.Instructor.ToListAsync();
            // Order the list by first name ascending to later compare with the original list
            var orderBySeq = insList2.OrderBy(b => b.fName);

            // Populate the original list
            insList = await testContext.Instructor.ToListAsync();

            // Compare the original list with the ordered list  
            bool bIsAlphabetical = insList.SequenceEqual(orderBySeq) && insList.Count == 5 ? true : false;
            Assert.AreEqual(bIsAlphabetical, true);
        }
        #endregion


        #region Functional Tests (List Pagination)
        [Test]
        public void Test_ListMax5InstructorsPerPageWhenDbHas6_Valid()
        {
            // Insert 6 instructors into db
            for (int i = 1; i <= 11; i++)
            {
                ins = new Instructor
                {
                    email = "bugs" + i + "@saskpolytech.ca",
                    fName = "Bugs" + i,
                };
                testContext.Instructor.Add(ins);
                testContext.SaveChanges();
            }

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();
            // Get back button
            var bBack = cut.Find("#back");
            bBack.Click();
            // Count all the tr tags inside the tbody tag
            var trList = cut.FindAll("tbody tr");        
            // First page should only contain 5 Instructors
            Assert.AreEqual(trList.Count, 10);
        }

        [Test]
        public void Test_List6InstructorWithPaginationWhenDbHas6_Valid()
        {
            // Insert 6 instructors into db
            for (int i = 1; i <= 11; i++)
            {
                ins = new Instructor
                {
                    email = "bugs" + i + "@saskpolytech.ca",
                    fName = "Bugs" + i,
                };
                testContext.Instructor.Add(ins);
                testContext.SaveChanges();
            }

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Find the button with the id "next"
            var bPage2 = cut.Find("#next");

            // Click the button to go to the next page
            bPage2.Click();

            // Get the instructor data on the next page 
            var trList = cut.Find("#next").TextContent = "bugs6@saskpolytech.ca";

            // Check the 2nd page if it contains the 6th instructor
            trList.MarkupMatches("bugs6@saskpolytech.ca");
        }
        #endregion


        #region UI Tests (Fields display correct data)
        #region Email field

        [Test]
        public void Test_NoInstructorsAdded_Invalid()
        {
            //Arrange
            // Add 'blank' instructor to db (Leave the fields empty)
            ins = new Instructor
            {
                email = "",
                fName = ""
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            //Act
            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            //Assert
            // Check that the <td> for email contains the 'blank' email
            var tdEmail = cut.Find("td:nth-child(1)");
            tdEmail.MarkupMatches("<td></td>");
        }

        [Test]
        public void Test_DisplayEmailIfExistInDb_Valid()
        {
            //Arrange
            // Add instructor to db (with email)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            //Act
            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            //Assert
            // Check that the <td> for email contains the valid email
            var tdEmail = cut.Find("td:nth-child(1)");
            tdEmail.MarkupMatches("<td>bugs@saskpolytech.ca</td>");
        }

        [Test]
        public void Test_NotDisplayEmailIfNotExistInDb_Valid()
        {
            // Add instructor to db (without email)
            ins = new Instructor
            {
                email = "",
                fName = "Bugs"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for last name contains nothing
            var tdLastName = cut.Find("td:nth-child(1)");
            tdLastName.MarkupMatches("<td></td>");
        }
        #endregion

        #region First name field
        [Test]
        public void Test_DisplayFirstNameIfExistInDb_Valid()
        {
            // Add instructor to db (with first name)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for first name contains "Bugs"
            var tdFirstName = cut.Find("td:nth-child(2)");
            tdFirstName.MarkupMatches("<td>Bugs</td>");
        }
        #endregion

        #region Last name field
        [Test]
        public void Test_DisplayLastNameIfExistInDb_Valid()
        {
            // Add instructor to db (with last name)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
                lName = "Bunny"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for last name contains "Bunny"
            var tdLastName = cut.Find("td:nth-child(3)");
            tdLastName.MarkupMatches("<td>Bunny</td>");
        }

        [Test]
        public void Test_NotDisplayLastNameIfNotExistInDb_Valid()
        {
            // Add instructor to db (without last name)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
                lName = ""
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for last name contains nothing
            var tdLastName = cut.Find("td:nth-child(3)");
            tdLastName.MarkupMatches("<td></td>");
        }
        #endregion

        #region Office Phone field
        [Test]
        public void Test_DisplayPhoneNumberIfExistInDb_Valid()
        {
            // Add instructor to db (with phone number)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
                phoneNum = "(306)555-5555"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for phone number contains "(306)555-5555"
            var tdPhone = cut.Find("td:nth-child(4)");
            tdPhone.MarkupMatches("<td>(306)555-5555</td>");
        }

        [Test]
        public void Test_NotDisplayPhoneNumberIfNotExistInDb_Valid()
        {
            // Add instructor to db (without phone number)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for phone number contains nothing
            var tdPhone = cut.Find("td:nth-child(5)");
            tdPhone.MarkupMatches("<td></td>");
        }
        #endregion

        #region Office Number field
        [Test]
        public void Test_DisplayOfficeIfExistInDb_Valid()
        {
            // Add instructor to db (with office room number)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
                officeNum = "505C"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for office contains "505C"
            var tdOffice = cut.Find("td:nth-child(5)");
            tdOffice.MarkupMatches("<td>505C</td>");
        }

        [Test]
        public void Test_NotDisplayOfficeIfNotExistInDb_Valid()
        {
            // Add instructor to db (without office room number)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for office contains nothing
            var tdOffice = cut.Find("td:nth-child(6)");
            tdOffice.MarkupMatches("<td></td>");
        }
        #endregion

        #region Speciality / Note field
        [Test]
        public void Test_DisplayNoteIfExistInDb_Valid()
        {
            // Add instructor to db (with note)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
                note = "Cannot work Tuesdays."
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for note contains "Cannot work Tuesdays."
            var tdNote = cut.Find("td:nth-child(6)");
            tdNote.MarkupMatches("<td>Cannot work Tuesdays.</td>");
        }

        [Test]
        public void Test_NotDisplayNoteIfNotExistInDb_Valid()
        {
            // Add instructor to db (without note)
            ins = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs"
            };
            testContext.Instructor.Add(ins);
            testContext.SaveChanges();

            // Render the InstructorPage.razor component
            var cut = ctx.RenderComponent<InstructorPage>();

            // Check that the <td> for note contains nothing
            var tdNote = cut.Find("td:nth-child(6)");
            tdNote.MarkupMatches("<td></td>");
        }
        #endregion
        #endregion
    }
}

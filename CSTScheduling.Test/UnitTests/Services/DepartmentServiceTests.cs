using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Bunit;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Services;
using CSTScheduling.Pages;
using CSTScheduling.Shared;
using Intro.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;

using System.Collections.Generic;
using System.IO;
using System.Linq;



namespace CSTScheduling.Test.UnitTests.Services
{

    class DepartmentServiceTests
    {

        List<Department> depList;
        private Department testDept;
        CstScheduleDbContext testContext;

        Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            //Initiliaze variables for test here
            testDept = new Department
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3

            };

            //var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
            //                    .UseInMemoryDatabase(databaseName: "TestDatabase")
            //                    .Options;

            String dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
             .UseSqlite("Data Source=" + dbDirectory)
             .Options;

            // Create a new database context
            testContext = new CstScheduleDbContext(options);
            // Wipe the database between tests
            testContext.Database.EnsureDeleted();
            //make sure new database is made
            testContext.Database.EnsureCreated();

            ctx = new Bunit.TestContext();
            ctx.Services.AddDbContextFactory<CstScheduleDbContext>(opt =>
              opt.UseSqlite("Data Source=" + dbDirectory));
            ctx.Services.AddSingleton<CstScheduleDbService>();
            ctx.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            depList = new();
        }


        [TearDown]
        public void Teardown()
        {
            testContext.Dispose();
            depList = null;
        }

        #region story1c tests

        [Test]
        public void Test_OneDepartmentsAdded_Valid()
        {
            var cut = ctx.RenderComponent<DepartmentPage>();

            //grab the list from the page
            depList = cut.Instance.depList;

            Assert.AreEqual(0, depList.Count());
        }

        [Test]
        public void Test_5DepartmentsAdded1Page_Valid()
        {
            for (int i = 0; i < 5; i++)
            {
                testDept = new Department
                {
                    departmentName = i + "",
                    lengthInYears = 2,
                    startDate = new DateTime(2021, 12, 30),
                    EndDate = new DateTime(2022, 08, 30),
                    semesterCount = 3
                };
                testContext.Department.Add(testDept);
                testContext.SaveChanges();
                depList.Add(testDept);
            }
            depList.Sort();

            //render the componenet
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<DepartmentPage>();

            for (int i = 1; i < 6; i++)
            {
                var depsFromComponent = cut.Find($"#depno_{i}").TextContent;
                Assert.AreEqual(depsFromComponent, depList[i-1].departmentName);
            }


        }

        [Test]
        public void Test_11DepartmentsAdded2Page_Valid()
        {
            for (int i = 1; i < 12; i++)
            {
                testDept = new Department
                {
                    departmentName = i.ToString(),
                    lengthInYears = 2,
                    startDate = new DateTime(2021, 12, 30),
                    EndDate = new DateTime(2022, 08, 30),
                    semesterCount = 3
                };
                testContext.Department.Add(testDept);
                testContext.SaveChanges();
                depList.Add(testDept);
            }

            //render the componenet
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<DepartmentPage>();
            
            cut.Find("#next").Click();


            var temp = cut.Find($"td:nth-child(1)");
            Assert.AreEqual(temp.TextContent, depList[8].departmentName);
        }
        #endregion
    }
}

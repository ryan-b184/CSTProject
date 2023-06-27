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
    class RoomServiceTests
    {
        
        List<Room> roomList;
        private Room room;
        Department testDept;
        CstScheduleDbContext testContext;
        Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            //Initiliaze variables for test here
            room = new();

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


            ctx = new Bunit.TestContext();
            ctx.Services.AddDbContextFactory<CstScheduleDbContext>(opt =>
              opt.UseSqlite("Data Source=" + dbDirectory));
            ctx.Services.AddSingleton<CstScheduleDbService>();
            ctx.Services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();

            roomList = new();
        }


        [TearDown]
        public void Teardown()
        {
            testContext.Dispose();
            roomList = null;
        }

        #region story1c tests

        [Test]
        public void Test_NoRoomsAdded_Valid()
        {
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<RoomPage>();

            //grab the list from the page
            roomList = cut.Instance.roomList;

            Assert.AreEqual(0, roomList.Count());
        }

        [Test]
        public void Test_5RoomsAdded1Page_Valid()
        {
            for (int i = 0; i < 5; i++)
            {
                room = new Room
                {
                    roomNumber = i + "",
                    capacity = 4,
                    city = "saskatoon",
                    campus = "campus"
                };
                testContext.Room.Add(room);
                testContext.SaveChanges();

                roomList.Add(room);
            }
            roomList.Sort();

            //render the componenet
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<RoomPage>();

            for (int i = 1; i < 6; i++)
            {
                var depsFromComponent = cut.Find($"#roomno_{i}").TextContent;
                Assert.AreEqual(depsFromComponent, roomList[i - 1].roomNumber);
            }


        }

        [Test]
        public void Test_11RoomsAdded2Page_Valid()
        {
            for (int i = 0; i < 11; i++)
            {
                room = new Room
                {
                    roomNumber = i + "",
                    capacity = 4,
                    city = "saskatoon",
                    campus = "campus"
                };
                testContext.Room.Add(room);
                testContext.SaveChanges();

                roomList.Add(room);
            }

            //render the componenet
            ctx.RenderComponent<MainLayout>();
            var cut = ctx.RenderComponent<RoomPage>();

            roomList.OrderBy(e => e.roomNumber);

            cut.Find("#next").Click();


            var temp = cut.Find($"td");
            Assert.AreEqual(temp.TextContent, roomList[9].roomNumber);
        }
        #endregion
    }
}

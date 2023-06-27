using System;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using CSTScheduling.Pages;
using CSTScheduling;
using Intro.Tests.Helpers;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace CSTScheduling.Test.UnitTests.Models
{
    class RoomTest
    {
        //Set up null/default variables here

        //EXAMPLE: private Bunit.TestContext testContext;

        private Room tempRoom;
        private ValidationHelper vhelper;
        private CstScheduleDbContext context;

        [SetUp]
        public void Setup()
        {
            //Initiliaze variables for test here
            tempRoom = new Room
            {

                ID = 12,
                roomNumber = "123",
                capacity = 20,
                city = "saskatoon",
                campus = "saskatoon",
            };
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            context = new CstScheduleDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            this.vhelper = new ValidationHelper();
        }

        [TearDown]
        public void Teardown()
        {
            tempRoom = new Room
            {
                ID = 12,
                roomNumber = "123",
                capacity = 20,
                city = "saskatoon",
                campus = "saskatoon",
            };

            

        }



        #region Database Tests

        [Test]
        public void Test_DepartmentAddedToDb_Valid()
        {

            Room roomTest = new Room()
            {
                ID = 12,
                roomNumber = "123",
                capacity = 20,
                city = "saskatoon",
                campus = "saskatoon",
            };
            context.Room.Add(roomTest);

            context.SaveChanges();

            var test = context.Room.ToList<Room>();
            Assert.AreEqual(1, test.Count());
            Assert.AreEqual(roomTest, test[0]);
            context.Dispose();
        }

        [Test]
        public void Test_DepartmentDuplicateAddedToDb_Invalid()
        {

            context.Room.Add(new Room
            {
                ID = 12,
                roomNumber = "123",
                capacity = 20,
                city = "saskatoon",
                campus = "saskatoon",
            });

            context.SaveChanges();

            Assert.Throws<InvalidOperationException>(() =>
            {
                context.Room.Add(new Room
                {
                    ID = 12,
                    roomNumber = "123",
                    capacity = 20,
                    city = "saskatoon",
                    campus = "saskatoon",
                });

            });
            context.Dispose();


        }

        #endregion


        [Test]
        public void Test_RoomNumRequired_Invalid()
        {
            tempRoom = new Room
            {
                ID = 12,
                capacity = 20,
                city = "saskatoon",
                campus = "saskatoon",
            };
            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Room number is required", errors[0].ErrorMessage);
        }

        [Test]
        public void testRoomNumberMinimum_Valid()
        {
            //Put test code here
            tempRoom.roomNumber = "1";

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);

        }

        [Test]
        public void Test_RoomNumberMaximum_Valid()
        {
            //Put test code here
            tempRoom.roomNumber = "200";

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        //[Test]
        //public void Test_RoomNumberNegative_Invalid()
        //{
        //    //Put test code here
        //    tempRoom.roomNumber = "-1";

        //    var errors = ValidationHelper.Validate(tempRoom);
        //    Assert.IsTrue(errors.Count() == 1);
        //    Assert.AreEqual("Room number should be less than 7", errors[0].ErrorMessage);
        //}

        [Test]
        public void Test_RoomNumberMaximum_Invalid()
        {
            //Put test code here
            tempRoom.roomNumber = "10001777";

            var errors = ValidationHelper.Validate(tempRoom);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Room number length should be less than 7", errors[0].ErrorMessage);
        }


        [Test]
        public void Test_CapacityRequired_Invalid()
        {
            tempRoom = new Room
            {
                ID = 12,
                roomNumber = "123",
                city = "saskatoon",
                campus = "something",
            };
            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Capacity number should be between 1 and 50", errors[0].ErrorMessage);
        }

        [Test]// combine this one
        public void Test_CapacityMinimum_Valid()
        {
            //Put test code here
            tempRoom.capacity = 1;

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_CapacityMaximum_Valid()
        {
            //Put test code here
            tempRoom.capacity = 50;

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_CapacityMinimum_Invalid()
        {
            //Put test code here
            tempRoom.capacity = 0;

            var errors = ValidationHelper.Validate(tempRoom);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Capacity number should be between 1 and 50", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_CapacityMaximum_Invalid()
        {
            //Put test code here
            tempRoom.capacity = 51;

            var errors = ValidationHelper.Validate(tempRoom);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Capacity number should be between 1 and 50", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_CapacityNegative_Invalid()
        {
            //Put test code here
            tempRoom.capacity = -1;

            var errors = ValidationHelper.Validate(tempRoom);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Capacity number should be between 1 and 50", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_CityNameRequired_Invalid()
        {
            tempRoom = new Room
            {
                ID = 12,
                roomNumber = "123",
                city = "saskatoon",
                campus = "something",
            };
            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Capacity number should be between 1 and 50", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_City_Valid()
        {
            //Put test code here
            tempRoom.city = new string('a', 50);

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);

        }

        [Test]
        public void Test_City_Invalid()
        {
            //Put test code here
            tempRoom.city = new String('a', 51);

            var errors = ValidationHelper.Validate(tempRoom);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("City name must be 50 characters or below", errors[0].ErrorMessage);
        }


        [Test]
        public void Test_CampusNameRequired_Invalid()
        {
            tempRoom = new Room
            {
                ID = 12,
                roomNumber = "123",
                capacity = 20,
                city = "saskatoon",
            };

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Campus name is required", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_Campus_Valid()
        {
            //Put test code here
            tempRoom.campus = new String('a', 75);

            var errors = ValidationHelper.Validate(tempRoom);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_Campus_Invalid()
        {
            //Put test code here
            tempRoom.campus = new String('a', 76);

            var errors = ValidationHelper.Validate(tempRoom);


            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Campus name must be 75 characters or below", errors[0].ErrorMessage);

        }

    }
}
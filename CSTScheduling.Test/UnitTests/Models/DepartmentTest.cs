using System;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using CSTScheduling.Data.Context;
using System.Linq;
using CSTScheduling.Pages;
using CSTScheduling;
using Intro.Tests.Helpers;
using CSTScheduling.Data.Models;
using Microsoft.EntityFrameworkCore;



namespace CSTScheduling.Test.UnitTests.Models
{
    class DepartmentTest
    {
        //Set up null/default variables here

        //EXAMPLE: private Bunit.TestContext testContext;

        private Department testDept;
        private ValidationHelper vhelper;
        CstScheduleDbContext context;


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
            // get rid of test code here/stop test code
            //reset the test dept
            testDept = new Department
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3

            };

            
        }



        #region Database Tests

        [Test]
        public void Test_DepartmentAddedToDb_Valid()
        {

            Department depTest = new Department()
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3
            };
            context.Department.Add(depTest);

            context.SaveChanges();

            var test = context.Department.ToList<Department>();
            Assert.AreEqual(1, test.Count());
            Assert.AreEqual(depTest, test[0]);
            context.Dispose();
        }

        [Test]
        public void Test_DepartmentDuplicateAddedToDb_Invalid()
        {

            context.Department.Add(new Department
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3
            });

            context.SaveChanges();

            Assert.Throws<InvalidOperationException>((TestDelegate)(() =>
            {
                context.Department.Add(new Department
                {
                    ID = 12,
                    departmentName = "Computer Systems Technology",
                    lengthInYears = 2,
                    startDate = new DateTime(2021, 12, 30),
                    EndDate = new DateTime(2022, 08, 30),
                    semesterCount = 3
                });

            }));
            context.Dispose();
        }

        [Test]
        public void Test_SemesterUpdatedWhenAlreadyInDb_Valid()
        {


            Department test = new Department
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 2,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3
            };
            context.Department.Add(test);
            context.SaveChanges();

            // Modify an attribute of the semester object and update
            test.semesterCount = 4;

            context.Department.Update(test);
            context.SaveChanges();


            var depList = context.Department.ToList<Department>();
            Assert.AreEqual(1, depList.Count());
            Assert.AreNotEqual(3, depList[0].semesterCount);
            Assert.AreEqual(4, depList[0].semesterCount);

            context.Dispose();

        }

        #endregion

        #region unit tests
        [Test]
        public void Test_LenghtInYearsRequired_Invalid()
        {
            testDept = new Department
            {
                ID = 12,
                departmentName = "Computer Systems Technology",
                lengthInYears = 0,
                startDate = new DateTime(2021, 12, 30),
                EndDate = new DateTime(2022, 08, 30),
                semesterCount = 3

            };

            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Department length should be greater than 0 and less than 4", errors[0].ErrorMessage);
        }


        [Test]
        public void Test_MaxLengthInYears_Valid()
        {
            //This is the test for the valid integer number input
            //by the prgram head for the length of the program year
            testDept.lengthInYears = 4;
            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_MaxLengthInYears_Invalid()
        {
            //This is the test for the invalid integer number input
            //by the prgram head for the length of the program year
            testDept.lengthInYears = 5;

            var errors = ValidationHelper.Validate(testDept);
            //Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Department length should be greater than 0 and less than 4", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_MinLengthInYears_Valid()
        {
            //This is the test for the minimum length 
            testDept.lengthInYears = 0.5;

            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_MinLengthInYears_Invalid()
        {
            //Put test code here
            testDept.lengthInYears = 0;

            var errors = ValidationHelper.Validate(testDept);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Department length should be greater than 0 and less than 4", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_MinLengthInYearsNegative_Invalid()
        {
            //Put test code here
            testDept.lengthInYears = -1;

            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Department length should be greater than 0 and less than 4", errors[0].ErrorMessage);
        }


        [Test]
        public void Test_ProgramLength_Valid()
        {
            //Put test code here
            testDept.lengthInYears = 1;

            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        [Test]
        public void Test_ProgramLengthInDecimal_Valid()
        {
            //Put test code here
            testDept.lengthInYears = 1.5;

            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }


        [Test]
        public void Test_EndDateAfterStartDate_Valid()
        {
            this.testDept.startDate = new DateTime(2021, 12, 30);
            this.testDept.EndDate = new DateTime(2022, 12, 30);
            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);

        }

        [Test]
        public void Test_EndDateBeforeStartDate_Invalid()
        {
            this.testDept.startDate = new DateTime(2021, 12, 30);
            this.testDept.EndDate = new DateTime(2020, 12, 30);
            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("End date must be after the start date", errors[0].ErrorMessage);

        }

        [Test]
        public void Test_SemesterCountMinimum_Valid()
        {
            //Put test code here
            testDept.semesterCount = 1;

            var errors = ValidationHelper.Validate(testDept);

            Assert.IsTrue(true);
            Assert.IsEmpty(errors);
        }

        //[Test]
        //public void Test_SemesterCountMaximum_Valid()
        //{
        //    //Put test code here
        //    testDept.semesterCount = 12;

        //    var errors = ValidationHelper.Validate(testDept);

        //    Assert.IsTrue(true);
        //    Assert.IsEmpty(errors);
        //}

        [Test]
        public void Test_SemesterCountMinimum_Invalid()
        {
            //Put test code here
            testDept.semesterCount = 0;

            var errors = ValidationHelper.Validate(testDept);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Semester count should be less than 4 and greater than 1", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_SemesterCountNegative_Invalid()
        {
            //Put test code here
            testDept.semesterCount = -1;

            var errors = ValidationHelper.Validate(testDept);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Semester count should be less than 4 and greater than 1", errors[0].ErrorMessage);
        }

        [Test]
        public void Test_SemesterCountMaximum_Invalid()
        {
            //Put test code here
            testDept.semesterCount = 13;

            var errors = ValidationHelper.Validate(testDept);
            Assert.IsTrue(errors.Count() == 1);
            Assert.AreEqual("Semester count should be less than 4 and greater than 1", errors[0].ErrorMessage);
        }
        #endregion
    }
}
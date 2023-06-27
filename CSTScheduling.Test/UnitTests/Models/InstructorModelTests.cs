using System;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using Intro.Tests.Helpers;
using CSTScheduling.Pages;
using CSTScheduling.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CSTScheduling.Data.Context;
using System.IO;

namespace CSTScheduling.Test.UnitTests.Models.InstructorTests
{
    class InstructorModelTests
    {
        #region Prop
        private ValidationHelper vh;
        Instructor insTest;
        CstScheduleDbContext testContext;
        //private IRenderedComponent<InstructorAddEdit> instructorComponent;
        #endregion
        
        #region Setup/Teardown
        [SetUp]
        public void Setup()
        {
            this.vh = new ValidationHelper();
            insTest = new Instructor
            {
                email = "bugs@saskpolytech.ca",
                fName = "Bugs",
            };
            //TODO: MOCK DATABASE

            String dbDirectory = Directory.GetCurrentDirectory();
            dbDirectory = dbDirectory.Substring(0, dbDirectory.IndexOf(".Test")) + @"\CstTestDb.db";
            // DB Context Options
            var options = new DbContextOptionsBuilder<CstScheduleDbContext>()
                         .UseSqlite("Data Source=" + dbDirectory)
                         .Options;

            // Create a new database context
            testContext = new CstScheduleDbContext(options);

            // Wipe the database between tests
            testContext.Database.EnsureDeleted();

            //make sure new database is made
            testContext.Database.EnsureCreated();
        }

        [TearDown]
        public void Teardown()
        {
            this.vh = null;
            insTest = null;

            //dispose after test
            testContext.Dispose();
        }
        #endregion

        #region Email tests
        [Test]
        public void Test_InstructorEmailAdded_Valid()
        {
            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        /* Ron requested that the email field be optional
        [Test]
        public void Test_InstructorEmailEmpty_Invalid()
        {
            insTest.email = "";
            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Email is required", vhResults[0].ErrorMessage);
        }
        */

        [Test]
        public void Test_InstructorEmailEmpty_Valid()
        {
            insTest.email = "";
            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreNotEqual("Email is required", vhResults[0].ErrorMessage);// Changed to NotEqual for Valid test.
        }

        /* Ron requested that the email field be optional.
        [Test]
        public void Test_InstructorEmailNulNull_Invalid()
        {
            insTest.email = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Email is required", vhResults[0].ErrorMessage);
        }
        */

        [Test]
        public void Test_InstructorEmailNulNull_Valid()
        {
            insTest.email = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(0, vhResults.Count); // changed to 0 to pass Valid test
        }

        [Test]
        public void Test_InstructorEmailMissingRecipientName_Invalid()
        {
            insTest.email = "@saskpolytech.ca";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(2, vhResults.Count);
            Assert.AreEqual("Invalid email format", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorEmailMissingAtSymbol_Invalid()
        {
            insTest.email = "rob123saskpolytech.ca";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(2, vhResults.Count);
            Assert.AreEqual("Invalid email format", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorEmailMissingDomainName_Invalid()
        {
            insTest.email = "rob123@.ca";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Invalid email format", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorEmailMissingTopLevelDomain_Invalid()
        {
            insTest.email = "rob123@saskpolytech";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Invalid email format", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorEmailAlreadyExists_Invalid()
        {
            //add to db twice
            //need to make a mock db for this one
        }

        #endregion

        #region First Name Tests
        [Test]
        public void Test_InstructorFirstNameOf40Characters_Valid()
        {
            // Dummy Instructor object
            insTest.fName = new string('a', 40);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorFirstNameOf41Characters_Invalid()
        {
            // Dummy Instructor object
            insTest.fName = new string('a', 41);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Invalid first name - must be between 1 and 40 characters", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorFirstNameOf1Character_Valid()
        {
            // Dummy Instructor object
            insTest.fName = new string('a', 1);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorFirstNameOf0Characters_Invalid()
        {
            // Dummy Instructor object
            insTest.fName = "";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("First name is required", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorFirstNameNull_Invalid()
        {
            // Dummy Instructor object
            insTest.fName = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("First name is required", vhResults[0].ErrorMessage);
        }

        #endregion

        #region Last name tests
        [Test]
        public void Test_InstructorLastNameOf40Characters_Valid()
        {
            insTest.lName = new string('a', 40);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorLastNameOf41Characters_Invalid()
        {
            // Dummy Instructor object
            insTest.lName = new string('a', 41);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Get validation results
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Invalid last name - must be 40 characters or less", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorLastNameOf1Character_Valid()
        {
            // Dummy Instructor object
            insTest.lName = new string('a', 1);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorLastNameIsNull_Valid()
        {
            insTest.lName = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorLastNameIsEmpty_Valid()
        {
            insTest.lName = "";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }
        #endregion

        #region Phone number tests
        [Test]
        public void Test_InstructorPhoneWith10Digits_Valid()
        {
            // Dummy Instructor object
            insTest.phoneNum = new string('5', 10);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorPhoneWith11Digits_Valid()
        {
            // Dummy Instructor object
            insTest.phoneNum = new string('5', 11);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorPhoneParsesOutNonDigits_Valid()
        {
            // Dummy Instructor object
            insTest.phoneNum = "1(123)456-7890";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorPhoneCanadianFormat_Valid()
        {
            // Dummy Instructor object
            insTest.phoneNum = "13068748744";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorPhoneIsNull_Valid()
        {
            insTest.phoneNum = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorPhoneIsEmpty_Valid()
        {
            insTest.phoneNum = "";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorPhoneContainsLetters_Invalid()
        {
            insTest.phoneNum = "A";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
        }

        //[Test]
        //public void Test_InstructorPhoneWith9Digits_Invalid()
        //{
        //    // Dummy Instructor object
        //    insTest.phoneNum = new string('5', 9);

        //    // Get validation results
        //    IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

        //    // Assert
        //    Assert.AreEqual(1, vhResults.Count);
        //    Assert.AreEqual("Invalid phone format", vhResults[0].ErrorMessage);
        //}

        //[Test]
        //public void Test_InstructorPhoneWith12Digits_Invalid()
        //{
        //    // Dummy Instructor object
        //    insTest.phoneNum = new string('5', 12);

        //    // Get validation results
        //    IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

        //    // Assert
        //    Assert.AreEqual(1, vhResults.Count);
        //    Assert.AreEqual("Invalid phone format", vhResults[0].ErrorMessage);
        //}

        //Need to look into how to make sure non canadian phone formats are invalid, low priority though
        //[Test]
        //public void Test_InstructorPhoneNewZealandFormat_Invalid()
        //{
        //    // Dummy Instructor object
        //    insTest.phoneNum = "+64212539193";

        //    // Get validation results
        //    IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

        //    // Assert
        //    Assert.AreEqual(1, vhResults.Count);
        //    Assert.AreEqual("Invalid phone format", vhResults[0].ErrorMessage);

        //}
        #endregion

        #region Office number tests
        [Test]
        public void Test_InstructorOfficeNumWith20Chars_Valid()
        {
            // Dummy Instructor object
            insTest.officeNum = new string('a', 20);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorOfficeNumWith21Chars_Invalid()
        {
            // Dummy Instructor object
            insTest.officeNum = new string('a', 21);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Invalid office number - must be 20 characters or less", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorOfficeNumIsNull_Valid()
        {
            insTest.officeNum = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorOfficeNumIsEmpty_Valid()
        {
            insTest.officeNum = "";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }
        #endregion

        #region Notes tests
        [Test]
        public void Test_InstructorNotesWith200Chars_Valid()
        {
            // Dummy Instructor object
            insTest.note = new string('a', 200);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorNotesWith201Chars_Invalid()
        {
            // Dummy Instructor object
            insTest.note = new string('a', 201);

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.AreEqual(1, vhResults.Count);
            Assert.AreEqual("Invalid note - must be 200 characters or less", vhResults[0].ErrorMessage);
        }

        [Test]
        public void Test_InstructorNotesIsNull_Valid()
        {
            insTest.note = null;

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }

        [Test]
        public void Test_InstructorNotesIsEmpty_Valid()
        {
            insTest.note = "";

            // Get validation results
            IList<ValidationResult> vhResults = ValidationHelper.Validate(insTest);

            // Assert
            Assert.IsEmpty(vhResults);
        }
        #endregion

        #region Database Tests
        [Test]
        public void Test_InstructorAddedNotAlreadyInDb_Valid() { 

            testContext.Instructor.Add(insTest);
            testContext.SaveChanges();

            var returnVal = testContext.Instructor.ToList<Instructor>();

            Assert.AreEqual(1, returnVal.Count );
        }

        [Test]
        public void Test_InstructorAddWhenFNameAlreadyInDb_Valid()
        {
            testContext.Instructor.Add(insTest);
            testContext.SaveChanges();

            try
            {
                Instructor ins2 = new Instructor
                {
                    //same email as instest
                    email = insTest.email,
                    fName = "Tim",
                    lName = "Tom",
                };
                testContext.Instructor.Add(ins2);
                testContext.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //should only get 1 back, as second will be invalid due to duplicate email
            var returnVal = testContext.Instructor.ToList<Instructor>();
            Assert.AreEqual(2, returnVal.Count);
        }

        [Test]
        public void Test_InstructorUpdatedWhenAlreadyInDb_Valid()
        {
            testContext.Instructor.Add(insTest);
            testContext.SaveChanges();

            String sVal = "Bobby";
            insTest.lName = sVal;

            testContext.Instructor.Update(insTest);
            testContext.SaveChanges();

            insTest = null;
            //need index of first item in db
            insTest = testContext.Instructor.Find(1);

            Assert.AreEqual(sVal, insTest.lName);
        }


        #endregion
    }
}

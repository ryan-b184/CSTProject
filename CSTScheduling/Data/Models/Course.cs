using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CSTScheduling.Utilities;

namespace CSTScheduling.Data.Models
{
    public class Course
    {
        /// <summary>
        /// <br/>->Database ID
        /// <br/>->Primary Key
        /// <br/>->Auto-Generated
        /// <br/>->Int
        /// <br/>->DO NOT SET BY HAND
        /// <br/>->This ID will be generated and set to the object when it is saved to the Database
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// <br/>->Course Abbreviation
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->Must follow the format XXXX#
        /// <br/>->Four Characters, 1-20 Numbers
        /// </summary>
        [Required(ErrorMessage = "Course abbreviation is required")]
        [MaxLength(20, ErrorMessage = "Course abbreviations can only be 20 characters long")]
        [RegularExpression(@"^[a-zA-Z\d]{1,20}", ErrorMessage = "Invalid Course Abbreviation format")]
        public string courseAbbr { get; set; }

        /// <summary>
        /// <br/>->Course Name
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->Can range from 1-80 Characters
        /// </summary>
        [Required(ErrorMessage = "Course Name is required")]
        [MaxLength(80, ErrorMessage = "Specified name is longer than the limit of 80 characters")]
        [RegularExpression(@"^[a-zA-Z\s\d]{1,80}$", ErrorMessage = "Invalid Course Name")]
        public string courseName { get; set; }

        #region Instructor References

        /// <summary>
        /// <br/>->Primary Instructor ID
        /// <br/>->Int
        /// <br/>->The ID of this Courses default Primary Instructor
        /// <br/>->Not Saved to the Database
        /// </summary>
        [MinInstructorPrimary(ErrorMessage = "A primary instructor is required for this course")]
        public int primaryInstructorIDBindable { get; set; }

        /// <summary>
        /// <br/>->Primary Instructor ID
        /// <br/>->Int
        /// <br/>->The ID of this Courses default Primary Instructor
        /// <br/>->Cannot be the same as the Primary Instructor
        /// </summary>
        [Duplicate(ErrorMessage = "Secondary instructor cannot be the same as Primary Instructor")]
        public int secondaryInstructorIDBindable { get; set; }

        /// <summary>
        /// <br/>->Primary Instructor
        /// <br/>->Instructor
        /// <br/>->Not Mapped
        /// <br/>->A reference to this course objects default PrimaryInstructor
        /// <br/>->Not Saved to the Database
        /// </summary>
        [NotMapped]
        public Instructor primaryInstructor { get; set; }

        /// <summary>
        /// <br/>->Primary Instructor
        /// <br/>->Instructor
        /// <br/>->Not Mapped
        /// <br/>->A reference to this course objects default Secondary Instructor
        /// <br/>->Not Saved to the Database
        /// </summary>
        [NotMapped]
        public Instructor secondaryInstructor { get; set; }

        #endregion

        #region Classroom Default
        /// <summary>
        /// <br/>->Classroom ID
        /// <br/>->Int
        /// <br/>->The ID of this courses default Classroom
        /// </summary>
        [MinClassroom(ErrorMessage = "Classroom is required")]
        public int classroomIDBindable { get; set; }
        //public Room classroom { get; set; }
        #endregion

        #region Semester FK
        [Required]
        public String semesterID { get; set; }
        #endregion

        /// <summary>
        /// <br/>->Hours Per Week
        /// <br/>->Int
        /// <br/>->Required
        /// <br/>->The number of hours per week this course must be scheduled
        /// <br/>->Range 0-35
        /// </summary>
        [Required(ErrorMessage = "Hours per week is required")]
        [Range(0, 35, ErrorMessage = "Hours per week must be at least 0 hours and a maximum of 35 hours")]
        public int hoursPerWeek { get; set; }

        /// <summary>
        /// <br/>->Course Start Date
        /// <br/>->DateTime
        /// <br/>->Required
        /// <br/>->This Courses Start Date
        /// </summary>
        [Required]
        public DateTime? startDate { get; set; }

        /// <summary>
        /// <br/>->Course End Date
        /// <br/>->DateTime
        /// <br/>->Required
        /// <br/>->This Courses End Date
        /// <br/>->CANNOT be before or equal to its Start Date
        /// </summary>
        [Required]
        [EndDate(ErrorMessage = "End date cannot be before start date")]
        public DateTime? endDate { get; set; }

        /// <summary>
        /// <br/>->Credits
        /// <br/>->Int
        /// <br/>->Required
        /// <br/>->The number of Credit Units this course offers
        /// <br/>->Range 0-10
        /// </summary>
        [Required(ErrorMessage = "Credit units is required")]
        [Range(0, 10, ErrorMessage = "Credit units must be between 0 and 10")]
        public int creditUnits { get; set; }

        /// <summary>
        /// <br/>->List of CISR objects rekated to this course
        /// <br/>->Used to instantiate a one to many relationship between a Course and its CISR objects
        /// </summary>
        public ICollection<CISR> cisrList { get; set; }

        #region Calculated fields
        //Calculated field
        //[NotMapped]
        //public int numOfWeeks { get { return numOfWeeks; } set { numOfWeeks = calculateTotalWeeks(this.startDate, this.endDate); } }


        public int numOfWeeks { get; set; }
        // public int totalHours { get { return totalHours; } set { calculateTotalHours(hoursPerWeek, numOfWeeks); } }
        public int totalHours { get; set; }

        private int calculateTotalWeeks(DateTime? startDate, DateTime? endDate)
        {

            int totalWeeks = 0;

            if (startDate != null && endDate != null)
            {
                DateTime start = (DateTime)startDate;
                DateTime end = (DateTime)endDate;

                TimeSpan temp = end.Subtract(start);

                totalWeeks = (int)(Math.Round(temp.TotalDays) / 7);
            }

            return totalWeeks;
        }
        #endregion

        /// <summary>
        /// Override
        /// <br/>Returns the course name in the format [courseAbbr.]courseName
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "[" + courseAbbr + "] " + courseName;
        }


    }
}
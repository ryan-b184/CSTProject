using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CSTScheduling.Utilities;

namespace CSTScheduling.Data.Models
{
    public class TValue
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage = "Course abbreviation is required")]
        [MaxLength(20, ErrorMessage = "Course abbreviations can only be 20 characters long")]
        [RegularExpression(@"^[a-zA-Z\d]{1,20}", ErrorMessage = "Invalid Course Abbreviation format")]
        public string courseAbbr { get; set; }

        [Required(ErrorMessage = "Course Name is required")]
        [MaxLength(80, ErrorMessage = "Specified name is longer than the limit of 80 characters")]
        [RegularExpression(@"^[a-zA-Z\s\d]{1,80}$", ErrorMessage = "Invalid Course Name")]
        public string courseName { get; set; }

        #region Primary Instructor FK
        // [ForeignKey("Instructor")]
        [MinInstructorPrimary(ErrorMessage = "A primary instructor is required for this course")]
        public int primaryInstructorIDBindable { get; set; }

        [NotMapped]
        public Instructor primaryInstructor { get; set; }

        [NotMapped]
        public Instructor secondaryInstructor { get; set; }

        // public Instructor primaryInstructor { get; set; }
        #endregion

        #region Secondary Instructor FK
        [ForeignKey("Instructor")]
        [Duplicate(ErrorMessage = "Secondary instructor cannot be the same as Primary Instructor")]
        public int secondaryInstructorIDBindable { get; set; }
        //public Instructor? secondaryInstructor { get; set; }
        #endregion

        #region Classroom FK
        // [ForeignKey("classroom")]
        [MinClassroom(ErrorMessage = "Classroom is required")]
        public int classroomIDBindable { get; set; }
        //public Room classroom { get; set; }
        #endregion

        #region Semester FK
        [Required]
        public String semesterID { get; set; }
        #endregion

        [Required(ErrorMessage = "Hours per week is required")]
        [Range(0, 35, ErrorMessage = "Hours per week must be at least 0 hours and a maximum of 35 hours")]
        public int hoursPerWeek { get; set; }

        [Required]
        public DateTime? startDate { get; set; }

        [Required]
        [EndDate(ErrorMessage = "End date cannot be before start date")]
        public DateTime? endDate { get; set; }

        [Required(ErrorMessage = "Credit units is required")]
        [Range(0, 10, ErrorMessage = "Credit units must be between 0 and 10")]
        public int creditUnits { get; set; }

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

            if(startDate != null && endDate != null)
            {
                DateTime start = (DateTime)startDate;
                DateTime end = (DateTime)endDate;

                TimeSpan temp = end.Subtract(start);

                totalWeeks = (int) (Math.Round(temp.TotalDays) / 7);
            }

            return totalWeeks;
        }
        #endregion

        public override string ToString()
        {
            return "[" + courseAbbr + "] " + courseName;
        }


    }
}
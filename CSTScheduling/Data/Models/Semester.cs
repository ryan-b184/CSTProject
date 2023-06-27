using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ExpressiveAnnotations.Attributes;
using System.Text.RegularExpressions;

namespace CSTScheduling.Data.Models
{
    public class Semester : IComparable<Semester>
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
        public int ID { set; get; }
        #region SemesterID -> Primary Key

        /// <summary>
        ///<br/>-> Semester ID
        ///<br/>-> Primary Key
        ///<br/>-> Required
        ///<br/>-> String
        ///<br/>-> Composed of ProgramID + Year# + Semester# + StudentGroup#
        ///<br/>-> Format like x,x,x,x
        /// </summary>
        [DisplayName("Semester ID")]
        [Required(ErrorMessage = "SemesterID Is Required")]
        [RegularExpression(@"\d+,\d+,\d+,\d+$", ErrorMessage = "Internal Error, Semester ID does not match REGEX")]
        public string SemesterID { get; set; }

        /// <summary>
        /// <br/>-> Department ID
        /// <br/>-> Primary Key
        /// <br/>-> Required
        /// <br/>-> Int
        /// <br/>-> The ID of this Semesters parent Department
        /// </summary>
        [Required(ErrorMessage = "Internal Error, department reference is required")]
        public int deptID { get; set; }

        #endregion

        #region Semester Start/End Dates

        /// <summary>
        ///<br/>-> Semester Start Date
        ///<br/>-> Required
        ///<br/>-> DateTime Object
        ///</summary>
        [DisplayName("Start Date")]
        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        ///<br/>-> Semester End Date
        ///<br/>-> Required
        ///<br/>-> DateTime Object
        ///<br/>-> Must be Greater than StartDate
        /// </summary>
        [DisplayName("End Date")]
        [Required(ErrorMessage = "End Date is Required")]
        [AssertThat("EndDate > StartDate", ErrorMessage = "Semester End Date must be after the semester Start Date")]
        public DateTime? EndDate { get; set; }

        #endregion

        #region Day Start/End Time

        /// <summary>
        /// <br/>-> Day Start Time
        /// <br/>-> Required
        /// <br/>-> Int
        /// <br/>-> Format using 24h clock
        /// <br/>-> 00 -> 23
        /// </summary>
        [DisplayName("Day Start")]
        [Required(ErrorMessage = "Start Time is Required")]
        [Range(0,23, ErrorMessage ="Internal Error, day start time must be an INT between 0 and 23 inclusive")]
        public int StartTime { get; set; }

        /// <summary>
        /// <br/>-> Day End Time
        /// <br/>-> Required
        /// <br/>-> Int
        /// <br/>-> Must be greater than StartTime
        /// <br/>-> Format using 24h clock
        /// <br/>-> 00 -> 23
        /// </summary>
        [DisplayName("Day End")]
        [Required(ErrorMessage = "The Day end time is required")]
        [Range(0, 23, ErrorMessage = "Internal Error, day end time must be an INT between 0 and 23 inclusive")]
        [AssertThat("EndTime > StartTime", ErrorMessage = "The day must end after it has started")]
        public int EndTime { get; set; }

        #endregion

        #region Semester Break Values

        /// <summary>
        /// <br/>-> Does This semester have a break?
        /// <br/>-> Required
        /// <br/>-> Boolean
        /// <br/>-> If set to true, makes the BreakStart and BreakEnd fields required
        /// </summary>
        [DisplayName("Has Break")]
        [Required(ErrorMessage ="Internal Error, HasBreak not set")]
        [DefaultValue(false)]
        public bool HasBreak { get; set; }


        /// <summary>
        /// <br/>-> Break Start date
        /// <br/>-> Required if HasBreak == True
        /// <br/>-> DateTime Object
        /// <br/>-> Must Fall within the bouds of StartDate and EndDate
        /// <br/>-> StartDate -> <b>BreakStart</b> &lt;- BreakEnd &lt;- EndDate
        /// </summary>
        [DisplayName("Break Start")]
        [RequiredIf("HasBreak", ErrorMessage = "'Has Break' checkbox has been checked, this field is required")]
        [AssertThat("(BreakStart > StartDate)  && (BreakStart < EndDate)", ErrorMessage = "The break start date must fall between the Semester Start and End dates")]
        public DateTime? BreakStart { get; set; }

        /// <summary>
        /// <br/>-> Break End date
        /// <br/>-> Required if HasBreak == True
        /// <br/>-> DateTime Object
        /// <br/>-> Must fall within the bouds of StartDate and EndDate
        /// <br/>-> AND must be greater than BreakStart
        /// <br/>-> StartDate -> BreakStart -> <b>BreakEnd</b> &lt;- EndDate
        /// </summary>
        [DisplayName("Break End")]
        [RequiredIf("HasBreak", ErrorMessage = "'Has Break' checkbox has been checked, this field is required")]
        [AssertThat("BreakEnd > BreakStart", ErrorMessage = "The break end must occur after the break start")]
        [AssertThat("(BreakEnd > StartDate)  && (BreakEnd < EndDate)", ErrorMessage = "The break end date must fall between the Semester Start and End dates")]
        public DateTime? BreakEnd { get; set; }

        #endregion


        /// <summary>
        /// <br/>-> List of CISR objects for that semester
        /// <br/>-> Used to instantiate a one-to-many relationship between a Semester and its CISR objects
        /// </summary>
        public ICollection<CISR> cisrList { get; set; }

        public int CompareTo(Semester other)
        {
            if (SemesterID == other.SemesterID) return 0;
            return 1;
        }
    }


}

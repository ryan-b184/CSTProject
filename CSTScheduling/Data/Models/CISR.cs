using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CSTScheduling.Data.Models
{
    public class CISR
    {
        /// <summary>
        /// <br/>->Database ID
        /// <br/>->Primary Key
        /// <br/>->Auto-Generated
        /// <br/>->Int
        /// <br/>->This ID will be generated and set to the object when it is saved to the Database
        /// <br/>->DO NOT SET BY HAND
        /// <br/>->MUST BE SET TO 0 WHEN DELETED FROM DATABASE
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        /// <summary>
        /// <br/>->Semester
        /// <br/>->Nullable
        /// <br/>->This is a reference to this CISRs Semester
        /// <br/>-> It is on the Many side of the one-to-many relationship
        /// </summary>
        public Semester? semester { get; set; }

        #region ScheduleComponent properties - Not Mapped
        [NotMapped]
        public bool isSelected { get; set; }

        [NotMapped]
        public bool isEditing { get; set; }
        #endregion

        #region Day And Time
        /// <summary>
        /// <br/>->Day Of the Week
        /// <br/>->DayOfWeek
        /// <br/>->Nullable
        /// <br/>->Required
        /// <br/>->The day of the week this CISR is scheduled, monday(1) to friday(5)
        /// <br/>->Range 1-5 or NaN
        /// </summary>
        [Required(ErrorMessage = "You must have a day selected before saving a course")]
        public DayOfWeek? Day { get; set; } = null;

        /// <summary>
        /// <br/>->Time of Day
        /// <br/>->Int
        /// <br/>->Required
        /// <br/>->The time of day this CISR is scheduled
        /// <br/>->Range 0-24
        /// </summary>
        [Required(ErrorMessage = "You must have a time selected before saving a course")]
        [Range(0, 24, ErrorMessage = "You must select a time between 0:00 and 24:00")]
        public int Time { get; set; } = -1;
        #endregion

        #region Course Attributes
        /// <summary>
        /// <br/>->Course ID
        /// <br/>->Private Int?
        /// <br/>->Nullable
        /// <br/>->This is a Foreign Key Reference to this CISRs course object
        /// <br/>->CAN BE NULL, USE this.courseIDBindable for Int value
        /// DO NOT SET ON THIS PROPERTY, IT IS GET ONLY
        /// </summary>
        [ForeignKey("course")]
        public int? CourseID { get; set; }

        /// <summary>
        /// <br/>->Course ID
        /// <br/>->Int
        /// <br/>->Not Mapped
        /// <br/>->Use this property to retrive the attribute CourseID
        /// <br/>->This will return a 0 if courseIDBindable is null, and set courseIDBindable to null if supplied a 0
        /// <br/>->This is not saved to the databse, see CourseID for the Database value
        /// </summary>
        [NotMapped]
        public int courseIDBindable
        {
            get
            {
                if (CourseID == null)
                {
                    return 0;
                }
                else
                {
                    return CourseID.Value;
                }
            }
            set
            {
                if (value == 0)
                {
                    this.CourseID = null;
                }
                else
                {
                    this.CourseID = value;
                }
            }
        }

        /// <summary>
        /// <br/>->Course Object
        /// <br/>->Nullable
        /// <br/>->This is used to instantiate the one-to-many relationship between this CISR and its Semester
        /// <br/>->The CISR object is on the many side of this relatioship
        /// </summary>
        public Course? course { get; set; }

        #endregion

        #region Primary Instructor Attributes
        /// <summary>
        /// <br/>->Primary Instructor
        /// <br/>->Private Int?
        /// <br/>->Nullable
        /// <br/>->This is a foreign key reference to this CISRs primary Instructor Object
        /// <br/>->CAN BE NULL, use this.primaryInstructorIDBindable for Int value
        /// DO NOT SET ON THIS PROPERTY, IT IS GET ONLY
        /// </summary>
        [ForeignKey("primaryInstructor")]
        public int? PrimaryInstructorID { get; set; }

        /// <summary>
        /// <br/>->Primary Instructor ID
        /// <br/>->Int
        /// <br/>->Not Mapped
        /// <br/>->Use this property to retrive the attribute PrimaryInstructorID
        /// <br/>->This will return a 0 if PrimaryInstructorID is null, and set PrimaryInstructorID to null if supplied a 0
        /// <br/>->This is not saved to the databse, see PrimaryInstructorID for the Database value
        /// </summary>
        [NotMapped]
        public int primaryInstructorIDBindable
        {
            get
            {
                if (PrimaryInstructorID == null)
                {
                    return 0;
                }
                else
                {
                    return PrimaryInstructorID.Value;
                }
            }
            set
            {
                if (value == 0)
                {
                    this.PrimaryInstructorID = null;
                }
                else
                {
                    this.PrimaryInstructorID = value;
                }
            }
        }

        /// <summary>
        /// <br/>->Primary Instructor Object
        /// <br/>->Nullable
        /// <br/>->This is used to instantiate the one-to-many relationship between this CISR and its primary Instructor
        /// <br/>->The CISR object is on the many side of this relatioship
        /// </summary>
        public Instructor? primaryInstructor { get; set; }
        #endregion

        #region Secondary Instructor Attributes
        /// <summary>
        /// <br/>->Secondary Instructor
        /// <br/>->Private Int?
        /// <br/>->Nullable
        /// <br/>->This is a foreign key reference to this CISRs primary Secondary Object
        /// <br/>->CAN BE NULL, use this.SecondaryInstructorID for Int value
        /// DO NOT SET ON THIS PROPERTY, IT IS GET ONLY
        /// </summary>
        [ForeignKey("secondaryInstructor")]
        public int? SecondaryInstructorID { get; set; }

        /// <summary>
        /// <br/>->Secondary Instructor ID
        /// <br/>->Int
        /// <br/>->Not Mapped
        /// <br/>->Use this property to retrive the attribute SecondaryInstructorID
        /// <br/>->This will return a 0 if SecondaryInstructorID is null, and set SecondaryInstructorID to null if supplied a 0
        /// <br/>->This is not saved to the databse, see SecondaryInstructorID for the Database value
        /// </summary>
        [NotMapped]
        public int secondaryInstructorIDBindable
        {
            get
            {
                if (SecondaryInstructorID == null)
                {
                    return 0;
                }
                else
                {
                    return SecondaryInstructorID.Value;
                }
            }
            set
            {
                if (value == 0)
                {
                    this.SecondaryInstructorID = null;
                }
                else
                {
                    this.SecondaryInstructorID = value;
                }
            }
        }

        /// <summary>
        /// <br/>->Secondary Instructor Object
        /// <br/>->Nullable
        /// <br/>->This is used to instantiate the one-to-many relationship between this CISR and its secondary Instructor
        /// <br/>->The CISR object is on the many side of this relatioship
        /// </summary>
        public Instructor? secondaryInstructor { get; set; }
        #endregion

        #region Room Attributes
        /// <summary>
        /// <br/>->Room
        /// <br/>->Private Int?
        /// <br/>->Nullable
        /// <br/>->This is a foreign key reference to this CISRs primary Room
        /// <br/>->CAN BE NULL, use Room for Int value
        /// DO NOT SET ON THIS PROPERTY, IT IS GET ONLY
        /// </summary>
        [ForeignKey("room")]
        public int? RoomID { get; set; }

        /// <summary>
        /// <br/>->Room ID
        /// <br/>->Int
        /// <br/>->Not Mapped
        /// <br/>->Use this property to retrive the attribute Room
        /// <br/>->This will return a 0 if Room is null, and set Room to null if supplied a 0
        /// <br/>->This is not saved to the databse, see Room for the Database value
        /// </summary>
        [NotMapped]
        public int roomIDBindable
        {
            get
            {
                if (RoomID == null)
                {
                    return 0;
                }
                else
                {
                    return RoomID.Value;
                }
            }
            set
            {
                if (value == 0)
                {
                    this.RoomID = null;
                }
                else
                {
                    this.RoomID = value;
                }
            }
        }

        /// <summary>
        /// <br/>->Room Object
        /// <br/>->Nullable
        /// <br/>->This is used to instantiate the one-to-many relationship between this CISR and its Room
        /// <br/>->The CISR object is on the many side of this relatioship
        /// </summary>
        public Room? room { get; set; }
        #endregion

    }
}

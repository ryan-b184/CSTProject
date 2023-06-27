using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExpressiveAnnotations.Attributes;
using System.Text.RegularExpressions;

namespace CSTScheduling.Data.Models
{
    public class Instructor
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


        /// <summary>
        /// <br/>->Instructor Email
        /// <br/>->String
        /// <br/>->Must follow standard email format, allows for 2 or 3 character Top-Level Domains
        /// </summary>
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Invalid email format")]
        public string email { get; set; }

        /// <summary>
        /// <br/>->Instructor First Name
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->The Instructors first name
        /// <br/>->Range 1-40 Characters
        /// </summary>
        [Required(ErrorMessage = "First name is required")]
        [StringLength(40, ErrorMessage = "Invalid first name - must be between 1 and 40 characters")]
        public string fName { get; set; }

        /// <summary>
        /// <br/>->Instrucor Last Name
        /// <br/>->String
        /// <br/>->The Instructors last name
        /// </summary>
        [StringLength(40, ErrorMessage = "Invalid last name - must be 40 characters or less")]
        public string lName { get; set; }


        /// <summary>
        /// <br/>->Instructor Phone Number
        /// <br/>->String
        /// <br/>->REGEX is a mess, fix or avoid
        /// </summary>
        [RegularExpression(@"[^A-Za-z]*", ErrorMessage = "Invalid phone format")]
        public string phoneNum { get; set; }

        /// <summary>
        /// <br/>->Instructors Office Number
        /// <br/>->String
        /// <br/>->The instrucors office number
        /// <br/>->Range 0-20 Characters
        /// </summary>
        [StringLength(20, ErrorMessage = "Invalid office number - must be 20 characters or less")]
        public string officeNum { get; set; }

        /// <summary>
        /// <br/>->Instructor Note
        /// <br/>->String
        /// <br/>->A note about this instructor
        /// <br/>->Range 0-200 Characters
        /// </summary>
        [StringLength(200, ErrorMessage = "Invalid note - must be 200 characters or less")]
        public string note { get; set; }

        /// <summary>
        /// A List of CISR objects where this Instructor is the Primary Instructor
        /// <br/>->Used to instantiate a one-to-many relationship between this Instructor and its CISR objects
        /// </summary>
        public ICollection<CISR> cisrPrimaryList { get; set; }

        /// <summary>
        /// A List of CISR objects where this Instructor is the Secondary Instructor
        /// <br/>->Used to instantiate a one-to-many relationship between this Instructor and its CISR objects
        /// </summary>
        public ICollection<CISR> cisrSecondaryList { get; set; }

    
        public string calcName
        {
            get
            {
                return this.lName != null && this.lName.Length > 0 ? $"{this.lName}, {this.fName.Substring(0, 1).ToUpper()}" : this.fName;
            }
        }

        /// <summary>
        /// Override
        /// <br/>->Used To return this unstructors full name for display in one of the following formats:
        /// <br/>->If the Instructors Last Name (this.lName) is not null: LastNanem, FirstName
        /// <br/>->Else FirstName
        /// </summary>
        public override string ToString()
        {
            return this.lName != null ? this.lName + ", " + this.fName : this.fName;
        }

    }
}

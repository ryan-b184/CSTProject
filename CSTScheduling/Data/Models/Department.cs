
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CSTScheduling.Utilities;
using Microsoft.EntityFrameworkCore;
namespace CSTScheduling.Data.Models
{
    public class Department : IComparable
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
        public int ID { get; set; }

        /// <summary>
        /// <br/>->Department Name
        /// <br/>->String
        /// <br/>->Required
        /// <br/>->This Departments Name, Also known as the Program Name
        /// <br/>->Range 1-50 Characters
        /// </summary>
        [MaxLength(50, ErrorMessage = "Department name must be 50 characters or below")]
        [Required(ErrorMessage = "Department name is required")]
        public String departmentName { get; set; }

        /// <summary>
        /// <br/>->Length In Years
        /// <br/>->Double
        /// <br/>->Required
        /// <br/>->This Departments(Programs) length in years
        /// </summary>
        [Range(0.5, 4.0, ErrorMessage = "Department length should be greater than 0 and less than 4")]
        public double lengthInYears { get; set; } = 2;

        /// <summary>
        /// <br/>->Start Date
        /// <br/>->DateTime
        /// <br/>->Required
        /// <br/>->This Departments(Programs) Start Date
        /// </summary>
        [Required(ErrorMessage = "Start date is required")]
        public DateTime startDate { get; set; } = DateTime.Today;

        /// <summary>
        /// <br/>->EndDate
        /// <br/>->DateTime
        /// <br/>->Required
        /// <br/>->This Departments(Programs) End Date
        /// </summary>
        [DepEndDate(ErrorMessage = "End date must be after the start date")]
        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; } = DateTime.Today;

        /// <summary>
        /// <br/>->Semester Count
        /// <br/>->Int
        /// <br/>->Required
        /// <br/>->The number of Semester per Year of this course
        /// <br/>->Range 1-5
        /// </summary>
        [Range(1, 5, ErrorMessage = "Semester count should be less than 4 and greater than 1")]
        [Required(ErrorMessage = "Semester count is required")]
        public int semesterCount { get; set; } = 3;

        /// <summary>
        /// <br/>->Number of Groups
        /// <br/>->Int
        /// <br/>->Required
        /// <br/>->The Number of Student Groups this program has
        /// <br/>->Range 1-10
        /// </summary>
        //[Range(1, 10, ErrorMessage = "Number of student groups must be between {1} and {2}")]
        public int? ProgramNumberOfGroups { get; set; } = 2;

        /// <summary>
        /// Compares this Department Object to another based on the departmentName attiribute.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            return this.departmentName.CompareTo(((Department)obj).departmentName);
        }
    }
}

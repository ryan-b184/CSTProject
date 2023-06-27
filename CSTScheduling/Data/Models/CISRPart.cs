using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CSTScheduling.Data.Models
{
    /// <summary>
    /// This is a basic object meant to pass to the scheduling component
    /// </summary>
    public class CISRPart
    {
        public Course course { get; set; }

        public Instructor primaryInstructor { get; set; }

        public Instructor secondaryInstructor { get; set; }

        public Semester semester { get; set; }

        public Room room { get; set; }
    }
}

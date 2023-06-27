using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CSTScheduling.Data.Models;

namespace CSTScheduling.Utilities
{
    public class MinClassroomAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"Classroom is required";
        public string Error = "";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var course = (Course)validationContext.ObjectInstance;
            
            
            var classroomIDBindable = course.classroomIDBindable;


            if (classroomIDBindable < 0)
            {
                return new ValidationResult(Error = GetErrorMessage(), new[] { validationContext.MemberName });
            }
            

            return ValidationResult.Success;
        }


    }
}


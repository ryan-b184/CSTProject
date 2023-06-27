using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CSTScheduling.Data.Models;

namespace CSTScheduling.Utilities
{
    public class MinInstructorPrimaryAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"A primary instructor is required for this course";
        public string Error = "";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var course = (Course)validationContext.ObjectInstance;


            var primaryInstructorIDBindable = course.primaryInstructorIDBindable;


            if (primaryInstructorIDBindable < 0)
            {
                return new ValidationResult(Error = GetErrorMessage(), new[] { validationContext.MemberName });
            }


            return ValidationResult.Success;
        }


    }
}
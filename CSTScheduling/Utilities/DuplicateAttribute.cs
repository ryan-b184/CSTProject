using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CSTScheduling.Data.Models;

namespace CSTScheduling.Utilities
{
    public class DuplicateAttribute : ValidationAttribute
    {
        public string GetErrorMessage() => $"Secondary instructor cannot be the same as Primary Instructor";
        public string Error = "";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var course = (Course)validationContext.ObjectInstance;
            var primaryInstructorIDBindable = course.primaryInstructorIDBindable;
            var secondaryInstructorIDBindable = course.secondaryInstructorIDBindable;

            if (secondaryInstructorIDBindable != -1 && primaryInstructorIDBindable != -1)
            {
                if (primaryInstructorIDBindable == secondaryInstructorIDBindable && secondaryInstructorIDBindable != 0)
                {
                    return new ValidationResult(Error = GetErrorMessage(), new[] { validationContext.MemberName });
                }
            }
            

            return ValidationResult.Success;
        }


    }
}

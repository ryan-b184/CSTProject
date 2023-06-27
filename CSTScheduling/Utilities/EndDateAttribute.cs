using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CSTScheduling.Data.Models;

namespace CSTScheduling.Utilities
{
    public class EndDateAttribute : ValidationAttribute
    {

        public string GetErrorMessage() => $"End date cannot be before start date";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var course = (Course)validationContext.ObjectInstance;
            //var endDate = course.endDate;
            //var startDate = course.startDate;
            //if (endDate <= startDate)
            //if (endDate.CompareTo(startDate) == -1)
            if(course.endDate <= course.startDate)
            {
                return new ValidationResult(GetErrorMessage(), new[] { validationContext.MemberName });
            }


            return ValidationResult.Success;
        }


    }
}
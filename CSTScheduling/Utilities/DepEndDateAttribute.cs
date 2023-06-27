using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using CSTScheduling.Data.Models;

namespace CSTScheduling.Utilities
{
    public class DepEndDateAttribute : ValidationAttribute
    {

        public string GetErrorMessage() => $"End date must be after the start date";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var department = (Department)validationContext.ObjectInstance;
            var endDate = department.EndDate;
            var startDate = department.startDate;

            if (endDate.CompareTo(startDate) <= 0)
            {
                return new ValidationResult(GetErrorMessage(), new[] { validationContext.MemberName });
            }


            return ValidationResult.Success;
        }


    }
}
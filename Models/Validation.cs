using System.ComponentModel.DataAnnotations;
using System;
namespace BeltExam
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime CurrentTime = DateTime.Now;
            if((DateTime)value < CurrentTime)
                return new ValidationResult("Cannot input a past date");
            return ValidationResult.Success;
        }
    }
    public class PasswordFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string pw = (string)value;
            bool numSp = false;
            bool numAlp = false;
            bool numInt = false;
            if(pw != null)
            {
                for(int i = 0; i < pw.Length; i++)
                {
                    if(Char.IsDigit(pw[i]))
                    {
                        numInt = true;
                    }
                    else if(Char.IsLetter(pw[i]))
                    {
                        numAlp = true;
                    }
                }
            
            
                string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
                foreach (var item in specialChar)
                {
                    if (pw.Contains(item))
                    {
                        numSp = true;
                    } 
                }
            }
            if(numSp && numAlp && numInt)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("Password must contain at least one number, letter and special char");
            }
        }
    }
}
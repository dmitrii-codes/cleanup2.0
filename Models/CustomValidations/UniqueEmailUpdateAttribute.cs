using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class UniqueEmailUpdateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UserUpdateViewModel test = (UserUpdateViewModel)validationContext.ObjectInstance;
            if (test.PreviousEmail != (string)value)
            {
                var _context = (CleanupContext)validationContext.GetService(typeof(CleanupContext));
                var allUsers = _context.users;
                foreach (var each in allUsers)
                {
                    if ((string)value == (string)each.Email)
                    {
                        return new ValidationResult("Email already exists in database");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
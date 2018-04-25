using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cleanup.Models;

namespace Cleanup.Models
{
    public class UniqueUsernameUpdateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UserUpdateViewModel test = (UserUpdateViewModel)validationContext.ObjectInstance;
            if(test.UserName != (string)value)
            {
                var _context = (CleanupContext) validationContext.GetService(typeof(CleanupContext));
                var allUsers = _context.users;
                foreach(var each in allUsers)
                {
                    if((string)value == (string)each.UserName)
                    {
                        return new ValidationResult("Username already exists in database");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
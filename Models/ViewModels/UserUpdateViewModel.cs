using System;
using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class UserUpdateViewModel: BaseEntity
    {
        [Required(ErrorMessage = "First name must not be blank")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 letters")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage = "First name cannot contain numerals")]
        [Display(Name = "First Name:")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name must not be blank")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 letters")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage = "Last name cannot contain numerals")]
        [Display(Name = "Last Name:")]
        public string LastName { get; set; }
        [UniqueUsernameUpdate]
        [Display(Name = "Username:")]
        [Required(ErrorMessage = "Username must not be blank")]
        [MinLength(2, ErrorMessage = "Username must be at least 2 letters")]
        public string UserName { get; set; }
        public string PreviousUserName { get; set; }
        [EmailAddress]
        [UniqueEmailUpdate]
        [Display(Name = "Email  :")]
        public string Email { get; set; }
        public string PreviousEmail { get; set; }
        [Range(0, Int32.MaxValue)]
        public int Score { get; set; }
        [Range(0, Int32.MaxValue)]
        public int Token { get; set; }
        [Range(0, 9)]
        public int UserLevel { get; set; }
        [Display(Name = "Profile Photo:")]
        public string ProfilePic { get; set; }
    }
}
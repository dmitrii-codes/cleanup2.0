using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cleanup.Models;

namespace Cleanup.Models
{
    public class UserUpdateViewModel : BaseEntity
    {
        
        [Required(ErrorMessage="First name must not be blank")]
        [MinLength(2, ErrorMessage="First name must be at least 2 letters")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage="First name cannot contain numerals")]
        public string FirstName{get;set;}
        [Required(ErrorMessage="Last name must not be blank")]
        [MinLength(2, ErrorMessage="Last name must be at least 2 letters")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage="Last name cannot contain numerals")]
        public string LastName{get;set;}
        [UniqueUsernameUpdate] 
        [Display(Name = "Username:")]
        [Required(ErrorMessage="Username must not be blank")]
        [MinLength(2, ErrorMessage="Username must be at least 2 letters")]
        public string UserName{get;set;}
        public string PreviousUserName {get;set;}
        [EmailAddress]
        [UniqueEmailUpdate]
        public string Email{get;set;}
        public string PreviousEmail {get;set;}
        [Range(0,Int32.MaxValue)]
        public int Score{get;set;}
        [Range(0,Int32.MaxValue)]
        public int Token{get;set;}
        [Range(0,9)]
        public int UserLevel{get;set;}
        // [MinLength(8)]
        // [Display(Name="Password:")]
        // [Required(ErrorMessage="Password must not be blank")]
        // [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[$@$!%*#?&])[A-Za-z\d$@$!%*#?&]{8,}$", ErrorMessage="Password must contain at least 1 number, 1 letter, and a special character")]
        // public string Password{get;set;}
        // [Compare("Password", ErrorMessage="Password Confirmation must match")]
        // [Display(Name="Confirm Password:")]
        // [Required(ErrorMessage="Password Confirmation must not be blank")]
        // public string PasswordConfirmation{get;set;}
        [Display(Name="Profile Photo:")]
        public string ProfilePic{get;set;}
    }
}
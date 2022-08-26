using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagment.ViewModels
{
    public class ResetPasswordModelView
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Your password and confirmation password must be the same")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}


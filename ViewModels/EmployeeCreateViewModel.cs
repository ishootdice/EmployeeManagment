using System;
using System.ComponentModel.DataAnnotations;
using EmployeeManagment.Models;

namespace EmployeeManagment.ViewModels
{
	public class EmployeeCreateViewModel
	{
        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        //[RegularExpression(@"[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.@[a-zA-Z0-9-.]+$", ErrorMessage = "Invalid Email Form")]
        [Display(Name = "Office Email")]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }

        public IFormFile? Photo { get; set; }
    }
}


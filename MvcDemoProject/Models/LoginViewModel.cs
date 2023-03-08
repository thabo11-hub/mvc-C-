using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcDemoProject.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(150)]
        [Display(Name = "Email Address: ")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(150, MinimumLength = 6)]
        [Display(Name = "Password: ")]
        public string Password { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace userdb.Models
{
    public class UserView
    {
        [Key]
        public int user_id {get; set;}
        
        [Display(Name="First Name")]
        [Required]
        [MinLength(5)]
        public string first_name {get; set;}
        [Display(Name="Last Name")]
        [Required]
        [MinLength(5)]
        public string last_name {get; set;}
        [Display(Name="Email")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string email {get; set;}
        [Display(Name="Password")]
        [DataType(DataType.Password)]
        [Required]
        public string password {get; set;}
        [Display(Name="Confirm Passwords")]
        [Required]
        [DataType(DataType.Password)]
        public string confirmpwd {get; set;}
    }
}
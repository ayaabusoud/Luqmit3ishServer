using System;
using System.ComponentModel.DataAnnotations;

namespace Luqmit3ish_forMobile.Models
{
	public class UserRegister
	{

        public String Name { get; set; }


        [Required, EmailAddress(ErrorMessage = "The email invalid.")]
        public String Email { get; set; }

        [Required, Phone(ErrorMessage = "The phone number invalid.")]
        public String Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public String Password { get; set; }

        public String Location { get; set; }
        public String Type { get; set; }
        public String Photo { get; set; }
    }
}


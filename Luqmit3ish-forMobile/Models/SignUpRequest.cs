using System;
using System.ComponentModel.DataAnnotations;

namespace Luqmit3ish_forMobile.Models
{
	public class SignUpRequest
	{
        [Required, RegularExpression(@"^[a-zA-Z0-9_-]{4,16}$", ErrorMessage = "The name invalid.")]
        public String name { get; set; }


        [Required, EmailAddress(ErrorMessage = "The email invalid.")]
        public String email { get; set; }

        [Required, Phone(ErrorMessage = "The phone number invalid.")]
        public String phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public String password { get; set; }

        public String location { get; set; }
        public String type { get; set; }
        public String photo { get; set; }
    }
}


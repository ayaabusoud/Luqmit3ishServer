using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ishBackend.Models
{
    public class User
    {
        [Key, Required]
        public int id { get; set; }

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

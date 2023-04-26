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
        public int Id { get; set; }

        public String Name { get; set; }


        [Required, EmailAddress(ErrorMessage = "The email invalid.")]
        public String Email { get; set; }

        [Required, Phone(ErrorMessage = "The phone number invalid.")]
        public String Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public String Password { get; set; }
        public String OpeningHours { get; set; }

        public String Location { get; set; }
        public String Type { get; set; }
        public String Photo { get; set; }
        
    }
}

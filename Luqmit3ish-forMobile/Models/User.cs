using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ishBackend.Models
{
    public class User
    {
        [Key]
        [Required]
        public int id { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9_-]{4,16}$", ErrorMessage = "The name invalid.")]
        public String name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage ="The email invalid.")]
        public String email { get; set; }

        [RegularExpression(@"^(?:(?:(?:\+|00)970)|0)?5[69]\d{7}$", ErrorMessage= "The phone number invalid.")]
        public String phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public String password { get; set; }

        public String location { get; set; }
        public String type { get; set; }
        public String photo { get; set; }
    }
}

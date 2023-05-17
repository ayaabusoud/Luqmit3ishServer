using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ishBackend.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [RegularExpression(@"^[A-Za-z0-9\s]{1,200}$", ErrorMessage = "Invalid password format")]
        public String Name { get; set; }

        [Required, EmailAddress(ErrorMessage = "The email invalid.")]
        public String Email { get; set; }

        [Required, Phone(ErrorMessage = "The phone number invalid.")]
        public String Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W)(?=.*\d)(?!.*\s).{8,}$", ErrorMessage = "Invalid password format")]
        public String Password { get; set; }

        [RegularExpression(@"^(0?[1-9]|1[0-2]):[0-5][0-9](am|pm)-(0?[1-9]|1[0-2]):[0-5][0-9](am|pm)$", ErrorMessage = "Invalid opening hours format")]
        public String OpeningHours { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public String Location { get; set; }

        [Required(ErrorMessage = "Type is required")]
        public String Type { get; set; }

        [Required(ErrorMessage = "Photo is required")]
        public String Photo { get; set; }
        
    }
}

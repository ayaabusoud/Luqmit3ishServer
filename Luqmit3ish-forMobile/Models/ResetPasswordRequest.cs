using System.ComponentModel.DataAnnotations;
using System;

namespace Luqmit3ish_forMobile.Models
{
    public class ResetPasswordRequest

    {
        [Required(ErrorMessage = "id is required")]

        public int Id { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\W)(?=.*\d)(?!.*\s).{8,}$", ErrorMessage = "Invalid password format")]
        public String Password { get; set; }


    }
}
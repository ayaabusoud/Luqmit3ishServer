using System.ComponentModel.DataAnnotations;
using System;

namespace Luqmit3ish_forMobile.Models
{
    public class ResetPasswordRequest

    {
        [Required(ErrorMessage = "id is required")]

        public int Id { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public String Password { get; set; }


    }
}
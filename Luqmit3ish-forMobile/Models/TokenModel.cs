using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class TokenModel
    {

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Token { get; set; }

        public string Role { get; set; }


    }
}

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
        public int id { get; set; }
        public String name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public String email { get; set; }
        public String phone { get; set; }
        [Required(ErrorMessage = "Password is required")]

        public String password { get; set; }
        public String location { get; set; }
        public String type { get; set; }
        public String photo { get; set; }
    }
}

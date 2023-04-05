using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Luqmit3ish_forMobile.Models
{
    public class AddFoodRequest
    {
     
        [Required(ErrorMessage = "User id number is required")]
        public int user_id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String type { get; set; }
        public String photo { get; set; }
        public int keep_listed { get; set; }
        public String pick_up_time { get; set; }
        [Required(ErrorMessage = "Dishes number is required")]
        public int number { get; set; }
    }
}

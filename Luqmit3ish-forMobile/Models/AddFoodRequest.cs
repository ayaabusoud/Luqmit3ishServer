using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Luqmit3ish_forMobile.Models
{
    public class AddFoodRequest
    {
     
        [Required(ErrorMessage = "User id number is required")]
        public int UserId { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public String Type { get; set; }
        public String Photo { get; set; }
        public int KeepValid { get; set; }
        [Required(ErrorMessage = "Dishes number is required")]
        public int Quantity { get; set; }
    }
}

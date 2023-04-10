using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class OrderRequest
    {
        [Required(ErrorMessage = "restaurant id is required")]
        public int res_id { get; set; }

        [Required(ErrorMessage = "charity id is required")]
        public int char_id { get; set; }

        [Required(ErrorMessage = "dish id is required")]
        public int dish_id { get; set; }


        public DateTime date { get; set; }
        [Required(ErrorMessage = "number of dishes is required")]

        public int number_of_dish { get; set; }
        [Required(ErrorMessage = "recieve is required")]

        public Boolean receive { get; set; }

    }
}

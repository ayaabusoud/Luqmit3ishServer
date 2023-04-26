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
        public int ResId { get; set; }

        [Required(ErrorMessage = "charity id is required")]
        public int CharId { get; set; }

        [Required(ErrorMessage = "dish id is required")]
        public int DishId { get; set; }


        public DateTime Date { get; set; }
        [Required(ErrorMessage = "number of dishes is required")]

        public int Quantity { get; set; }
        [Required(ErrorMessage = "recieve is required")]

        public Boolean Receive { get; set; }

    }
}

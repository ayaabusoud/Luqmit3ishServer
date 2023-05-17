using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ishBackend.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "restaurant id is required")]
        public int ResId { get; set; }

        [Required(ErrorMessage = "charity id is required")]
        public int CharId { get; set; }

        [Required(ErrorMessage = "dish id is required")]
        public int DishId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "recieve is required")]
        public Boolean Receive { get; set; }


    }
}

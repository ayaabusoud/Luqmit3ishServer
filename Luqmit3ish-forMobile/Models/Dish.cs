using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ishBackend.Models
{
    public class Dish
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int Id { get; set; }
        [Required(ErrorMessage = "User id number is required")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public String Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public String Description { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public String Type { get; set; }
        [Required(ErrorMessage = "Photo is required")]
        public String Photo { get; set; }
        [Required(ErrorMessage = "Keep valid number is required")]
        public int KeepValid { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
    }
}

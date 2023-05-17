using Luqmit3ishBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class OrderDish
    {
        public int Id { get; set; }
        public int ResId { get; set; }
        public int CharId { get; set; }
        public Dish Dish { get; set; }
        public int Quantity { get; set; }
        public Boolean Receive { get; set; }
    }
}

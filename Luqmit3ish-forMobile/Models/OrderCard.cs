using Luqmit3ishBackend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class OrderCard
    {
        public int Id { get; set; }
        public User Owner { get; set; }
        public List<OrderDish> Orders { get; set; }
    }
}

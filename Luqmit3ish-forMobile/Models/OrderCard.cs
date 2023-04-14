using Luqmit3ishBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class OrderCard
    {
        public int id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public List<OrderDish> data { get; set; }
    }
}

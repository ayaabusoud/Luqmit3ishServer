using Luqmit3ishBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class DishCard
    {
        public int  Id  { get; set; }
        public string DishName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Photo { get; set; }
        public int KeepValid { get; set; }
        public int Quantity { get; set; }
        public User Restaurant { get; set; }

    }
}

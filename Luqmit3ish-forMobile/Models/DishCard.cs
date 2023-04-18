using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Models
{
    public class DishCard
    {
        public int  id  { get; set; }
        public int restaurantId { get; set; }
        public string dishName { get; set; }
        public string restaurantName { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string photo { get; set; }
        public int keepValid { get; set; }
        public string pickUpTime { get; set; }
        public int quantity { get; set; }
        public string RestaurantImage { get; set; }
    }
}

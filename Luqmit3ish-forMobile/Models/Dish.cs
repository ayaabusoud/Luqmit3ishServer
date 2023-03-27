﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ishBackend.Models
{
    public class Dish
    {

        public int id { get; set; }
        [Required(ErrorMessage = "User id number is required")]
        public int user_id { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public String type { get; set; }
        public String photo { get; set; }
        public int keep_listed { get; set; }
        public DateTime pick_up_time { get; set; }
        [Required(ErrorMessage = "Dishes number is required")]
        public int number { get; set; }
    }
}

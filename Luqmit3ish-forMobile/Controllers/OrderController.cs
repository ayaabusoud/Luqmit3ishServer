using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Luqmit3ish_forMobile.Controllers
{
    [Route("api/Orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public OrderController(DatabaseContext context)
        {
            _context = context;


        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try { 
            if (_context == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var users = await _context.Order.ToListAsync();
                return Ok(users);
            }catch(Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody]OrderRequest request)
        {
            try {
                var order = new Order()
                {
                    res_id = request.res_id,
                    char_id = request.char_id,
                    dish_id = request.dish_id,
                    date = request.date,
                    number_of_dish = request.number_of_dish,
                    receive = request.receive,
                };
                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                return Ok("Added successfuly");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(NoContent());
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<UserRegister>> GetUserByEmail(int id)
        {
            try
            {
                if (_context is null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.User.FirstOrDefaultAsync(u => u.id == id);
                if (user is null)
                {
                    return NotFound();
                }
                return Ok(user);
            }

            catch (Exception e)
            {
                return StatusCode(500, "Internal server error " + e.Message);
            }
        }

        [HttpGet("/api/OrderPage/{id}")]
        public async Task<List<OrderCard>> GetOrdersGroupByRestaurantId(int id)
        {
            var orders = from o in _context.Order
                         join d in _context.Dish
                         on o.dish_id equals d.id
                         select new OrderDish
                         {
                             id = o.id,
                             res_id = o.res_id,
                             char_id = o.char_id,
                             dish_id = o.dish_id,
                             dishName = d.name,
                             date = o.date,
                             number_of_dish = o.number_of_dish,
                             receive = o.receive
                         };

            var myDictionary = orders.ToList()
           .Where(x => x.receive == false && x.char_id == id)
           .GroupBy(x => x.res_id)
           .OrderBy(x => x.Key)
           .ToDictionary(x => x.Key, x => x.Select(y => y).ToList());

            List<OrderCard> myList = new List<OrderCard>();
            foreach (var item in myDictionary)
            {
                var restaurant = await _context.User.FirstOrDefaultAsync(u => u.id == item.Key);
                OrderCard result = new OrderCard
                {
                    id = item.Key,
                    restaurantName = restaurant.name,
                    restaurantImage = restaurant.photo,
                    data = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }

        [HttpGet("/api/OrderPage")]
        public async Task<List<OrderCard>> GetOrdersGroupByRestaurant()
        {
            var orders =  from o in _context.Order
                     join d in _context.Dish
                     on o.dish_id equals d.id              
                     select new OrderDish
                     {
                     id = o.id,
                     res_id = o.res_id,
                     char_id = o.char_id,
                     dish_id = o.dish_id,
                     dishName = d.name,
                     date = o.date,
                     number_of_dish = o.number_of_dish,
                     receive = o.receive
                     };

             var myDictionary = orders.ToList()
            .Where(x => x.receive == false)
            .GroupBy(x => x.res_id)
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Select(y => y).ToList());

            List<OrderCard> myList = new List<OrderCard>();
            foreach (var item in myDictionary)
            {
                var restaurant = await _context.User.FirstOrDefaultAsync(u => u.id == item.Key);
                OrderCard result = new OrderCard
                {
                    id = item.Key,
                    restaurantName = restaurant.name,
                    restaurantImage = restaurant.photo,
                    data = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }


    }
}

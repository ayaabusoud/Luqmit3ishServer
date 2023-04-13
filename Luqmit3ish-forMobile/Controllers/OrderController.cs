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
                var oldOrder = await _context.Order.SingleOrDefaultAsync(d => d.dish_id == request.dish_id &&
                                                                          d.char_id == request.char_id &&
                                                                          d.receive == false);
                if (oldOrder == null)
                {
                    var order = new Order()
                    {
                        res_id = request.res_id,
                        char_id = request.char_id,
                        dish_id = request.dish_id,
                        date = request.date,
                        number_of_dish = request.number_of_dish,
                        receive = request.receive,
                    };
                    var dish = await _context.Dish.SingleOrDefaultAsync(d => d.id == order.dish_id);
                    dish.number -= order.number_of_dish;
                    _context.Order.Add(order);
                    await _context.SaveChangesAsync();
                    return Ok("Added successfuly");
                }
                oldOrder.number_of_dish+= request.number_of_dish;
                var newDish = await _context.Dish.SingleOrDefaultAsync(d => d.id == oldOrder.dish_id);
                newDish.number -= request.number_of_dish;
                await _context.SaveChangesAsync();
                return Ok("The number of dish increased successfully");
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

        
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrderById(int id) 
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

                var order = await _context.Order.FirstOrDefaultAsync(u => u.id == id);
                if (order is null)
                {
                    return NotFound();
                }
                return Ok(order);
            }

            catch (Exception e)
            {
                return StatusCode(500, "Internal server error " + e.Message);
            }
        }

        [HttpGet("/api/CharityOrders/{id}")]
        public async Task<List<OrderCard>> GetCharityOrders(int id)
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
                    name = restaurant.name,
                    image = restaurant.photo,
                    data = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }

        [HttpGet("/api/RestaurantOrders/{id}/{receive:bool}")]
        public async Task<List<OrderCard>> GetRestaurantOrders(int id, bool receive)
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
           .Where(x => x.receive == receive && x.res_id == id)
           .GroupBy(x => x.char_id)
           .OrderBy(x => x.Key)
           .ToDictionary(x => x.Key, x => x.Select(y => y).ToList());

            List<OrderCard> myList = new List<OrderCard>();
            foreach (var item in myDictionary)
            {
                var charity = await _context.User.FirstOrDefaultAsync(u => u.id == item.Key);
                OrderCard result = new OrderCard
                {
                    id = item.Key,
                    name = charity.name,
                    image = charity.photo,
                    data = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }

        [HttpGet("/api/AllCharitiesOrders")]
        public async Task<List<OrderCard>> GetAllCharityOrders()
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
                    name = restaurant.name,
                    image = restaurant.photo,
                    data = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }
        
      [HttpPatch("/api/CharityOrders/{id}")]
      public async Task<IActionResult> UpdateOrderDishCount(int id, string operation)
        {
            var order = await _context.Order.SingleOrDefaultAsync(o => o.id == id);
            var food = await _context.Dish.SingleOrDefaultAsync(d => d.id == order.dish_id);
            if (order == null)
            {
                return NotFound();
            }

            if (operation == "plus")
            {
                order.number_of_dish++;
                food.number--;
            }
            else
            {
                order.number_of_dish--;
                food.number++;
                if (order.number_of_dish == 0)
                {
                    _context.Order.Remove(order);
                }
               
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("/api/CharityOrders/{id}/{operation}")]
        public async Task<IActionResult> UpdateOrderDishCount(int id, string operation)
        {
            var order = await _context.Order.SingleOrDefaultAsync(o => o.id == id);
            var food = await _context.Dish.SingleOrDefaultAsync(d => d.id == order.dish_id);
            if (order == null)
            {
                return NotFound();
            }

            if (operation == "plus")
            {
                order.number_of_dish++;
                food.number--;
            }
            else
            {
                order.number_of_dish--;
                food.number++;
                if (order.number_of_dish == 0)
                {
                    _context.Order.Remove(order);
                }

            }
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}

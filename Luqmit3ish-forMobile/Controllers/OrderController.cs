using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody]Order request)
        {
            try {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var oldOrder = await _context.Order.SingleOrDefaultAsync(d => d.DishId == request.DishId &&
                                                                          d.CharId == request.CharId &&
                                                                          d.Receive == false);
                if (oldOrder == null)
                {
                    var dish = await _context.Dish.SingleOrDefaultAsync(d => d.Id == request.DishId);
                    dish.Quantity -= request.Quantity;
                    _context.Order.Add(request);
                    await _context.SaveChangesAsync();
                    return Ok("Added successfuly");
                }
                oldOrder.Quantity += request.Quantity;
                var newDish = await _context.Dish.SingleOrDefaultAsync(d => d.Id == oldOrder.DishId);
                newDish.Quantity -= request.Quantity;
                await _context.SaveChangesAsync();
                return Ok("The number of dish increased successfully");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [Authorize]
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

                var order = await _context.Order.FirstOrDefaultAsync(u => u.Id == id);
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

        [Authorize]
        [HttpGet("/api/CharityOrders/{id}")]
        public async Task<List<OrderCard>> GetCharityOrders(int id)
        {
            var orders = from o in _context.Order
                         join d in _context.Dish
                         on o.DishId equals d.Id
                         select new OrderDish
                         {
                             Id = o.Id,
                             ResId = o.ResId,
                             CharId = o.CharId,
                             Dish = d,
                             Quantity = o.Quantity,
                             Receive = o.Receive
                         };

            var myDictionary = orders.ToList()
           .Where(x => x.Receive == false && x.CharId == id)
           .GroupBy(x => x.ResId)
           .OrderBy(x => x.Key)
           .ToDictionary(x => x.Key, x => x.Select(y => y).ToList());

            List<OrderCard> myList = new List<OrderCard>();
            foreach (var item in myDictionary)
            {
                var restaurant = await _context.User.FirstOrDefaultAsync(u => u.Id == item.Key);
                OrderCard result = new OrderCard
                {
                    Id = item.Key,
                    Owner = restaurant,
                    Orders = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }

        [Authorize]
        [HttpGet("/api/RestaurantOrders/{id}/{receive:bool}")]
        public async Task<List<OrderCard>> GetRestaurantOrders(int id, bool receive)
        {
            var orders = from o in _context.Order
                         join d in _context.Dish
                         on o.DishId equals d.Id
                         select new OrderDish
                         {
                             Id = o.Id,
                             ResId = o.ResId,
                             CharId = o.CharId,
                             Dish = d,
                             Quantity = o.Quantity,
                             Receive = o.Receive
                         };

            var myDictionary = orders.ToList()
           .Where(x => x.Receive == receive && x.ResId == id)
           .GroupBy(x => x.CharId)
           .OrderBy(x => x.Key)
           .ToDictionary(x => x.Key, x => x.Select(y => y).ToList());


            List<OrderCard> myList = new List<OrderCard>();
            foreach (var item in myDictionary)
            {
                var charity = await _context.User.FirstOrDefaultAsync(u => u.Id == item.Key);
                OrderCard result = new OrderCard
                {
                    Id = item.Key,
                    Owner = charity,
                    Orders = item.Value
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
                     on o.DishId equals d.Id              
                     select new OrderDish
                     {
                         Id = o.Id,
                         ResId = o.ResId,
                         CharId = o.CharId,
                         Dish = d,
                         Quantity = o.Quantity,
                         Receive = o.Receive
                     };

             var myDictionary = orders.ToList()
            .Where(x => x.Receive == false)
            .GroupBy(x => x.ResId)
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Select(y => y).ToList());

            List<OrderCard> myList = new List<OrderCard>();
            foreach (var item in myDictionary)
            {
                var restaurant = await _context.User.FirstOrDefaultAsync(u => u.Id == item.Key);
                OrderCard result = new OrderCard
                {
                    Id = item.Key,
                    Owner = restaurant,
                    Orders = item.Value
                };
                myList.Add(result);
            }
            return myList;
        }

        [Authorize]
        [HttpDelete("delete/{charityId}/{restaurantId}")]
        public async Task<IActionResult> DeleteOrderCard(int charityId = 0, int restaurantId = 0)
        {
            var orders = await _context.Order.ToListAsync();

            foreach (Order order in orders)
            {
                if (order.ResId == restaurantId && order.CharId == charityId)
                {
                    var dish = await _context.Dish.SingleOrDefaultAsync(d => d.Id == order.DishId);
                    dish.Quantity = dish.Quantity + order.Quantity;
                    _context.Order.Remove(order);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok();
        }

        [Authorize]
        [HttpPatch("/api/CharityOrders/{id}/{operation}")]
        public async Task<IActionResult> UpdateOrderDishCount(int id, string operation)
        {
            var order = await _context.Order.SingleOrDefaultAsync(o => o.Id == id);
            var food = await _context.Dish.SingleOrDefaultAsync(d => d.Id == order.DishId);
            if (order == null)
            {
                return NotFound();
            }

            if (operation == "plus")
            {
                order.Quantity++;
                food.Quantity--;
            }
            else
            {
                order.Quantity--;
                food.Quantity++;
                if (order.Quantity == 0)
                {
                    _context.Order.Remove(order);
                }

            }
            await _context.SaveChangesAsync();

            return Ok();
        }
        
  [HttpGet("/BestRestaurant")]
        public async Task<DishesOrder> GetBestRestaurant()
        {
             
            int max = 0;
            DishesOrder bestRestarant = null;
            var orders = _context.Order.ToList()
           .Where(x => x.Receive == true && x.Date <= DateTime.Now && x.Date >= DateTime.Now.AddMonths(-1))
           .GroupBy(x => x.ResId)
           .ToDictionary(x => x.Key, x => x.Sum(y => y.Quantity));

            List<DishesOrder> myList = new List<DishesOrder>();
            foreach (var item in orders)
            {
                var restaurant = await _context.User.FirstOrDefaultAsync(u => u.Id == item.Key);
                DishesOrder result = new DishesOrder
                {
                    RestaurantName = restaurant.Name,
                    Dishes = item.Value
                };
                myList.Add(result);
            }
            foreach (var item in myList)
            {
               if(item.Dishes > max)
                {
                    bestRestarant = item;
                }
            }
            return bestRestarant;
        }

        [Authorize]
        [HttpPatch("/api/{id}/receive")]
        public async Task<IActionResult> UpdateOrderRecieveStatus(int id)
        {
            try
            {
                var order = await _context.Order.FindAsync(id);
                if (order.Receive == true) return Ok();
                order.Receive = true;
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }

        }
    }
}

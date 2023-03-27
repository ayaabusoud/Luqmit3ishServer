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
            return await _context.Order.ToListAsync();
            }catch(Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Order>> AddOrder([FromBody]Order order)
        {
            try {
                _context.Order.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetOrders", new { id = order.user_id }, order);
            }
            catch(Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpPut("{user_id}/{dish_id}/{date}")]
        public async Task<ActionResult<Order>>PutOrder( int user_id, int dish_id, DateTime date, [FromBody] Order order)
        {
            if (user_id != order.user_id || dish_id != order.dish_id||date!=order.date||!ModelState.IsValid)
            {
                Console.WriteLine(order.ToString());


                return BadRequest();
            }
        

            var orderResult = await _context.Order.FindAsync(user_id, dish_id, date);
            Console.WriteLine(orderResult.ToString());

            try
            {
                if (orderResult == null)
                {
                    return NotFound();
                }
                Console.WriteLine(1);
                _context.Entry(orderResult).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                Console.WriteLine(2);
      
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }

            return NoContent();


        }

        [HttpDelete("{user_id}/{dish_id}/{date}")]
        public async Task<IActionResult> DeleteOrder(int user_id, int dish_id, DateTime date)
        {
            var order = await _context.Order.FindAsync(user_id,dish_id,date);

            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }



    }
}

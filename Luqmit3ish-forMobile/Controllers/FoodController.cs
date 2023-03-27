using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Luqmit3ish_forMobile.Controllers
{
    [Route("api/Food")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public FoodController(DatabaseContext context)
        {
            _context = context;


        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishes()
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
            return await _context.Dish.ToListAsync();
            }catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpDelete("{id}/{user_id}")]
        public async Task<IActionResult> DeleteDish(int id,int user_id)
        {
            var orders = _context.Order.Where(order => order.dish_id == id).ToList();

            _context.Order.RemoveRange(orders);
            await _context.SaveChangesAsync();
            var dish = await _context.Dish.FindAsync(id, user_id);

            if (dish == null)
            {
                return NotFound();
            }
            _context.Dish.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}

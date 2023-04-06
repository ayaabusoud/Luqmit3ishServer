using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            try
            {
                if (_context == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return await _context.Dish.ToListAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }
        [HttpGet("DishCard")]
        public IQueryable<Object> GetDishCards()
        {
            
                return from d in _context.Dish
                       join u in _context.User
                       on d.user_id equals u.id 
                       select new
                       {
                           id = d.id,
                           restaurantId = d.user_id,
                           dishName = d.name,
                           restaurantName = u.name,
                           description = d.description,
                           type = d.type,
                           photo = d.photo,
                           keepValid = d.keep_listed,
                           pickUpTime = d.pick_up_time,
                           quantity = d.number

                        };
            
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Dish>>> GetDishById(int id)
        {
            var dish = await _context.Dish.FindAsync(id);
            try
            {
                if (_context == null || dish == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(dish);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }
        [HttpGet("Restaurant/{user_id}")]
        public async Task<ActionResult<ObservableCollection<Dish>>> GetDishByResId(int user_id)
        {
            
            try
            {
                ObservableCollection<Dish> dishes = new ObservableCollection<Dish>();
                if (_context == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var allDishes = await _context.Dish.ToListAsync();
                foreach(Dish dish in allDishes)
                {
                    if (dish.user_id == user_id)
                        dishes.Add(dish);
                }
                return dishes;

            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }


        [HttpPost("AddDish")]
        public async Task<IActionResult> AddeDish([FromBody] AddFoodRequest dish)
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
                var Dish = new Dish()
                {
                    name = dish.name,
                    description = dish.description,
                    keep_listed = dish.keep_listed,
                    number = dish.number,
                    photo = dish.photo,
                    pick_up_time = dish.pick_up_time,
                    type = dish.type,
                    user_id = dish.user_id,
                };
                _context.Dish.Add(Dish);
                await _context.SaveChangesAsync();

                return Ok("Added successfuly");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }




        [HttpPost]
        public async Task<ActionResult<Dish>> CreateUser([FromBody] Dish dish)
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
                _context.Dish.Add(dish);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetDishes", new { id = dish.id }, dish);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateDish(int id, [FromBody] AddFoodRequest dish)
        {
            try
            {
                if (_context == null)
                {
                    return NotFound();
                }
                
                _context.Entry(dish).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            try { 
            var orders = _context.Order.Where(order => order.dish_id == id).ToList();

            _context.Order.RemoveRange(orders);
            await _context.SaveChangesAsync();
            var dish = await _context.Dish.FindAsync(id);

            if (dish == null)
            {
                return NotFound();
            }
            _context.Dish.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }


    }
}

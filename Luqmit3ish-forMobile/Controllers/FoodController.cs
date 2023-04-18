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
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.IO;

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
        [HttpGet("Search/{searchRequest}/{type}")]
        public List<DishCard> GetSearchedDishCards(string searchRequest, string type)
        {

            var dishes = from d in _context.Dish
                         join u in _context.User
                         on d.user_id equals u.id
                         select new DishCard
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

            List<DishCard> searchResult = new List<DishCard>();

            if (type == "Restaurants")
            {
                foreach (DishCard dish in dishes)
                {
                    if (dish.restaurantName.ToLower().Contains(searchRequest.ToLower()))
                    {
                        searchResult.Add(dish);
                    }
                }
            }
            else if (type == "Dishes")
            {
                foreach (DishCard dish in dishes)
                {
                    if (dish.dishName.ToLower().Contains(searchRequest.ToLower()))
                    {
                        searchResult.Add(dish);
                    }
                }
            }
            else
            {
                foreach (DishCard dish in dishes)
                {
                    if (dish.dishName.ToLower().Contains(searchRequest.ToLower()) || dish.restaurantName.ToLower().Equals(searchRequest.ToLower()))
                    {
                        searchResult.Add(dish);
                    }
                }
            }


            return searchResult;
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

                return Ok(Dish);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }
        [HttpGet("DishCard/{id}")]
        public IQueryable<DishCard> GetDishCardById(int id)
        {

            return from d in _context.Dish
                   join u in _context.User
                   on d.user_id equals u.id
                   where d.id == id
                   select new DishCard
                   {
                       id = d.id,
                       restaurantId = d.user_id,
                       dishName = d.name,
                       restaurantName = u.name,
                       description = d.description,
                       type = d.type,
                       photo = d.photo,
                       RestaurantImage = u.photo,
                       keepValid = d.keep_listed,
                       pickUpTime = d.pick_up_time,
                       quantity = d.number

                   };

        }



        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateDish(int id, [FromBody] Dish dish)
        {
            try
            {
                if (_context == null)
                {
                    return NotFound();
                }
                if (id != dish.id || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
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
        
        [HttpPost("UploadPhoto/{food_id}")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo, int food_id)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No photo uploaded");
            }

            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=luqmit3ish5;AccountKey=wf/sCEDpRkFExVY91mqUaZgzd/H0v1sl/a69oaGYtGGVMr9a4KnuHY5YCeKgtiQSWhiUoGEwjZyE+AStqTYKQA==;EndpointSuffix=core.windows.net"; string containerName = "photos";

                
                CloudBlobClient blobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
               
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                
                CloudBlockBlob blob = container.GetBlockBlobReference($"{food_id}_{photo.FileName}");
                using (Stream stream = photo.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }
                
                Dish dish = await _context.Dish.FirstOrDefaultAsync(u => u.id == food_id);
                dish.photo = blob.Uri.ToString();
                await _context.SaveChangesAsync();

                return Ok(blob.Uri.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading photo: {ex.Message}");
            }
        }




    }
}

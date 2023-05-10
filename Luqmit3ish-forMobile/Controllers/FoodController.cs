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
using Microsoft.AspNetCore.Authorization;

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
                       on d.UserId equals u.Id 
                       select new
                       {
                           Id = d.Id,
                           DishName = d.Name,
                           Description = d.Description,
                           Type = d.Type,
                           Photo = d.Photo,
                           KeepValid = d.KeepValid,
                           Quantity = d.Quantity,
                           Restaurant = u

                        };
            
        }
        [HttpGet("Search/{searchRequest}/{type}")]
        public List<DishCard> GetSearchedDishCards(string searchRequest, string type)
        {

            var dishes = from d in _context.Dish
                         join u in _context.User
                         on d.UserId equals u.Id
                         select new DishCard
                         {
                             Id = d.Id,
                             DishName = d.Name,
                             Description = d.Description,
                             Type = d.Type,
                             Photo = d.Photo,
                             KeepValid = d.KeepValid,
                             Quantity = d.Quantity,
                             Restaurant = u

                         };

            List<DishCard> searchResult = new List<DishCard>();

            if (type == "Restaurants")
            {
                foreach (DishCard dish in dishes)
                {
                    if (dish.Restaurant.Name.ToLower().Contains(searchRequest.ToLower()))
                    {
                        searchResult.Add(dish);
                    }
                }
            }
            else if (type == "Dishes")
            {
                foreach (DishCard dish in dishes)
                {
                    if (dish.DishName.ToLower().Contains(searchRequest.ToLower()))
                    {
                        searchResult.Add(dish);
                    }
                }
            }
            else
            {
                foreach (DishCard dish in dishes)
                {
                    if (dish.DishName.ToLower().Contains(searchRequest.ToLower()) || dish.Restaurant.Name.ToLower().Equals(searchRequest.ToLower()))
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
                    if (dish.UserId == user_id)
                        dishes.Add(dish);
                }
                return dishes;

            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }


        [Authorize]
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
                    Name = dish.Name,
                    Description = dish.Description,
                    KeepValid = dish.KeepValid,
                    Quantity = dish.Quantity,
                    Photo = dish.Photo,
                    Type = dish.Type,
                    UserId = dish.UserId,
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
                   on d.UserId equals u.Id
                   where d.Id == id
                   select new DishCard
                   {
                       Id = d.Id,
                       DishName = d.Name,
                       Description = d.Description,
                       Type = d.Type,
                       Photo = d.Photo,
                       KeepValid = d.KeepValid,
                       Quantity = d.Quantity,
                       Restaurant = u

                   };

        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateDish(int id, [FromBody] Dish dish)
        {
            try
            {
                if (_context == null)
                {
                    return NotFound();
                }
                if (id != dish.Id || !ModelState.IsValid)
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            try { 
            var orders = _context.Order.Where(order => order.DishId == id).ToList();

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

        [Authorize]
        [HttpPost("UploadPhoto/{food_id}")]
        public async Task<IActionResult> UploadPhoto(IFormFile photo, int food_id)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No photo uploaded");
            }

            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=luqmit3ish2;AccountKey=DFmyG75KqtNRwOwqEf4vpxGRcel7lu9d8VLBh4jui/sD8c3l/xP+gOL4OCNvlFHvZihXxhRSdWdK+AStYsHV7w==;EndpointSuffix=core.windows.net";
                string containerName = "photos";

                
                CloudBlobClient blobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();
               
                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                
                CloudBlockBlob blob = container.GetBlockBlobReference($"{food_id}_{photo.FileName}");
                using (Stream stream = photo.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }
                
                Dish dish = await _context.Dish.FirstOrDefaultAsync(u => u.Id == food_id);
                dish.Photo = blob.Uri.ToString();
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

using Luqmit3ish_forMobile.Encode;
using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luqmit3ish_forMobile.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IEncrypt _encrypt;
        private string key = "E546C8DF278CD5931069B522E695D4F2";
        private readonly DatabaseContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly IConfiguration _config;


        public UserController(DatabaseContext context, IPasswordHasher<User> passwordHasher, IConfiguration config)
        {
            _context = context;
            _config = config;
            _passwordHasher = passwordHasher;
            _encrypt = new Encrypt();

        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            if(user.Type == "Restaurant"){
                var ordersToDelete = _context.Order.Where(o => o.ResId == user.Id);
                _context.Order.RemoveRange(ordersToDelete);
                _context.SaveChanges();

                var dishesToDelete = _context.Dish.Where(o => o.UserId == user.Id);
                _context.Dish.RemoveRange(dishesToDelete);
                _context.SaveChanges();
            }
            else
            {
                var ordersToDelete = _context.Order.Where(o => o.CharId == user.Id);
                _context.Order.RemoveRange(ordersToDelete);
                _context.SaveChanges();
            }
            

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(NoContent());
        }

     
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRegister>>> GetUsers()
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
                var users = await _context.User.ToListAsync();
                return Ok(users);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }

        }

        [HttpGet("{email}")]
        public async Task<ActionResult<UserRegister>> GetUserByEmail(string email)
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

                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == email);
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

        [HttpGet("id/{id}")]
        public async Task<ActionResult<UserRegister>> GetUserById(int id)
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

                var user = await _context.User.FirstOrDefaultAsync(u => u.Id == id);
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

        [HttpPost]
        public async Task<ActionResult<string>> CreateUser([FromBody] User user)
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

                user.Location = "Palestine";
                user.Photo = "https://luqmit3ish2.blob.core.windows.net/photos/DefaultProfile.png";
                user.OpeningHours = "11:00am-11:00pm";
                user.Password = _encrypt.EncryptPassword(user.Password);
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                var token = await GenerateToken(user);
                return token;

            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserRegister>> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (_context is null)
                {
                    return NotFound();
                }
                if (id != user.Id || !ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(NoContent());
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [Authorize]
        [HttpPatch("resetPassword/{id}/{newPassword}")]
        public async Task<IActionResult> ResetPassword(int id, String newPassword)
        {
        try{
            ResetPasswordRequest request = new ResetPasswordRequest
            {
                Id = id,
                Password = newPassword,
            };

            var user = await _context.User.SingleOrDefaultAsync(u => u.Id == request.Id);

            if (user == null)
            {
                return NotFound("userNotFound");
            }
            user.Password = _encrypt.EncryptPassword(request.Password);
            await _context.SaveChangesAsync();
            return Ok("Password updated");
            }catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }
        [HttpPatch("ForgetPassword/{id}/{newPassword}")]
        public async Task<IActionResult> ForgetPassword(int id, String newPassword)
        {
            try
            {
                ResetPasswordRequest request = new ResetPasswordRequest
                {
                    Id = id,
                    Password = newPassword,
                };

                var user = await _context.User.SingleOrDefaultAsync(u => u.Id == request.Id);

                if (user == null)
                {
                    return NotFound("userNotFound");
                }
                user.Password = _encrypt.EncryptPassword(request.Password);
                await _context.SaveChangesAsync();
                return Ok("Password updated");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginRequest request)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == request.Email);
                if (request.Email == null || request.Password == null)
                {
                    return BadRequest("Email and password should not be empty");
                }
                if (user is null)
                {
                    return BadRequest("User not found.");
                }
                bool passwordMatches = _encrypt.VerifyPassword(request.Password, user.Password);

                if (!passwordMatches)
                {
                    return BadRequest("The email or password is not correct");
                }


                var token = await GenerateToken(user);
                return token;
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        private async Task<string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:secretKey").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
             new Claim(ClaimTypes.Role, user.Type)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddMonths(1)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var strToken = tokenHandler.WriteToken(token);
            return await Task.FromResult(strToken);
        }

  

        [Authorize]
        [HttpPost("UploadPhoto/{user_id}")]
        public async Task<IActionResult> UploadUserPhoto(IFormFile photo, int user_id)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No photo uploaded");
            }

            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=luqmit3ish7;AccountKey=YLxG10fBFBQF0UAI6C2IcvnGThtkKOGiix9i/lEwuPuzPiyYNrob755YPOGoEmxBLUzs8w5uVNVy+AStr6F1Fg==;EndpointSuffix=core.windows.net";
                string containerName = "photos";


                CloudBlobClient blobClient = CloudStorageAccount.Parse(connectionString).CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                CloudBlockBlob blob = container.GetBlockBlobReference($"{user_id}_{photo.FileName}");
                using (Stream stream = photo.OpenReadStream())
                {
                    await blob.UploadFromStreamAsync(stream);
                }

                User user = await _context.User.FirstOrDefaultAsync(u => u.Id == user_id);
                user.Photo = blob.Uri.ToString();
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

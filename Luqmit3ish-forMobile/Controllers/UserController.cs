using Luqmit3ish_forMobile.Encrypt;
using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Luqmit3ish_forMobile.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private string key = "E546C8DF278CD5931069B522E695D4F2";
        private readonly DatabaseContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;


        public UserController(DatabaseContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
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
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
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
                user.Password = EncryptDecrypt.EncodePasswordToBase64(user.Password);
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUsers", new { id = user.Id }, user);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }



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
            user.Password = EncryptDecrypt.EncodePasswordToBase64(request.Password);
            await _context.SaveChangesAsync();
            return Ok("Password updated");
            }catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Email == request.Email);
                request.Password = EncryptDecrypt.EncodePasswordToBase64(request.Password);
                if (request.Email == null || request.Password == null)
                {
                    return BadRequest("Email and password should not be empty");
                }
                if (user is null)
                {
                    return BadRequest("User not found.");
                }
                if (user.Password != request.Password)
                {
                    return BadRequest("The email or password is not correct");
                }
                return Ok($"Welcome Back {request.Email}");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            try
            {

            if (_context.User.Any(u => u.Email == request.Email))
            {
                return BadRequest("user already exist!");
            }
            var user = new User()
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Password = request.Password,
                Type = request.Type,
            };
            user.Password = EncryptDecrypt.EncodePasswordToBase64(user.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Added successfuly");
            }catch(Exception )
            {
                return BadRequest();
            }

        }

        [HttpPost("UploadPhoto/{user_id}")]
        public async Task<IActionResult> UploadUserPhoto(IFormFile photo, int user_id)
        {
            if (photo == null || photo.Length == 0)
            {
                return BadRequest("No photo uploaded");
            }

            try
            {
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=luqmit3ish1;AccountKey=urs+FFPvhubRO11inUtniXSQt8ciyXtYGiQ5o/eW0mcOnXmItgcEqh6xRORPNgvExqApnb0uQ5Fc+AStHKouvg==;EndpointSuffix=core.windows.net";
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

using Luqmit3ish_forMobile.Encrypt;
using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Data;
using Luqmit3ishBackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

                var user = await _context.User.FirstOrDefaultAsync(u => u.email == email);
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
                user.password = EncryptDecrypt.EncodePasswordToBase64(user.password);
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUsers", new { id = user.id }, user);
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<UserRegister>> UpdateUser(int id, [FromBody] SignUpRequest user)
        {
            try
            {
                if (_context is null)
                {
                    return NotFound();
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
        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, String NewPassword)
        {

            var user = await _context.User.SingleOrDefaultAsync(u => u.id == request.id);

            if (user == null)
            {
                return NotFound("userNotFound");
            }
            user.password = EncryptDecrypt.EncodePasswordToBase64(NewPassword);
            await _context.SaveChangesAsync();
            return Ok("Password updated");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            try
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.email == request.Email);
                request.Password = EncryptDecrypt.EncodePasswordToBase64(request.Password);
                if (request.Email == null || request.Password == null)
                {
                    return BadRequest("Email and password should not be empty");
                }
                if (user is null)
                {
                    return BadRequest("User not found.");
                }
                if (user.password != request.Password)
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
            if (_context.User.Any(u => u.email == request.email))
            {
                return BadRequest("user already exist!");
            }
            var user = new User()
            {
                name = request.name,
                email = request.email,
                phone = request.phone,
                password = request.password,
                location = request.location,
                type = request.type,
                photo = request.photo
            };
            user.password = EncryptDecrypt.EncodePasswordToBase64(user.password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Added successfuly");

        }

    }
}

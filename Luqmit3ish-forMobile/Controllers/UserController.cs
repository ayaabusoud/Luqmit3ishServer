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
            try {
            if(_context is null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var users =  await _context.User.ToListAsync();
                return Ok(users);
            }
            catch(Exception e)
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
                if(_context.User.Any(u => u.email == email))
                {
                    return BadRequest("User already exists");
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

        [HttpPost]
        public async Task<ActionResult<UserRegister>> CreateUser([FromBody]UserRegister user)
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
                if(_context.User.Any(u => u.email == user.email))
                {
                    return BadRequest("User alredy exists.");
                }

                CreatePasswordHash(user.password,
                out byte[] passwordHash,
                out byte[] passwordSalt);

                var newUser = new User
                {
                    //id = user.id,
                    name = user.name,
                    email = user.email,
                    phone = user.phone,
                    photo=user.photo,
                    password = user.password,
                    location = user.location,
                    type = user.type,
                    PasswordHa = passwordHash,
                    PasswordSalt = passwordSalt,
                    VereficationToken = CreateRandomToken()
                };

                _context.User.Add(newUser);
                await _context.SaveChangesAsync();
                
                return Ok("User successfuly created!");
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac
                    .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }


        private string CreateRandomToken()
        {
            byte[] bytes = new byte[64];
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToHexString(bytes);
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
                if (id != user.id || !ModelState.IsValid)
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
        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] User User, String NewPassword)
        {

            var user = await _context.User.SingleOrDefaultAsync(u => u.id == User.id);

            if (user == null)
            {
                return NotFound();
            }


            user.password = _passwordHasher.HashPassword(user, NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.email == request.Email);

            if(user is null)
            {
                return BadRequest("User not found.");
            }
            if(user.password != request.Password)
            {
                return BadRequest("The email or password is not correct");
            }
            return Ok($"Welcome Back {request.Email}");
        }


    }
}

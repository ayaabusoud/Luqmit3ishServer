﻿using Luqmit3ishBackend.Data;
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
    [Route("api/Users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        
        public UserController(DatabaseContext context)
        {
            _context = context;


        }
        [HttpGet]
      public async Task<ActionResult<IEnumerable<User>>> GetUsers()
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
            return await _context.User.ToListAsync();
            }
            catch(Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
            
        }
        
        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
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
                return user;
            }

            catch (Exception e)
            {
                return StatusCode(500, "Internal server error " + e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody]User user)
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
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody]User user)
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
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Internal server error" + e.Message);
            }
        }
    }
}

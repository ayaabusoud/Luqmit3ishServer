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



    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Luqmit3ish_forMobile.Services;
using Luqmit3ishBackend.Data;
using Luqmit3ish_forMobile.Models;

namespace Luqmit3ish_forMobile.Controllers
{
    [Route("api/Email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailSender _emailSender;
        public EmailController()
        {
        _emailSender = new EmailSender();

        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest emailRequest)
        {
            if (string.IsNullOrEmpty(emailRequest.recipientEmail))
            {
                return BadRequest("One or more input parameters are missing.");
            }
            try
            {
                await _emailSender.SendEmailAsync(emailRequest.recipientName,emailRequest.recipientEmail);
                return Ok(new { VerificationCode = _emailSender.code });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }

    }
}

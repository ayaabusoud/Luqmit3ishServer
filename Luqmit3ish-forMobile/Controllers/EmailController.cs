using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Luqmit3ish_forMobile.Services;

namespace Luqmit3ish_forMobile.Controllers
{
    public class EmailController : ControllerBase
    {
        private readonly EmailSender _emailSender;
        public EmailController()
        {
            _emailSender = new EmailSender();
        }
        
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(string recipientName, string recipientEmail)
        {
            if (string.IsNullOrEmpty(recipientEmail))
            {
                return BadRequest("One or more input parameters are missing.");
            }
            try
            {
                await _emailSender.SendEmailAsync(recipientName,recipientEmail);
                return Ok(new { VerificationCode = _emailSender.code });
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }

    }
}

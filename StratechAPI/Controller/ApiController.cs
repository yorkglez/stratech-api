using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StratechAPI.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StratechAPI.Controller
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly AudienceService _audienceService;
        private readonly EmailService _emailService;

        public ApiController(AudienceService audienceService, EmailService emailService)
        {
            _audienceService = audienceService;
            _emailService = emailService;
        }

        /**
          * Retrieves audience data for the specified year and quarter.
          * Validates the input parameters and returns the corresponding data.
          * Sends a CSV file link to the provided email if data is found.
         */
        [HttpGet("get")]
        public async Task<IActionResult> GetAudiences([FromQuery] string year, [FromQuery] string quarter, [FromQuery] string email)
        {
            if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(quarter) || string.IsNullOrEmpty(email))
            {
                return BadRequest("The parameters year, quarter, and email are required.");
            }

            try
            {
                var data = await _audienceService.FindAudienceData(year, quarter, email);
             
                if (data == null || !data.Any())
                {
                    return NotFound("No data found for the provided parameters.");
                }

                return Ok("sueccess");
            }catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }     
    }
}

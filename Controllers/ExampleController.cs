using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sindika.aspnet.validation.Validators;


namespace ExampleAspProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        public IActionResult ValidateExample()
        {
            // if validation not valid
            var jsonInput = "{\"aneh\":\"test@example.com\"}";

            // if validation valid
            // var jsonInput = "{\"email\":\"test@example.com\"}";
            var jsonElement = System.Text.Json.JsonDocument.Parse(jsonInput).RootElement;

            try
            {
                ContentValidator.ValidateRequiredFields(jsonElement, new List<string> { "email" });
                return Ok("Validation Success");
            }
            catch (Exception ex)
            {
                return BadRequest($"Validation Error: {ex.Message}");
            }
        }
    }
}
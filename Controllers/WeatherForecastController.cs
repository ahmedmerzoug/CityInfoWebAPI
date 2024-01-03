using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherForecastController : Controller
    {
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray();

            return forecasts;
        }

        [HttpGet("today")]
        public WeatherForecast GetToday()
        {
            // Replace this with your logic to get today's weather forecast
            return new WeatherForecast
            {
                Date = DateTime.Now,
                TemperatureC = 25,
                Summary = "Sunny"
            };
        }

        [HttpGet("custom")]
        public IActionResult GetCustom([FromQuery] string customParam)
        {
            // Replace this with your logic based on the custom parameter
            return Ok($"You provided a custom parameter: {customParam}");
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    }

    public record WeatherForecast
    {
        public DateTime Date { get; init; }
        public int TemperatureC { get; init; }
        public string Summary { get; init; }
    }
}

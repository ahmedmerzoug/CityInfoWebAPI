using CityInfoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetTestValues()
        {
            var cityToReturn = CitiesDataStore.Instance.Cities;
            return Ok(cityToReturn);
        }

        [HttpGet("{id}")] // GET api/Cities/{id}
        public ActionResult<CityDto> Get(int id)
        {
            // Logic for the second GET method
            var cityToReturn = CitiesDataStore.Instance.Cities.FirstOrDefault(x=>x.Id==id);
            if(cityToReturn==null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);

        }


    }
}

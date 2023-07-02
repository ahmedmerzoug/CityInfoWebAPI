using CityInfoAPI.Abstraction;
using CityInfoAPI.Entities;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {

        public CitiesDataStore _citiesDataStore;

        public CitiesController(CitiesDataStore citiesDataStore)
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }
        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetTestValues()
        {
            var cityToReturn = _citiesDataStore.Cities;
            return Ok(cityToReturn);
        }

        [HttpGet("{id}")] // GET api/Cities/{id}
        public ActionResult<CityDto> Get(int id)
        {
            // Logic for the second GET method
            var cityToReturn = _citiesDataStore.Cities.FirstOrDefault(x=>x.Id==id);
            if(cityToReturn==null)
            {
                return NotFound();
            }
            return Ok(cityToReturn);

        }


    }
}

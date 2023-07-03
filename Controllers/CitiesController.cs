using AutoMapper;
using CityInfoAPI.Abstraction;
using CityInfoAPI.Entities;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {

        public ICityInfoRepository _cityInfoRepository;
        public IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsOfInterestDto>>> GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsOfInterestDto>>(cityEntities));

            //Old way, we use Auto Mapper now
            //var results = new List<CityWithoutPointsOfInterestDto>();
            //foreach (var city in cityEntities)
            //{
            //    results.Add(new CityWithoutPointsOfInterestDto
            //    {
            //        Description = city.Description,
            //        Id = city.Id,
            //        Name = city.Name,
            //    });
            //}

        }



        [HttpGet("{id}")] // GET api/Cities/{id}
        public async Task<IActionResult> GetCity(int id, bool includePointOfIntrest = false)
        {
            // Logic for the second GET method
            var cityToReturn = await _cityInfoRepository.GetCityAsync(id, includePointOfIntrest);
            if (cityToReturn == null)
            {
                return NotFound();
            }

            if (includePointOfIntrest)
            {
                return Ok(_mapper.Map<CityDto>(cityToReturn));
            }
            
            return Ok(_mapper.Map<CityWithoutPointsOfInterestDto>(cityToReturn));

        }


    }
}

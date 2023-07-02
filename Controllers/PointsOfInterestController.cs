using CityInfoAPI.Abstraction;
using CityInfoAPI.Models;
using CityInfoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/Cities/{cityId}/PointsOfInterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;

        public readonly IMailService _localMailService;

        public CitiesDataStore _citiesDataStore;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService localMailService,
            CitiesDataStore citiesDataStore)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _localMailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }


        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            try
            {
                var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);

                if (city == null)
                {
                    return NotFound();
                }
                return Ok(city.NumberOfPointsOfInterest);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"city with the Id :{cityId} does not exit!", ex);
                return StatusCode(500, "A problem happend while handling your request");
            }
        }

        [HttpGet("{pointOfIntrestId}", Name = "GetPointsOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointsOfInterest(int cityId, int pointOfIntrestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestDto = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfIntrestId);
            if (pointOfInterestDto == null)
            {
                return NotFound();
            }
            return Ok(pointOfInterestDto);
        }


        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointsOfInterest(int cityId, PointOfInterestForCreationDto pointOfIntrest)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //var test = city.PointsOfInterest.Max(x => x.Id);
            var maxPointOfIntrestId = _citiesDataStore.Cities.SelectMany(x => x.PointsOfInterest).Max(p => p.Id);

            var finalPointOfIntrest = new PointOfInterestDto
            {
                Id = ++maxPointOfIntrestId,
                Description = pointOfIntrest.Description,
                Name = pointOfIntrest.Name,
            };

            city.PointsOfInterest.Add(finalPointOfIntrest);
            return CreatedAtRoute("GetPointsOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfIntrestId = finalPointOfIntrest.Id
                }, finalPointOfIntrest);
        }

        [HttpPatch("{pointOfIntrestId}")]
        public ActionResult<PointOfInterestDto> UpdatePointsOfInterest(int cityId, int pointOfIntrestId,
           JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestDto = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfIntrestId);
            if (pointOfInterestDto == null)
            {
                return NotFound();
            }

            var pointOfIntrestToPatch = new PointOfInterestForUpdateDto
            {
                Description = pointOfInterestDto.Description,
                Name = pointOfInterestDto.Name
            };

            patchDocument.ApplyTo(pointOfIntrestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfIntrestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestDto.Description = pointOfIntrestToPatch.Description;
            pointOfInterestDto.Name = pointOfIntrestToPatch.Name;


            return NoContent();
        }

        [HttpDelete("{pointOfIntrestId}")]
        public ActionResult<PointOfInterestDto> DeletPointsOfInterest(int cityId, int pointOfIntrestId)
        {
            var city = _citiesDataStore.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestDto = city.PointsOfInterest.FirstOrDefault(x => x.Id == pointOfIntrestId);
            if (pointOfInterestDto == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointOfInterestDto);
            _localMailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestDto.Name} with id {pointOfInterestDto.Id} was deleted.");
            return NoContent();
        }


    }
}
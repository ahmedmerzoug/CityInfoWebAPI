using CityInfoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfoAPI.Controllers
{
    [Route("api/Cities/{cityId}/PointsOfInterest")]
    [ApiController]
    public class PointsOfInterestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest(int cityId)
        {
            var city = CitiesDataStore.Instance.Cities.FirstOrDefault(x => x.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }
            return Ok(city.NumberOfPointsOfInterest);
        }

        [HttpGet("{pointOfIntrestId}", Name = "GetPointsOfInterest")]
        public ActionResult<PointOfInterestDto> GetPointsOfInterest(int cityId, int pointOfIntrestId)
        {
            var city = CitiesDataStore.Instance.Cities.FirstOrDefault(x => x.Id == cityId);
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
            var city = CitiesDataStore.Instance.Cities.FirstOrDefault(x => x.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            //var test = city.PointsOfInterest.Max(x => x.Id);
            var maxPointOfIntrestId = CitiesDataStore.Instance.Cities.SelectMany(x => x.PointsOfInterest).Max(p => p.Id);

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
            var city = CitiesDataStore.Instance.Cities.FirstOrDefault(x => x.Id == cityId);
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
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(!TryValidateModel(pointOfIntrestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointOfInterestDto.Description = pointOfIntrestToPatch.Description;
            pointOfInterestDto.Name = pointOfIntrestToPatch.Name;


            return NoContent();

        }
    }
}

using AutoMapper;
using CityInfoAPI.Abstraction;
using CityInfoAPI.Entities;
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

        public readonly IMailService _mailService;
        public IMapper _mapper;
        public ICityInfoRepository _cityInfoRepository;


        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService localMailService,
            IMapper mapper, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = localMailService ?? throw new ArgumentNullException(nameof(localMailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));

        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfInterest(int cityId)
        {       
            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"city with the Id :{cityId} does not exit!");
                    return NotFound();
                }
                var pointOfIntrestForCity = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);
                
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointOfIntrestForCity));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"city with the Id :{cityId} does not exit!", ex);
                return StatusCode(500, "A problem happend while handling your request");
            }
        }

        [HttpGet("{pointOfIntrestId}", Name = "GetPointsOfInterest")]
        public async Task <ActionResult<PointOfInterestDto>> GetPointsOfInterest(int cityId, int pointOfIntrestId)
        {

            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"city with the Id :{cityId} does not exit!");
                return NotFound();
            }

            var pointOfIntrest = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntrestId);
            if (pointOfIntrest == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PointOfInterestDto>(pointOfIntrest));
        }


        [HttpPost]
        public async Task <ActionResult<PointOfInterestDto>> CreatePointsOfInterest(int cityId, PointOfInterestForCreationDto pointOfIntrest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"city with the Id :{cityId} does not exit!");
                return NotFound();
            }

            var finalPointOfIntrest = _mapper.Map<PointOfInterest>(pointOfIntrest);
            await _cityInfoRepository.AddAPointOfIntrestToACityAsync(cityId, finalPointOfIntrest);
            await _cityInfoRepository.SaveChnagesAsync();

            var createdPointOfIntrestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfIntrest);

            return CreatedAtRoute("GetPointsOfInterest",
                new
                {
                    cityId = cityId,
                    pointOfIntrestId = createdPointOfIntrestToReturn.Id
                }, createdPointOfIntrestToReturn);
        }

        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestEntity);

            await _cityInfoRepository.SaveChnagesAsync();

            return NoContent();
        }

        [HttpPatch("{pointOfIntrestId}")]
        public async Task <ActionResult<PointOfInterestDto>> UpdatePointsOfInterest(int cityId, int pointOfIntrestId,
           JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"city with the Id :{cityId} does not exit!");
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntrestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChnagesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfIntrestId}")]
        public async Task<ActionResult<PointOfInterestDto>> DeletPointsOfInterest(int cityId, int pointOfIntrestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = await _cityInfoRepository.GetPointOfInterestForCityAsync(cityId, pointOfIntrestId);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            await _cityInfoRepository.SaveChnagesAsync();

            _mailService.Send(
                "Point of interest deleted.",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted.");

            return NoContent();
        }


    }
}
using AutoMapper;
using BookReview.Dto;
using BookReview.Models;
using BookReview.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Country>> GetCountries()
        {
            try
            {
                var countries = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(countries);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{countryId}")]
        public ActionResult<Country> GetCountry(int countryId)
        {
            try
            {
                if (!_countryRepository.CountryExists(countryId))
                    return NotFound();

                var country = _mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(country);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("owners/{ownerId}")]
        public ActionResult<Country> GetCountryOfAnOwner(int ownerId)
        {
            try
            {
                var country = _mapper.Map<CountryDto>(
                _countryRepository.GetCountryByOwner(ownerId));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(country);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpPost, Authorize]
        public ActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            try
            {
                if (countryCreate == null)
                {
                    return BadRequest(ModelState);
                }

                var country = _countryRepository.GetCountries().Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();

                if (country != null)
                {
                    ModelState.AddModelError("", "Country already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var countryMap = _mapper.Map<Country>(countryCreate);

                if (!_countryRepository.CreateCountry(countryMap))
                {
                    ModelState.AddModelError("", "Sonething went wrong while saving");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpPut("{countryId}"), Authorize]
        public ActionResult UpdateCategory(int countryId, [FromBody] CountryForUpdateDto updatedCountry)
        {
            try
            {
                if (updatedCountry == null)
                    return BadRequest(ModelState);


                if (!_countryRepository.CountryExists(countryId))
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest();

                var countryMap = _mapper.Map<Country>(updatedCountry);

                if (!_countryRepository.UpdateCountry(countryMap))
                {
                    ModelState.AddModelError("", "Something went wrong updating category");
                    return StatusCode(500, ModelState);
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);

            }

        }

        [HttpDelete("{countryId}"), Authorize]
        public ActionResult DeleteCountry(int countryId)
        {
            try
            {
                if (!_countryRepository.CountryExists(countryId))
                {
                    return NotFound();
                }

                var countryToDelete = _countryRepository.GetCountry(countryId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_countryRepository.DeleteCountry(countryToDelete))
                {
                    ModelState.AddModelError("", "Something went wrong deleting category");
                }

                return NoContent();

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);

            }

        }
    }
}

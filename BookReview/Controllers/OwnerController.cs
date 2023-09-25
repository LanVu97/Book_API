using AutoMapper;
using BookReview.Dto;
using BookReview.Models;
using BookReview.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository,
            ICountryRepository countryRepository,
            IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Owner>> GetOwners()
        {
            try
            {
                var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(owners);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{ownerId}")]
        public ActionResult<Owner> GetOwner(int ownerId)
        {
            try
            {
                if (!_ownerRepository.OwnerExists(ownerId))
                    return NotFound();

                var owner = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(owner);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{ownerId}/book")]
        public ActionResult<Owner> GetBookByOwner(int ownerId)
        {
            try
            {
                if (!_ownerRepository.OwnerExists(ownerId))
                {
                    return NotFound();
                }

                var owner = _mapper.Map<List<BookDto>>(
                    _ownerRepository.GetBookByOwner(ownerId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(owner);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }


        }

        [HttpPost]
        public ActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            try
            {
                if (ownerCreate == null)
                    return BadRequest(ModelState);

                var owners = _ownerRepository.GetOwners()
                    .Where(c => c.LastName.Trim().ToUpper() == ownerCreate.LastName.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (owners != null)
                {
                    ModelState.AddModelError("", "Owner already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var ownerMap = _mapper.Map<Owner>(ownerCreate);

                ownerMap.Country = _countryRepository.GetCountry(countryId);

                if (!_ownerRepository.CreateOwner(ownerMap))
                {
                    ModelState.AddModelError("", "Something went wrong while savin");
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

        [HttpPut("{ownerId}")]
        public ActionResult UpdateOwner(int ownerId, [FromBody] OwnerForUpdateDto updatedOwner)
        {
            try
            {
                if (updatedOwner == null)
                    return BadRequest(ModelState);

                if (!_ownerRepository.OwnerExists(ownerId))
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest();

                var ownerMap = _mapper.Map<Owner>(updatedOwner);

                if (!_ownerRepository.UpdateOwner(ownerMap))
                {
                    ModelState.AddModelError("", "Something went wrong updating owner");
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

        [HttpDelete("{ownerId}")]
        public ActionResult DeleteOwner(int ownerId)
        {
            try
            {
                if (!_ownerRepository.OwnerExists(ownerId))
                {
                    return NotFound();
                }

                var ownerToDelete = _ownerRepository.GetOwner(ownerId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_ownerRepository.DeleteOwner(ownerToDelete))
                {
                    ModelState.AddModelError("", "Something went wrong deleting owner");
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

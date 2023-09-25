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
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Reviewer>> GetReviewers()
        {
            try
            {
                var reviewers = _mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(reviewers);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{reviewerId}")]
        public ActionResult<Reviewer> GetBook(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                    return NotFound();

                var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(reviewerId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(reviewer);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }


        [HttpGet("{reviewerId}/reviews")]
        public ActionResult<Reviewer> GetReviewsByAReviewer(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                    return NotFound();

                var reviews = _mapper.Map<List<ReviewDto>>(
                    _reviewerRepository.GetReviewsByReviewer(reviewerId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(reviews);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpPost, Authorize]
        public ActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            try
            {
                if (reviewerCreate == null)
                    return BadRequest(ModelState);

                var country = _reviewerRepository.GetReviewers()
                    .Where(c => c.LastName.Trim().ToUpper() == reviewerCreate.LastName.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (country != null)
                {
                    ModelState.AddModelError("", "Country already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var reviewerMap = _mapper.Map<Reviewer>(reviewerCreate);

                if (!_reviewerRepository.CreateReviewer(reviewerMap))
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

        [HttpPut("{reviewerId}"), Authorize]
        public ActionResult UpdateReviewer(int reviewerId, [FromBody] ReviewerForUpdateDto updatedReviewer)
        {
            try
            {
                if (updatedReviewer == null)
                    return BadRequest(ModelState);


                if (!_reviewerRepository.ReviewerExists(reviewerId))
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest();

                var reviewerMap = _mapper.Map<Reviewer>(updatedReviewer);

                if (!_reviewerRepository.UpdateReviewer(reviewerMap))
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

        [HttpDelete("{reviewerId}"), Authorize]
        public ActionResult DeleteReviewer(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                {
                    return NotFound();
                }

                var reviewerToDelete = _reviewerRepository.GetReviewer(reviewerId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_reviewerRepository.DeleteReviewer(reviewerToDelete))
                {
                    ModelState.AddModelError("", "Something went wrong deleting reviewer");
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

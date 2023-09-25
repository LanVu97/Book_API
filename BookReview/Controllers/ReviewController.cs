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
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IBookRepository _bookRepository;
        

        public ReviewController(IReviewRepository reviewRepository,
            IMapper mapper,
            IReviewerRepository reviewerRepository,
            IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _reviewerRepository = reviewerRepository;
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Review>> GetReviews()
        {
            try {
                var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

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

        [HttpGet("{reviewId}")]
        public ActionResult<Review> GetBook(int reviewId)
        {
            try
            {
                if (!_reviewRepository.ReviewExists(reviewId))
                    return NotFound();

                var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(review);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);

            }

        }

        [HttpGet("book/{bookId}")]
        public ActionResult<Review> GetReviewsForABook(int bookId)
        {
            try
            {
                var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfABook(bookId));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(reviews);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);

            }

        }

        [HttpPost, Authorize]
        public ActionResult CreateReview([FromQuery] int reviewerId, [FromQuery] int bookId, [FromBody] ReviewDto reviewCreate)
        {
            try
            {
                if (reviewCreate == null)
                    return BadRequest(ModelState);

                var reviews = _reviewRepository.GetReviews()
                    .Where(c => c.Title.Trim().ToUpper() == reviewCreate.Title.TrimEnd().ToUpper())
                    .FirstOrDefault();

                if (reviews != null)
                {
                    ModelState.AddModelError("", "Review already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var reviewMap = _mapper.Map<Review>(reviewCreate);

                reviewMap.Book = _bookRepository.GetBook(bookId);
                reviewMap.Reviewer = _reviewerRepository.GetReviewer(reviewerId);


                if (!_reviewRepository.CreateReview(reviewMap))
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
        [HttpPut("{reviewId}"), Authorize]
        public ActionResult UpdateReview(int reviewId, [FromBody] ReviewForUpdateDto updatedReview)
        {
            try
            {
                if (updatedReview == null)
                    return BadRequest(ModelState);

                if (!_reviewRepository.ReviewExists(reviewId))
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest();

                var reviewMap = _mapper.Map<Review>(updatedReview);

                if (!_reviewRepository.UpdateReview(reviewMap))
                {
                    ModelState.AddModelError("", "Something went wrong updating review");
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

        [HttpDelete("{reviewId}"), Authorize]
        public ActionResult DeleteReview(int reviewId)
        {
            try
            {
                if (!_reviewRepository.ReviewExists(reviewId))
                {
                    return NotFound();
                }

                var reviewToDelete = _reviewRepository.GetReview(reviewId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_reviewRepository.DeleteReview(reviewToDelete))
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

        [HttpDelete("/DeleteReviewsByReviewer/{reviewerId}")]
        public ActionResult DeleteReviewsByReviewer(int reviewerId)
        {
            try
            {
                if (!_reviewerRepository.ReviewerExists(reviewerId))
                    return NotFound();

                var reviewsToDelete = _reviewerRepository.GetReviewsByReviewer(reviewerId).ToList();
                if (!ModelState.IsValid)
                    return BadRequest();

                if (!_reviewRepository.DeleteReviews(reviewsToDelete))
                {
                    ModelState.AddModelError("", "error deleting reviews");
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
    }
}

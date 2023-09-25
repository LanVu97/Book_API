using AutoMapper;
using BookReview.Dto;
using BookReview.Models;
using BookReview.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace BookReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public BookController(IBookRepository bookRepository, IMapper mapper, IReviewRepository reviewRepository)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _reviewRepository = reviewRepository;

        }

        [HttpGet]      
        public ActionResult<IEnumerable<Book>> GetBooks([FromQuery] int pageNumber=1, int pageSize = 1)
        {
            try
            {

                var books = _mapper.Map<List<BookDto>>(_bookRepository.GetBooks(pageNumber, pageSize));

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
               
                var totalCount = _bookRepository.CountBook();

               var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var metadata = new { CurrentPage = pageNumber, PageSize = pageSize, TotalCount= totalCount, TotalPages=totalPages};

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));
               
                return Ok(books);
            }
            catch (Exception ex) {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{bookId}")]
        public ActionResult<Book> GetBook(int bookId) {

            try
            {
                if (!_bookRepository.BookExists(bookId))
                {
                    return NotFound();
                }

                var book = _bookRepository.GetBook(bookId);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(book);
            }
            catch (Exception ex) {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{bookId}/rating")]
        public ActionResult<decimal> GetBookRating(int bookId)
        {
            try
            {
                if (!_bookRepository.BookExists(bookId))
                {
                    return NotFound();
                }

                var rating = _bookRepository.GetBookRating(bookId);

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(rating);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpPost, Authorize]
        public ActionResult<Book> CreateBook([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] BookForCreateDto bookCreate)
        {
            try
            {
                if (bookCreate == null)
                    return BadRequest(ModelState);

                var books = _bookRepository.GetBookTrimToUpper(bookCreate);

                if (books != null)
                {
                    ModelState.AddModelError("", "Book already exists");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var bookMap = _mapper.Map<Book>(bookCreate);


                _bookRepository.CreateBook(ownerId, catId, bookMap);

                var bookCreated = _mapper.Map<BookDto>(bookMap);

                return Ok(bookCreated);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpPut("{bookId}"), Authorize]
        public ActionResult UpdateBook(int bookId,  [FromQuery] int ownerId, [FromQuery] int catId,
                                                      [FromBody] BookForUpdateDto updatedBook)
        {
            try {
                if (updatedBook == null)
                    return BadRequest(ModelState);

                if (!_bookRepository.BookExists(bookId))
                    return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest();

                var bookMap = _mapper.Map<Book>(updatedBook);

                if (!_bookRepository.UpdateBook(ownerId, catId, bookMap))
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


        [HttpGet("{bookId}/reviews")]    
        public ActionResult<Review> GetReviewsForABook(int bookId)
        {
            try
            {
                var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfABook(bookId));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(reviews);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpDelete("{bookId}"), Authorize]
        public ActionResult DeleteBook(int bookId)
        {
            try
            {
                if (!_bookRepository.BookExists(bookId))
                {
                    return NotFound();
                }

                var bookToDelete = _bookRepository.GetBook(bookId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                if (!_bookRepository.DeleteBook(bookToDelete))
                {
                    ModelState.AddModelError("", "Something went wrong deleting owner");
                }

                return NoContent();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

    }
}

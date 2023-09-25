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
    public class CategoryController: Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> GetCategories()
        {
            try
            {
                var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(categories);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{categoryId}")]
        public ActionResult<Category> GetCategory(int categoryId)
        {
            try
            {
                if (!_categoryRepository.CategoryExists(categoryId))
                    return NotFound();

                var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(categoryId));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(category);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);
            }

        }

        [HttpGet("{categoryId}/book")]
        public ActionResult<Category> GetBookByCategoryId(int categoryId)
        {
            try
            {
                var books = _mapper.Map<List<BookDto>>(
                _categoryRepository.GetBookByCategory(categoryId));

                if (!ModelState.IsValid)
                    return BadRequest();

                return Ok(books);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Something went wrong while savin: {ex.Message}");
                return StatusCode(500, ModelState);

            }

        }

        [HttpPost, Authorize]
        public ActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            try
            {
                if (categoryCreate == null)
                    return BadRequest(ModelState);

                var category = _categoryRepository.GetCategories()
        .Where(c => c.Name.Trim().ToUpper() == categoryCreate.Name.TrimEnd().ToUpper())
        .FirstOrDefault();

                if (category != null)
                {
                    ModelState.AddModelError("", "Category already exists");
                    return StatusCode(422, ModelState);
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var categoryMap = _mapper.Map<Category>(categoryCreate);

                if (!_categoryRepository.CreateCategory(categoryMap))
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

        [HttpPut("{categoryId}"), Authorize]
        public ActionResult UpdateCategory(int categoryId, [FromBody] CategoryForUpdateDto updatedCategory)
        {
            try
            {
                if (updatedCategory == null)
                {
                    return BadRequest(ModelState);
                }

                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                    return BadRequest();

                var categoryMap = _mapper.Map<Category>(updatedCategory);

                if (!_categoryRepository.UpdateCategory(categoryMap))
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


        [HttpDelete("{categoryId}"), Authorize]
        public ActionResult DeleteCategory(int categoryId)
        {
            try
            {
                if (!_categoryRepository.CategoryExists(categoryId))
                {
                    return NotFound();
                }

                var categoryToDelete = _categoryRepository.GetCategory(categoryId);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_categoryRepository.DeleteCategory(categoryToDelete))
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

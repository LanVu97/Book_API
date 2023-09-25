using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class ReviewForCreateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title can't be longer than 100 characters")]
        public string Title { get; set; }
        public string Text { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        public int Rating { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class BookForUpdateDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot be loner then 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Published day is required")]
        public DateTime Published_date { get; set; }
    }
}

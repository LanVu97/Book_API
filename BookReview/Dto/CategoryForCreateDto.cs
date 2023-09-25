using System.ComponentModel.DataAnnotations;

namespace BookReview.Dto
{
    public class CategoryForCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(60, ErrorMessage = "Name can't be longer than 60 characters")]
        public string Name { get; set; }
    }
}

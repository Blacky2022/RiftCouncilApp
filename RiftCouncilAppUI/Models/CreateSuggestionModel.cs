using System.ComponentModel.DataAnnotations;

namespace RiftCouncilAppUI.Models
{
    public class CreateSuggestionModel
    {
        [Required]
        [MaxLength(75, ErrorMessage = "Name is too long.")]
        public string Suggestion { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public string CategoryId { get; set; }

        [MaxLength(500, ErrorMessage = "Description is too long.")] 
        public string Description { get; set; }

    }
}

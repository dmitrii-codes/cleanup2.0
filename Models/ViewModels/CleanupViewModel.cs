using System.ComponentModel.DataAnnotations;

namespace Cleanup.Models
{
    public class CleanupViewModel: BaseEntity
    {
        [Required(ErrorMessage = "Title required")]
        [MaxLength(30, ErrorMessage = "Title cannot be longer than 30 characters")]
        public string Title { get; set; }
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [Required(ErrorMessage = "Description required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long")]
        public string DescriptionOfArea { get; set; }
        [Required(ErrorMessage = "Description required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long")]
        public string DescriptionOfTrash { get; set; }
    }
}
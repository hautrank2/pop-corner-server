using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class MovieUpdatePosterDto
    {
        [Required]
        public IFormFile Poster { get; set; } = default!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class FileDto
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}

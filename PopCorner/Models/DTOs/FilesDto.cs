using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class FilesDto
    {
        [Required]
        public IFormFile[] Files { get; set; } = Array.Empty<FormFile>();
    }
}

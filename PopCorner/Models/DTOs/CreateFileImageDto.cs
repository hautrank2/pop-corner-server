using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class CreateFileImageDto
    {
        [Required]
        public IFormFile File { get; set; }

        [Required]
        public string FileName { get; set; }

        [DefaultValue("/folder1/folder2")]
        public string? Folder { get; set; }

        public string? FileDescription { get; set; }
    }
}

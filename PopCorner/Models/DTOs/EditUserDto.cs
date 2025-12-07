using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class EditUserDto
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile Avatar { get; set; } = null!;
    }
}

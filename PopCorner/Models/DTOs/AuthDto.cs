using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class EditPasswordDto
    {
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}

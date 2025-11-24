using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class AddUserDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(255, ErrorMessage = "Email must not exceed 255 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [MaxLength(100, ErrorMessage = "Password must not exceed 100 characters.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Required(ErrorMessage = "Avatar file is required.")]
        [DataType(DataType.Upload)]
        public IFormFile Avatar { get; set; } = null!;

        [MaxLength(20, ErrorMessage = "Role must not exceed 20 characters.")]
        public string Role { get; set; } = "User";
    }

}

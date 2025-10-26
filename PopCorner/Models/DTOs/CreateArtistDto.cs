using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class CreateArtistDto
    {

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        public DateTime? Birthday { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(2000)]
        public string? Bio { get; set; }

        [Required]
        public IFormFile Avatar { get; set; } = default!;
    }
}

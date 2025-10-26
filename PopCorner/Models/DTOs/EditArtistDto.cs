using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PopCorner.Models.DTOs
{
    public class EditArtistDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required, Column(TypeName = "date")]

        public DateTime? Birthday { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(2000)]
        public string? Bio { get; set; }

        [MaxLength(500)]
        public string AvatarUrl { get; set; }

        public IFormFile? Avatar { get; set; }
    }
}

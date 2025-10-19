using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PopCorner.Models.DTOs
{
    public class CreateMovieDto
    {

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required, Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Duration { get; set; }

        // --- Files ---
        [Required]
        public IFormFile Poster { get; set; } = default!; 
        
        [Required]
        public List<IFormFile> ImgFiles { get; set; } = new();  

        [Required, Url]
        public string TrailerUrl { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Director { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        // --- Quan hệ (ID các entity liên kết) ---
        public List<Guid>? GenreIds { get; set; }
        public List<Guid>? ActorIds { get; set; }
        public List<Guid>? CreditIds { get; set; }
    }
}

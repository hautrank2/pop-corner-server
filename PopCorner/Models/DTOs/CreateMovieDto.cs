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

        [Range(0, int.MaxValue)]
        public int View { get; set; } = 0;

        // --- Files ---
        [Required]
        public IFormFile Poster { get; set; } = default!; 
        
        [Required]
        public List<IFormFile> ImgFiles { get; set; } = new();  

        [Required, Url]
        public string TrailerUrl { get; set; } = string.Empty;

        [Required]
        public Guid DirectorId { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        // --- Quan hệ (ID các entity liên kết) ---
        public List<int> GenreIds { get; set; } = new List<int> {};

        public List<Guid>? CreditIds { get; set; }
    }
}

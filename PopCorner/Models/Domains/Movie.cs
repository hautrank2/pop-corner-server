using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PopCorner.Models.Domains
{
    public class Movie
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required, Column(TypeName="date")]
        public DateTime ReleaseDate { get; set; }

        [Required, Range(0, int.MaxValue)]
        public int Duration { get; set; }

        [Required, Url]
        public required string PosterUrl { get; set; }
        [Required , Url]
        public required string TrailerUrl { get; set; }   

        [Required, Column(TypeName = "text[]")]
        public required string[] ImgUrls { get; set; }

        [Required]
        public Guid DirectorId { get; set; }

        [ForeignKey(nameof(DirectorId))]
        public Artist Director { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        /// <summary>Đếm lượt xem</summary>
        [Range(0, int.MaxValue)]
        public int View { get; set; } = 0;

        /// <summary>Điểm TB 0..10 (denormalized, update khi CRUD Ratings)</summary>
        [Range(0, 10)]
        public double AvgRating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation ---
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PopCorner.Models.Domains
{
    public class Movie
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Range(0, int.MaxValue)]
        public int? Duration { get; set; }

        [Url]
        public string? PosterUrl { get; set; }
        [Url]
        public string? TrailerUrl { get; set; }   

        [Column(TypeName = "text[]")]
        public string[]? ImgUrls { get; set; }

        [MaxLength(100)]
        public string? Director { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        /// <summary>Đếm lượt xem</summary>
        [Range(0, int.MaxValue)]
        public int View { get; set; } = 0;

        /// <summary>Điểm TB 0..10 (denormalized, update khi CRUD Ratings)</summary>
        [Range(0, 10)]
        public double AvgRating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // --- Navigation ---
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public ICollection<MovieCredit> Credits { get; set; } = new List<MovieCredit>();

    }
}

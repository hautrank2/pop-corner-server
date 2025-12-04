using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PopCorner.Models.Domains
{
    public class Comment
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required] public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = null!;

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public Guid? ParentId { get; set; } // hỗ trợ thread

        [JsonIgnore]
        public Comment? Parent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsEdited { get; set; }
    }
}

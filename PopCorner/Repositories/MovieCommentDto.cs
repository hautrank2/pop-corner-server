using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Repositories
{
    public class MovieCommentDto
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required] public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required] public Guid MovieId { get; set; }

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public Guid? ParentId { get; set; } // hỗ trợ thread
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsEdited { get; set; }
    }
}

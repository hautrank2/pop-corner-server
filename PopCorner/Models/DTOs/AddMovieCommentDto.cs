using PopCorner.Models.Domains;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class AddMovieCommentDto
    {

        [Required] public Guid UserId { get; set; }

        [Required, MaxLength(2000)]
        public string Content { get; set; } = string.Empty;

        public Guid? ParentId { get; set; } // hỗ trợ thread

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}

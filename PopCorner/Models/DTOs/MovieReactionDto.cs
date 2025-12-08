using PopCorner.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.DTOs
{
    public class UpsertMovieReactionDto
    {
        [Required]
        public ReactionType ReactionType { get; set; }
    }
}

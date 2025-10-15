using System.ComponentModel.DataAnnotations;

namespace PopCorner.Models.Domains
{
    public class User
    {
        [Key] public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required] public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(100), MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [Required] public DateTime BirthDate { get; set; }          

        [Required, Url, MaxLength(500)]
        public string AvatarUrl { get; set; } = string.Empty;         

        [MaxLength(20)] public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}

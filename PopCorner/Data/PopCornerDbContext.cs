using Microsoft.EntityFrameworkCore;
using PopCorner.Models.Domains;
using System.Reflection.Emit;

namespace PopCorner.Data
{
    public class PopCornerDbContext : DbContext
    {
        public PopCornerDbContext(DbContextOptions<PopCornerDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<Artist> Artist { get; set; }
        public DbSet<MovieGenre> MovieGenre { get; set; }
        public DbSet<MovieActor> MovieActor { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // ===== USER =====
            b.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            b.Entity<User>()
                .Property(x => x.AvatarUrl)
                .IsRequired()
                .HasMaxLength(500);

            b.Entity<User>()
                .Property(x => x.Birthday)
                .IsRequired();

            b.Entity<MovieActor>(e =>
            {
                e.HasKey(x => new { x.MovieId, x.ArtistId });

                e.HasOne(x => x.Movie)
                 .WithMany(m => m.MovieActors)
                 .HasForeignKey(x => x.MovieId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Artist)
                 .WithMany(a => a.MovieActors)
                 .HasForeignKey(x => x.ArtistId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== MOVIE =====
            b.Entity<Movie>()
                .Property(m => m.ImgUrls)
                .HasColumnType("text[]"); // PostgreSQL array

            b.Entity<Movie>()
                .Property(m => m.AvgRating)
                .HasPrecision(4, 2);

            b.Entity<Movie>()
                .Property(m => m.TrailerUrl)
                .HasMaxLength(500);

            b.Entity<Movie>()
                .Property(m => m.ReleaseDate)
                .HasColumnType("date");

            b.Entity<Movie>()
                .HasOne(m => m.Director)
                .WithMany()
                .HasForeignKey(m => m.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== RATING =====
            b.Entity<Rating>()
                .HasIndex(x => new { x.UserId, x.MovieId })
                .IsUnique();

            b.Entity<Rating>()
                .Property(x => x.Score)
                .HasPrecision(3, 1);

            b.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== COMMENT =====
            b.Entity<Comment>()
                .HasOne(c => c.Parent)
                .WithMany(p => p.Replies)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== GENRE =====
            b.Entity<MovieGenre>()
                .HasKey(x => new { x.MovieId, x.GenreId });

            b.Entity<MovieGenre>()
                .HasOne(x => x.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(x => x.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<MovieGenre>()
                .HasOne(x => x.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== ARTIST =====
            b.Entity<Artist>()
                .Property(a => a.AvatarUrl)
                .HasMaxLength(500);

            // ===== CREDIT ROLE =====
            b.Entity<CreditRole>()
                .HasKey(cr => cr.Id);

            b.Entity<CreditRole>()
                .Property(cr => cr.Name)
                .IsRequired()
                .HasMaxLength(100);

            b.Entity<CreditRole>()
                .Property(cr => cr.Description)
                .HasMaxLength(255);

            // Seed data cho CreditRole
            b.Entity<CreditRole>().HasData(
                new CreditRole { Id = 1, Name = "Actor", Description = "Performs in the movie" },
                new CreditRole { Id = 2, Name = "Director", Description = "Directs the movie" },
                new CreditRole { Id = 3, Name = "Writer", Description = "Writes the screenplay" },
                new CreditRole { Id = 4, Name = "Producer", Description = "Produces the movie" },
                new CreditRole { Id = 5, Name = "Cinematographer", Description = "Leads the camera and lighting crew" },
                new CreditRole { Id = 6, Name = "Composer", Description = "Composes the soundtrack" }
            );

            // ===== MOVIE CREDIT =====
            b.Entity<MovieCredit>()
                .HasKey(x => new { x.MovieId, x.ArtistId, x.CreditRoleId });

            b.Entity<MovieCredit>()
                .HasOne(mc => mc.Movie)
                .WithMany(m => m.Credits)
                .HasForeignKey(mc => mc.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<MovieCredit>()
                .HasOne(mc => mc.Artist)
                .WithMany(a => a.Credits)
                .HasForeignKey(mc => mc.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<MovieCredit>()
                .HasOne(mc => mc.CreditRole)
                .WithMany(cr => cr.Credits)
                .HasForeignKey(mc => mc.CreditRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            b.Entity<MovieCredit>()
                .HasIndex(x => new { x.MovieId, x.CreditRoleId, x.Order });
        }
    }
}

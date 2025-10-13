using Microsoft.EntityFrameworkCore;
using PopCorner.Models.Domains;

namespace PopCorner.Data
{
    public class PopCornerDbContext : DbContext
    {
        public PopCornerDbContext(DbContextOptions<PopCornerDbContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>(b =>
            {
                b.Property(x => x.Title).HasMaxLength(255).IsRequired();
                b.Property(x => x.Description).HasMaxLength(255);
            });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Models.Domains;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly PopCornerDbContext popCornerDbContext;
        public GenreRepository(PopCornerDbContext popCornerDbContext) {
            this.popCornerDbContext = popCornerDbContext;
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            var genres = await popCornerDbContext.Genre.ToListAsync();
            return genres;
        }
    }
}

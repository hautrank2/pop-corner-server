using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Models.Domains;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly PopCornerDbContext dbContext;
        public MovieRepository(PopCornerDbContext dbContext) {
            this.dbContext = dbContext;
        }
        public Task<Movie> CreateAsync(Movie region)
        {
            this.dbContext.Movies.AddAsync(region);
            this.dbContext.SaveChangesAsync();
            return Task.FromResult(region);
        }
        public Task<Movie?> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<List<Movie>> GetAllAsync()
        {
            return await dbContext.Movies.ToListAsync();
        }
        public Task<Movie?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public Task<Movie?> UpdateAsync(Guid id, Movie region)
        {
            throw new NotImplementedException();
        }
    }
}

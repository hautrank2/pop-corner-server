using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly PopCornerDbContext dbContext;
        public MovieRepository(PopCornerDbContext dbContext) {
            this.dbContext = dbContext;
        }
        public async Task<Movie> CreateAsync(Movie movie)
        {
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync();
            return movie;
        }
        public Task<Movie?> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<PaginationResponse<Movie>> GetAllAsync(MovieQueryDto query)
        {
            var movieQuery = dbContext.Movies.Include(x => x.MovieGenres).Include(x => x.MovieActors).AsQueryable() ;
            var total = await movieQuery.CountAsync();

            var page = Math.Max(query.Page ?? 1, 1);
            var pageSize = Math.Max(query.PageSize ?? 10, 1);
            var totalPage = total / pageSize;

            var movies = await movieQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginationResponse<Movie> { Page = page, TotalPage = totalPage, PageSize = pageSize, Items = movies };
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

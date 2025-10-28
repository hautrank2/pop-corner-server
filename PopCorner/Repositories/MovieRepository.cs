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
        public async Task<PaginationResponse<Movie>> GetAllAsync(MovieQueryDto query)
        {
            var movieQuery = dbContext.Movies
                .Include(x => x.Director)
                .Include(x => x.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(x => x.MovieActors).ThenInclude(mg => mg.Artist)
                .AsQueryable();
            var total = await movieQuery.CountAsync();

            var page = Math.Max(query.Page ?? 1, 1);
            var pageSize = Math.Max(query.PageSize ?? 10, 1);
            var totalPage = total / pageSize;

            var movies = await movieQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginationResponse<Movie> { Page = page, TotalPage = totalPage, PageSize = pageSize, Items = movies };
        }
        public async Task<Movie> CreateAsync(Movie movie)
        {
            await dbContext.Movies.AddAsync(movie);
            await dbContext.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie?> DeleteAsync(Guid id)
        {
            var entity = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }

            dbContext.Movies.Remove(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Movie?> GetByIdAsync(Guid id)
        {
            var entity = await dbContext.FindAsync<Movie>(id);
            return entity == null ? null : entity;
        }

        public async Task<Movie?> UpdateAsync(Guid id, Movie dto)
        {
            var entity = await dbContext.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }

            entity.Title = dto.Title;
            entity.Description = dto.Description;
            entity.MovieGenres = dto.MovieGenres;
            entity.MovieActors = dto.MovieActors;
            entity.UpdatedAt = dto.UpdatedAt;
            entity.Comments = dto.Comments;
            entity.Country = dto.Country;
            entity.ReleaseDate = dto.ReleaseDate;
            entity.TrailerUrl = dto.TrailerUrl;
            entity.DirectorId = dto.DirectorId;

            await dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

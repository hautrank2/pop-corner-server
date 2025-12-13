using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
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
        private readonly IMapper mapper;

        public MovieRepository(PopCornerDbContext dbContext, IMapper mapper) {
            this.dbContext = dbContext; 
            this.mapper = mapper;
        }
        public async Task<PaginationResponse<Movie>> GetAllAsync(MovieQueryDto query)
        {
            var movieQuery = dbContext.Movies
                .Include(x => x.Director)
                .Include(x => x.MovieGenres).ThenInclude(mg => mg.Genre)
                .Include(x => x.MovieCredits).ThenInclude(mg => mg.CreditRole)
                .AsQueryable();

            if(!string.IsNullOrEmpty(query.Title))
            {
                var key = query.Title.Trim().ToLower();
                movieQuery = movieQuery.Where(x =>
                     EF.Functions.ILike(x.Title, $"%{query.Title}%")
                 );
            }


            if (query.GenreId.HasValue && query.GenreId != null)
            {
                var key = query.GenreId.Value;
                var existGenre = await dbContext.Genre.FirstOrDefaultAsync(x => x.Id.Equals(key));

                if(existGenre != null)
                {
                    movieQuery = movieQuery.Where(x => x.MovieGenres.Any(genre => genre.GenreId == key));
                }
            }

            var total = await movieQuery.CountAsync();
            var page = Math.Max(query.Page ?? 1, 1);
            var pageSize = Math.Max(query.PageSize ?? 10, 1);
            var totalPage = total / pageSize;

            var movies = await movieQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginationResponse<Movie> { Page = page, TotalPage = totalPage, PageSize = pageSize, Total = total, Items = movies };
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
            entity.UpdatedAt = dto.UpdatedAt;
            entity.Comments = dto.Comments;
            entity.Country = dto.Country;
            entity.ReleaseDate = dto.ReleaseDate;
            entity.TrailerUrl = dto.TrailerUrl;
            entity.DirectorId = dto.DirectorId;

            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<Comment[]> GetCommentsAsync(Guid id)
        {
            var query = dbContext.Comment.Include(x => x.User).AsNoTracking();
            if (id != Guid.Empty)
            {
                query = query.Where(x => x.MovieId == id);
            }
            return await query.ToArrayAsync();
        }

        public async Task<Movie> Rate(Guid movieId, Guid userId, MovieRateDto dto)
        {
            var movie = await GetByIdAsync(movieId);

            if (movie == null)
            {
                throw new Exception("Movie is not exist");
            }

            // Find rate
            var rate = await dbContext.Rating.FirstOrDefaultAsync(x => x.MovieId == movieId && x.UserId == userId);
            var rating = new Rating
            {
                MovieId = movieId,
                UserId = userId,
                Score = dto.Score,
                CreatedAt = DateTime.UtcNow
            };

            if (rate == null)
            {
                await dbContext.AddAsync(rating);
            } else
            {
                rate.Score = dto.Score;
                rate.UpdatedAt = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            movie.AvgRating = await dbContext.Rating
                .Where(x => x.MovieId == movieId)
                .AverageAsync(x => (double)x.Score);
            movie.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return movie;
        }

        public async Task<MovieRatingResponseDto[]> GetRatingSync(Guid movieId)
        {
            var ratings = await dbContext.Rating.Where(x => x.MovieId == movieId).Include(x => x.User).ToArrayAsync();
            var movieRatings = mapper.Map<MovieRatingResponseDto[]>(ratings);
            return movieRatings;
        }
        public async Task<MovieReaction?> GetReactionSync(Guid movieId, Guid userId)
        {
            var data = await dbContext.MovieReactions.FirstOrDefaultAsync(x => x.UserId == userId && x.MovieId == movieId);
            return data;
        }

        public async Task<MovieReaction[]> GetReactionsSync(Guid movieId)
        {
            var data = await dbContext.MovieReactions.Where(x => x.MovieId == movieId).ToArrayAsync();
            return data;
        }

        public async Task<MovieReaction> AddReactionSync(MovieReaction dto)
        {
            await dbContext.MovieReactions.AddAsync(dto);
            await dbContext.SaveChangesAsync();
            return dto;
        }

        public async Task<MovieCredit[]> GetAllCreditsSync(Guid movieId)
        {
            var data = await dbContext.MovieCredit.Where(x => x.MovieId == movieId).Include(x => x.Artist).ToArrayAsync();
            return data;
        }
    }
}

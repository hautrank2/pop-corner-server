using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;

namespace PopCorner.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<PaginationResponse<Movie>> GetAllAsync(MovieQueryDto movieQuery);

        Task<Movie?> GetByIdAsync(Guid id);

        Task<Movie> CreateAsync(Movie movie);

        Task<Movie?> UpdateAsync(Guid id, Movie movie);

        Task<Movie?> DeleteAsync(Guid id);

        Task<Comment[]> GetCommentsAsync(Guid id);

        Task<Movie> Rate(Guid movieId, Guid userId, MovieRateDto dto);
        Task<MovieRatingResponseDto[]> GetRatingSync(Guid movieId);
        Task<MovieReaction> AddReactionSync(MovieReaction dto);
        Task<MovieReaction> GetReactionSync(Guid movieId, Guid userId);
        Task<MovieReaction[]> GetReactionsSync(Guid movieId);
    }
}

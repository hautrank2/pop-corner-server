using PopCorner.Models.Domains;

namespace PopCorner.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync();

        Task<Movie?> GetByIdAsync(Guid id);

        Task<Movie> CreateAsync(Movie movie);

        Task<Movie?> UpdateAsync(Guid id, Movie movie);

        Task<Movie?> DeleteAsync(Guid id);
    }
}

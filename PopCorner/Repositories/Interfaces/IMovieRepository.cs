using PopCorner.Models.Domains;

namespace PopCorner.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAllAsync();

        Task<Movie?> GetByIdAsync(Guid id);

        Task<Movie> CreateAsync(Movie region);

        Task<Movie?> UpdateAsync(Guid id, Movie region);

        Task<Movie?> DeleteAsync(Guid id);
    }
}

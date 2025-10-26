using PopCorner.Models.Domains;

namespace PopCorner.Repositories.Interfaces
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllAsync();
    }
}

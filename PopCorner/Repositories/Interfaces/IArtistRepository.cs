using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;

namespace PopCorner.Repositories.Interfaces
{
    public interface IArtistRepository
    {
        Task<PaginationResponse<Artist>> GetAllAsync(ArtistQueryDto movieQuery);

        Task<Artist?> GetByIdAsync(Guid id);

        Task<Artist> CreateAsync(Artist artist);

        Task<Artist?> UpdateAsync(Guid id, Artist artist);

        Task<Artist?> DeleteAsync(Guid id);
    }
}

using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;

namespace PopCorner.Repositories.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating> CreateAsync(Guid movieId, Guid userId, float score);
    }
}

using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;

namespace PopCorner.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<PaginationResponse<User>> GetAllAsync(UserQueryDto query);

        Task<User?> GetByIdAsync(Guid id);

        Task<User> CreateAsync(User user);

        Task<User?> UpdateAsync(Guid id, User dto);

        Task<User?> DeleteAsync(Guid id);
    }
}

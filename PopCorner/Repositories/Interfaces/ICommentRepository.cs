using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;

namespace PopCorner.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment[]> GetAllAsync(CommentQueryDto query);

        Task<Comment?> GetByIdAsync(Guid id);

        Task<Comment> CreateAsync(Comment dto);

        Task<Comment?> UpdateAsync(Guid id, Comment movie);

        Task<Comment?> DeleteAsync(Guid id);
    }
}

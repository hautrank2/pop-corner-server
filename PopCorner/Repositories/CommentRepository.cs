using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly PopCornerDbContext dbContext;

        public CommentRepository(PopCornerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // ---------------------------
        // GET ALL (with filtering + sorting + pagination)
        // ---------------------------
        public async Task<Comment[]> GetAllAsync(CommentQueryDto dto)
        {
            var query = dbContext.Comment.AsQueryable();
            if (dto.MovieId != Guid.Empty)
            {
                query = query.Where(x => x.MovieId == dto.MovieId);
            }
            return await query.ToArrayAsync();
        }

        // ---------------------------
        // GET BY ID
        // ---------------------------
        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await dbContext.Comment.FirstOrDefaultAsync(x => x.Id == id);
        }

        // ---------------------------
        // CREATE
        // ---------------------------
        public async Task<Comment> CreateAsync(Comment Comment)
        {
            dbContext.Comment.Add(Comment);
            await dbContext.SaveChangesAsync();
            return Comment;
        }

        // ---------------------------
        // UPDATE
        // ---------------------------
        public async Task<Comment?> UpdateAsync(Guid id, Comment dto)
        {
            var existing = await dbContext.Comment.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
                return null;
            existing.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return existing;
        }

        // ---------------------------
        // DELETE
        // ---------------------------
        public async Task<Comment?> DeleteAsync(Guid id)
        {
            var Comment = await dbContext.Comment.FirstOrDefaultAsync(x => x.Id == id);
            if (Comment == null)
                return null;

            dbContext.Comment.Remove(Comment);
            await dbContext.SaveChangesAsync();

            return Comment;
        }
    }
}

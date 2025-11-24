using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;
using System;

namespace PopCorner.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly PopCornerDbContext dbContext;

        public UserRepository(PopCornerDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // ---------------------------
        // GET ALL (with filtering + sorting + pagination)
        // ---------------------------
        public async Task<PaginationResponse<User>> GetAllAsync(UserQueryDto query)
        {
            var users = dbContext.User.AsQueryable();

            // Filter Email
            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                var key = query.Email.Trim().ToLower();
                users = users.Where(x => x.Email.ToLower().Contains(key));
            }

            // Filter Name
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var key = query.Name.Trim().ToLower();
                users = users.Where(x => x.Name.ToLower().Contains(key));
            }

            // Filter Role
            if (!string.IsNullOrWhiteSpace(query.Role))
            {
                var key = query.Role.Trim();
                users = users.Where(x => x.Role == key);
            }

            users = QueryHelper.ApplySorting(users, query.OrderBy, query.OrderDirection);

            return await QueryHelper.PaginateAsync(users, query.Page, query.PageSize);
        }

        // ---------------------------
        // GET BY ID
        // ---------------------------
        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
        }

        // ---------------------------
        // CREATE
        // ---------------------------
        public async Task<User> CreateAsync(User user)
        {
            dbContext.User.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }

        // ---------------------------
        // UPDATE
        // ---------------------------
        public async Task<User?> UpdateAsync(Guid id, User dto)
        {
            var existing = await dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
                return null;

            existing.Email = dto.Email;
            existing.Name = dto.Name;
            existing.Birthday = dto.Birthday;
            existing.AvatarUrl = dto.AvatarUrl;
            existing.Role = dto.Role;
            existing.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return existing;
        }

        // ---------------------------
        // DELETE
        // ---------------------------
        public async Task<User?> DeleteAsync(Guid id)
        {
            var user = await dbContext.User.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return null;

            dbContext.User.Remove(user);
            await dbContext.SaveChangesAsync();

            return user;
        }
    }
}

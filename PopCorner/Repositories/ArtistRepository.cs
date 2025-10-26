using Microsoft.EntityFrameworkCore;
using PopCorner.Data;
using PopCorner.Helpers;
using PopCorner.Models.Common;
using PopCorner.Models.Domains;
using PopCorner.Models.DTOs;
using PopCorner.Repositories.Interfaces;

namespace PopCorner.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly PopCornerDbContext dbContext;
        public ArtistRepository(PopCornerDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public async Task<Artist> CreateAsync(Artist artist)
        {
            await dbContext.Artist.AddAsync(artist);
            await dbContext.SaveChangesAsync();
            return artist;
        }

        public async Task<Artist?> DeleteAsync(Guid id)
        {
            var entity = await dbContext.Artist.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return null;
            }

            dbContext.Artist.Remove(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<PaginationResponse<Artist>> GetAllAsync(ArtistQueryDto query)
        {
            var artists = dbContext.Artist.AsQueryable();

            var result = await PaginationHelper.PaginateAsync<Artist>(artists, query.Page, query.PageSize);
          
            return result;
        }

        public async Task<Artist?> GetByIdAsync(Guid id)
        {
            var entity = await dbContext.FindAsync<Artist>(id);
            return entity == null ? null : entity;
        }

        public async Task<Artist?> UpdateAsync(Guid id, Artist artist)
        {
            var entity = await dbContext.Artist.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) {
                return null;
            }

            entity.Name = artist.Name;
            entity.AvatarUrl = artist.AvatarUrl;
            entity.Birthday = artist.Birthday;
            entity.Country = artist.Country;
            entity.Bio = artist.Bio;

            await dbContext.SaveChangesAsync();
            return entity;
        }
    }
}

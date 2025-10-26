using Microsoft.EntityFrameworkCore;
using PopCorner.Models.Common;

namespace PopCorner.Helpers
{
    public class PaginationHelper
    {
        public static async Task<PaginationResponse<T>> PaginateAsync<T>(
           IQueryable<T> query,
           int? page,
           int? pageSize)
        {
            // Nếu không có page/pageSize -> trả hết dữ liệu
            if (page == null || pageSize == null)
            {
                var allItems = await query.ToListAsync();
                return new PaginationResponse<T>
                {
                    Page = 1,
                    PageSize = allItems.Count,
                    TotalPage = 1,
                    Items = allItems
                };
            }

            var pageNumber = Math.Max(page ?? 1, 1);
            var size = Math.Max(pageSize ?? 10, 1);
            var total = await query.CountAsync();
            var totalPage = (int)Math.Ceiling(total / (double)size);

            var items = await query
                .Skip((pageNumber - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PaginationResponse<T>
            {
                Page = pageNumber,
                PageSize = size,
                TotalPage = totalPage,
                Items = items
            };
        }
    }
}

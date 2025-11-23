using Microsoft.EntityFrameworkCore;
using PopCorner.Models.Common;
using System.Reflection;
using System.Linq.Expressions;

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
                Total = total,
                Items = items
            };
        }

        public static IQueryable<T> ApplySorting<T>(
        IQueryable<T> query,
        string? orderBy,
        SortDirection? orderDirection)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return query;

            // Lấy property theo tên (không phân biệt hoa thường)
            var property = typeof(T).GetProperty(
                orderBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            if (property == null)
                return query; // field không hợp lệ => bỏ qua sort

            // Nếu null thì default Asc, còn lại xem có phải Desc không
            bool descending = orderDirection == SortDirection.Desc;

            // x =>
            var param = Expression.Parameter(typeof(T), "x");

            // x.Field
            var propertyAccess = Expression.Property(param, property);

            // x => x.Field
            var orderByExp = Expression.Lambda(propertyAccess, param);

            // Chọn method OrderBy hoặc OrderByDescending
            string methodName = descending ? "OrderByDescending" : "OrderBy";

            // gọi Queryable.OrderBy<T, TKey>(query, x => x.Field)
            var resultExp = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExp)
            );

            return query.Provider.CreateQuery<T>(resultExp);
        }
    }
}

using PopCorner.Models.Common;

namespace PopCorner.Models.DTOs
{
    public class UserQueryDto : IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }

        public string? OrderBy { get; set; }
        public SortDirection? OrderDirection { get; set; }

        // Filters
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
    }

}

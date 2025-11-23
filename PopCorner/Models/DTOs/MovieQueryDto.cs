using PopCorner.Models.Common;

namespace PopCorner.Models.DTOs
{
    public class MovieQueryDto : IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? OrderBy { get; set; }
        public SortDirection? OrderDirection { get; set; }
        public string? Title { get; set; }
    }
}

using PopCorner.Models.Common;

namespace PopCorner.Models.DTOs
{
    public class MovieQueryDto : IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}

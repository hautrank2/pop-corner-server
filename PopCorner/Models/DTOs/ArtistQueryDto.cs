using PopCorner.Models.Common;
using PopCorner.Models.Domains;

namespace PopCorner.Models.DTOs
{
    public class ArtistQueryDto : IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? OrderBy { get; set; }
        public SortDirection? OrderDirection { get; set; }
        public string? Name { get; set; }
    }
}

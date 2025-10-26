using PopCorner.Models.Common;
using PopCorner.Models.Domains;

namespace PopCorner.Models.DTOs
{
    public class ArtistQueryDto : IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}

namespace PopCorner.Models.Common
{
    public interface IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}

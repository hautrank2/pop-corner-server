namespace PopCorner.Models.Common
{
    public interface IPaginationQuery
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public string? OrderBy { get; set; }
        public SortDirection? OrderDirection { get; set; }
    }
    public enum SortDirection
    {
        Asc,
        Desc
    }
}


namespace PopCorner.Models.Common
{
    public class PaginationResponse<T>: IPaginationResponse<T>
    {
        public int Total { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int Page { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
    }
}

namespace PopCorner.Models.Common
{
    public interface IPaginationResponse<T>
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}

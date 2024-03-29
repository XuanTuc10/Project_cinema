namespace Project_cinema.Handler.HandlePagination
{
    public class PageResult<T>
    {
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
        public IEnumerable<T> Data { get; }

        public PageResult(IEnumerable<T> data, int totalItems, int totalPages, int pageNumber, int pageSize)
        {
            Data = data;
            TotalItems = totalItems;
            TotalPages = totalPages;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}

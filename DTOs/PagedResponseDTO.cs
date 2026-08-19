namespace MovieAPI.DTOs
{
    public class PagedResponseDTO<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }

        public PagedResponseDTO(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = (int) Math.Ceiling(totalRecords / (double) PageSize);
        }
    }
}

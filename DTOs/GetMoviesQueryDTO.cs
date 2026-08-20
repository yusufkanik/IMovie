namespace MovieAPI.DTOs
{
    // class to handle the GetMovie url queries 
    public class GetMoviesQueryDTO
    {
        public string? Search {  get; set; }
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

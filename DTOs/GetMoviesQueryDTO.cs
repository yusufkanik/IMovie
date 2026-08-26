namespace MovieAPI.DTOs
{
    // class to handle the GetMovie url queries 
    public class GetMoviesQueryDTO
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } 
        public string? SortOrder { get; set; } = "desc"; 

        
        public string? SearchTerm { get; set; }
        public List<int>? GenreIds { get; set; } 
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public double? MinRating { get; set; }
        public double? MaxRating { get; set; }
        public int? MinVoteCount { get; set; }
    }
}

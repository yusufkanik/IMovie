namespace MovieAPI.Common
{
    // response function to handle the errors while requesting from TMDB
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; } = 200;
    }
}

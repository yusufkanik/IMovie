using System.Text.Json;
using static System.Net.WebRequestMethods;
using MovieAPI.DTOs;

namespace MovieAPI.Services;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["TmdbSettings:ApiKey"]
            ?? throw new InvalidOperationException("API Key bulunamadı.");
    }

    public async Task<TmdbPopularMoviesWrapper?> GetPopularMoviesAsync()
    {
        string url = $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}";

        HttpResponseMessage response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<TmdbPopularMoviesWrapper>(json);
    }
}
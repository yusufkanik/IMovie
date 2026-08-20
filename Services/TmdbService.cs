using System.Text.Json;
using static System.Net.WebRequestMethods;
using MovieAPI.DTOs;
using MovieAPI.Common;

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

    public async Task<ServiceResponse<TmdbPopularMoviesWrapper>> GetPopularMoviesAsync(int page = 1)
    {
        var response = new ServiceResponse<TmdbPopularMoviesWrapper>();

        try
        {
            string url = $"https://api.themoviedb.org/3/movie/popular?api_key={_apiKey}&page={page}";

            HttpResponseMessage httpResponse = await _httpClient.GetAsync(url);
            
            if (!httpResponse.IsSuccessStatusCode)
            {
                response.IsSuccess = false;
                response.StatusCode = (int) httpResponse.StatusCode;
                response.Message = $"TMDB servisinden hata!: ${httpResponse.ReasonPhrase} (Kod: {(int)httpResponse.StatusCode})";
                return response;
            }

            string json = await httpResponse.Content.ReadAsStringAsync();
            response.Data = JsonSerializer.Deserialize<TmdbPopularMoviesWrapper>(json);
            return response;
        }

        catch (HttpRequestException)
        {
            response.IsSuccess = false;
            response.Message = "TMDB sunucularına erişilemiyor. İnternet bağlantınızı kontrol edin.";
            response.StatusCode = 503;
            return response;
        }

        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.StatusCode = 500;
            response.Message = $"Beklenmeyen bir hata oluştu: {ex.Message}";
            return response;
        }

    }

    public async Task<ServiceResponse<TmdbSearchResponse>> SearchMoviesAsync(string query, int page)
    {
        var url = $"search/movie?query={Uri.EscapeDataString(query)}&page={page}&language=en-US";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return new ServiceResponse<TmdbSearchResponse>
            {
                IsSuccess = false,
                Message = $"TMDB Arama Hatası: {response.StatusCode}",
                StatusCode = (int)response.StatusCode
            };
        }

        var stringData = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<TmdbSearchResponse>(stringData);

        return new ServiceResponse<TmdbSearchResponse>
        {
            Data = data
        };
    }

    public async Task<ServiceResponse<TmdbMovieDto>> GetMovieByTmdbIdAsync(int tmdbId)
    {
        var url = $"movie/{tmdbId}?language=en-US";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return new ServiceResponse<TmdbMovieDto>
            {
                IsSuccess = false,
                Message = $"TMDB'den film bulunamadı (ID: {tmdbId}).",
                StatusCode = (int)response.StatusCode
            };
        }

        var stringData = await response.Content.ReadAsStringAsync();
        var dto = JsonSerializer.Deserialize<TmdbMovieDto>(stringData);

        return new ServiceResponse<TmdbMovieDto>
        {
            Data = dto,

        };
    }
}
using Microsoft.EntityFrameworkCore;
using MovieAPI.DTOs;
using MovieAPI.Extensions;
using MovieAPI.Models;
using MovieAPI.Services;
using MovieAPI.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<TmdbService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Test the tmdb services if it works correctly
app.MapGet("/test-tmdb", async (TmdbService TmdbService) =>
{
    var wrapper = await TmdbService.GetPopularMoviesAsync();

    if (wrapper?.Results is null)
    {
        return Results.NotFound("No data from TMDB.");
    }

    List<TmdbMovieDto> Dtos = wrapper.Results;
    List<Movie> movieList = Dtos.ToModelList();

    foreach (Movie movie in movieList)
    {
        Console.WriteLine(movie.TmdbId);
        Console.WriteLine(movie.Title);
        Console.WriteLine(movie.VoteAverage);
        Console.WriteLine("\n");
    }

    return Results.Ok(movieList);
});

app.Run();

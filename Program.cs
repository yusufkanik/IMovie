using MovieAPI.Services;
using MovieAPI.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<TmdbService>();
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

    return wrapper;
});

app.Run();

using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Extensions;
using SpotifyLocalStats.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// todo : move this to diffferent folder
SpotifyLocalStats.Server.Data.Dependencies.ConfigureServices(builder.Configuration, builder.Services);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Configuration.AddConfiguration("appsettings.test.json");
builder.Logging.AddConsole();

builder.Services.AddOpenApi();

var app = builder.Build();

// need to check has there been a user created, if there has, nothing, else create one. 
if (builder.Services.sc)

app.UseDefaultFiles();
app.MapStaticAssets();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();

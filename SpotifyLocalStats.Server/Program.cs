

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.Adde

// todo : move this to diffferent folder
SpotifyLocalStats.Server.Data.Dependencies.ConfigureServices(builder.Configuration, builder.Services);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Configuration.AddConfiguration("appsettings.test.json");
builder.Logging.AddConsole();

builder.Services.AddOpenApi();

var app = builder.Build();

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

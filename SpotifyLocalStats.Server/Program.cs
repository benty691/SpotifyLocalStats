using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using WebApi.Config;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// todo : move this to diffferent folder
Dependencies.ConfigureServices(builder.Configuration, builder.Services);

var configSection = builder.Configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
builder.Services.Configure<BaseUrlConfiguration>(configSection);
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Logging.AddConsole();
builder.Services.AddOpenApi();
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Configuration.AddEnvironmentVariables();

//builder.Configuration.AddConfiguration("appsettings.test.json");
var app = builder.Build();

app.Logger.LogInformation("Backend App created...");

// need to check has there been a user created, if there has, nothing, else create one. 
var user = Dependencies.DoesUserExist(builder.Services);

app.UseDefaultFiles();
app.MapStaticAssets();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Logger.LogInformation("LAUNCHING Backend");
app.Run();

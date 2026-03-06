using SpotifyLocalStats.Server.Data;
using WebApi.Config;

var builder = WebApplication.CreateBuilder(args);

// todo : move this to diffferent folder
Dependencies.ConfigureServices(builder.Configuration, builder.Services);

var configSection = builder.Configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
builder.Services.Configure<BaseUrlConfiguration>(configSection);
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>();

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Logging.AddConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(builder => builder.AddConsole());
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Your Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


//builder.Configuration.AddConfiguration("appsettings.test.json");
var app = builder.Build();

app.Logger.LogInformation("Backend App created...");

// need to check has there been a user created, if there has, nothing, else create one. 
//var user = Dependencies.DoesUserExist(builder.Services);

app.UseDefaultFiles();
app.MapStaticAssets();

app.UseRouting();
app.UseCors("AllowViteDevServer");
app.UseAuthorization();

app.MapControllers();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}



app.Logger.LogInformation("LAUNCHING Backend");
app.Run();


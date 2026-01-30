using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyLocalStats.Server.Models;
using WebApi.Services;
using WebApi.Services.Implementations;
using WebApi.Services.Interfaces;

namespace SpotifyLocalStats.Server.Data;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        bool useOnlyInMemoryDatabase = false;
        if (configuration["UseOnlyInMemoryDatabase"] != null)
        {
            useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]!);
        }

        if (useOnlyInMemoryDatabase)
        {
            services.AddDbContext<SpotifyStatsContext>(c =>
               c.UseInMemoryDatabase("SpotifyStats"));
        }
        else
        {
            // use real database
            // Requires LocalDB which can be installed with SQL Server Express 2016
            // https://www.microsoft.com/en-us/download/details.aspx?id=54284
            services.AddDbContext<SpotifyStatsContext>(c =>
                c.UseSqlServer(configuration.GetConnectionString("SpotifyStats")));
            services.AddScoped<IImportedTrackService, ImportedTrackService>();
            services.AddScoped(typeof(BaseService<>));
            services.AddScoped<IImportOrchestrationService, ImportOrchestrationService>();
            services.AddScoped<IAggreationService, AggreationService>();
            services.AddScoped<IModelPopulationService, ModelPopulationService>();


        }
    }

    public static User DoesUserExist(IServiceCollection services)
    {
        var user = new User();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var context = serviceProvider.GetRequiredService<SpotifyStatsContext>())
            {
                var userCount = context.Users.Count();
                if (userCount == 0)
                {
                    user.UserName = "DefaultUser";
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    user = context.Users.Single();
                }
            }
        }
        return user;
    }
}

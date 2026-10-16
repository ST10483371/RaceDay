
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RaceDay.API.Data;


namespace RaceDay.API.Tests;

public class RaceDayApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName =
        "RaceDayTests_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = Convert.ToBase64String(
                        System.Security.Cryptography.RandomNumberGenerator
                            .GetBytes(32))
                });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MinhasFinancas.Infrastructure.Data;

namespace MinhasFinancas.Tests.Integration.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<MinhasFinancasDbContext>>();
            services.RemoveAll<MinhasFinancasDbContext>();

            services.AddDbContext<MinhasFinancasDbContext>(options =>
            {
                options.UseSqlite($"DataSource=file:{_dbName}?mode=memory&cache=shared");
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MinhasFinancasDbContext>();
            db.Database.EnsureCreated();
        });
    }
}

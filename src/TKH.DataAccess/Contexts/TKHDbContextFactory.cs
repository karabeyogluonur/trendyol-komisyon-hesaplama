using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TKH.DataAccess.Contexts
{
    public class TKHDbContextFactory : IDesignTimeDbContextFactory<TKHDbContext>
    {
        public TKHDbContext CreateDbContext(string[] args)
        {

            var path = Path.Combine(Directory.GetCurrentDirectory(), "../TKH.Presentation");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var builder = new DbContextOptionsBuilder<TKHDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseNpgsql(connectionString);

            return new TKHDbContext(builder.Options);
        }
    }
}

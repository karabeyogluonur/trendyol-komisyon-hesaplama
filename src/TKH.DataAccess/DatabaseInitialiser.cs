using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TKH.DataAccess.Contexts;

namespace TKH.DataAccess.Persistence
{
    public class DatabaseInitialiser
    {
        private readonly ILogger<DatabaseInitialiser> _logger;
        private readonly TKHDbContext _context;

        public DatabaseInitialiser(ILogger<DatabaseInitialiser> logger, TKHDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while starting the database.");
                throw;
            }
        }
    }

}


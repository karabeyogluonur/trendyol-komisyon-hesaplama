using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TKH.Core.Entities.Abstract;
using TKH.DataAccess.Contexts;
using TKH.Entities;

namespace TKH.DataAccess
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
                if (_context.Database.IsRelational())
                {
                    var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                    if (pendingMigrations.Any())
                    {
                        _logger.LogInformation("Pending migrations found. Updating database...");
                        await _context.Database.MigrateAsync();
                        _logger.LogInformation("Database migration completed successfully.");
                    }
                }

                await SeedSettingsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        private async Task SeedSettingsAsync()
        {
            _logger.LogInformation("Checking and seeding settings...");

            List<Type> settingTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.FullName != null && assembly.FullName.StartsWith("TKH"))
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(ISettings).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            if (!settingTypes.Any()) return;

            int newSettingsCount = 0;

            foreach (var type in settingTypes)
            {
                var instance = Activator.CreateInstance(type);
                if (instance is null) continue;

                string sectionName = type.Name.EndsWith("Settings") ? type.Name.Substring(0, type.Name.Length - "Settings".Length) : type.Name;

                List<string> existingKeys = await _context.Settings.Where(setting => setting.Name.StartsWith(sectionName + ".")).Select(setting => setting.Name).ToListAsync();

                foreach (var prop in type.GetProperties())
                {
                    if (!prop.CanRead || !prop.CanWrite) continue;

                    string key = $"{sectionName}.{prop.Name}";

                    if (existingKeys.Any(k => k.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    object? value = prop.GetValue(instance);
                    string valueStr = value is not null ? Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty : string.Empty;

                    _context.Settings.Add(new Setting
                    {
                        Name = key,
                        Value = valueStr
                    });

                    newSettingsCount++;
                    _logger.LogDebug("New setting added: {Key} = {Value}", key, valueStr);
                }
            }

            if (newSettingsCount > 0)
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("{Count} new settings seeded successfully.", newSettingsCount);
            }
            else
                _logger.LogInformation("All settings are up to date.");
        }
    }
}

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API
{
    public static class Migration
    {
        public static async Task Migrate(WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var scopeServices = scope.ServiceProvider;
                var loggerFactory = scopeServices.GetRequiredService<ILoggerFactory>();
                try
                {
                    var context = scopeServices.GetRequiredService<StoreContext>();
                    await context.Database.MigrateAsync();
                    await StoreContextSeed.SeedAsync(context, loggerFactory);
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occured during migration");
                }
            }
        }
    }
}

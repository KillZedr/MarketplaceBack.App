using Marketplace.Application;
using MarketplaceBack.Contracts.Exeptions;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceBack
{
    internal class DbInitializer
    {
        public static void InitializeDb(IServiceProvider serviceProvider)
        {
            if (!ApplyMigrations(serviceProvider))
            {
                throw new DbInitializationException("Could not initialize DB! See errors above.");
            }
        }


        private static bool ApplyMigrations(IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetRequiredService<Serilog.ILogger>();
            logger.Information("Applying migrations...");

            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MarketplaceBack_DbContext>();

            try
            {
                context.Database.Migrate();
                logger.Information("Migration(s) applied!");
            }
            catch (Exception ex)
            {
                logger.Error("Database.Migrate failed!");
                logger.Error(ex.ToString());
                return false;
            }
            return true; //add
        }
    }
}

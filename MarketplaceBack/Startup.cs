using Marketplace.Application;
using Marketplace.Application.Marketplace.DAL.Contracts;
using Marketplace.Application.Marketplace.DAL.RealisationInterfaces;
using Marketplace.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MarketplaceBack
{
    public class Startup
    {
        internal static void AddServices(WebApplicationBuilder builder)
        {
            AddSerilog(builder);
            RegisterDAL(builder.Services);
            AddErrorLogging(builder);
        }

        public static void RegisterDAL(IServiceCollection services)
        {
            services.AddDbContext<MarketplaceBack_DbContext>(options =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<MarketplaceBack_DbContext>();
                return new UnitOfWork(context);
            });

            // Добавление Identity с контекстом данных
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<MarketplaceBack_DbContext>()
                .AddDefaultTokenProviders();
        }

        internal static void AddErrorLogging(WebApplicationBuilder builder)
        {
            var errorLoggerConfig = new LoggerConfiguration()
                .WriteTo.File("errors.txt", rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Error();


            if (builder.Environment.IsDevelopment())
            {
                errorLoggerConfig = errorLoggerConfig.MinimumLevel.Debug();
            }
            else
            {
                errorLoggerConfig = errorLoggerConfig.MinimumLevel.Error();
            }

            var errorLogger = errorLoggerConfig.CreateLogger();
            builder.Services.AddSingleton<ILogger>(errorLogger);
        }
        internal static void AddSerilog(WebApplicationBuilder builder)
        {
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Month,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            if (builder.Environment.IsDevelopment())
            {
                loggerConfig = loggerConfig.MinimumLevel.Debug();
            }
            else
            {
                loggerConfig = loggerConfig.MinimumLevel.Warning();
            }
            var logger = loggerConfig.CreateLogger();
            builder.Services.AddSingleton<ILogger>(logger);
        }

        private static bool TestConnection(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            var logger = provider.GetRequiredService<ILogger>();
            var context = provider.GetRequiredService<DbContext>();
            logger.Information("Testing the DB connection....");
            try
            {
                var createdAnew = context.Database.EnsureCreated();
                if (createdAnew)
                {
                    logger.Information("Successfully created the DB");
                }
                else
                {
                    logger.Information("The DB is already there");
                }
            }
            catch (Exception ex)
            {
                logger.Information("EnsureCreated failed");
                logger.Information(ex.ToString());
                return false;
            }
            return true;
        }
    }
}


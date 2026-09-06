using ECommerce.Domain.Contracts;
using ECommerce.Perstistence.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Extentions
{
    public static class WebApplicationRegister
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(
            this WebApplication application
        )
        {
            await using var scope = application.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync();

            return application;
        }

        public static async Task<WebApplication> SeedDataAsync(this WebApplication application)
        {
            await using var scope = application.Services.CreateAsyncScope();

            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>(
                "Default"
            );

            await dataInitializer.InitializeAsync();
            return application;
        }

        public static async Task<WebApplication> SeedIdentityDataAsync(
            this WebApplication application
        )
        {
            await using var scope = application.Services.CreateAsyncScope();

            var dataInitializer = scope.ServiceProvider.GetRequiredKeyedService<IDataInitializer>(
                "Identity"
            );

            await dataInitializer.InitializeAsync();
            return application;
        }
    }
}

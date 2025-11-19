using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Persistence.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.ExtensionMethods
{
    public static class WebApplicationRegister
    {
        public static async Task<WebApplication> MigrateDatabase(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
            var pending = await dbContext.Database.GetPendingMigrationsAsync();
            //we used Any not AnyAsync because the pending is not queryable its IEnumrable
            if (pending.Any())
            {
                await dbContext.Database.MigrateAsync();
            }
            return app;
        }

        public static async Task<WebApplication> SeedData(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dataInitializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
            await dataInitializer.InitializeAsync();

            return app;
        }
    }
}

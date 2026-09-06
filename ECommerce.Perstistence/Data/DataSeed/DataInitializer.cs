using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Domain;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities.OrderModule;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Perstistence.Data.DbContexts;
using ECommerce.Perstistence.Data.Migrations;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Perstistence.Data.DataSeed
{
    public class DataInitializer : IDataInitializer
    {
        private readonly StoreDbContext _dbContext;

        public DataInitializer(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task InitializeAsync()
        {
            var hasTypes = await _dbContext.ProductTypes.AnyAsync();
            var hasBrands = await _dbContext.ProductBrands.AnyAsync();
            var hasProducts = await _dbContext.Products.AnyAsync();
            var hasDeliveryMethods = await _dbContext.DeliveryMethods.AnyAsync();

            if (hasTypes && hasBrands && hasProducts && hasDeliveryMethods)
                return;

            if (!hasTypes)
                await SeedDataFromJson<ProductType, int>("types.json", _dbContext.ProductTypes);
            if (!hasBrands)
                await SeedDataFromJson<ProductBrand, int>("brands.json", _dbContext.ProductBrands);

            await _dbContext.SaveChangesAsync();

            if (!hasDeliveryMethods)
                await SeedDataFromJson<DeliveryMethod, int>(
                    "delivery.json",
                    _dbContext.DeliveryMethods
                );

            if (!hasProducts)
                await SeedDataFromJson<Product, int>("products.json", _dbContext.Products);

            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedDataFromJson<T, TKey>(string fileName, DbSet<T> dbSet)
            where T : BaseEntity<TKey>, new()
        {
            var filePath = @$"..\ECommerce.Perstistence\Data\DataSeed\JsonFiles\{fileName}";

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Json File Doesn't Exist");

            try
            {
                var fileStream = File.OpenRead(filePath);

                var data = await JsonSerializer.DeserializeAsync<List<T>>(
                    fileStream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (data is not null && data.Count > 0)
                    await dbSet.AddRangeAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seeding Files Error {ex}");
            }
        }
    }
}

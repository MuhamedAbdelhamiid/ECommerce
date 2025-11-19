using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Domain.Contracts;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Entities.ProductModule;
using ECommerce.Persistence.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Persistence.Data.DataSeed
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
            try
            {
                var hasProducts = await _dbContext.Products.AnyAsync();
                var hasBrands = await _dbContext.ProductBrands.AnyAsync();
                var hasTypes = await _dbContext.ProductTypes.AnyAsync();

                if (hasProducts && hasBrands && hasTypes)
                    return;

                if (!hasBrands)
                    await SeedDataFromJson<ProductBrand, int>(
                        "brands.json",
                        _dbContext.ProductBrands
                    );

                if (!hasTypes)
                    await SeedDataFromJson<ProductType, int>("types.json", _dbContext.ProductTypes);

                await _dbContext.SaveChangesAsync();

                if (!hasProducts)
                {
                    await SeedDataFromJson<Product, int>("products.json", _dbContext.Products);
                    await _dbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        // It reads from files and add locally using dbset
        private async Task SeedDataFromJson<T, TKey>(string fileName, DbSet<T> set)
            where T : BaseEntity<TKey>, new()
        {
            var filePath = @"..\ECommerce.Persistence\Data\DataSeed\JsonFiles\" + fileName;

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File Not Found In Path", filePath);

            try
            {
                using var dataStream = File.OpenRead(filePath);
                var data = await JsonSerializer.DeserializeAsync<List<T>>(
                    dataStream,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );

                if (data is not null)
                    await set.AddRangeAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error While Reading Data From Json {ex}");
                throw;
            }
        }
    }
}

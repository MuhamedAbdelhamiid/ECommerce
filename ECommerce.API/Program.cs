using System.Threading.Tasks;
using ECommerce.API.ExtensionMethods;
using ECommerce.Domain.Contracts;
using ECommerce.Persistence.Data.Contexts;
using ECommerce.Persistence.Data.DataSeed;
using ECommerce.Persistence.Repositories;
using ECommerce.Services;
using ECommerce.Services.Abstraction;
using ECommerce.Services.MappingProfiles;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Register DI Container
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")
                );
            });
            builder.Services.AddScoped<IDataInitializer, DataInitializer>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddTransient<ProductPictureUrlResolver>();

            #endregion

            var app = builder.Build();

            await app.MigrateDatabase();
            await app.SeedData();

            #region Configure Middleware
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            await app.RunAsync();
        }
    }
}

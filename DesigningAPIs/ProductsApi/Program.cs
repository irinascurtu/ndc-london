
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProductsApi.Data;
using ProductsApi.Data.Repositories;
using ProductsApi.Infrastructure.Mappings;
using ProductsApi.Service;
using System.Threading.RateLimiting;

namespace ProductsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();


            builder.Services.AddAutoMapper(typeof(ProductProfileMapping).Assembly);


            builder.Services.AddDbContext<ProductContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddRateLimiter(options =>
            {
                options
                .AddFixedWindowLimiter(policyName: "fixed", options =>
                {
                    options.PermitLimit = 1;
                    options.Window = TimeSpan.FromSeconds(30);
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    options.QueueLimit = 0;
                });
                options.RejectionStatusCode = 429;
            });

            builder.Services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                //   o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                //  o.ApiVersionReader = new MediaTypeApiVersionReader();
                // o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
                //o.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
               
                //.ApiVersionReader = builder.Template("application/vnd.my.company.v{version}+json")
                //                         .Build();
                //

                o.ApiVersionReader = new MediaTypeApiVersionReaderBuilder().Template("application/vnd.example.v{api-version}+json").Build();
            })
                  .AddMvc().AddApiExplorer(
                      options =>
                      {
                          // the default is ToString(), but we want "'v'major[.minor][-status]"
                          options.GroupNameFormat = "'v'VVV";
                      }); ;

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddResponseCaching();
            builder.Services.AddSingleton<IMemoryCache>(new MemoryCache(
              new MemoryCacheOptions
              {
                  TrackStatistics = true,
                  SizeLimit = 50 // Products.
              }));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseDeveloperExceptionPage();
                using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    serviceScope.ServiceProvider.GetService<ProductContext>().Database.EnsureCreated();
                    serviceScope.ServiceProvider.GetService<ProductContext>().EnsureSeeded();
                }
            }

            app.UseRateLimiter();
            app.UseResponseCaching();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

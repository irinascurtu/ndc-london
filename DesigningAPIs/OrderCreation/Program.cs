using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersApi.Data.Repositories;
using OrdersApi.Data;
using OrdersApi.Service;
using OrdersApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using OrdersApi.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace OrderCreation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<OrderContext>(options => options.UseSqlServer(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    services.AddScoped<IOrderRepository, OrderRepository>();
                    services.AddScoped<IOrderService, OrderService>();
                    services.AddAutoMapper(typeof(OrderProfileMapping));
                    services.AddMassTransit(x =>
                    {
                        x.SetKebabCaseEndpointNameFormatter();

                        var entryAssembly = Assembly.GetEntryAssembly();

                        x.AddConsumers(entryAssembly);

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ReceiveEndpoint("create-order-command", e =>
                            {
                               //e.ConfigureConsumer<CreateOrderConsumer>(context);
                            });
                            cfg.ConfigureEndpoints(context);
                        });
                    });
                });
    }
}

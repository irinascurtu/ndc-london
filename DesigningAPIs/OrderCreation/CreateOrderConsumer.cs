using AutoMapper;
using Contracts.Events;
using MassTransit;
using Orders.Domain.Entities;
using OrdersApi.Models;
using OrdersApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCreation
{
    public class CreateOrderConsumer : IConsumer<OrderModel>
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public CreateOrderConsumer(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }


        public async Task Consume(ConsumeContext<OrderModel> context)
        {
            //save the product in the database
            // access domain
            //Repository
            // Mappings -> OrderModel to Order
            Console.WriteLine($"I got a command to create an order:{context.Message}");
            //mapping from Message to an order object
            var orderToAdd = mapper.Map<Order>(context.Message);

            /// Implement the logic to create an order
            var savedOrder = await orderService.AddOrderAsync(orderToAdd);
            /// send a notification to Admin

            var notifyOrderCreated = context.Publish(new OrderCreated()
            {
                CreatedAt = savedOrder.OrderDate,
                OrderId = savedOrder.Id
            });

          
            await Task.CompletedTask;
        }
    }
}

using Contracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminNotification
{
    public class OrderCreatedNotificationConsumer : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await Task.Delay(1000);
            Console.WriteLine($"Just got a notification about an OrderCreated{context.Message.OrderId}");
        }
    }
}

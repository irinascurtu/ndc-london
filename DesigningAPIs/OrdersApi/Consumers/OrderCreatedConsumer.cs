using Contracts.Events;
using MassTransit;

namespace OrdersApi.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            await Task.Delay(1000);
            Console.WriteLine("Just got a message");
            Console.WriteLine(context.Message.OrderId);
        }
    }
}

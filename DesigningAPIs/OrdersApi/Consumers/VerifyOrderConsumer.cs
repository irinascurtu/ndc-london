using AutoMapper;
using Contracts.Response;
using MassTransit;
using OrdersApi.Services;

namespace OrdersApi.Consumers
{
    public class VerifyOrderConsumer : IConsumer<VerifyOrder>
    {
        private readonly IOrderService orderService;
        private readonly IMapper mapper;

        public VerifyOrderConsumer(IOrderService orderService, IMapper mapper)
        {
            this.orderService = orderService;
            this.mapper = mapper;
        }

        public async Task Consume(ConsumeContext<VerifyOrder> context)
        {
            throw new ArgumentNullException();
            var existingOrder = await orderService.GetOrderAsync(context.Message.Id);
            if (existingOrder != null)
            {
                await context.RespondAsync(new OrderResult()
                {
                    Id = existingOrder.Id,
                    OrderDate = existingOrder.OrderDate,
                    Status = existingOrder.Status
                });
            }
            else
            {
                await context.RespondAsync(new OrderNotFoundResult()
                {
                    ErrorMessage = "Sorry, your order is not there. Please try again."
                });
            }

            await Task.CompletedTask;
        }
    }
}

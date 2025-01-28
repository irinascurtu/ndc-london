using AutoMapper;
using Contracts.Events;
using Contracts.Response;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Mvc;
using Orders.Domain.Entities;
using OrdersApi.Models;
using OrdersApi.Service.Clients;
using OrdersApi.Services;
using Stocks;

namespace OrdersApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IProductStockServiceClient _productStockServiceClient;
        private readonly IMapper _mapper;
        private readonly Greeter.GreeterClient grpcClient;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IRequestClient<VerifyOrder> requestClient;
        private readonly ISendEndpointProvider sendEndpointProvider;

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper,
            Stocks.Greeter.GreeterClient grpcClient,
            IPublishEndpoint publishEndpoint,
            IRequestClient<VerifyOrder> requestClient,
             ISendEndpointProvider sendEndpointProvider
            )
        {
            _orderService = orderService;
            _productStockServiceClient = productStockServiceClient;
            _mapper = mapper;
            this.grpcClient = grpcClient;
            this.publishEndpoint = publishEndpoint;
            this.requestClient = requestClient;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {

            var orderToAdd = _mapper.Map<Order>(model);
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);

            //var notifyOrderCreated = publishEndpoint.Publish(new OrderCreated()
            //{
            //    CreatedAt = createdOrder.OrderDate,
            //    OrderId = createdOrder.Id
            //});
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-command"));
            await sendEndpoint.Send(model);

            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            //var order = await _orderService.GetOrderAsync(id);
            //if (order == null)
            //{
            //    return NotFound();
            //}
            // return Ok(order);
            try
            {


                var response = await requestClient.GetResponse<OrderResult, OrderNotFoundResult>(
                    new VerifyOrder()
                    {
                        Id = id
                    });

                if (response.Is(out Response<OrderResult> incomingMessage))
                {
                    return Ok(incomingMessage.Message);
                }

                if (response.Is(out Response<OrderNotFoundResult> notFound))
                {
                    return NotFound(notFound.Message);
                }
            }
            catch (Exception EX)
            {
                return Accepted();
            }
            return BadRequest();
        }
    }
}

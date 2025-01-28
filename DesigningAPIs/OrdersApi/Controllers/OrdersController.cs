using AutoMapper;
using Azure.Core;
using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Data.Domain;
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

        public OrdersController(IOrderService orderService,
            IProductStockServiceClient productStockServiceClient,
            IMapper mapper,
            Stocks.Greeter.GreeterClient grpcClient
            )
        {
            _orderService = orderService;
            _productStockServiceClient = productStockServiceClient;
            _mapper = mapper;
            this.grpcClient = grpcClient;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderModel model)
        {
            // classical http client
            var stocks = await _productStockServiceClient.GetStock(
                model.OrderItems.Select(p => p.ProductId).ToList());

            // grpc client
            //try
            //{

            //}
            //catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            //{
            //    Console.WriteLine("Stream cancelled.");
            //}

            var stockRequest = new StockRequest();
            stockRequest.ProductId.AddRange(model.OrderItems.Select(p => p.ProductId));
            var request = grpcClient.GetStock(stockRequest);

            //To do: Verify stock 
            var orderToAdd = _mapper.Map<Order>(model);
            var createdOrder = await _orderService.AddOrderAsync(orderToAdd);
 

            return CreatedAtAction("GetOrder", new { id = createdOrder.Id }, createdOrder);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }
    }
}

using MassTransit;
using OrdersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderCreation
{
    public class CreateOrderConsumer : IConsumer<OrderModel>
    {
        public Task Consume(ConsumeContext<OrderModel> context)
        {
            //save the product in the database
            // access domain
            //Repository
            // Mappings -> OrderModel to Order
            throw new NotImplementedException();
        }
    }
}

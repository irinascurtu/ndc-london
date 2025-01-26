using OrdersApi.Data.Domain;

namespace OrdersApi.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task<Order> GetOrderAsync(int id);
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<bool> OrderExistsAsync(int id);
        Task<Order> UpdateOrderAsync(Order order);
    }
}
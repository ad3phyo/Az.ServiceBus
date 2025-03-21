using ServiceBus.Models;

namespace ServiceBus.Repository.Interfaces;

public interface IOrderRepository
{
    Task<OrderMessage> CreateOrderAsync(OrderMessage order);
    Task<OrderMessage?> GetOrderAsync(string orderId);
    Task<IEnumerable<OrderMessage>> GetAllOrdersAsync();
    Task<OrderMessage> UpdateOrderAsync(OrderMessage order);
    Task DeleteOrderAsync(string orderId);
} 
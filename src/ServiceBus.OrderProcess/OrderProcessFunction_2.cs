using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ServiceBus.Core;
using ServiceBus.Models;
using ServiceBus.Repository.Interfaces;
using Azure.Messaging.ServiceBus;

namespace ServiceBus.OrderProcess.Functions;

public class OrderProcessFunction
{
    private readonly ILogger<OrderProcessFunction> _logger;
    private readonly IOrderRepository _orderRepository;

    public OrderProcessFunction(
        ILogger<OrderProcessFunction> logger,
        IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    [Function("ProcessOrder")]
    public async Task RunAsync(
        [ServiceBusTrigger(
            "topic.1",
            "subscription.1",
            Connection = "ServiceBusConnection",
            IsSessionsEnabled = false)] ServiceBusReceivedMessage message)
    {
        try
        {
            _logger.LogInformation("Received message from Service Bus topic. MessageId: {MessageId}", message.MessageId);

            // Get the message body as string
            var messageBody = message.Body.ToString();
            _logger.LogInformation("Message body: {MessageBody}", messageBody);

            // Deserialize the message
            var order = JsonSerializer.Deserialize<OrderMessage>(messageBody);
            if (order == null)
            {
                _logger.LogError("Failed to deserialize order message");
                return;
            }

            _logger.LogInformation("Processing order for OrderId: {OrderId}", order.OrderId);

            // Save order to Cosmos DB
            var savedOrder = await _orderRepository.CreateOrderAsync(order);
            _logger.LogInformation("Order saved to Cosmos DB. OrderId: {OrderId}", savedOrder.OrderId);

            // Process the order
            await ProcessOrderAsync(order);

            _logger.LogInformation("Successfully processed order for OrderId: {OrderId}", order.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order. MessageId: {MessageId}", message.MessageId);
            throw; // Retries will be handled by Service Bus
        }
    }

    private async Task ProcessOrderAsync(OrderMessage order)
    {
        try
        {
            // Add your order processing logic here
            // For example:
            // 1. Validate order
            // 2. Check inventory
            // 3. Process payment
            // 4. Send notifications
            
            await Task.Delay(1000); // Simulate processing time
            _logger.LogInformation("Order processed for OrderId: {OrderId}", order.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order for OrderId: {OrderId}", order.OrderId);
            throw;
        }
    }
} 
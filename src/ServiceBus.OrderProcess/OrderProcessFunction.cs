using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ServiceBus.Models;
using ServiceBus.Repository.Interfaces;

namespace ServiceBus.OrderProcess
{
    public class OrderProcessFunction
    {
        private readonly ILogger<OrderProcessFunction> _logger;

        private readonly IOrderRepository _orderRepository;

        public OrderProcessFunction(ILogger<OrderProcessFunction> logger,
         IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [Function(nameof(OrderProcessFunction))]
        public async Task Run(
            [ServiceBusTrigger(
                "topic.1",
                "subscription.1",
                Connection = "ServiceBusConnection")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
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

            order.Id = Guid.NewGuid().ToString();

            // Save order to Cosmos DB
            var savedOrder = await _orderRepository.CreateOrderAsync(order);
            _logger.LogInformation("Order saved to Cosmos DB. OrderId: {OrderId}", savedOrder.OrderId);

            

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}

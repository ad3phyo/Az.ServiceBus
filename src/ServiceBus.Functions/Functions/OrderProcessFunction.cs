using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using ServiceBus.Models;

namespace ServiceBus.Functions
{
    public class OrderProcessFunction
    {
        private readonly ILogger<OrderProcessFunction> _logger;

        public OrderProcessFunction(ILogger<OrderProcessFunction> logger)
        {
            _logger = logger;
        }

        [Function(nameof(OrderProcessFunction))]
        public async Task Run(
            [ServiceBusTrigger(
                "%ServiceBusTopicName%",
                "%ServiceBusSubscriptionName%",
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

            // Save order to Cosmos DB
            _logger.LogInformation($"Data saved to Cosmos DB at: {DateTime.UtcNow}");

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}

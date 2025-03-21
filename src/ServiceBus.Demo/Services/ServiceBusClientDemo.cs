using ServiceBus.Core;
using ServiceBus.Models;

namespace ServiceBus.Demo.Services;

public class ServiceBusClientDemo : IAsyncDisposable, IDisposable
{
    private readonly ServiceBusService _serviceBus;
    private bool _disposed;

    public ServiceBusClientDemo(string connectionString, string queueName, string topicName, string subscriptionName)
    {
        var config = new ServiceBusConfig
        {
            ConnectionString = connectionString,
            QueueName = queueName,
            TopicName = topicName,
            SubscriptionName = subscriptionName
        };

        _serviceBus = new ServiceBusService(config);
    }

    public async Task SendOrderToQueueAsync()
    {
        var order = new OrderMessage
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerName = "John Doe",
            Amount = 150.50m,
            OrderDate = DateTime.UtcNow
        };

        await _serviceBus.SendMessageAsync(order);
        Console.WriteLine($"Order {order.OrderId} sent to queue successfully.");
    }

    public async Task SendOrderToTopicAsync()
    {
        var order = new OrderMessage
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerName = "Jane Smith",
            Amount = 299.99m,
            OrderDate = DateTime.UtcNow
        };

        await _serviceBus.SendTopicMessageAsync(order);
        Console.WriteLine($"Order {order.OrderId} sent to topic successfully.");
    }

    public async Task ReceiveOrderFromQueueAsync()
    {
        var order = await _serviceBus.ReceiveMessageAsync<OrderMessage>();
        if (order != null)
        {
            Console.WriteLine($"Received order - ID: {order.OrderId}, Customer: {order.CustomerName}, Amount: {order.Amount:C}");
        }
        else
        {
            Console.WriteLine("No order message available in the queue.");
        }
    }

    public async Task StartQueueProcessingAsync()
    {
        Console.WriteLine("Starting queue message processing...");
        await _serviceBus.StartProcessingMessagesAsync<OrderMessage>(async order =>
        {
            Console.WriteLine($"Processing order - ID: {order.OrderId}");
            Console.WriteLine($"Customer: {order.CustomerName}");
            Console.WriteLine($"Amount: {order.Amount:C}");
            Console.WriteLine($"Order Date: {order.OrderDate}");
            Console.WriteLine("------------------------");

            // Simulate some processing time
            await Task.Delay(1000);
        });
    }

    public async Task StartTopicProcessingAsync()
    {
        Console.WriteLine("Starting topic message processing...");
        await _serviceBus.StartProcessingTopicMessagesAsync<OrderMessage>(async order =>
        {
            Console.WriteLine($"Processing topic order - ID: {order.OrderId}");
            Console.WriteLine($"Customer: {order.CustomerName}");
            Console.WriteLine($"Amount: {order.Amount:C}");
            Console.WriteLine($"Order Date: {order.OrderDate}");
            Console.WriteLine("------------------------");

            // Simulate some processing time
            await Task.Delay(1000);
        });
    }

    public async Task StopProcessingAsync()
    {
        await _serviceBus.StopProcessingAsync();
        Console.WriteLine("Message processing stopped.");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _serviceBus.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }

        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _serviceBus.DisposeAsync();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
} 
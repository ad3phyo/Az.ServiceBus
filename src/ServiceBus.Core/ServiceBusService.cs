using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace ServiceBus.Core;

public class ServiceBusService : IAsyncDisposable
{
    private readonly ServiceBusClient _client;
    private readonly ServiceBusConfig _config;
    private ServiceBusSender? _sender;
    private ServiceBusReceiver? _receiver;
    private ServiceBusProcessor? _processor;

    public ServiceBusService(ServiceBusConfig config)
    {
        _config = config;
        _client = new ServiceBusClient(config.ConnectionString);
    }

    public async Task SendMessageAsync<T>(T message)
    {
        try
        {
            _sender ??= _client.CreateSender(_config.QueueName);
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageBody);
            await _sender.SendMessageAsync(serviceBusMessage);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task SendTopicMessageAsync<T>(T message)
    {
        try
        {
            _sender ??= _client.CreateSender(_config.TopicName);
            var messageBody = JsonSerializer.Serialize(message);
            var serviceBusMessage = new ServiceBusMessage(messageBody);
            await _sender.SendMessageAsync(serviceBusMessage);
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<T?> ReceiveMessageAsync<T>()
    {
        try
        {
            _receiver ??= _client.CreateReceiver(_config.QueueName);
            var message = await _receiver.ReceiveMessageAsync();
            if (message != null)
            {
                var messageBody = message.Body.ToString();
                await _receiver.CompleteMessageAsync(message);
                return JsonSerializer.Deserialize<T>(messageBody);
            }
            return default;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task StartProcessingMessagesAsync<T>(Func<T, Task> messageHandler)
    {
        try
        {
            _processor = _client.CreateProcessor(_config.QueueName);

            _processor.ProcessMessageAsync += async args =>
            {
                var messageBody = args.Message.Body.ToString();
                var message = JsonSerializer.Deserialize<T>(messageBody);
                if (message != null)
                {
                    await messageHandler(message);
                }
                await args.CompleteMessageAsync(args.Message);
            };

            _processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine($"Error occurred: {args.Exception.Message}");
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task StartProcessingTopicMessagesAsync<T>(Func<T, Task> messageHandler)
    {
        try
        {
            _processor = _client.CreateProcessor(
                topicName: _config.TopicName,
                subscriptionName: _config.SubscriptionName);

            _processor.ProcessMessageAsync += async args =>
            {
                var messageBody = args.Message.Body.ToString();
                var message = JsonSerializer.Deserialize<T>(messageBody);
                if (message != null)
                {
                    await messageHandler(message);
                }
                await args.CompleteMessageAsync(args.Message);
            };

            _processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine($"Error occurred: {args.Exception.Message}");
                return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task StopProcessingAsync()
    {
        if (_processor != null)
        {
            await _processor.StopProcessingAsync();
            await _processor.DisposeAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_processor != null)
        {
            await _processor.DisposeAsync();
        }
        if (_sender != null)
        {
            await _sender.DisposeAsync();
        }
        if (_receiver != null)
        {
            await _receiver.DisposeAsync();
        }
        await _client.DisposeAsync();
    }
} 
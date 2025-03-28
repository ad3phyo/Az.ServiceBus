using Azure.Messaging.ServiceBus;

namespace ServiceBus.Core;

public class ServiceBusConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public string QueueName { get; set; } = string.Empty;
    public string TopicName { get; set; } = string.Empty;
    public string SubscriptionName { get; set; } = string.Empty;

    public ServiceBusTransportType TransportType { get; set; } = ServiceBusTransportType.AmqpTcp; // default
} 
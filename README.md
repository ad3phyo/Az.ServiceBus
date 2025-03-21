# Azure Service Bus Library

This is a .NET Core 8 library that provides a simple wrapper around Azure Service Bus functionality. It supports sending and receiving messages using both queues and topics/subscriptions.

## Features

- Send messages to queues
- Send messages to topics
- Receive messages from queues
- Subscribe to topics
- Message processing with automatic completion
- Async/await support
- Generic message type support
- Proper resource disposal

## Installation

Add the project reference to your solution or install via NuGet package manager.

## Usage

### Configuration

First, create a configuration object with your Azure Service Bus connection details:

```csharp
var config = new ServiceBusConfig
{
    ConnectionString = "your_connection_string",
    QueueName = "your_queue_name",
    TopicName = "your_topic_name",
    SubscriptionName = "your_subscription_name"
};
```

### Sending Messages to a Queue

```csharp
using var serviceBus = new ServiceBusService(config);
var message = new YourMessageType { /* your message properties */ };
await serviceBus.SendMessageAsync(message);
```

### Sending Messages to a Topic

```csharp
using var serviceBus = new ServiceBusService(config);
var message = new YourMessageType { /* your message properties */ };
await serviceBus.SendTopicMessageAsync(message);
```

### Receiving Messages from a Queue

```csharp
using var serviceBus = new ServiceBusService(config);
var message = await serviceBus.ReceiveMessageAsync<YourMessageType>();
```

### Processing Messages Continuously

```csharp
using var serviceBus = new ServiceBusService(config);

await serviceBus.StartProcessingMessagesAsync<YourMessageType>(async message =>
{
    // Process your message here
    await Task.CompletedTask;
});

// To stop processing:
await serviceBus.StopProcessingAsync();
```

### Processing Topic Messages

```csharp
using var serviceBus = new ServiceBusService(config);

await serviceBus.StartProcessingTopicMessagesAsync<YourMessageType>(async message =>
{
    // Process your message here
    await Task.CompletedTask;
});

// To stop processing:
await serviceBus.StopProcessingAsync();
```

## Important Notes

- Always dispose of the `ServiceBusService` instance properly using the `using` statement or by calling `DisposeAsync()`
- Make sure to handle exceptions appropriately in your application
- The library uses System.Text.Json for message serialization
- Connection strings and other sensitive information should be stored securely (e.g., using Azure Key Vault or user secrets)

## Requirements

- .NET 8.0 or later
- Azure Service Bus namespace and appropriate connection string
- Azure.Messaging.ServiceBus NuGet package (automatically included) 
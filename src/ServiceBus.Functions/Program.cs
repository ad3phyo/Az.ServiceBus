using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ServiceBus.Core;
using ServiceBus.Models;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();
        
        // Register ServiceBusService
        services.AddSingleton(sp => new ServiceBusService(new ServiceBusConfig
        {
            ConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection") ?? throw new InvalidOperationException("ServiceBusConnection not found"),
            QueueName = Environment.GetEnvironmentVariable("ServiceBusQueueName") ?? "orders-queue",
            TopicName = Environment.GetEnvironmentVariable("ServiceBusTopicName") ?? "orders-topic",
            SubscriptionName = Environment.GetEnvironmentVariable("ServiceBusSubscriptionName") ?? "order-processing-subscription"
        }));
    })
    .Build();

host.Run(); 
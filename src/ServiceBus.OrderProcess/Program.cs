using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBus.Core;
using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;

// using ServiceBus.Repository.Config;
// using ServiceBus.Repository.Interfaces;
// using ServiceBus.Repository.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        
        // // Register Cosmos DB configuration
        // services.AddSingleton(new CosmosDbConfig
        // {
        //     EndpointUrl = Environment.GetEnvironmentVariable("EndpointUrl") ?? throw new InvalidOperationException("CosmosDbConnection not found"),
        //     AuthorizationKey = Environment.GetEnvironmentVariable("AuthorizationKey") ?? throw new InvalidOperationException("CosmosDbConnection not found"),
        //     DatabaseName = Environment.GetEnvironmentVariable("CosmosDbName") ?? "OrdersDb",
        //     ContainerName = Environment.GetEnvironmentVariable("CosmosDbContainer") ?? "Orders",
        //     PartitionKeyPath = "/orderId"
        // });

        // // Register Cosmos DB repository
        // services.AddSingleton<IOrderRepository, CosmosOrderRepository>();
        // Bind CosmosDbConfig from app settings
        // services.AddOptions<CosmosDbConfig>()
        //         .Configure<IConfiguration>((settings, configuration) =>
        //         {
        //             configuration.GetSection("CosmosDb").Bind(settings);
        //         });

        // Register service
        // services.AddSingleton(typeof(ICosmosDbService<>), typeof(CosmosDbService<>));

        // Register ServiceBus service

        // options
        var options = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };

        // Add options to servicebus service

        services.AddSingleton(options);

        services.AddSingleton(sp => new ServiceBus.Core.ServiceBusService(new ServiceBusConfig
        {
            ConnectionString = Environment.GetEnvironmentVariable("ServiceBusConnection") ?? throw new InvalidOperationException("ServiceBusConnection not found"),
            QueueName = Environment.GetEnvironmentVariable("ServiceBusQueueName") ?? "queue.1",
            TopicName = Environment.GetEnvironmentVariable("ServiceBusTopicName") ?? "topic.1",
            SubscriptionName = Environment.GetEnvironmentVariable("ServiceBusSubscriptionName") ?? "subscription.1",
            TransportType = ServiceBusTransportType.AmqpWebSockets

        }));
    })
    .Build();

host.Run();
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using ServiceBus.Models;
using ServiceBus.Repository.Config;
using ServiceBus.Repository.Interfaces;

namespace ServiceBus.Repository.Repositories;

public class CosmosOrderRepository : IOrderRepository
{
    private readonly Container _container;
    private readonly CosmosClient _cosmosClient;

    public CosmosOrderRepository(CosmosDbConfig config)
    {
        var cosmosClientBuilder = new CosmosClientBuilder(config.EndpointUrl,
            config.AuthorizationKey)
            .WithApplicationName("ServiceBusApp")
            .WithSerializerOptions(new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            });

        _cosmosClient = cosmosClientBuilder.Build();
        _container = _cosmosClient.GetContainer(config.DatabaseName, config.ContainerName);
    }

    public async Task<OrderMessage> CreateOrderAsync(OrderMessage order)
    {
        // Ensure the container is not null
        if (_container == null)
        {
            Console.WriteLine("Container is null");
            throw new ArgumentNullException(nameof(_container));
        }

        // Ensure the order has a valid id
        if (string.IsNullOrEmpty(order.Id))
        {
            order.Id = Guid.NewGuid().ToString();
        }

        var response = await _container.CreateItemAsync(order, new PartitionKey(order.OrderId));
        return response.Resource;
    }

    public async Task<OrderMessage?> GetOrderAsync(string orderId)
    {
        try
        {
            var response = await _container.ReadItemAsync<OrderMessage>(orderId, new PartitionKey(orderId));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<OrderMessage>> GetAllOrdersAsync()
    {
        var query = _container.GetItemQueryIterator<OrderMessage>(new QueryDefinition("SELECT * FROM c"));
        var results = new List<OrderMessage>();

        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response.ToList());
        }

        return results;
    }

    public async Task<OrderMessage> UpdateOrderAsync(OrderMessage order)
    {
        var response = await _container.UpsertItemAsync(order, new PartitionKey(order.OrderId));
        return response.Resource;
    }

    public async Task DeleteOrderAsync(string orderId)
    {
        await _container.DeleteItemAsync<OrderMessage>(orderId, new PartitionKey(orderId));
    }

    public void Dispose()
    {
        _cosmosClient?.Dispose();
    }
}

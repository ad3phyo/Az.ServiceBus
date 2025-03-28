using Microsoft.Extensions.Options;
using Microsoft.Azure.Cosmos;

namespace CosmosDbLibrary
{
    public class CosmosDbService<T> : ICosmosDbService<T> where T : class
    {
        private readonly Container _container;
        private readonly CosmosClient _cosmosClient;

        private readonly CosmosDbConfig _config;

        public CosmosDbService(IOptions<CosmosDbConfig> configOptions)
        {
            var config = configOptions.Value;
            _cosmosClient = new CosmosClient(config.Account, config.Key);
            _container = cosmosClient.GetContainer(config.DatabaseId, config.ContainerId);
        }

        public async Task AddItemAsync(T item)
        {
            await _container.CreateItemAsync(item);
        }
    }
}

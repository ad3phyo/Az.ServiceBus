namespace ServiceBus.Repository.Config;

public class CosmosDbConfig
{
    public string EndpointUrl { get; set; } = string.Empty;
    public string AuthorizationKey { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public string PartitionKeyPath { get; set; } = "/orderId";
} 
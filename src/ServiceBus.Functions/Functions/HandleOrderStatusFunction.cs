using System.Text.Json.Nodes;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServiceBus.Core;
using ServiceBus.Models;

namespace ServiceBus.Functions.Functions;

public class HandleOrderStatusFunction
{
    private readonly ILogger<HandleOrderStatusFunction> _logger;

    private readonly ServiceBusService _serviceBus;

    public HandleOrderStatusFunction(ILogger<HandleOrderStatusFunction> logger
    , ServiceBusService serviceBus)
    {
        _logger = logger;
        _serviceBus = serviceBus;
    }

    [Function("HandleOrderStatus")]
    public async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "order/status")] HttpRequestData req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            _logger.LogInformation("Received order status update: {Message}", requestBody);

            // Deserialize the request body explicitly to avoid any issues

            var statusUpdate = JsonConvert.DeserializeObject<OrderMessage>(requestBody);

            if (statusUpdate == null)
            {
                _logger.LogWarning("Invalid request body received.");
                var badRequestResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid request body.");
                return badRequestResponse;
            }

            string orderId = statusUpdate.OrderId;
            string customer = statusUpdate.CustomerName;
            decimal amount = statusUpdate.Amount;
            DateTime orderDate = DateTime.UtcNow;

            // Send order status update to Service Bus topic
            await _serviceBus.SendTopicMessageAsync(new OrderMessage
            {
                OrderId = orderId,
                CustomerName = customer,
                Amount = amount,
                OrderDate = orderDate
            });

            _logger.LogInformation($"Order {orderId} status sent to queue successfully.");

            // Here you could implement additional business logic
            // For example, sending notifications, updating databases, etc.
            await Task.Delay(500); // Simulate some processing

            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync("Order status update processed successfully.");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order status update");
            var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("An error occurred while processing the request.");
            return errorResponse;
        }
    }
}
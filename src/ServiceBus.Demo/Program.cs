using ServiceBus.Demo.Services;

namespace ServiceBus.Demo;

public class Program
{
    public static async Task Main(string[] args)
    {
        // Replace these with your actual Azure Service Bus connection details
        var connectionString = "Endpoint=sb://microsvcs.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=qshKnfss2jrzc8co/E38flW0Mp0k3jTkO+ASbPDQxrg=";
        // var connectionString = "Endpoint=sb://localhost;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true";
        var queueName = "queue.1";
        var topicName = "topic.1";
        var subscriptionName = "subscription.1";

        // Create demo instance
        using var demo = new ServiceBusClientDemo(connectionString, queueName, topicName, subscriptionName);

        try
        {
            // Demo 1: Send and receive messages using queue
            Console.WriteLine("Demo 1: Queue Operations");
            Console.WriteLine("------------------------");
            
            // Send multiple messages to queue
            await demo.SendOrderToQueueAsync();
            await demo.SendOrderToQueueAsync();
            
            // Receive messages from queue
            await demo.ReceiveOrderFromQueueAsync();
            await demo.ReceiveOrderFromQueueAsync();

            Console.WriteLine("\nPress any key to continue to topic demo...");
            Console.ReadKey();

            // Demo 2: Send messages to topic and process them
            Console.WriteLine("\nDemo 2: Topic Operations");
            Console.WriteLine("------------------------");
            
            // Send messages to topic
            await demo.SendOrderToTopicAsync();
            await demo.SendOrderToTopicAsync();

            // Start processing messages from topic subscription
            await demo.StartTopicProcessingAsync();

            Console.WriteLine("\nProcessing messages from topic. Press any key to stop...");
            Console.ReadKey();

            // Stop processing
            await demo.StopProcessingAsync();

            // Demo 3: Continuous Queue Processing
            Console.WriteLine("\nDemo 3: Continuous Queue Processing");
            Console.WriteLine("----------------------------------");

            // Send some messages first
            await demo.SendOrderToQueueAsync();
            await demo.SendOrderToQueueAsync();
            await demo.SendOrderToQueueAsync();

            // Start processing messages from queue
            await demo.StartQueueProcessingAsync();

            Console.WriteLine("\nProcessing messages from queue. Press any key to stop...");
            Console.ReadKey();

            // Stop processing
            await demo.StopProcessingAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
        }

        Console.WriteLine("\nDemo completed. Press any key to exit...");
        Console.ReadKey();
    }
} 
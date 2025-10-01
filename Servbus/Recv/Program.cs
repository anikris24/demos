using Azure.Messaging.ServiceBus;
namespace Program;


class Program{

    public static async Task Main()
    {
        string connectionString = Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");
        string queueName = Environment.GetEnvironmentVariable("SERVICE_BUS_QUEUE_NAME");

        Console.WriteLine($"{connectionString}");
        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(queueName))
        {
            Console.WriteLine("Please set the SERVICE_BUS_CONNECTION_STRING and SERVICE_BUS_QUEUE_NAME environment variables.");
            return;
        }

        await ReceiveMessagesAsync(connectionString, queueName);

    }
    

    public static async Task ReceiveMessagesAsync(string connectionString, string queueName)
    {
        var clientOptions = new ServiceBusClientOptions
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        // The ServiceBusClient is the main entry point.
        await using var client = new ServiceBusClient(connectionString, clientOptions);

        // Create a processor for the queue.
        // The processor will run in the background, continuously checking for messages.
        ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

        try
        {
            // Set the message and error handlers
            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            // Start the message processing in the background
            await processor.StartProcessingAsync();

            Console.WriteLine("Start receiving messages. Press any key to stop the processor.");
            Console.ReadKey();

            // Stop processing
            Console.WriteLine("\nStopping the processor...");
            await processor.StopProcessingAsync();
            Console.WriteLine("Stopped receiving messages.");
        }
        finally
        {
            // Clean up
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }
    }

// Handler for processing messages
async static Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received message: ID={args.Message.MessageId}, Content='{body}'");

    // Complete the message. This removes the message from the queue.
    // This is vital in PeekLock mode.
    await args.CompleteMessageAsync(args.Message);
}

    // Handler for processing errors
    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"Error occurred while processing: {args.Exception.Message}");
        Console.WriteLine($"Error source: {args.ErrorSource}");
        Console.WriteLine($"Entity path: {args.EntityPath}");
        return Task.CompletedTask;
    }
}
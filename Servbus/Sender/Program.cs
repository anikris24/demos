using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
namespace Program;


public class Program{

    public static async Task Main()
    {
        await SendMessagesAsync("", "");
    }
    
    

    public static async Task SendMessagesAsync(string connectionString, string queueName)
    {
        // The ServiceBusClient is the main entry point to interact with Azure Service Bus.
        // It is safe to cache and use as a singleton.
        await using var client = new ServiceBusClient(connectionString);

        // Create a sender for the queue.
        ServiceBusSender sender = client.CreateSender(queueName);

        // --- 1. Send a single message ---
        string singleMessageBody = "Hello, Service Bus Queue!";
        var singleMessage = new ServiceBusMessage(singleMessageBody);

        try
        {
            await sender.SendMessageAsync(singleMessage);
            Console.WriteLine($"Sent a single message: {singleMessageBody}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending single message: {ex.Message}");
        }

        // --- 2. Send a batch of messages ---
        int numberOfMessages = 3;

        try
        {
            // Create a message batch
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numberOfMessages; i++)
            {
                string batchMessageBody = $"Batch Message {i}";
                var batchMessage = new ServiceBusMessage(batchMessageBody);

                // Try to add the message to the batch. Check if it fits.
                if (!messageBatch.TryAddMessage(batchMessage))
                {
                    // The batch is full, send it now and create a new one for the remaining messages
                    await sender.SendMessagesAsync(messageBatch);
                    Console.WriteLine($"Batch is full, sent {messageBatch.Count} messages.");

                    // Re-create the batch and try to add the message again
                    messageBatch.Dispose(); // Dispose the old batch before re-creating
                    using ServiceBusMessageBatch newBatch = await sender.CreateMessageBatchAsync();
                    if (!newBatch.TryAddMessage(batchMessage))
                    {
                        // If it still can't be added, the message itself is too large.
                        throw new Exception($"Message {i} is too large for a batch.");
                    }
                }
            }

            // Send the final batch of messages
            if (messageBatch.Count > 0)
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"Sent final batch of {messageBatch.Count} messages.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message batch: {ex.Message}");
        }
    }
    }
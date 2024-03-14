// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;

//const string connectionString = "Endpoint=sb://az-course.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yFO/STLeo+Z6KeK0WzATNqnbDhV4FqyGg+ASbLq9RLw=";
const string connectionString = "Endpoint=sb://az-bus-standard.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=E27E2v1dbIe6nnTl9bgBGVrAbltv75V0I+ASbNLVg/4=";
const string queueName = "az-course-queue-1";
const string topicName = "az-course-topic";

const int maxNumberOfMessages = 3;

ServiceBusClient client;
ServiceBusSender sender;

client = new ServiceBusClient(connectionString);
sender = client.CreateSender(topicName);

using ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync();

for (int i = 0; i < maxNumberOfMessages; i++)
{
    if (!batch.TryAddMessage(new ServiceBusMessage($"This is a message - {i}")))
    {
        Console.WriteLine($"Message - {i} was not added to the batch.");
    }
}

try
{
    await sender.SendMessagesAsync(batch);
    Console.WriteLine("Messages sent");
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}
finally
{
    await sender.DisposeAsync();
    await client.DisposeAsync();
}
// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

//const string connectionString = "Endpoint=sb://az-course.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=yFO/STLeo+Z6KeK0WzATNqnbDhV4FqyGg+ASbLq9RLw=";
const string connectionString = "Endpoint=sb://az-bus-standard.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=E27E2v1dbIe6nnTl9bgBGVrAbltv75V0I+ASbNLVg/4=";
//const string queueName = "az-course-queue-1";
const string topicName = "az-course-topic";
const string sub1Name = "Sub1";
ServiceBusClient client;
ServiceBusProcessor processor = default;

async Task MessageHandler(ProcessMessageEventArgs processMessageEventArgs)
{ 
    string body = processMessageEventArgs.Message.Body.ToString();
    Console.WriteLine($"{body} - Subscription: {sub1Name}");

    await processMessageEventArgs.CompleteMessageAsync(processMessageEventArgs.Message);
}

Task ErrorHandler(ProcessErrorEventArgs processMessagweEventArgs)
{ 
    Console.WriteLine(processMessagweEventArgs.Exception.ToString());
    return Task.CompletedTask;
}

client = new ServiceBusClient(connectionString);
processor = client.CreateProcessor(topicName, sub1Name, new ServiceBusProcessorOptions());

try
{
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync += ErrorHandler;

    await processor.StartProcessingAsync();
    Console.WriteLine("Press any key to end the processing");
    Console.ReadKey();

    Console.WriteLine("\nStopping the receiver...");
    await processor.StopProcessingAsync();
    Console.WriteLine("Stopped receiving messages");
}
catch(Exception ex)
{
    Console.WriteLine($"Exception: {ex.Message}");
}
finally
{ 
    await processor.DisposeAsync();
    await client.DisposeAsync();
}
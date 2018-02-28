using System;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Queue;
class Printer
{
    static CloudQueue queue;
    static CloudTableClient tableClient;

    public Printer()
    {
        try
        {

            var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            Console.WriteLine(storageConnectionString);

            tableClient = cloudStorageAccount.CreateCloudTableClient();

            Console.WriteLine("Created Table Client");

            const string QueueName = "stickersqueue";

            var queueClient = cloudStorageAccount.CreateCloudQueueClient();
            queue = queueClient.GetQueueReference(QueueName);

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExistsAsync();

            Console.WriteLine("Created Queue Client");

            ReceiveMessages();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    private async void ReceiveMessages()
    {

        while (true)
        {
            var retrievedMessage = await queue.GetMessageAsync();
            ProcessMessage(retrievedMessage);
            await queue.DeleteMessageAsync(retrievedMessage);
        }
    }
    private void ProcessMessage(CloudQueueMessage message)
    {
        var body = message.AsString;
        // Process the message
        const string version = "1.0.0";
        body = $"{body} ; Printer version: {version}";

        InsertRowToTableStorage(body);
        Thread.Sleep(500);

    }

    private async void InsertRowToTableStorage(string body)
    {
        var table = tableClient.GetTableReference("status");
        await table.CreateIfNotExistsAsync();

        StatusEntity status = new StatusEntity();
        status.Message = body;

        // Create the TableOperation object that inserts the customer entity.
        TableOperation insertOperation = TableOperation.InsertOrReplace(status);
        // Execute the insert operation.
        await table.ExecuteAsync(insertOperation);
    }
}

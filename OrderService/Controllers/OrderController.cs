using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {


        // POST api/order
        [HttpPost]
        public async void Post(string value, int count)
        {
            var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            var cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            Console.WriteLine(storageConnectionString);
            const string QueueName = "stickersqueue";
            var client = cloudStorageAccount.CreateCloudQueueClient();
            var queue = client.GetQueueReference(QueueName);
            await queue.CreateIfNotExistsAsync();

            for (var i = 1; i < count + 1; i++)
            {
                try
                {
                    // Create a new message to send to the queue
                    string messageBody = $"Printing {value} #{i} ";
                    var message = new CloudQueueMessage(messageBody);
                    // Send the message to the queue
                    await queue.AddMessageAsync(message);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
                }
            }
        }
    }
}

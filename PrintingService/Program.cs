using System;
using System.Threading;
using System.Threading.Tasks;

namespace printingservice
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            new Printer();
            await Task.Delay(Timeout.Infinite);
        }
    }
}

using Avans_DevOps.AvansDevOps.Application.Notifications.Services;

namespace Avans_DevOps
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [Program] Demo start");

            new NotificationDemoRunner().Run();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [Program] Demo end");
        }
    }
}

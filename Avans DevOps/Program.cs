using Avans_DevOps.AvansDevOps.Application.Notifications.Services;
using Avans_DevOps.AvansDevOps.Application.Reports.Services;

namespace Avans_DevOps
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [Program] Demo start");

            new NotificationDemoRunner().Run();
            new ReportDemoRunner().Run();

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [Program] Demo end");
        }
    }
}

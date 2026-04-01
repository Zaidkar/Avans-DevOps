namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients
{
    public class SmsSdk
    {
        public void SendSms(string number, string body)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SmsSdk] SendSms -> number: {number} | body: {body}");
        }
    }
}

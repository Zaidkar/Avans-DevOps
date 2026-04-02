using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Adapters
{
    // Design Pattern: Adapter. Wraps SMS SDK behind ISmsProvider.
    public class TwilioSmsAdapter(SmsSdk smsSdk) : ISmsProvider
    {
        private readonly SmsSdk _smsSdk = smsSdk;

        public void SendSms(string number, string body)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [TwilioSmsAdapter] SendSms -> {number}");
            _smsSdk.SendSms(number, body);
        }
    }
}

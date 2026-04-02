using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients;
using Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Providers;

namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Adapters
{
    // Design Pattern: Adapter. Wraps external mail SDK behind IMailProvider.
    public class SendGridMailAdapter(ExternalMailClient mailClient) : IMailProvider
    {
        private readonly ExternalMailClient _mailClient = mailClient;

        public void SendEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [SendGridMailAdapter] SendEmail -> {to}");
            _mailClient.SendMail(to, subject, body);
        }
    }
}

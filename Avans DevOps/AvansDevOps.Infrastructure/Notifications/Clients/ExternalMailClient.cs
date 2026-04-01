namespace Avans_DevOps.AvansDevOps.Infrastructure.Notifications.Clients
{
    public class ExternalMailClient
    {
        public void SendMail(string to, string subject, string body)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] [ExternalMailClient] SendMail -> to: {to} | subject: {subject} | body: {body}");
        }
    }
}
